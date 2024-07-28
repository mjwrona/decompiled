// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent26
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent26 : ReleaseSqlComponent25
  {
    public override IEnumerable<Release> ResetScheduledReleaseEnvironments(
      Guid projectId,
      int releaseDefinitionId,
      IEnumerable<int> definitionEnvironmentIds,
      Guid changedBy,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_ResetScheduledReleaseEnvironments", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt32Table(nameof (definitionEnvironmentIds), definitionEnvironmentIds);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindChangeDetails(changeDetails);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObjects(projectId, true, true, true, true, false);
    }

    public override Release UpdateApprovalStepsStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStepType stepType,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_BulkUpdate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindByte(nameof (stepType), (byte) stepType);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual AutoTriggerIssueBinder GetAutoTriggerIssuesBinder() => new AutoTriggerIssueBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual IEnumerable<AutoTriggerIssue> GetAutoTriggerIssuesObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AutoTriggerIssue>((ObjectBinder<AutoTriggerIssue>) this.GetAutoTriggerIssuesBinder());
        return (IEnumerable<AutoTriggerIssue>) resultCollection.GetCurrent<AutoTriggerIssue>().Items;
      }
    }

    public override IEnumerable<AutoTriggerIssue> GetAutoTriggerIssues(
      Guid projectId,
      string artifactType,
      string sourceId,
      string artifactVersionId)
    {
      this.PrepareStoredProcedure("Release.prc_GetAutoTriggerIssues", projectId);
      int result;
      if (!int.TryParse(artifactVersionId, out result))
        return (IEnumerable<AutoTriggerIssue>) new List<AutoTriggerIssue>();
      this.BindInt("buildId", result);
      return this.GetAutoTriggerIssuesObject();
    }

    public override IEnumerable<AutoTriggerIssue> AddAutoTriggerIssues(
      IEnumerable<AutoTriggerIssue> autoTriggerIssuesList)
    {
      ContinuousDeploymentTriggerIssue deploymentTriggerIssue = autoTriggerIssuesList.FirstOrDefault<AutoTriggerIssue>() as ContinuousDeploymentTriggerIssue;
      string artifactVersionId = deploymentTriggerIssue.ArtifactVersionId;
      this.PrepareStoredProcedure("Release.prc_AddAutoTriggerIssues", deploymentTriggerIssue.ProjectId);
      this.BindInt("buildId", int.Parse(artifactVersionId, (IFormatProvider) CultureInfo.InvariantCulture));
      this.BindAutoTriggerIssuesTable("autoTriggerIssues", autoTriggerIssuesList);
      return this.GetAutoTriggerIssuesObject();
    }
  }
}
