// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent14
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent14 : ReleaseSqlComponent13
  {
    public override Release StartDraftRelease(
      Guid projectId,
      Release release,
      Guid modifiedBy,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.EnsureReleaseNameIsNotEmpty(release);
      this.PrepareStoredProcedure("Release.prc_StartDraftRelease", projectId);
      this.BindInt("releaseId", release.Id);
      this.BindString("releaseName", release.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("releaseDefinitionId", release.ReleaseDefinitionId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release PatchRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      IList<ReleaseEnvironment> releaseEnvironments,
      string comment,
      ReleaseStatus? status = null,
      bool? keepForever = null)
    {
      this.PrepareStoredProcedure("Release.prc_PatchRelease", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (status.HasValue)
        this.BindByte("releaseStatus", (byte) status.Value);
      if (keepForever.HasValue)
        this.BindBoolean(nameof (keepForever), keepForever.Value);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) (releaseEnvironments ?? (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>()));
      return this.GetReleaseObject(projectId);
    }

    public override Release UpdateReleaseEnvironmentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      Guid changedBy,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironmentStatus", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindChangeDetails(changeDetails);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    public override Release RejectReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_RejectReleaseEnvironment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    protected virtual void BindChangeDetails(
      ReleaseEnvironmentStatusChangeDetails changeDetails)
    {
    }
  }
}
