import RestAPI as ra

sendMessage(START_EVENT_MESSAGE, "")
wait()

tasks = getTasks(CANCELLATION_PROCESS_ID)
wait()

completeTask(tasks[0])
wait()

tasks = getTasks(CANCELLATION_PROCESS_ID)
wait()

completeRefunds(tasks)
wait()

completeSendEmailsAndRespond(tasks)

print("Execution finished")
