namespace Quantum.QSharpApplication6
{
    open Microsoft.Quantum.Primitive;
    open Microsoft.Quantum.Canon;
	
	operation HGateUsingMResetZ (initial: Result) : (Result)
    {
        body
        {
			mutable res = Zero;
			using (qubits = Qubit[1]) 
			{
                let qubit = qubits[0];
				H(qubit);
				set res = MResetZ(qubit);
			}
            return (res);
        }
    }
	
	operation HGate (initial: Result) : (Result)
    {
        body
        {
			mutable res = Zero;
			using (qubits = Qubit[1]) 
			{
                let qubit = qubits[0];
				H(qubit);
				set res = M (qubit);
				if (res == One)
				{
					X(qubit);
                }
			}
            return (res);
        }
    }

}
