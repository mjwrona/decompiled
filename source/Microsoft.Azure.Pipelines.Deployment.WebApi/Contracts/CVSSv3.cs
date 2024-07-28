// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.CVSSv3
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts
{
  [DataContract]
  public class CVSSv3
  {
    [DataMember(EmitDefaultValue = false)]
    public float BaseScore { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public float ExploitabilityScore { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public float ImpactScore { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AttackVector AttackVector { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AttackComplexity AttackComplexity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PrivilegesRequired PrivilegesRequired { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public UserInteraction UserInteraction { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Scope Scope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Impact ConfidentialityImpact { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Impact IntegrityImpact { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Impact AvailabilityImpact { get; set; }
  }
}
