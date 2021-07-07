
using System;
namespace DecodeSmartBadgeData
{
    public class payloadData
    {
        public string Type {get; set;}
        public string Status {get;set;}
        public string Battery {get;set;}
        public string Temperature {get;set;}
        public string Ack{get;set;}
        public string Opt { get; set; }
        public string Data{get;set;}

        public void SetStatus(string rawData)
        {
            Console.WriteLine(rawData);
        }

        public override string ToString()
        {
            return "Type: " + Type + ", Status: " + Status + ", Battery: " + Battery + ", Temperature: " + Temperature +
                ", Ack: " + Ack + ", Opt: " + Opt + ", Data: " + Data;                 
        }

        public string getCommonHeader()
        {
            return Type + Status + Battery + Temperature + Ack + Opt;
        }

    }
}