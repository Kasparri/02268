import RestAPI as API

# Message keys
START_EVENT_MESSAGE = "CancellationEventReceived"

# Process ID's
CANCELLATION_PROCESS_ID = "CancellationFlight"


API.wait("send message that starts the process")
API.sendMessage(START_EVENT_MESSAGE, "")
print("\nThe current active tasks are:")
tasks = API.getTasks(CANCELLATION_PROCESS_ID)


API.wait("collect list of passengers")
API.completeTask(tasks[0])
print("\n The current active tasks are:")
tasks = API.getTasks(CANCELLATION_PROCESS_ID)


API.wait("refund payments customers")
API.completeRefunds(tasks)


API.wait("create cancellation notification emails")
API.completeCreateEmailsActivity(tasks)