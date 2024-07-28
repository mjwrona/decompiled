// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.BlockInfoColumns
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.Data;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class BlockInfoColumns : ObjectBinder<SqlBlockInfo>
  {
    protected SqlColumnBinder BlockHashColumn = new SqlColumnBinder("BlockHash");
    protected SqlColumnBinder BlockFileIdColumn = new SqlColumnBinder("BlockFileId");
    protected SqlColumnBinder BlockFileLengthColumn = new SqlColumnBinder("BlockFileLength");
    protected SqlColumnBinder IsCommittedColumn = new SqlColumnBinder("IsCommitted");

    protected override SqlBlockInfo Bind() => new SqlBlockInfo()
    {
      BlockHash = new BlobBlockHash(this.BlockHashColumn.GetBytes((IDataReader) this.Reader, false)),
      BlockFileId = this.BlockFileIdColumn.GetInt32((IDataReader) this.Reader),
      BlockFileLength = this.BlockFileLengthColumn.GetInt64((IDataReader) this.Reader),
      IsCommitted = this.IsCommittedColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
