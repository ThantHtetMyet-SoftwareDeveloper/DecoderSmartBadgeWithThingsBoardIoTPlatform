using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData.Messages
{
    class BeaconScanWithShortIdentifier
    {
        public string MessageType { get; set; }
        public string Age { get; set; }

        public string UUid { get; set; }

        public string Rssi { get; set; }
        public override string ToString()
        {
            return "Age: " + Age + ", UUID: " + UUid + ", RSSI: " + Rssi;
        }
    }
}
