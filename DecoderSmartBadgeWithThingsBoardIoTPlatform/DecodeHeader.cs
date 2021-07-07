using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData
{
    public class DecodeHeader
    {
        public Dictionary<string, string> typeDict = new Dictionary<string, string>();
        public Dictionary<string, string> statusDict = new Dictionary<string, string>();
        public Dictionary<string, string> batteryDict = new Dictionary<string, string>();
        public DecodeHeader()
        {
            setTypeDict();
            SetStatusDict();
            SetBatteryDict();
        }
        
        // Type
        public void setTypeDict()
        {
            typeDict.Add("07", "ActivityStatus_Configuration_ShockDetection");
            typeDict.Add("0B", "CollectionScan");
            typeDict.Add("0F", "Debug");
            typeDict.Add("04", "EnergyStatus");
            typeDict.Add("0A", "Event");
            typeDict.Add("00", "FramePending");
            typeDict.Add("05", "Heartbeat");
            typeDict.Add("03", "Position");
            typeDict.Add("0C", "Proximity");
            typeDict.Add("09", "ShutDown");
        }
        public string GetType(string inputTypeKey)
        {
           return typeDict[inputTypeKey];
        }

        // Status
        public void SetStatusDict()
        {
            statusDict.Add("000", "StandBy");
            statusDict.Add("001", "MotionTracking");
            statusDict.Add("010", "PermanentTracking");
            statusDict.Add("011", "MotionStart/EndTracking");
            statusDict.Add("100", "ActivityTracking");
            statusDict.Add("101", "OFF");
        }

        public String GetStatus(String inputHex)
        {
            string binaryValue = ConvertDecimalToBinary(Convert.ToInt32(ConvertHexToDecimal(inputHex)));
            int binaryLength = binaryValue.Length;
            string sevenBit = binaryValue[binaryLength - 1].ToString();
            string sixthBit = binaryValue[binaryLength - 2].ToString();
            string fifthBit = binaryValue[binaryLength - 3].ToString();
            return statusDict[sevenBit + sixthBit + fifthBit];
        }

        // Battery 
        public void SetBatteryDict()
        {
            batteryDict.Add("00", "Battery Charging");
            batteryDict.Add("FF", "Error in measurement");
        }
        public String GetBattery(String inputHex)
        {   
            if(inputHex == "00")
            {
                return batteryDict["00"];
            }
            else
            {
                int battery = Convert.ToInt32(ConvertHexToDecimal(inputHex));

                if(0 < battery && battery <= 100)
                {
                    return battery + "%";
                }
                else
                {
                    return batteryDict["FF"];
                }
            }   
        }

        // Temperature
        public double GetTemperature(string tempHexValue)
        {
            return mt_value_decode(Convert.ToInt32(tempHexValue), -44, 85, 8, 0);
        }
        public double CalculateStepSize(float lo, float hi, float nbits, float nresv)
        { return 1.0 / (((128 - 1) - nresv) / (hi - lo)); }

        public double mt_value_decode(int value, float lo, float hi, int nbits, int nresv)
        { return ((value - nresv / 2) * CalculateStepSize(lo, hi, nbits, nresv) + lo); }

        //ACK & OPT
        public string GetAck(string ackOpt)
        {
            string binaryValue = ConvertDecimalToBinary(Convert.ToInt32(ConvertHexToDecimal(ackOpt)));
            string ackResult = binaryValue.Substring(0, 4);
            return ackResult;
        }
        public string GetOpt(string ackOpt)
        {
            string result = ConvertDecimalToBinary(Convert.ToInt32(ConvertHexToDecimal(ackOpt)));
            return result.Substring(4, 4);
        }

        // Object Initialization 
        // Convert Hex String Data to Two Byte Hex Array
        public List<String> convertHexOneByteArr(String inputHexData)
        {
            List<String> result = new List<String>();
            String tempData = "";
            for(var i = 0; i < 11; i++)
            {
                if(i%2 != 0)
                {
                    tempData = tempData + inputHexData[i];
                    result.Add(tempData);
                    tempData = "";
                }
                else if(i==10)
                {
                    tempData = inputHexData.Substring(i); 
                    result.Add(tempData);
                    tempData = "";
                }
                else{
                    tempData = tempData + inputHexData[i];
                }
            } 
            return result;    
        }

        public payloadData setEncodedHexDataObj(List<String> oneByteHexArray)
        {   
            payloadData hexDataObj = new payloadData();
            // Assign Common Header 
            hexDataObj.Type = oneByteHexArray[0];
            hexDataObj.Status = oneByteHexArray[1];
            hexDataObj.Battery = oneByteHexArray[2];
            hexDataObj.Temperature = oneByteHexArray[3];
            hexDataObj.Ack = GetAck(oneByteHexArray[4]);
            hexDataObj.Opt = GetOpt(oneByteHexArray[4]);
            hexDataObj.Data = oneByteHexArray[5];
            
            return hexDataObj;
        }

        // Convert Hex to Binary Format
        public String ConvertDecimalToBinary(int inputDec)
        {
            string binary = Convert.ToString(inputDec, 2);
            switch (binary.Length)
            {
                case 1:
                    binary = "0000000" + binary;
                    break;
                case 2:
                    binary = "000000" + binary;
                    break;
                case 3:
                    binary = "00000" + binary;
                    break;
                case 4:
                    binary = "0000" + binary;
                    break;
                case 5:
                    binary = "000" + binary;
                    break;
                case 6:
                    binary = "00" + binary;
                    break;
                case 7:
                    binary = "0" + binary;
                    break;
            }
            return binary;
        }

        // Convert Hex to Decimal
        public ulong ConvertHexToDecimal(string hexValue)
        {
            ulong number = UInt64.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            return number;
        }

        public JObject ConvertStringToJson(string rawData)
        {
            return JObject.Parse(rawData);
        }
    }
}
