// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.OAuthConfigurationParams
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class OAuthConfigurationParams
  {
    public OAuthConfigurationParams()
    {
    }

    public OAuthConfigurationParams(OAuthConfigurationParams configurationParams)
    {
      this.Name = configurationParams.Name;
      this.ClientId = configurationParams.ClientId;
      this.ClientSecret = configurationParams.ClientSecret;
      this.Url = configurationParams.Url;
      this.EndpointType = configurationParams.EndpointType;
    }

    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = true)]
    public string EndpointType { get; set; }

    [DataMember(IsRequired = true)]
    public Uri Url { get; set; }

    [DataMember(IsRequired = true)]
    public string ClientId { get; set; }

    [DataMember(IsRequired = true)]
    public string ClientSecret { get; set; }
  }
}
