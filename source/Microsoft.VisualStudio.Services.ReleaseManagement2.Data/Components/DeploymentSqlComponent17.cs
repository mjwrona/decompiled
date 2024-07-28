// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent17
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class DeploymentSqlComponent17 : DeploymentSqlComponent16
  {
    public override ReleaseEnvironmentSnapshotDelta AddReleaseEnvironmentSnapshotDelta(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDelta,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseEnvironmentSnapshotDelta", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (deploymentId), deploymentId);
      this.BindString(nameof (deploymentGroupPhaseDelta), ServerModelUtility.ToString((object) deploymentGroupPhaseDelta), 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (variables), ServerModelUtility.ToString((object) VariablesUtility.ReplaceSecretVariablesWithNull(variables)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentSnapshotDelta>((ObjectBinder<ReleaseEnvironmentSnapshotDelta>) this.GetReleaseEnvironmentSnapshotDeltaBinder());
        return resultCollection.GetCurrent<ReleaseEnvironmentSnapshotDelta>().Items.FirstOrDefault<ReleaseEnvironmentSnapshotDelta>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected override ReleaseEnvironmentSnapshotDeltaBinder GetReleaseEnvironmentSnapshotDeltaBinder() => (ReleaseEnvironmentSnapshotDeltaBinder) new ReleaseEnvironmentSnapshotDeltaBinder2();
  }
}
