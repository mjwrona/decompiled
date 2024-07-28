// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GraphQLSerializationBinder
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GraphQLSerializationBinder : ISerializationBinder
  {
    private readonly IDictionary<string, Type> s_typeMap;

    public GraphQLSerializationBinder(IDictionary<string, Type> typeMap) => this.s_typeMap = typeMap;

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      assemblyName = (string) null;
      typeName = serializedType.Name;
    }

    public Type BindToType(string assemblyName, string typeName)
    {
      Type type;
      this.s_typeMap.TryGetValue(typeName, out type);
      return type;
    }
  }
}
