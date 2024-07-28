// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GraphQLConverter
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GraphQLConverter
  {
    public GraphQLConverter(GraphQLTypeInspector inspector)
      : this((IDictionary<string, Type>) inspector.TypeInfoMap.Keys.ToDictionary<Type, string, Type>((Func<Type, string>) (t => t.Name), (Func<Type, Type>) (t => t), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
    {
    }

    public GraphQLConverter(IDictionary<string, Type> typeMap)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.TypeNameHandling = TypeNameHandling.Auto;
      settings.SerializationBinder = (ISerializationBinder) new GraphQLSerializationBinder(typeMap);
      settings.Converters.Add((JsonConverter) new StringEnumConverter());
      this.Serializer = JsonSerializer.CreateDefault(settings);
    }

    public JsonSerializer Serializer { get; }

    public T DeserializeObject<T>(string value)
    {
      using (JsonReader reader = (JsonReader) new GraphQLJsonTextReader((TextReader) new StringReader(value)))
        return this.Serializer.Deserialize<T>(reader);
    }
  }
}
