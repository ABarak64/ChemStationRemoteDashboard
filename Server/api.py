from flask import Flask, request, url_for, Response
import json
import sqlite3 as lite

app = Flask(__name__)
@app.route('/')
# return my dashboard view here.
@app.route('/chemstationstatus', methods=['GET', 'POST'])
def api_chemstationstatus():
    # If the requester wants to deliver a new Chemstation status.
    if request.method == 'POST':
        jsonStr = request.form['json']
        jsonObj = json.loads(jsonStr)
        try:
            con = lite.connect('/home/WhatupHPLC/chemstationdata.db')
            cur = con.cursor()
            values_to_insert = (jsonObj[u'Status'], jsonObj[u'Time'], jsonObj[u'SequenceName'], jsonObj[u'MethodName'],
                1 if jsonObj[u'MethodRunning'] == True else 0, 1 if jsonObj[u'SequenceRunning'] == True else 0)
            cur.execute("""INSERT INTO status VALUES(null, ?, ?, ?, ?, ?, ?);""", values_to_insert)
            con.commit()
        finally:
            if con:
                con.close()
        resp = Response(status=200, mimetype='application/json')
        return resp
   # Else the requester wants the latest ChemStation status.
    else:
        con = lite.connect('/home/WhatupHPLC/chemstationdata.db')
        with con:
            con.row_factory = lite.Row
            cur = con.cursor()
            cur.execute("SELECT * FROM status WHERE status_id = (SELECT MAX(status_id) FROM status);")
            row = cur.fetchone()

            statusObj = {
                'Status' : row['status'],
                'Time' : row['time'],
                'SequenceName' : row['sequence'],
                'MethodName' : row['method'],
                'SequenceRunning' : True if row['sequenceOn'] == 1 else False,
                'MethodRunning' : True if row['methodOn'] == 1 else False
            }
        resp = Response(json.dumps(statusObj), status=200, mimetype='application/json')
        return resp

