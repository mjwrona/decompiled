// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeploymentGroupPhaseDelta
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  [DataContract]
  public class DeploymentGroupPhaseDelta
  {
    [DataMember]
    public int DeploymentGroupId { get; set; }

    [DataMember]
    public int? DeployPhaseRank { get; set; }

    [DataMember]
    public string DeployPhaseName { get; set; }

    [DataMember]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed")]
    public IEnumerable<int> DeploymentTargetIds { get; set; }

    [DataMember]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed")]
    public IEnumerable<string> DeploymentGroupTags { get; set; }
  }
}
