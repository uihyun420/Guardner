using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Vector2Converter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(Vector2);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return Vector2.zero;

        try
        {
            JObject jObject = JObject.Load(reader);
            return new Vector2(
                jObject["X"] != null ? jObject["X"].Value<float>() : 0f,
                jObject["Y"] != null ? jObject["Y"].Value<float>() : 0f
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Vector2 읽기 오류: {ex.Message}");
            return Vector2.zero;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        try
        {
            Vector2 vector = (Vector2)value;
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("Y");
            writer.WriteValue(vector.y);
            writer.WriteEndObject();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Vector2 쓰기 오류: {ex.Message}");
        }
    }
}

// GuardnerSaveData 컨버터
public class GuardnerSaveDataConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(Dictionary<string, GuardnerSaveData>);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return new Dictionary<string, GuardnerSaveData>();

        Dictionary<string, GuardnerSaveData> result = existingValue as Dictionary<string, GuardnerSaveData> ?? new Dictionary<string, GuardnerSaveData>();

        try
        {
            JObject jObject = JObject.Load(reader);
            foreach (var prop in jObject.Properties())
            {
                string guardnerId = prop.Name;
                GuardnerSaveData data = prop.Value.ToObject<GuardnerSaveData>(serializer);
                result[guardnerId] = data;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GuardnerSaveData 읽기 오류: {ex.Message}");
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        try
        {
            var dictionary = value as Dictionary<string, GuardnerSaveData>;
            writer.WriteStartObject();

            if (dictionary != null)
            {
                foreach (var pair in dictionary)
                {
                    writer.WritePropertyName(pair.Key);
                    serializer.Serialize(writer, pair.Value);
                }
            }

            writer.WriteEndObject();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GuardnerSaveData 쓰기 오류: {ex.Message}");
        }
    }
}