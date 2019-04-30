import RestAPI as ra


# Message keys
START_EVENT_MESSAGE = "CancellationEventReceived"
EMAIL_CONFIRMATION_MESSAGE = "AllEmailNotificationsReceivedEvent"


# Process ID's
CANCELLATION_PROCESS_ID = "CancellationFlightConnector"

#ra.sendMessage(START_EVENT_MESSAGE, "")
#tasks = ra.getTasks(CANCELLATION_PROCESS_ID)
#ra.completeTask(tasks[0])

tasks = ra.getTasks(CANCELLATION_PROCESS_ID)
ra.completeRefunds(tasks)
#ra.completeCreateEmailsActivity(tasks)
