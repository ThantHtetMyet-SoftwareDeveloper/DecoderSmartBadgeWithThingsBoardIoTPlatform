using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DecodeSmartBadgeData
{
    class Index
    {
        static void Main(string[] args)
        {
            //MqttClient client = new MqttClient("192.168.6.121", 8883, false, null, null, 0, null, null);
            MqttClient client = new MqttClient("broker.hivemq.com", 1883, false, null, null, 0, null, null);

            //client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            //client.MqttMsgSubscribed += client_MqttMsgSubscribed;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            string clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);

            ushort msgId = client.Subscribe(new string[] { "mqtt/things/+/uplink" },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;
        }

        // this code runs when a message was received
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            JObject jObj = JObject.Parse(ReceivedMessage);

            Console.WriteLine(jObj);

            /*
            if(jObj.ContainsKey("DevEUI_uplink"))
            {
                dynamic json = JsonConvert.DeserializeObject(ReceivedMessage);

                Console.WriteLine("--- Received Message ---");
                Console.WriteLine(Convert.ToString(json));

                data dataResult = MainProgram(Convert.ToString(json));

                var returnObj = JsonConvert.SerializeObject(dataResult);

                //returnObj = "\""+ returnObj.Replace("\"", "\"\"") + "\"";
                //JObject jsonObj = JObject.Parse(returnObj);
                Console.WriteLine("--- Return Obj ---");
                Console.WriteLine(returnObj);

                MqttClient client = new MqttClient("192.168.6.121", 8883, false, null, null, 0, null, null);
                client.MqttMsgPublished += client_MqttMsgPublished;
                // use a unique id as client id, each time we start the application
                string clientId = Guid.NewGuid().ToString();

                client.Connect(clientId);

                ushort msgId1 = client.Publish("mqtt/things/decoded/uplink", // topic
                    Encoding.UTF8.GetBytes(returnObj), // message body
                    MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                    false); // retained 
            }
            */
        }
        static void client_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
        }

        public static data MainProgram(string receivedMessage)
        {
            // GPS Fix = 03184d82000e02040175937263010201
            // GPS Timeout Payload = 03184d82010e02040175
            // BLE Beacon scan with MAC Address = 03184d82070ece3bd0107f02e7fc40250f7937e1d567abf51641d8b0b448fc6376d2
            // BLE Beacon scan with Short Identifier = 03184d820A0ece3bd0107f02e7fc40250f7937e1d567abf51641d8b0b448fc6376d2
            // BLE Beacon scan with Long Identifier = 03184d820B0ece3bd0107f02e7fc40250f7937e1d567ab
            DecodeHeader businessData = new DecodeHeader();
            JObject inputJson = businessData.ConvertStringToJson(receivedMessage);
            string payloadHex = inputJson["DevEUI_uplink"]["payload_hex"].ToString();
            List<String> hexResult = businessData.convertHexOneByteArr(payloadHex);

            payloadData encodedObj = businessData.setEncodedHexDataObj(hexResult);

            //Console.WriteLine("Encoded HEX Obj");
            //Console.WriteLine(encodedObj);

            payloadData decodedObj = new payloadData();
            decodedObj.Type = businessData.GetType(encodedObj.Type);
            decodedObj.Status = businessData.GetStatus(encodedObj.Status);
            decodedObj.Battery = businessData.GetBattery(encodedObj.Battery);
            decodedObj.Temperature = businessData.GetTemperature(encodedObj.Temperature).ToString();

            decodedObj.Ack = "" + Convert.ToInt32(encodedObj.Ack, 2).ToString();
            decodedObj.Opt = "" + Convert.ToInt32(encodedObj.Opt.ToString(), 2).ToString();

            DecodeData messageObj = new DecodeData();
            messageObj.encodedData = encodedObj;
            messageObj.decodedData = decodedObj;
            decodedObj.Data = messageObj.GenerateData();

            Console.WriteLine("--- Decoded Object ---");
            dynamic tempObj = JsonConvert.DeserializeObject(decodedObj.Data);
            ConvertedData payloadConvertedObj = new ConvertedData();
            data outputData = new data();

            payloadConvertedObj.Type = decodedObj.Type;
            payloadConvertedObj.Status = decodedObj.Status;
            payloadConvertedObj.Battery = decodedObj.Battery;
            payloadConvertedObj.Temperature = decodedObj.Temperature;
            payloadConvertedObj.Ack = decodedObj.Ack;
            payloadConvertedObj.Opt = decodedObj.Opt;
            payloadConvertedObj.MessageType = tempObj["MessageType"];

            if (tempObj["MessageType"] == "GpsFix")
            {
                Console.WriteLine("GPS Fix");
                payloadConvertedObj.Age = tempObj["Age"];
                payloadConvertedObj.Latitude = tempObj["Latitude"];
                payloadConvertedObj.Longitude = tempObj["Longitude"];
                payloadConvertedObj.EHPE = tempObj["EHPE"];
                payloadConvertedObj.Encrypted = tempObj["Encrypted"];

                outputData.latitude = tempObj["Latitude"];
                outputData.longitude = tempObj["Longitude"];
            }
            else if (tempObj["MessageType"] == "GpsFix")
            {
                Console.WriteLine("GPS Timeout PayLoad");
                payloadConvertedObj.Cause = tempObj["Cause"];
                payloadConvertedObj.CnZero = tempObj["CnZero"];
                payloadConvertedObj.CnOne = tempObj["CnOne"];
                payloadConvertedObj.CnTwo = tempObj["CnTwo"];
                payloadConvertedObj.CnThree = tempObj["CnThree"];
            }
            else if (tempObj["MessageType"] == "BeaconScanWithMacAddress")
            {
                Console.WriteLine("Beacon Scan With MAC ");
                payloadConvertedObj.Age = tempObj["Age"];
                payloadConvertedObj.MacAddr = tempObj["MacAddr"];
                payloadConvertedObj.Rssi = tempObj["Rssi"];
            }
            else if (tempObj["MessageType"] == "BeaconScanWithShortIdentifier")
            {
                Console.WriteLine("BeaconScanWithShortIdentifier");
                payloadConvertedObj.Age = tempObj["Age"];
                payloadConvertedObj.UUid = tempObj["UUid"];
                payloadConvertedObj.Rssi = tempObj["Rssi"];
            }
            else if (tempObj["MessageType"] == "BeaconScanWithLongIdentifier")
            {
                Console.WriteLine("BeaconScanWithLongIdentifier");
                payloadConvertedObj.Age = tempObj["Age"];
                payloadConvertedObj.UUid = tempObj["UUid"];
                payloadConvertedObj.Rssi = tempObj["Rssi"];
            }
            
            outputData.MessageType = messageObj.GetMessageType(Convert.ToInt32(decodedObj.Opt));

            if (outputData.MessageType == "GPS")
            {
                outputData.BeaconId = "Null";
            }
            else if(outputData.MessageType == "BEACON")
            {
                dynamic json = JsonConvert.DeserializeObject(decodedObj.Data);
                if(json["MessageType"] == "BeaconScanWithMacAddress")
                {
                    outputData.BeaconId = json["MacAddr"];
                }
                else
                {
                    outputData.BeaconId = json["UUid"];
                }
            }
            else
            {
                outputData.BeaconId = "Unknown";
            }
            
            //outputData.ConvertedData = payloadConvertedObj;
            outputData.SensorType = "SmartBedge";
            outputData.Payload_hex = payloadConvertedObj.ToString();
           
            outputData.Time = inputJson["DevEUI_uplink"]["Time"].ToString();
            outputData.DevEUI = inputJson["DevEUI_uplink"]["DevEUI"].ToString();
            outputData.ModelCfg = inputJson["DevEUI_uplink"]["ModelCfg"].ToString();

            //string stringObj = JsonConvert.SerializeObject(outputData);
            //JObject jsonObj = JObject.Parse(stringObj);
            //Console.WriteLine("--- JSON Object ---");
            //Console.WriteLine(jsonObj);
            
            return outputData;
        }
    }
}
