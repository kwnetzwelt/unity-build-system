using System.Xml.Serialization;
using System.IO;

namespace UBS
{
	internal class Serializer
	{
		public static string Save(object obj)
		{
			XmlSerializer serializer = new XmlSerializer(obj.GetType());
			
			using (StringWriter writer = new StringWriter())
			{
				serializer.Serialize(writer, obj);
				return writer.ToString();
			}
		}
		
		public static T Load<T>(string content) where T : class
		{		
			XmlSerializer deserializer = new XmlSerializer(typeof(T));
			
			
			using(StringReader reader = new StringReader(content))
			{
				return deserializer.Deserialize (reader) as T;
			}
		}

	}
}