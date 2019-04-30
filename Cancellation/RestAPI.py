import requests
import json
import time


# Message keys
START_EVENT_MESSAGE = "CancellationEventReceived"
EMAIL_CONFIRMATION_MESSAGE = "AllEmailNotificationsReceivedEvent"

# Process ID's
CANCELLATION_PROCESS_ID = "CancellationFlight"

def wait():
    print("Press enter to continue...")
    raw_input()

def getTasks(processDefinitionKey):
    resp = requests.get('http://localhost:8080/engine-rest/task?processDefinitionKey=%s'%processDefinitionKey)
    if resp.status_code != 200:
        raise Exception('GET /tasks/ {}'.format(resp.status_code))
    items = []
    for todo_item in resp.json():
        items.insert(0, (todo_item['id'], todo_item['processInstanceId'], todo_item['name']))
        print('Item \'{}\' with id: {}'.format(todo_item['name'], todo_item['id']))
    return items

def completeTask(task):
    headers = {'content-type': 'application/json',}
    url = 'http://localhost:8080/engine-rest/task/%s/complete'%str(task[0])
    resp = requests.post(url, headers = headers)
    if resp.status_code != 200 and resp.status_code != 204:
        raise Exception('POST /task/id/complete {}'.format(resp.status_code))
    print('Completed %s with id %s'%(task[2], task[0]))

def sendMessage(messageName, processId):
    url = 'http://localhost:8080/engine-rest/message'
    headers = {'content-type': 'application/json',}
    if processId is "":
        data = {"messageName" : messageName}
    else:
        data = {"messageName" : messageName, "processInstanceId" : processId} 
    resp = requests.post(url, data = json.dumps(data), headers = headers)
    if resp.status_code != 200 and resp.status_code != 204:
        raise Exception('POST /task/id/complete {}'.format(resp.status_code))
    print('Message %s sent'%messageName)

def completeRefunds(task_list):
    for task in task_list:
        if "Refund payment" not in task[2]:
            continue
        time.sleep(1.5)
        completeTask(task)
        
    print("All refunds complete \n")

def completeCreateEmailsActivity(task_list):
    for task in task_list:
        if "create cancellation notification emails" not in task[2]:
            continue
        completeTask(task)
        

