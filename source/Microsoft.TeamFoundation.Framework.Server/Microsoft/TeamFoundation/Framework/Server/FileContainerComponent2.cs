// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent2 : FileContainerComponent
  {
    public FileContainerComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override void UpdateItemStatus(
      long containerId,
      string path,
      int fileId,
      ContainerItemStatus status,
      Guid? dataspaceIdentifier,
      long fileLength = -1,
      byte[] contentId = null,
      long? artifactId = null)
    {
      this.TraceEnter(0, nameof (UpdateItemStatus));
      if (!string.IsNullOrEmpty(path))
      {
        string databasePath = DBPath.UserToDatabasePath(path, true, true);
        this.PrepareStoredProcedure("prc_UpdateContainerItemStatus");
        this.BindLong("@containerId", containerId);
        this.BindString("@path", databasePath, -1, false, SqlDbType.NVarChar);
        this.BindInt("@fileId", fileId);
        this.BindByte("@status", (byte) status);
        this.BindGuid("@createdBy", this.Author);
        this.BindDataspace(dataspaceIdentifier);
        if (fileLength > -1L)
          this.BindLong("@fileLength", fileLength);
        this.ExecuteNonQuery();
      }
      this.TraceLeave(0, nameof (UpdateItemStatus));
    }
  }
}
