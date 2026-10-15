using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 只读写 Unity 向量 / 四元数的分量。
    /// 默认契约会把 <c>normalized</c> 等返回同类型的属性也写进去，形成自引用。
    /// </summary>
    public sealed class UnityVectorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2)
                   || objectType == typeof(Vector3)
                   || objectType == typeof(Vector4)
                   || objectType == typeof(Quaternion);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            switch (value)
            {
                case Vector2 v2:
                    WriteFloat(writer, "x", v2.x);
                    WriteFloat(writer, "y", v2.y);
                    break;
                case Vector3 v3:
                    WriteFloat(writer, "x", v3.x);
                    WriteFloat(writer, "y", v3.y);
                    WriteFloat(writer, "z", v3.z);
                    break;
                case Vector4 v4:
                    WriteFloat(writer, "x", v4.x);
                    WriteFloat(writer, "y", v4.y);
                    WriteFloat(writer, "z", v4.z);
                    WriteFloat(writer, "w", v4.w);
                    break;
                case Quaternion q:
                    WriteFloat(writer, "x", q.x);
                    WriteFloat(writer, "y", q.y);
                    WriteFloat(writer, "z", q.z);
                    WriteFloat(writer, "w", q.w);
                    break;
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return Default(objectType);

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException($"无法从 {reader.TokenType} 读取 {objectType.Name}");

            float x = 0f, y = 0f, z = 0f, w = 0f;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                string name = reader.Value as string;
                if (!reader.Read())
                    break;

                if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray)
                {
                    reader.Skip();
                    continue;
                }

                if (reader.TokenType == JsonToken.Null || name == null)
                    continue;

                float number = Convert.ToSingle(reader.Value);
                switch (name)
                {
                    case "x": x = number; break;
                    case "y": y = number; break;
                    case "z": z = number; break;
                    case "w": w = number; break;
                }
            }

            if (objectType == typeof(Vector2))
                return new Vector2(x, y);
            if (objectType == typeof(Vector3))
                return new Vector3(x, y, z);
            if (objectType == typeof(Vector4))
                return new Vector4(x, y, z, w);
            if (objectType == typeof(Quaternion))
                return new Quaternion(x, y, z, w);

            throw new JsonSerializationException($"不支持的类型 {objectType.Name}");
        }

        private static void WriteFloat(JsonWriter writer, string name, float value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        private static object Default(Type objectType)
        {
            if (objectType == typeof(Vector2))
                return Vector2.zero;
            if (objectType == typeof(Vector3))
                return Vector3.zero;
            if (objectType == typeof(Vector4))
                return Vector4.zero;
            if (objectType == typeof(Quaternion))
                return Quaternion.identity;
            return null;
        }
    }
}
