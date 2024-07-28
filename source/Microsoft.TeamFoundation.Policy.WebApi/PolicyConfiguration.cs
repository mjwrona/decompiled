// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyConfiguration
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [DataContract]
  public class PolicyConfiguration : VersionedPolicyConfigurationRef
  {
    public PolicyConfiguration()
    {
    }

    public PolicyConfiguration(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = true)]
    public bool IsEnabled { get; set; }

    [DataMember(IsRequired = true)]
    public bool IsBlocking { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember(IsRequired = true)]
    [JsonProperty(Required = Required.Always)]
    public JObject Settings { get; set; }

    [DataMember(IsRequired = false)]
    public bool IsEnterpriseManaged { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
