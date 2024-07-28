// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DesignerDeploymentSnapshot
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DesignerDeploymentSnapshot : IDeploymentSnapshot
  {
    [JsonConstructor]
    public DesignerDeploymentSnapshot() => this.DeployPhaseSnapshots = (IList<DeployPhaseSnapshot>) new List<DeployPhaseSnapshot>();

    private DesignerDeploymentSnapshot(DesignerDeploymentSnapshot snapshot) => this.DeployPhaseSnapshots = (IList<DeployPhaseSnapshot>) new List<DeployPhaseSnapshot>(snapshot.DeployPhaseSnapshots.Select<DeployPhaseSnapshot, DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, DeployPhaseSnapshot>) (x => x.DeepClone())));

    public IList<DeployPhaseSnapshot> DeployPhaseSnapshots { get; }

    public string SnapshotType => "Designer";

    public IDeploymentSnapshot DeepClone() => (IDeploymentSnapshot) new DesignerDeploymentSnapshot(this);

    public string GetPhaseName(int phaseRank, DeployPhaseTypes phaseType)
    {
      string phaseName = (string) null;
      DeployPhaseSnapshot deployPhaseSnapshot = this.DeployPhaseSnapshots.FirstOrDefault<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (p => p.Rank == phaseRank));
      if (deployPhaseSnapshot != null && deployPhaseSnapshot.PhaseType != DeployPhaseTypes.Undefined)
        phaseName = deployPhaseSnapshot.Name;
      if (string.IsNullOrWhiteSpace(phaseName))
        phaseName = ServerModelUtility.GetDefaultPhaseName(phaseType);
      return phaseName;
    }
  }
}
