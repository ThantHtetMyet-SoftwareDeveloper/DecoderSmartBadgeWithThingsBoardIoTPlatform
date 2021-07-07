using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData
{
    class ConvertedData
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public string Battery { get; set; }
        public string Temperature { get; set; }
        public string Ack { get; set; }
        public string Opt { get; set; }

        // General
        public string MessageType { get; set; }
        public string Age { get; set; }

        public string UUid { get; set; }

        public string Rssi { get; set; }

        public string MacAddr { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EHPE { get; set; }
        public string Encrypted { get; set; }

        public string Cause { get; set; }
        public string CnZero { get; set; }
        public string CnOne { get; set; }
        public string CnTwo { get; set; }
        public string CnThree { get; set; }

        public ConvertedData()
        {
            this.Type = "";
            this.Status = "";
            this.Battery = "";
            this.Temperature = "";
            this.Type = "";
            this.Status = "";
            this.Battery = "";
            this.Temperature = "";
            this.Ack = "";
            this.Opt = "";
            this.MessageType = "";
            this.Age = "";
            this.UUid = "";
            this.Rssi = "";
            this.MacAddr = "";
            this.Latitude = "";
            this.Longitude = "";
            this.EHPE = "";
            this.Encrypted = "";
            this.Cause = "";
            this.CnZero = "";
            this.CnOne = "";
            this.CnTwo = "";
            this.CnThree = "";

    }
        public override string ToString()
        {
            string result = " 'Type': '" + Type + "', 'Status': '" + Status + "', 'Battery': '" + Battery + "', 'Temperature': '" + Temperature + "', 'Ack': '" + Ack + "', 'Opt': '" + Opt + "'";


            if (MessageType.Length > 0)
            {
                result = result + ", 'MessageType': '" + MessageType + "'";
            }
            if (Age.Length > 0)
            {
                result = result + "', 'Age': '" + Age + "'";
            }
            if (UUid.Length > 0)
            {
                
                result = result + "', 'UUid': '" + UUid;
            }
            if (Rssi.Length > 0)
            {
                result = result + "', 'Rssi': '" + Rssi + "'";
            }
            if (MacAddr.Length > 0)
            {
                result = result + "', 'MacAddress': '" + MacAddr + "'";
            }

            if (Latitude.Length > 0)
            {
                result = result + "', 'Latitude': '" + Latitude + "'";
            }
            if (Longitude.Length > 0)
            {
                result = result + "', 'Longitude': '" + Longitude + "'";
            }
            if (EHPE.Length > 0)
            {
                result = result + "', 'EHPE': '" + EHPE + "'";
            }
            if (Encrypted.Length > 0)
            {
                result = result + "', 'Encrypted': '" + Encrypted + "'";
            }

            if (Cause.Length > 0)
            {
                result = result + "', 'Cause': '" + Cause + "'";
            }
            if (CnZero.Length > 0)
            {
                result = result + "', 'CNZERO': '" + CnZero + "'";
            }
            if (CnOne.Length > 0)
            {
                result = result + "', 'CNONE': '" + CnOne + "'";
            }
            if (CnTwo.Length > 0)
            {
                result = result + "', 'CNTWO': '" + CnTwo + "'";
            }
            if (CnThree.Length > 0)
            {
                result = result + "', 'CNTHREE': '" + CnThree + "'";
            }
            return result;
        }
    }
}
