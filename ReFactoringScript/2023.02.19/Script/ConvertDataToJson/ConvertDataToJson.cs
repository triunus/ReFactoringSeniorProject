using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public interface IConverterJsonType
{
    public string ConvertDataToJsonString<T>(string name, ref T data);
    public string MergeJObjectAndJObject(List<string> modelData);
}

public class ConverterJsonType : IConverterJsonType
{
    private JObject JObjectData = null;
    public string ConvertDataToJsonString<T>(string name, ref T data)
    {
        JObjectData = null;
        JObjectData = new JObject(new JProperty(name, data));
        return JsonConvert.SerializeObject(JObjectData, Formatting.Indented);
    }

    public string MergeJObjectAndJObject(List<string> modelData)
    {
        JObjectData = JObject.Parse(modelData[0]);

        for(int i = 1; i < modelData.Count; i++)
        {
            JObjectData.Merge(JObject.Parse(modelData[i]));
        }

        return JsonConvert.SerializeObject(JObjectData, Formatting.Indented);
    }
}
