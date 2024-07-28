// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent13
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent13 : ReleaseSqlComponent12
  {
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
      return this.GetReleaseObject(projectId);
    }
  }
}
