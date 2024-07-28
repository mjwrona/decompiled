// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent9
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent9 : DeploymentSqlComponent8
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override DeploymentApiBinder GetDeploymentApiBinder() => (DeploymentApiBinder) new DeploymentApiBinder3((ReleaseManagementSqlResourceComponentBase) this);

    public override ReleaseEnvironmentSnapshotDelta AddReleaseEnvironmentSnapshotDelta(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDelta,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseEnvironmentSnapshotDelta", projectId);
      string parameterValue = JsonConvert.SerializeObject((object) deploymentGroupPhaseDelta);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (deploymentId), deploymentId);
      this.BindString(nameof (deploymentGroupPhaseDelta), parameterValue, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentSnapshotDelta>((ObjectBinder<ReleaseEnvironmentSnapshotDelta>) this.GetReleaseEnvironmentSnapshotDeltaBinder());
        return resultCollection.GetCurrent<ReleaseEnvironmentSnapshotDelta>().Items.FirstOrDefault<ReleaseEnvironmentSnapshotDelta>();
      }
    }

    public override ReleaseEnvironmentSnapshotDelta GetReleaseEnvironmentSnapshotDelta(
      Guid projectId,
      int releaseId,
      int deploymentId)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseEnvironmentSnapshotDelta", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (deploymentId), deploymentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentSnapshotDelta>((ObjectBinder<ReleaseEnvironmentSnapshotDelta>) this.GetReleaseEnvironmentSnapshotDeltaBinder());
        return resultCollection.GetCurrent<ReleaseEnvironmentSnapshotDelta>().Items.FirstOrDefault<ReleaseEnvironmentSnapshotDelta>();
      }
    }

    public override IList<DeploymentIssue> GetDeploymentIssues(
      Guid projectId,
      int releaseId,
      int deploymentId)
    {
      this.PrepareStoredProcedure("Release.prc_GetDeploymentIssues", projectId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@deploymentId", deploymentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentIssue>((ObjectBinder<DeploymentIssue>) this.GetDeploymentIssueBinder());
        return (IList<DeploymentIssue>) resultCollection.GetCurrent<DeploymentIssue>().Items;
      }
    }

    public override IList<DeploymentIssue> AddDeploymentIssues(
      Guid projectId,
      int releaseId,
      int deploymentId,
      IList<Issue> issues)
    {
      this.PrepareStoredProcedure("Release.prc_AddDeploymentIssues", projectId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@deploymentId", deploymentId);
      this.BindDeploymentIssuesTable("@issues", (IEnumerable<Issue>) issues);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentIssue>((ObjectBinder<DeploymentIssue>) this.GetDeploymentIssueBinder());
        return (IList<DeploymentIssue>) resultCollection.GetCurrent<DeploymentIssue>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This may need to be overriden in future")]
    protected virtual DeploymentIssueBinder GetDeploymentIssueBinder() => new DeploymentIssueBinder();
  }
}
