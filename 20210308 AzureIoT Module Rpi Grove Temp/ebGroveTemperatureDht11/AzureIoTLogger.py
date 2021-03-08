import datetime
class AzureIoTLogger:
    @staticmethod
    def Log(message):
        current_time = datetime.datetime.now()  
        message = str(f"{current_time} | {message}")
        print(message)