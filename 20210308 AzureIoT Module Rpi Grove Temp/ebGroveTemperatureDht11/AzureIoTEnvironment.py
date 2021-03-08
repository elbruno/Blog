import datetime
import os
from AzureIoTLogger import AzureIoTLogger

class AzureIoTEnvironment:
    @staticmethod
    def GetEnvVarString(name):
        varVal = os.environ[name]
        AzureIoTLogger.Log(f"Read Environment Var String '{name}': {varVal}")
        return varVal

    @staticmethod
    def GetEnvVarBool(name):
        varVal = bool(os.environ[name])
        AzureIoTLogger.Log(f"Read Environment Var Bool '{name}': {varVal}")
        return varVal        

    @staticmethod
    def GetEnvVarInt(name):
        varVal = int(os.environ[name])
        AzureIoTLogger.Log(f"Read Environment Var Int '{name}': {varVal}")
        return varVal        