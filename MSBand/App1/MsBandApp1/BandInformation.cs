using System;
using System.Threading.Tasks;
using Microsoft.Band;

namespace App1
{
    public class BandInformation
    {
        public string Name { get; private set; }
        public string Firmware { get; private set; }
        public string Hardware { get; private set; }
        public BandConnectionType ConnectionType { get; private set; }

        public async Task<string> RetrieveInfo(IBandInfo bandInfo, IBandClient client)
        {
            Name = bandInfo.Name;
            ConnectionType = bandInfo.ConnectionType;
            Firmware = await client.GetFirmwareVersionAsync();
            Hardware = await client.GetHardwareVersionAsync();
            return String.Format(" Connected to: {0}" +
                          " \n Connection type : {1}" +
                          " \n Firmware : {2} \n Hardware : {3}",
                Name, ConnectionType, Firmware, Hardware);
        }
    }
}