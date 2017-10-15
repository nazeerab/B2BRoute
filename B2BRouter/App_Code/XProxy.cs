using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace B2BRouter.App_Code
{
    [XmlRoot(ElementName = "uri")]
    public class XProxy
    {
        [XmlElement(ElementName = "in")]
        public string In { get; set; }
        [XmlElement(ElementName = "out")]
        public string Out { get; set; }
        [XmlElement(ElementName = "CertHeader")]
        public string CertHeader { get; set; }
        [XmlElement(ElementName = "TimeOut")]
        public string TimeOut { get; set; }
        [XmlElement(ElementName = "ContentType")]
        public string ContentType { get; set; }
        [XmlElement(ElementName = "HttpMethod")]
        public string HttpMethod { get; set; }
        [XmlElement(ElementName = "HTTPUserAgent")]
        public string HTTPUserAgent { get; set; }
       [XmlElement(ElementName = "HTTPKeepAlive")]
        public bool HTTPKeepAlive { get; set; }
        [XmlElement(ElementName = "Instance")]
        public string Instance { get; set; }

    }

    [XmlRoot(ElementName = "mapper")]
    public class Mapper
    {
     
        [XmlElement(ElementName = "uri")]
        public List<XProxy> XProxy { get; set; }

      
    }
}