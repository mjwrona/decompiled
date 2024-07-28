// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseHistorySqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseHistorySqlComponent2 : ReleaseHistorySqlComponent1
  {
    public override void SaveRevision(Guid projectId, ReleaseRevision releaseRevision)
    {
      if (releaseRevision == null)
        throw new ArgumentNullException(nameof (releaseRevision));
      this.UpdateRevision(projectId, releaseRevision.ReleaseId, releaseRevision.DefinitionSnapshotRevision, releaseRevision.ChangeType, releaseRevision.FileId);
    }

    public override void UpdateRevision(
      Guid projectId,
      int releaseId,
      int definitionSnapshotRevision,
      ReleaseHistoryChangeTypes changeType,
      int fileId)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseDefinitionRevision", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (definitionSnapshotRevision), definitionSnapshotRevision);
      this.BindByte(nameof (changeType), (byte) changeType);
      this.BindInt(nameof (fileId), fileId);
      this.ExecuteNonQuery();
    }
  }
}
