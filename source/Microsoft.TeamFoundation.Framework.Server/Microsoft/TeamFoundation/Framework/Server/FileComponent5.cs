// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent5 : FileComponent4
  {
    public override void RenameFile(long fileId, string newFileName)
    {
      this.PrepareStoredProcedure("prc_RenameFile");
      this.BindInt("@fileId", (int) fileId);
      this.BindString("@newFileName", newFileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
