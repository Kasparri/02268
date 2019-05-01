# Script to be used to go through all the tasks in the check-in process

import requests
import random

base = "http://localhost:8080/engine-rest"
endpoint = base + "/message"
rand1 = random.random()
rand2 = random.random()

# set check-in type
if rand1 <= 0.3:
    checkIn = "online"
elif rand1 <= 0.6:
    checkIn = "counter"
else:
    checkIn = "terminal"

# set if passenger has luggage
if rand2 <= 0.5:
    hasLuggage = True
else:
    hasLuggage = False

data = {'messageName': 'notify',
        'processVariables': {
            'checkIn': {
                'value': checkIn,
                'type': 'String'
            },
            'hasLuggage': {
                'value': hasLuggage,
                'type': 'Boolean'
            }
        }}

r = requests.post(endpoint, json=data)
# print(r.status_code)

while True:
    input("Press Enter to continue...")

    response = requests.get(base + "/task")

    if len(response.json()) == 0:
        break

    taskId = response.json()[0]["id"]
    print(taskId)
    # claim the task
    response = requests.post(base + "/task/" + taskId + "/claim", json={'userId': 'demo'})
    print(response.status_code)

    response = requests.post(base + "/task/" + taskId + "/complete", json={})
    print(response.status_code)

