using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData
{
    class data
    {
        public string DevEUI { get; set; }
        public string SensorType { get; set; }
        public string ModelCfg { get; set; }
        public string MessageType { get; set; }
        public string BeaconId { get; set; }
        public string Payload_hex { get; set; }
        public string Time { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        public data()
        {
            this.DevEUI = "";
            this.SensorType = "";
            this.ModelCfg = "";
            this.MessageType = "";
            this.BeaconId = "";
            this.Payload_hex = "";
            this.Time = "";
            this.latitude = 0.0;
            this.longitude = 0.0;
        }
    }
}
