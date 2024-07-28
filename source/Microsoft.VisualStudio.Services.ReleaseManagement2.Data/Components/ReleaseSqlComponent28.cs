// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent28
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent28 : ReleaseSqlComponent27
  {
    public override IEnumerable<AutoTriggerIssue> AddAutoTriggerIssues(
      IEnumerable<AutoTriggerIssue> autoTriggerIssuesList)
    {
      this.PrepareStoredProcedure("Release.prc_AddAutoTriggerIssues");
      this.BindAutoTriggerIssuesTable2("autoTriggerIssues", autoTriggerIssuesList);
      return this.GetAutoTriggerIssuesObject();
    }

    public override IEnumerable<AutoTriggerIssue> GetAutoTriggerIssues(
      Guid projectId,
      string artifactType,
      string sourceId,
      string artifactVersionId)
    {
      this.PrepareStoredProcedure("Release.prc_GetAutoTriggerIssues");
      this.BindString(nameof (artifactType), artifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (sourceId), sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetAutoTriggerIssuesObject();
    }

    protected override AutoTriggerIssueBinder GetAutoTriggerIssuesBinder() => (AutoTriggerIssueBinder) new AutoTriggerIssueBinder2((ReleaseManagementSqlResourceComponentBase) this);
  }
}
