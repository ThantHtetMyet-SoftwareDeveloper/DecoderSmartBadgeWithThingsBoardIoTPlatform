using System;
using System.Collections.Generic;
using System.Text;

namespace DecodeSmartBadgeData.Messages
{
    class GpsFix
    {
        public string MessageType { get; set; }
        public string Age { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EHPE { get; set; }
        public string Encrypted { get; set; }

        public override string ToString()
        {
            return "Age: " + Age + ", Latitude: " + Latitude + ", Longitude: " + Longitude + ", EHPE: " + EHPE + ", Encrypte: " + Encrypted;
        }

        // Latitude
        public string convertLatitude(string hexLatitude)
        {
            uint x = Convert.ToUInt32(hexLatitude);
            uint binaryLatitude = x << 8 ;
            long decLatitude = Convert.ToInt32(binaryLatitude);
            long thresholdLatitude = ConvertHexToDecimal("0x7FFFFFFF"); // 7FFFFFFF = 2147483647
            long subMeanValue = ConvertHexToDecimal("0x100000000"); // 100000000 = 4294967296
            
            if(decLatitude > thresholdLatitude)
            {
                decLatitude = decLatitude - subMeanValue;
            }
            return (decLatitude / Math.Pow(10, 7)).ToString();
        }

        // Longitude
        public string convertLongitude(string hexLongitude)
        {
            uint x = Convert.ToUInt32(hexLongitude);
            uint binaryLongitude = x << 8;
            long decLongitude = Convert.ToInt32(binaryLongitude);
            long thresholdLongitude = ConvertHexToDecimal("0x7FFFFFFF"); // 7FFFFFFF = 2147483647
            long subMeanValue = ConvertHexToDecimal("0x100000000"); // 100000000 = 4294967296

            if (decLongitude > thresholdLongitude)
            {
                decLongitude = decLongitude - subMeanValue;
            }
            return (decLongitude / Math.Pow(10, 7)).ToString();
        }

        // EHPE 
        public string GetEHPE(string hexInput)
        {
            double result = DecodeValue(Convert.ToUInt32(hexInput),0,1000,8,0);
            return result.ToString();
        }

        public double StepSize(float lo, float hi, int nbits, int nresv)
        {
            uint x = Convert.ToUInt32(1);
            uint temp = x << nbits;
            return 1.0 / (((temp - 1) - nresv) / (hi - lo)); 
        }

        public double DecodeValue(uint value, float lo, float hi, int nbits, int nresv)
        { return ((value - nresv / 2) * StepSize(lo, hi, nbits, nresv) + lo); }


        // Convert Hex to Decimal
        public long ConvertHexToDecimal(string hexValue)
        {
            hexValue = hexValue.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out result);
            return result;
        }

    }
}
