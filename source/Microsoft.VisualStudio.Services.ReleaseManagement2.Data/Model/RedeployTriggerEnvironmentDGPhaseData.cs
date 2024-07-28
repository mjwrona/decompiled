// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RedeployTriggerEnvironmentDGPhaseData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [DataContract]
  public class RedeployTriggerEnvironmentDGPhaseData
  {
    public RedeployTriggerEnvironmentDGPhaseData() => this.Tags = (IList<string>) new List<string>();

    public RedeployTriggerEnvironmentDGPhaseData(
      int releaseDefinitionId,
      int environmentId,
      int deploymentGroupId,
      IList<string> tags)
    {
      this.ReleaseDefinitionId = releaseDefinitionId;
      this.EnvironmentId = environmentId;
      this.DeploymentGroupId = deploymentGroupId;
      this.Tags = tags;
    }

    [DataMember]
    public int ReleaseDefinitionId { get; set; }

    [DataMember]
    public int EnvironmentId { get; set; }

    [DataMember]
    public int DeploymentGroupId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public IList<string> Tags { get; set; }
  }
}
