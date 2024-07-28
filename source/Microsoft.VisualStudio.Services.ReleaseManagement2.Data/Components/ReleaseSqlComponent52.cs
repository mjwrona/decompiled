// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent52
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent52 : ReleaseSqlComponent51
  {
    public override Release UpdateReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      DateTime? environmentScheduledDateTime,
      Guid? stageSchedulingJobId,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindNullableDateTime("scheduledDeploymentTime", environmentScheduledDateTime);
      if (stageSchedulingJobId.HasValue && stageSchedulingJobId.Value != Guid.Empty)
        this.BindGuid("schedulingJobId", stageSchedulingJobId.Value);
      this.BindGuid(nameof (changedBy), changedBy);
      return this.GetReleaseObject(projectId);
    }
  }
}
