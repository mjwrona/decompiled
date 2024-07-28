// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualFileStatsBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VirtualFileStatsBinder : ObjectBinder<VirtualFileStats>
  {
    private SqlColumnBinder DatabaseId = new SqlColumnBinder("database_id");
    private SqlColumnBinder FileId = new SqlColumnBinder("file_id");
    private SqlColumnBinder SampleMs = new SqlColumnBinder("sample_ms");
    private SqlColumnBinder NumReads = new SqlColumnBinder("num_of_reads");
    private SqlColumnBinder NumBytesRead = new SqlColumnBinder("num_of_bytes_read");
    private SqlColumnBinder IoStallReadMs = new SqlColumnBinder("io_stall_read_ms");
    private SqlColumnBinder NumWrites = new SqlColumnBinder("num_of_writes");
    private SqlColumnBinder NumBytesWritten = new SqlColumnBinder("num_of_bytes_written");
    private SqlColumnBinder IoStallWriteMs = new SqlColumnBinder("io_stall_write_ms");
    private SqlColumnBinder IoStall = new SqlColumnBinder("io_stall");
    private SqlColumnBinder SizeOnDiskBytes = new SqlColumnBinder("size_on_disk_bytes");
    private SqlColumnBinder IoStallQueuedReadMs = new SqlColumnBinder("io_stall_queued_read_ms");
    private SqlColumnBinder IoStallQueueWriteMs = new SqlColumnBinder("io_stall_queued_write_ms");

    protected override VirtualFileStats Bind() => new VirtualFileStats()
    {
      DatabaseId = this.DatabaseId.GetInt16((IDataReader) this.Reader),
      FileId = this.FileId.GetInt16((IDataReader) this.Reader),
      SampleMs = this.SampleMs.GetInt64((IDataReader) this.Reader),
      NumReads = this.NumReads.GetInt64((IDataReader) this.Reader),
      NumBytesRead = this.NumBytesRead.GetInt64((IDataReader) this.Reader),
      IoStallReadMs = this.IoStallReadMs.GetInt64((IDataReader) this.Reader),
      NumWrites = this.NumWrites.GetInt64((IDataReader) this.Reader),
      NumBytesWritten = this.NumBytesWritten.GetInt64((IDataReader) this.Reader),
      IoStallWriteMs = this.IoStallWriteMs.GetInt64((IDataReader) this.Reader),
      IoStall = this.IoStall.GetInt64((IDataReader) this.Reader),
      SizeOnDiskBytes = this.SizeOnDiskBytes.GetInt64((IDataReader) this.Reader),
      IoStallQueuedReadMs = this.IoStallQueuedReadMs.GetInt64((IDataReader) this.Reader),
      IoStallQueueWriteMs = this.IoStallQueueWriteMs.GetInt64((IDataReader) this.Reader)
    };
  }
}
