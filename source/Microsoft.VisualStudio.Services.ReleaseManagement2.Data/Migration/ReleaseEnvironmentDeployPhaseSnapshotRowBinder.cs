// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration.ReleaseEnvironmentDeployPhaseSnapshotRowBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "VSSF term.")]
  public class ReleaseEnvironmentDeployPhaseSnapshotRowBinder : 
    ObjectBinder<ReleaseEnvironmentDeployPhasesSnapshotData>
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder deployPhaseSnapshots = new SqlColumnBinder("DeployPhaseSnapshots");
    private SqlColumnBinder deployphaseSnapshotRevision = new SqlColumnBinder("DeployPhaseSnapshotRevision");

    protected override ReleaseEnvironmentDeployPhasesSnapshotData Bind()
    {
      ReleaseEnvironmentDeployPhasesSnapshotData phasesSnapshotData = new ReleaseEnvironmentDeployPhasesSnapshotData()
      {
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        DeployPhaseSnapshotRevision = this.deployphaseSnapshotRevision.GetInt32((IDataReader) this.Reader)
      };
      string str = this.deployPhaseSnapshots.GetString((IDataReader) this.Reader, true);
      if ((str != null ? JsonConvert.DeserializeObject<IDeploymentSnapshot>(str) : (IDeploymentSnapshot) new DesignerDeploymentSnapshot()) is DesignerDeploymentSnapshot deploymentSnapshot)
      {
        ReleaseEnvironmentDeployPhaseSnapshotRowBinder.NormalizeDeployPhaseSnapshots(deploymentSnapshot.DeployPhaseSnapshots);
        phasesSnapshotData.DeployPhaseSnapshots.AddRange<DeployPhaseSnapshot, IList<DeployPhaseSnapshot>>((IEnumerable<DeployPhaseSnapshot>) deploymentSnapshot.DeployPhaseSnapshots);
      }
      return phasesSnapshotData;
    }

    private static void NormalizeDeployPhaseSnapshots(IList<DeployPhaseSnapshot> snapshots)
    {
      foreach (DeployPhaseSnapshot snapshot in (IEnumerable<DeployPhaseSnapshot>) snapshots)
      {
        if (snapshot.PhaseType == DeployPhaseTypes.Undefined)
        {
          snapshot.PhaseType = DeployPhaseTypes.AgentBasedDeployment;
          snapshot.Name = snapshot.Name ?? Resources.AgentBasedDeploymentDefaultName;
          snapshot.Rank = snapshot.Rank == 0 ? 1 : snapshot.Rank;
        }
        if (snapshot.PhaseType == DeployPhaseTypes.RunOnServer && snapshot.DeploymentInput == null)
          snapshot.DeploymentInput = JObject.FromObject((object) new ServerDeploymentInput()
          {
            ParallelExecution = (ExecutionInput) new NoneExecutionInput()
          });
      }
    }
  }
}
