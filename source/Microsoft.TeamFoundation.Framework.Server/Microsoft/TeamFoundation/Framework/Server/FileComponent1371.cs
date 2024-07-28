// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1371
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1371 : FileComponent1370
  {
    public override void CommitSingleFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.PrepareStoredProcedure("prc_CommitSingleFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));
      this.BindBinary("@hashValue", hashValue, 16, SqlDbType.Binary);
      this.ExecuteNonQuery();
    }
  }
}
