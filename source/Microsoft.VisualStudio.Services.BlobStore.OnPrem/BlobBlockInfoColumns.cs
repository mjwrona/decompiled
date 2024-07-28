// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.BlobBlockInfoColumns
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class BlobBlockInfoColumns : ObjectBinder<SqlBlobBlockInfo>
  {
    protected SqlColumnBinder BlobIdColumn = new SqlColumnBinder("BlobId");
    protected SqlColumnBinder BlockHashColumn = new SqlColumnBinder("BlockHash");
    protected SqlColumnBinder BlockFileIdColumn = new SqlColumnBinder("BlockFileId");

    protected override SqlBlobBlockInfo Bind() => new SqlBlobBlockInfo()
    {
      BlobId = new BlobIdentifier(this.BlobIdColumn.GetBytes((IDataReader) this.Reader, false)),
      BlockHash = new Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash(this.BlockHashColumn.GetBytes((IDataReader) this.Reader, false)),
      BlockFileId = this.BlockFileIdColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
