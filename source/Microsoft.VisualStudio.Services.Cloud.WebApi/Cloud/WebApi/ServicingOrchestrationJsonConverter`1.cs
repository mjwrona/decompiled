// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationJsonConverter`1
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  public class ServicingOrchestrationJsonConverter<T> : VssSecureJsonConverter
  {
    private static readonly Type[] s_areaTypes = new Type[3]
    {
      typeof (FrameworkDataImportRequest),
      typeof (FrameworkReparentCollectionRequest),
      typeof (FrameworkNewDomainUrlMigrationRequest)
    };
    private static readonly Dictionary<string, Type> s_requestTypes = new Dictionary<string, Type>();

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (T).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jObject = JObject.Load(reader);
      T target = this.Create(objectType, jObject);
      serializer.Populate(jObject.CreateReader(), (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    private T Create(Type objectType, JObject jObject)
    {
      JToken key;
      if (jObject.TryGetValue("TypeName", StringComparison.OrdinalIgnoreCase, out key))
      {
        Type type;
        if (!ServicingOrchestrationJsonConverter<T>.s_requestTypes.TryGetValue((string) key, out type))
          throw new InvalidOperationException(string.Format("Type is not registered: '{0}'", (object) key));
        objectType = type;
      }
      return (T) Activator.CreateInstance(objectType);
    }

    static ServicingOrchestrationJsonConverter()
    {
      foreach (MemberInfo areaType in ServicingOrchestrationJsonConverter<T>.s_areaTypes)
      {
        foreach (XmlIncludeAttribute customAttribute in areaType.GetCustomAttributes(typeof (XmlIncludeAttribute), false))
          ServicingOrchestrationJsonConverter<T>.s_requestTypes.Add(customAttribute.Type.FullName, customAttribute.Type);
      }
    }
  }
}
