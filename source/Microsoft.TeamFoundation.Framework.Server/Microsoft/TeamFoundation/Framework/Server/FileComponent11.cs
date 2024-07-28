// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent11 : FileComponent10
  {
    public override void DeleteFile(FileIdentifier fileId)
    {
      this.PrepareStoredProcedure("prc_DeleteFile", 3600);
      this.BindInt("@fileId", (int) fileId.FileId);
      if (!fileId.DataspaceIdentifier.HasValue)
        this.BindInt("@dataspaceId", 0);
      else
        this.BindInt("@dataspaceId", this.GetDataspaceId(fileId.DataspaceIdentifier.Value));
      this.ExecuteNonQuery();
    }

    public override void DeleteFiles(IEnumerable<FileIdentifier> filesToDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteFiles", 3600);
      this.BindKeyValuePairInt32Int32Table("@filesToDelete", filesToDelete.Select<FileIdentifier, KeyValuePair<int, int>>((Func<FileIdentifier, KeyValuePair<int, int>>) (f => new KeyValuePair<int, int>((int) f.FileId, this.GetDataspaceId(f)))));
      this.ExecuteNonQuery();
    }

    public int GetDataspaceId(FileIdentifier f) => !f.DataspaceIdentifier.HasValue ? 0 : this.GetDataspaceId(f.DataspaceIdentifier.Value, f.OwnerId.HasValue ? TeamFoundationFileService.GetCategoryFromOwnerId(f.OwnerId.Value) : "Default");
  }
}
