// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentSnapshotDeltaBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentSnapshotDeltaBinder : ObjectBinder<ReleaseEnvironmentSnapshotDelta>
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder deploymentId = new SqlColumnBinder("DeploymentId");
    private SqlColumnBinder deploymentGroupPhaseDelta = new SqlColumnBinder("DeploymentGroupPhaseDelta");

    protected override ReleaseEnvironmentSnapshotDelta Bind()
    {
      string str = this.deploymentGroupPhaseDelta.GetString((IDataReader) this.Reader, true);
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDeltas = (IEnumerable<DeploymentGroupPhaseDelta>) new List<DeploymentGroupPhaseDelta>();
      if (str != null)
        deploymentGroupPhaseDeltas = ServerModelUtility.FromString<IEnumerable<DeploymentGroupPhaseDelta>>(str);
      return new ReleaseEnvironmentSnapshotDelta()
      {
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        DeploymentId = this.deploymentId.GetInt32((IDataReader) this.Reader),
        DeploymentGroupPhaseDelta = deploymentGroupPhaseDeltas
      };
    }
  }
}
