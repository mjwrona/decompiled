// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration.ReleaseEnvironmentDeployPhasesSnapshotData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration
{
  public class ReleaseEnvironmentDeployPhasesSnapshotData
  {
    public int ReleaseId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public IList<DeployPhaseSnapshot> DeployPhaseSnapshots { get; private set; }

    public int DeployPhaseSnapshotRevision { get; set; }

    public ReleaseEnvironmentDeployPhasesSnapshotData() => this.DeployPhaseSnapshots = (IList<DeployPhaseSnapshot>) new List<DeployPhaseSnapshot>();
  }
}
