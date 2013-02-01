from flask import Flask, request, url_for, Response, send_from_directory
import json
import sqlite3 as lite

DATABASE = '/home/WhatupHPLC/chemstationdata.db'

app = Flask(__name__)

# The requester wants the dashboard view.
@app.route('/')
def serve_dashboard():
     return send_from_directory('/home/WhatupHPLC/mysite', 'index.html')

# The requester wants a variable amount of the most recent ChemStation statuses.
@app.route('/chemstationstatus/last/<count>')
def api_get_recent_chemstation_status(count):
    con = lite.connect(DATABASE)
    statusSet = []
    with con:
        con.row_factory = lite.Row
        cur = con.cursor()
        vals_to_insert = (count, )
        cur.execute("SELECT * FROM status ORDER BY status_id DESC LIMIT ?;", vals_to_insert)
        rows = cur.fetchall()

        for row in rows:
            statusObj = {
            'Id' : row['status_id'],
            'Status' : row['status'],
            'Time' : row['time'],
            'SequenceName' : row['sequence'],
            'MethodName' : row['method'],
            'SequenceRunning' : True if row['sequenceOn'] == 1 else False,
            'MethodRunning' : True if row['methodOn'] == 1 else False
            }
            statusSet.append(statusObj)

    resp = Response(json.dumps(statusSet), status=200, mimetype='application/json')
    return resp

# The requester wants to deliver a new Chemstation status.
@app.route('/chemstationstatus', methods=['POST'])
def api_chemstationstatus():
    jsonStr = request.form['json']
    jsonObj = json.loads(jsonStr)
    try:
        con = lite.connect(DATABASE)
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