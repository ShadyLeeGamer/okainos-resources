using System.Collections.Generic;
using FullSerializer;

public static class Serialiser
{
    static fsSerializer serializer = new fsSerializer();

    public static T DeserialiseDataFromJson<T>(string jsonText)
    {
        serializer = new fsSerializer();
        fsData data = fsJsonParser.Parse(jsonText);
        T dataInfo = default;
        serializer.TryDeserialize(data, ref dataInfo);
        return dataInfo;
    }

    public static Dictionary<string, T> DeserialiseArrayFromJson<T>(string jsonText)
    {
        serializer = new fsSerializer();
        fsData data = fsJsonParser.Parse(jsonText);
        Dictionary<string, T> datas = null;
        serializer.TryDeserialize(data, ref datas);
        return datas;
    }
}