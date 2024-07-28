// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.ArtifactProvenance
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class ArtifactProvenance
  {
    [DataMember]
    public string ResourceUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Occurrence Build { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Occurrence Image { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Occurrence> Deployment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Occurrence> Attestation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Occurrence> Vulnerability { get; set; }
  }
}
