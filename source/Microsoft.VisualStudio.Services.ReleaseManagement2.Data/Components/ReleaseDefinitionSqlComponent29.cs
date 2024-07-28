// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent29
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent29 : ReleaseDefinitionSqlComponent28
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override void SoftDeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId = 0,
      string comment = null,
      bool forceDelete = false)
    {
      this.PrepareForAuditingAction(ReleaseAuditConstants.ReleasePipelineDeleted, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_SoftDeleteReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindGuid("modifiedBy", requestorId);
      this.BindString(nameof (comment), comment, 2048, true, SqlDbType.NVarChar);
      this.BindBoolean(nameof (forceDelete), forceDelete);
      this.ExecuteNonQuery();
    }

    public override void HardDeleteReleaseDefinition(Guid projectId, int releaseDefinitionId)
    {
      this.PrepareStoredProcedure("Release.prc_HardDeleteReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.ExecuteNonQuery();
    }

    public override void UndeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_UndeleteReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindGuid("modifiedBy", requestorId);
      this.BindString(nameof (comment), comment, 2048, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
