﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;

namespace JsonPatch.Operations.Abstractions
{
    public abstract class Operation
    {
        public JsonPointer Path { get; set; }

        public abstract void Write(JsonWriter writer);

        protected static void WriteOp(JsonWriter writer, string op)
        {
            writer.WritePropertyName("op");
            writer.WriteValue(op);
        }

        protected static void WritePath(JsonWriter writer, JsonPointer pointer)
        {
            writer.WritePropertyName("path");
            writer.WriteValue(pointer.ToString());
        }

        protected static void WriteFromPath(JsonWriter writer, JsonPointer pointer)
        {
            writer.WritePropertyName("from");
            writer.WriteValue(pointer.ToString());
        }
        protected static void WriteValue(JsonWriter writer, JToken value)
        {
            writer.WritePropertyName("value");
            value.WriteTo(writer);
        }

        public abstract void Read(JObject jOperation);
    }
}