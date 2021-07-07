using System;
using System.Collections.Generic;
using System.Text;
using DecodeSmartBadgeData.Messages;
using System.Text.Json;
using Newtonsoft.Json;

namespace DecodeSmartBadgeData
{
    class DecodeData
    {
        public payloadData encodedData { get; set; }

        public payloadData decodedData { get; set; }

        public String GenerateData()
        {
            //Console.WriteLine("---- Analyze Message ----");
            
            switch(this.decodedData.Type)
            {
                case "ActivityStatus_Configuration_ShockDetection":
                    Console.WriteLine("*** Activity - Configuration - Shock ***");
                    break;
                case "CollectionScan":
                    Console.WriteLine("*** CollectionScan ***");
                    break;
                case "Debug":
                    Console.WriteLine("*** Debug ***");
                    break;
                case "EnergyStatus":
                    Console.WriteLine("*** EnergyStatus ***");
                    break;
                case "Event":
                    Console.WriteLine("*** Event ***");
                    break;
                case "FramePending":
                    Console.WriteLine("*** FramePending ***");
                    break;
                case "Heartbeat":
                    Console.WriteLine("*** Heartbeat ***");
                    break;
                case "Position":
                    // GPS Fix
                    if (Convert.ToInt32(this.decodedData.Opt) == 0) 
                    {
                        //Console.WriteLine("GPS Fix");
                        if(this.encodedData.Data.Length == 22)
                        {
                            GpsFix gpsFixObj = new GpsFix();
                            gpsFixObj.MessageType = "GpsFix";
                            gpsFixObj.Age = ConvertHexToDecimal(this.encodedData.Data.Substring(0, 2)).ToString();
                            gpsFixObj.Latitude = gpsFixObj.convertLatitude(this.encodedData.Data.Substring(2, 6));
                            gpsFixObj.Longitude = gpsFixObj.convertLongitude(this.encodedData.Data.Substring(8, 6));
                            gpsFixObj.EHPE = gpsFixObj.GetEHPE(this.encodedData.Data.Substring(14, 2));
                            gpsFixObj.Encrypted = this.encodedData.Data.Substring(16, 6);
                            
                            return JsonConvert.SerializeObject(gpsFixObj);
                        }
                        else
                        {
                            Console.WriteLine("Data Format does not match as expected!");
                        }
                    }
                    // GPS Timeout Payload
                    if (Convert.ToInt32(this.decodedData.Opt) == 1)
                    {
                        //Console.WriteLine("GPS Timeout Payload");
                        GpsTimeoutPayload gpsTimeoutObj = new GpsTimeoutPayload();
                        gpsTimeoutObj.MessageType = "GPSTimeoutPayload";
                        gpsTimeoutObj.Cause = this.encodedData.Data.Substring(0, 2);
                        gpsTimeoutObj.CnZero = this.encodedData.Data.Substring(2, 2);
                        gpsTimeoutObj.CnOne = this.encodedData.Data.Substring(4, 2);
                        gpsTimeoutObj.CnTwo = this.encodedData.Data.Substring(6, 2);
                        gpsTimeoutObj.CnThree = this.encodedData.Data.Substring(8, 2);

                        return JsonConvert.SerializeObject(gpsTimeoutObj);
                    }
                    
                    // BLE Beacon Scan with MAC Address
                    // BLE Beacon scan payload (MAC Address)
                    if (Convert.ToInt32(this.decodedData.Opt) == 7)
                    {
                       // Console.WriteLine("BEACON scan with MAC Address");
                        // Validate Format
                        if((this.encodedData.Data.Length-2) % 14 == 0)
                        {
                            BeaconScanWithMacAddress beaconWithMacObj = new BeaconScanWithMacAddress();
                            beaconWithMacObj.MessageType = "BeaconScanWithMacAddress";
                            beaconWithMacObj.Age = ConvertHexToDecimal(this.encodedData.Data.Substring(0, 2)).ToString();
                            string tempMacAddRssi = this.encodedData.Data.Substring(2, this.encodedData.Data.Length-2);
                            List<Dictionary<string, string>> macAddAndRssioList = new List<Dictionary<string, string>>();
                            double rssiValue = 0;
                            string macAddress = "";
                            for (int j=0;j<tempMacAddRssi.Length;j+=14)
                            {
                                string macAddr = tempMacAddRssi.Substring(j, 14);
                                if (j == 0)
                                {
                                    rssiValue = ConvertHexToDecimal(macAddr.Substring(12, 2));
                                    macAddress = macAddr.Substring(0, 12);
                                }
                                else
                                {
                                    if(rssiValue<ConvertHexToDecimal(macAddr.Substring(12,2)))
                                    {
                                        rssiValue = ConvertHexToDecimal(macAddr.Substring(12, 2));
                                        macAddress = macAddr.Substring(0, 12);
                                    }
                                }
                            }
                            beaconWithMacObj.MacAddr = macAddress;
                            beaconWithMacObj.Rssi = rssiValue.ToString();
                            return JsonConvert.SerializeObject(beaconWithMacObj);
                        }
                        else
                        {
                            Console.WriteLine("Data Format does not match as expected!");
                        }
                    }
                    // BLE Beacon scan payload (Short Identifier)
                    if (Convert.ToInt32(this.decodedData.Opt) == 10)
                    {   
                        //Console.WriteLine("BEACON scan Payload(Short Identifier)");
                        // Validate Format
                        if ((this.encodedData.Data.Length - 2) % 14 == 0)
                        {
                            BeaconScanWithShortIdentifier beaconWithShortIdentifier = new BeaconScanWithShortIdentifier();
                            beaconWithShortIdentifier.MessageType = "BeaconScanWithShortIdentifier";
                            beaconWithShortIdentifier.Age = ConvertHexToDecimal(this.encodedData.Data.Substring(0, 2)).ToString();
                            string tempMacAddRssi = this.encodedData.Data.Substring(2, this.encodedData.Data.Length - 2);
                            double rssiValue = 0;
                            string uuid = "";
                            for (int j = 0; j < tempMacAddRssi.Length; j += 14)
                            {
                                string uuidRssi = tempMacAddRssi.Substring(j, 14);
                                if (j == 0)
                                {
                                    rssiValue = ConvertHexToDecimal(uuidRssi.Substring(12, 2));
                                    uuid = uuidRssi.Substring(0, 12);
                                }
                                else
                                {
                                    if (rssiValue < ConvertHexToDecimal(uuidRssi.Substring(12, 2)))
                                    {
                                        rssiValue = ConvertHexToDecimal(uuidRssi.Substring(12, 2));
                                        uuid = uuidRssi.Substring(0, 12);
                                    }
                                }

                            }
                            beaconWithShortIdentifier.UUid = uuid;
                            beaconWithShortIdentifier.Rssi = rssiValue.ToString();
                            return JsonConvert.SerializeObject(beaconWithShortIdentifier);
                        }
                        else
                        {
                            Console.WriteLine("Data Format does not match as expected!");
                        }
                    }
                    // BLE Beacon scan payload (Long Identifier)
                    if (Convert.ToInt32(this.decodedData.Opt) == 11)
                    {
                        //Console.WriteLine("BEACON scan Payload(Long Identifier)");
                        // Validate Format
                        if (this.encodedData.Data.Length == 36)
                        {
                            BeaconScanWithLongIdentifier beaconWithlongIdentifier = new BeaconScanWithLongIdentifier();
                            beaconWithlongIdentifier.MessageType = "BeaconScanWithLongIdentifier";
                            beaconWithlongIdentifier.Age = ConvertHexToDecimal(this.encodedData.Data.Substring(0, 2)).ToString();
                            beaconWithlongIdentifier.UUid = this.encodedData.Data.Substring(2, 32);
                            beaconWithlongIdentifier.Rssi = ConvertHexToDecimal(this.encodedData.Data.Substring(34, 2)).ToString();
                            return JsonConvert.SerializeObject(beaconWithlongIdentifier);
                        }
                        else
                        {
                            Console.WriteLine("Data Format does not match as expected!");
                        }
                    }
                    break;
                case "Proximity":
                    Console.WriteLine("*** Proximity ***");
                    break;
                case "ShutDown":
                    Console.WriteLine("*** ShutDown ***");
                    break;
            }
            return null;
        }

        // Convert Hex to Decimal
        public ulong ConvertHexToDecimal(string hexValue)
        {
            ulong number = UInt64.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return number;
        }

        public string GetMessageType(int optValue)
        {
            if(optValue == 0 || optValue == 1)
            {
                return "GPS";
            }
            else if(optValue == 10 || optValue == 11 || optValue == 7)
            {
                return "BEACON";
            }
            else
            {
                return "Unknown";
            }
        }

    }
}
