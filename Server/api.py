from flask import Flask, request, url_for, Response, send_from_directory
import json
import sqlite3 as lite

DATABASE = '/home/WhatupHPLC/chemstationdata.db'

# Helper method to ORMify a dataset.
def status_dataset_to_object(dataset):
    obj = {
        'Id' : dataset['status_id'],
        'Status' : dataset['status'],
        'Time' : dataset['time'],
        'SequenceName' : dataset['sequence'],
        'MethodName' : dataset['method'],
        'SequenceRunning' : True if dataset['sequenceOn'] == 1 else False,
        'MethodRunning' : True if dataset['methodOn'] == 1 else False
        }
    return obj

app = Flask(__name__)

# The requester wants the dashboard view.
@app.route('/')
def serve_dashboard():
     return send_from_directory('/home/WhatupHPLC/mysite', 'index.html')

# The requester wants a particular ChemStation status.
@app.route('/chemstationstatus/<id>')
def api_get_chemstation_status(id):
    con = lite.connect(DATABASE)
    with con:
        con.row_factory = lite.Row
        cur = con.cursor()
        vals_to_insert = (id, )
        cur.execute("SELECT * FROM status WHERE status_id = ?;", vals_to_insert)
        row = cur.fetchone()
        if row == None:
            statusObj = {}
        else:
            statusObj = status_dataset_to_object(row)

    resp = Response(json.dumps(statusObj), status=200, mimetype='application/json')
    return resp

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
            statusObj = status_dataset_to_object(row)
            statusSet.append(statusObj)

    resp = Response(json.dumps(statusSet), status=200, mimetype='application/json')
    return resp

# The requester wants the status statistics on a variable amount of the last statuses.
@app.route('/chemstationstatus/statistics/<count>')
def api_get_chemstation_status_stats(count):
    con = lite.connect(DATABASE)
    statSet = []
    with con:
        con.row_factory = lite.Row
        cur = con.cursor()
        vals_to_insert = (count, )
        cur.execute("""SELECT status, count(*) AS count FROM (SELECT status FROM status
            ORDER BY status_id DESC LIMIT ?) GROUP BY status;""", vals_to_insert)
        rows = cur.fetchall()

        for row in rows:
            obj = {
                'label' : row['status'],
                'value' : row['count']
                }
            statSet.append(obj)

        cur.execute("""SELECT substr(time,0,10) AS time FROM status WHERE
            status_id = ((SELECT MAX(status_id) from status) - ?);""", vals_to_insert)
        row = cur.fetchone()
        stats = { 'time' : row['time'],
                  'data' : statSet }

    resp = Response(json.dumps(stats), status=200, mimetype='application/json')
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