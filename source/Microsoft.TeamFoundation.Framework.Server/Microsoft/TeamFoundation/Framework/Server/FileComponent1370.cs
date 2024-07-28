// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1370
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1370 : FileComponent19
  {
    public override int GetMaxFileId()
    {
      this.PrepareStoredProcedure("prc_GetMaxFileId");
      return (int) this.ExecuteScalar();
    }

    public override void SoftDeleteFile(FileIdentifier fileId)
    {
      this.PrepareStoredProcedure("prc_DeleteFile", 3600);
      this.BindInt("@fileId", (int) fileId.FileId);
      this.BindDateTime2("@DeletionDate", DateTime.Parse("9999-01-01"));
      if (!fileId.DataspaceIdentifier.HasValue)
        this.BindInt("@dataspaceId", 0);
      else
        this.BindInt("@dataspaceId", this.GetDataspaceId(fileId.DataspaceIdentifier.Value));
      this.ExecuteNonQuery();
    }
  }
}
