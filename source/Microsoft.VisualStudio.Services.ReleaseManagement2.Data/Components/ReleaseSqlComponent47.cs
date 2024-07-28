// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent47
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class ReleaseSqlComponent47 : ReleaseSqlComponent46
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
      this.BindChangeDetails(changeDetails);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt(nameof (attempt), attempt);
      return this.GetReleaseObject(projectId);
    }
  }
}
