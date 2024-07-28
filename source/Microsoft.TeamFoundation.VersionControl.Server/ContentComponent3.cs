// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ContentComponent3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ContentComponent3 : ContentComponent2
  {
    protected static readonly byte[] s_nullHash = new byte[16];

    public override PreUploadFileResult PreUploadFile(Workspace workspace, string targetServerItem)
    {
      this.PrepareStoredProcedure("prc_PreUploadFile");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", targetServerItem, true);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        reader.Read();
        int int32_1 = new SqlColumnBinder("TempFileId").GetInt32((IDataReader) reader, 0);
        int int32_2 = new SqlColumnBinder("ExistingFileId").GetInt32((IDataReader) reader, 0);
        byte[] a1 = new SqlColumnBinder("ExistingHashValue").GetBytes((IDataReader) reader, true);
        if (a1 != null && (a1.Length != 16 || ArrayUtil.Equals(a1, ContentComponent3.s_nullHash)))
          a1 = (byte[]) null;
        int existingFileId = int32_2;
        byte[] existingHashValue = a1;
        return new PreUploadFileResult(int32_1, existingFileId, existingHashValue);
      }
    }
  }
}
