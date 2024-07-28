// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.SharedParameterJsonConverter
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  internal class SharedParameterJsonConverter : JsonConverter
  {
    private static readonly string c_localParamDefFullName = typeof (LocalParameterDefinition).FullName;
    private static readonly string c_sharedParamDefFullName = typeof (SharedParameterDefinition).FullName;

    public override bool CanConvert(Type objectType) => string.Equals(objectType.FullName, SharedParameterJsonConverter.c_localParamDefFullName, StringComparison.OrdinalIgnoreCase) || string.Equals(objectType.FullName, SharedParameterJsonConverter.c_sharedParamDefFullName, StringComparison.OrdinalIgnoreCase);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken = JToken.Load(reader);
      if (jtoken[(object) "sharedParameterName"] != null)
        return (object) new SharedParameterDefinition(jtoken[(object) "localParamName"].ToString(), jtoken[(object) "sharedParameterName"].ToString(), int.Parse(jtoken[(object) "sharedParameterDataSetId"].ToString()));
      return jtoken[(object) "localParamName"] != null ? (object) new LocalParameterDefinition(jtoken[(object) "localParamName"].ToString()) : (object) new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      SharedParameterDefinition sharedParamDef = value as SharedParameterDefinition;
      SharedParameterJsonConverter.WriteSharedParameterDefinitionJson(writer, serializer, sharedParamDef);
      LocalParameterDefinition localParamDef = value as LocalParameterDefinition;
      SharedParameterJsonConverter.WriteLocalParameterDefinitionJson(writer, serializer, localParamDef);
    }

    private static void WriteLocalParameterDefinitionJson(
      JsonWriter writer,
      JsonSerializer serializer,
      LocalParameterDefinition localParamDef)
    {
      if (localParamDef == null)
        return;
      writer.WriteStartObject();
      writer.WritePropertyName("localParamName");
      serializer.Serialize(writer, (object) localParamDef.LocalParamName);
      writer.WriteEndObject();
    }

    private static void WriteSharedParameterDefinitionJson(
      JsonWriter writer,
      JsonSerializer serializer,
      SharedParameterDefinition sharedParamDef)
    {
      if (sharedParamDef == null)
        return;
      writer.WriteStartObject();
      writer.WritePropertyName("localParamName");
      serializer.Serialize(writer, (object) sharedParamDef.LocalParamName);
      writer.WritePropertyName("sharedParameterName");
      serializer.Serialize(writer, (object) sharedParamDef.SharedParameterName);
      writer.WritePropertyName("sharedParameterDataSetId");
      serializer.Serialize(writer, (object) sharedParamDef.SharedParameterDataSetId);
      writer.WriteEndObject();
    }
  }
}
