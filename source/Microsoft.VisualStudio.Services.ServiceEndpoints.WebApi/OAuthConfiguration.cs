// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.OAuthConfiguration
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class OAuthConfiguration
  {
    public OAuthConfiguration()
    {
    }

    public OAuthConfiguration(OAuthConfiguration config)
    {
      this.Id = config.Id;
      this.Name = config.Name;
      this.Url = config.Url;
      this.EndpointType = config.EndpointType;
      this.ClientId = config.ClientId;
      this.ClientSecret = config.ClientSecret;
    }

    [DataMember]
    public Guid Id { get; set; }

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

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; internal set; }
  }
}
