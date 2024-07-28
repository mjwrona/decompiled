// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent50
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent50 : ReleaseSqlComponent49
  {
    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      return this.GetReleaseLogContainers(projectId, releaseId, 0, skipIsDeletedCheck);
    }

    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      bool skipIsDeletedCheck)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseRunPlanIdRefs", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      return this.FetchReleaseLogContainers();
    }
  }
}
