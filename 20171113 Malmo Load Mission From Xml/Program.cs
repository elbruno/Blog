using System;
using System.IO;
using System.Threading;
using Microsoft.Research.Malmo;

namespace Malmo04
{
    class Program
    {
        private static AgentHost _agentHost;
        private static MissionSpec _mission;
        private static MissionRecordSpec _missionRecord;
        private static WorldState _worldState;

        public static void Main()
        {
            InitAgentHost();
            InitMissionSpecs();
            StartMission();
            WaitMissionToStart();

            Console.WriteLine("Mission in progress !");
            Console.WriteLine();
            do
            {
                Console.Write("-");
                Thread.Sleep(1000);
            } while (_worldState.is_mission_running);
            Console.WriteLine("Mission has stopped.");
        }

        private static void WaitMissionToStart()
        {
            Console.WriteLine("Waiting for the mission to start");
            do
            {
                Console.Write(".");
                Thread.Sleep(100);
                _worldState = _agentHost.getWorldState();
            } while (!_worldState.has_mission_begun);
        }

        private static void InitMissionSpecs()
        {
            var missionXmlDefinition = File.ReadAllText("SampleMission.xml");

            _mission = new MissionSpec(missionXmlDefinition, false);
            _mission.requestVideo(640, 480);

            var recordedFileName = $"./mission_data{DateTime.Now:hh mm ss}.tgz";
            _missionRecord = new MissionRecordSpec(recordedFileName);
            _missionRecord.recordCommands();
            _missionRecord.recordMP4(20, 400000);
            _missionRecord.recordRewards();
            _missionRecord.recordObservations();

        }

        private static void StartMission()
        {
            try
            {
                _agentHost.startMission(_mission, _missionRecord);
            }
            catch (MissionException ex)
            {
                // Using catch(Exception ex) would also work, but specifying MissionException allows
                // us to access the error code:
                Console.Error.WriteLine("Error starting mission: {0}", ex.Message);
                Console.Error.WriteLine("Error code: {0}", ex.getMissionErrorCode());
                // We can do more specific error handling using this code, eg:
                if (ex.getMissionErrorCode() == MissionException.MissionErrorCode.MISSION_INSUFFICIENT_CLIENTS_AVAILABLE)
                    Console.Error.WriteLine("Have you started a Minecraft client?");
                Environment.Exit(1);
            }
        }

        private static void InitAgentHost()
        {
            _agentHost = new AgentHost();
            try
            {
                _agentHost.parse(new StringVector(Environment.GetCommandLineArgs()));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: {0}", ex.Message);
                Console.Error.WriteLine(_agentHost.getUsage());
                Environment.Exit(1);
            }
            if (_agentHost.receivedArgument("help"))
            {
                Console.Error.WriteLine(_agentHost.getUsage());
                Environment.Exit(0);
            }
        }
    }
}
