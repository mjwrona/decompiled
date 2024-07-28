// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangeEntryFileDataBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class ChangeEntryFileDataBinder : ObjectBinder<ContentAccessMetadata>
  {
    private SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    private SqlColumnBinder FilePathColumn = new SqlColumnBinder("FilePath");
    private SqlColumnBinder ContentHashColumn = new SqlColumnBinder("ContentHash");
    private SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");

    protected override ContentAccessMetadata Bind() => new ContentAccessMetadata(this.FileIdColumn.GetInt32((IDataReader) this.Reader, 0), this.FilePathColumn.GetString((IDataReader) this.Reader, true), this.ContentHashColumn.GetBytes((IDataReader) this.Reader, false), this.FlagsColumn.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0));
  }
}
