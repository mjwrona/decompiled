// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.WebApi.CreatePipelineConnectionInputs
// Assembly: Microsoft.TeamFoundation.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29F2A1B3-A3F7-4291-91FA-6C4508EECB65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.WebApi
{
  [DataContract]
  public class CreatePipelineConnectionInputs
  {
    [DataMember(IsRequired = true)]
    public string ProviderId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RedirectUrl { get; set; }

    [DataMember]
    public string RequestSource { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProject Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<string, string> ProviderData { get; set; }
  }
}
