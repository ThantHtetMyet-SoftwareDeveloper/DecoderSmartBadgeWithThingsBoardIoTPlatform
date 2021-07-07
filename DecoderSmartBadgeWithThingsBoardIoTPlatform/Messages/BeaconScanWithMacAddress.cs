using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData.Messages
{
    class BeaconScanWithMacAddress
    {
        public string MessageType { get; set; }
        public string Age { get; set; }

        public string MacAddr { get; set; }
        public string Rssi { get; set; }
        public override string ToString()
        {
            return "Age: " + Age + ", Mac Address: " + MacAddr + ", RSSI: " + Rssi;
        }
    }
}
