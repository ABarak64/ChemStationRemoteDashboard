using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationDataProviders;
using System.Web.Script.Serialization;
using System.Net;
using System.Web;

namespace ChemStationDataConsumers
{
    /// <summary>
    /// An IChemStationDataConsumer that sends the most recent ChemStationStatus to a remote server via HTTP.
    /// </summary>
    public class ChemStationDataHTTPSender : IChemStationDataConsumer
    {
        private const string _postUrl = "http://whatuphplc.pythonanywhere.com/chemstationstatus";
        
        /// <summary>
        /// Posts the ChemStationStatus (serialized in JSON) to a particular Url.
        /// </summary>
        /// <param name="status"></param>
        public void ConsumeChemStationStatus(ChemStationStatus status)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new ChemStationStatusConverter() });
            var js = serializer.Serialize(status);
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(_postUrl, "json=" + HttpUtility.UrlEncode(js));
            }
        }

        /// <summary>
        /// A nested class in order to serialize DateTimes into JSON properly.
        /// </summary>
        private class ChemStationStatusConverter : JavaScriptConverter
        {
            private const string _dateFormat = "G";

            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return new[] { typeof(ChemStationStatus) };
                }
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                return new ChemStationStatus();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                ChemStationStatus p = (ChemStationStatus)obj;
                IDictionary<string, object> serialized = new Dictionary<string, object>();
                serialized["Status"] = p.Status;
                serialized["SequenceName"] = p.SequenceName;
                serialized["MethodName"] = p.MethodName;
                serialized["SequenceRunning"] = p.SequenceRunning;
                serialized["MethodRunning"] = p.MethodRunning;
                serialized["Time"] = p.Time.ToString(_dateFormat);
                return serialized;
            }
        }
    }
}
