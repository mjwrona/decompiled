// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionHistorySqlComponent3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDefinitionHistorySqlComponent3 : ReleaseDefinitionHistorySqlComponent2
  {
    public override void SaveRevision(
      Guid projectId,
      ReleaseDefinition clientReleaseDefinition,
      int fileId,
      string apiVersion,
      AuditAction changeType)
    {
      if (clientReleaseDefinition == null)
        throw new ArgumentNullException(nameof (clientReleaseDefinition));
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinitionRevision_Add", projectId);
      this.BindInt("definitionId", clientReleaseDefinition.Id);
      this.BindInt("definitionRevision", clientReleaseDefinition.Revision);
      this.BindGuid("changedBy", Guid.Parse(clientReleaseDefinition.ModifiedBy.Id));
      this.BindDateTime("changedDate", clientReleaseDefinition.ModifiedOn);
      this.BindInt("@changeType", (int) changeType);
      this.BindInt(nameof (fileId), fileId);
      this.BindString("comment", clientReleaseDefinition.Comment, 2048, true, SqlDbType.NVarChar);
      this.BindString(nameof (apiVersion), apiVersion, 32, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
