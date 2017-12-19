using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;

namespace Quantum.QSharpApplication6
{
    class Driver
    {
        private static int _oneCounter;
        private static int _zeroCounter;

        static void Main(string[] args)
        {
            var sim = new QuantumSimulator(throwOnReleasingQubitsNotInZeroState: true);

            // Apply a Hadamard operation H to the state, thereby creating the state 1/sqrt(2)(|0〉+|1〉). 
            AnalyzeHadamardOperation(sim, Result.Zero);
            AnalyzeHadamardOperation(sim, Result.One);

            System.Console.WriteLine("");

            // Apply a Hadamard operation H to the state, thereby creating the state 1/sqrt(2)(|0〉+|1〉). 
            // Use the MResetZ operation to release the used qubits
            AnalyzeHadamardOperationUsingMResetZ(sim, Result.Zero);
            AnalyzeHadamardOperationUsingMResetZ(sim, Result.One);


            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
        }

        private static void AnalyzeHadamardOperationUsingMResetZ(QuantumSimulator sim, Result initialValue)
        {
            _oneCounter = 0;
            _zeroCounter = 0;

            for (int i = 0; i < 10000; i++)
            {
                var res = HGateUsingMResetZ.Run(sim, initialValue).Result;
                if (res == Result.One)
                    _oneCounter++;
                else
                    _zeroCounter++;
            }
            System.Console.WriteLine($"H Gate using MResetZ for [{initialValue,4}] - One count: {_oneCounter,-4} Zero  count: {_zeroCounter,-4}");
        }

        private static void AnalyzeHadamardOperation(QuantumSimulator sim, Result initialValue)
        {
            _oneCounter = 0;
            _zeroCounter = 0;

            for (int i = 0; i < 10000; i++)
            {
                var res = HGate.Run(sim, initialValue).Result;
                if (res == Result.One)
                    _oneCounter++;
                else
                    _zeroCounter++;
            }
            System.Console.WriteLine($"H Gate for [{initialValue, 4}] - One count: {_oneCounter,-4} Zero  count: {_zeroCounter,-4}");
        }
    }
}