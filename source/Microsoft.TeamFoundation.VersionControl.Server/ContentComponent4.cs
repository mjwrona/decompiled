// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ContentComponent4
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ContentComponent4 : ContentComponent3
  {
    public override PreUploadFileResult PreUploadFile(Workspace workspace, string targetServerItem)
    {
      this.PrepareStoredProcedure("prc_PreUploadFile");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", this.ConvertToPathWithProjectGuid(targetServerItem), true);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        reader.Read();
        int int32_1 = new SqlColumnBinder("TempFileId").GetInt32((IDataReader) reader, 0);
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TempFileDataspaceId");
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(sqlColumnBinder.GetInt32((IDataReader) reader, 0));
        sqlColumnBinder = new SqlColumnBinder("ExistingFileId");
        int int32_2 = sqlColumnBinder.GetInt32((IDataReader) reader, 0);
        sqlColumnBinder = new SqlColumnBinder("ExistingHashValue");
        byte[] a1 = sqlColumnBinder.GetBytes((IDataReader) reader, true);
        if (a1 != null && (a1.Length != 16 || ArrayUtil.Equals(a1, ContentComponent3.s_nullHash)))
          a1 = (byte[]) null;
        Guid tempFileDataspaceId = dataspaceIdentifier;
        int existingFileId = int32_2;
        byte[] existingHashValue = a1;
        return new PreUploadFileResult(int32_1, tempFileDataspaceId, existingFileId, existingHashValue);
      }
    }

    public override void PostUploadFile(
      Workspace workspace,
      string targetServerItem,
      int fileId,
      bool isLastChunk)
    {
      this.PrepareStoredProcedure("prc_PostUploadFile");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", this.ConvertToPathWithProjectGuid(targetServerItem), true);
      this.BindInt("@fileId", fileId);
      this.BindBoolean("@isLastChunk", isLastChunk);
      this.ExecuteNonQuery();
    }
  }
}
