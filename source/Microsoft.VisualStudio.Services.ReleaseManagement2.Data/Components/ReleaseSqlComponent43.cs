// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent43
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseSqlComponent43 : ReleaseSqlComponent42
  {
    public override Release CancelDeploymentOnEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid changedBy,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool evaluateForCanceling)
    {
      this.PrepareForAuditingAction(ReleaseAuditConstants.DeploymentCompleted, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_CancelDeployment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean(nameof (addCommentAsDeploymentIssue), addCommentAsDeploymentIssue);
      this.BindBoolean("evaluateForCancelling", evaluateForCanceling);
      return this.GetReleaseObject(projectId);
    }
  }
}
