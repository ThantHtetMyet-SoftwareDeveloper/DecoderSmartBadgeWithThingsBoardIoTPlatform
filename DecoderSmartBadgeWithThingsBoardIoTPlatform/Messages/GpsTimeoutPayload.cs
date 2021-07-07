using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData.Messages
{
    class GpsTimeoutPayload
    {
        public string MessageType { get; set; }
        public string Cause { get; set; }
        public string CnZero { get; set; }
        public string CnOne { get; set; }
        public string CnTwo { get; set; }
        public string CnThree { get; set; }

        public override string ToString()
        {
            return "Cause: " + Cause + ", CnZero: " + CnZero + ", CnOne: " + CnOne + ", CnTwo: " + CnTwo + "CnThree: " + CnThree;
         }
    }
}
