// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Directories
{
  internal class DirectoryEntityJsonConverter : VssSecureJsonConverter
  {
    private const string EntityTypePropertyName = "entityType";

    public override bool CanConvert(Type objectType) => objectType == typeof (IDirectoryEntity);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken = JToken.ReadFrom(reader);
      if (jtoken.Type == JTokenType.Null)
        return (object) null;
      string str = jtoken.Value<string>((object) "entityType");
      switch (str)
      {
        case "User":
          return (object) jtoken.ToObject<DirectoryUser>();
        case "Group":
          return (object) jtoken.ToObject<DirectoryGroup>();
        case "ServicePrincipal":
          return (object) jtoken.ToObject<DirectoryServicePrincipal>();
        default:
          throw new DirectoryEntityTypeException("Cannot create IDirectoryEntity with entity type '" + str + "'");
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      serializer.Serialize(writer, value);
    }
  }
}
