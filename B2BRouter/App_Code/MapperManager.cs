using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace B2BRouter.App_Code
{
    public class MapperManager
    {
        private static string uriPath = System.Configuration.ConfigurationManager.AppSettings["uri"];
        public static Mapper Load()
        {
            using (var stream = System.IO.File.OpenRead(uriPath))
            {
                var serializer = new XmlSerializer(typeof(Mapper));
                return serializer.Deserialize(stream) as Mapper;
            }
        }
    }
}