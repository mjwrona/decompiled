// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent15
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent15 : FileComponent14
  {
    public override void DeleteOrphanPendingUploadFiles(
      IEnumerable<KeyValuePair<int, int>> filesToDelete,
      bool reuseFileIdSecondaryRange)
    {
      this.PrepareStoredProcedure("prc_DeleteOrphanPendingUploadFiles");
      this.BindKeyValuePairInt32Int32Table("@filesToDelete", filesToDelete);
      this.ExecuteNonQuery();
    }

    public override void UpdatePendingUploadFileDate(
      long fileId,
      int dataspaceId,
      DateTime newDate)
    {
      this.PrepareStoredProcedure("prc_UpdateFilePendingUploadDate");
      this.BindInt("@fileId", (int) fileId);
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindDateTime2("@newDate", newDate);
      this.ExecuteNonQuery();
    }

    public override List<Tuple<int, int, Guid>> QueryOrphanPendingUploadFiles(TimeSpan retentionDate)
    {
      if (retentionDate.TotalSeconds > (double) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (retentionDate));
      this.PrepareStoredProcedure("prc_QueryOrphanPendingUploadFiles");
      this.BindInt("@retentionPeriodInDays", (int) -retentionDate.TotalDays);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<int, int, Guid>>((ObjectBinder<Tuple<int, int, Guid>>) new FileComponent15.OrphanFilesBinder());
      return resultCollection.GetCurrent<Tuple<int, int, Guid>>().Items;
    }

    internal sealed class OrphanFilesBinder : ObjectBinder<Tuple<int, int, Guid>>
    {
      private SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      private bool m_32BitFileId;
      private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder ResourceIdColumn = new SqlColumnBinder("ResourceId");

      protected override Tuple<int, int, Guid> Bind()
      {
        long num;
        if (!this.m_32BitFileId)
        {
          try
          {
            num = this.FileIdColumn.GetInt64((IDataReader) this.Reader);
          }
          catch (InvalidCastException ex)
          {
            this.m_32BitFileId = true;
            num = (long) this.FileIdColumn.GetInt32((IDataReader) this.Reader);
          }
        }
        else
          num = (long) this.FileIdColumn.GetInt32((IDataReader) this.Reader);
        return new Tuple<int, int, Guid>((int) num, this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader), this.ResourceIdColumn.GetGuid((IDataReader) this.Reader));
      }
    }
  }
}
