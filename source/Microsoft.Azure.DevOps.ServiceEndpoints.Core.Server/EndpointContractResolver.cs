// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server.EndpointContractResolver
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7318EB94-86FC-4B6F-8A5A-8BD0659030A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server.dll

using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server
{
  public class EndpointContractResolver : DefaultContractResolver
  {
    private Dictionary<string, string> PropertyMappings { get; set; }

    public EndpointContractResolver() => this.PropertyMappings = new Dictionary<string, string>()
    {
      {
        "Scheme",
        "type"
      },
      {
        "AuthorizationHeaders",
        "headers"
      },
      {
        "EndpointUrl",
        "url"
      }
    };

    protected override string ResolvePropertyName(string propertyName)
    {
      string str;
      return !this.PropertyMappings.TryGetValue(propertyName, out str) ? base.ResolvePropertyName(propertyName) : str;
    }
  }
}
