// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConflictResourceTypeJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ConflictResourceTypeJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Type type = (Type) value;
      string str;
      if (type == typeof (CosmosElement))
        str = "document";
      else if (type == typeof (StoredProcedureProperties))
        str = "storedProcedure";
      else if (type == typeof (TriggerProperties))
      {
        str = "trigger";
      }
      else
      {
        if (!(type == typeof (UserDefinedFunctionProperties)))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported resource type {0}", (object) value.ToString()));
        str = "userDefinedFunction";
      }
      writer.WriteValue(str);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.Value == null)
        return (object) null;
      string b = reader.Value.ToString();
      if (string.Equals("document", b, StringComparison.OrdinalIgnoreCase))
        return (object) typeof (CosmosElement);
      if (string.Equals("storedProcedure", b, StringComparison.OrdinalIgnoreCase))
        return (object) typeof (StoredProcedureProperties);
      if (string.Equals("trigger", b, StringComparison.OrdinalIgnoreCase))
        return (object) typeof (TriggerProperties);
      return string.Equals("userDefinedFunction", b, StringComparison.OrdinalIgnoreCase) ? (object) typeof (UserDefinedFunctionProperties) : (object) null;
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType) => true;
  }
}
