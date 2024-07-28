// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DirTreeInfoColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class DirTreeInfoColumns : ObjectBinder<DirTreeInfo>
  {
    private SqlColumnBinder m_subDirectoryColumn = new SqlColumnBinder("subdirectory");
    private SqlColumnBinder m_depthColumn = new SqlColumnBinder("depth");
    private SqlColumnBinder m_fileColumn = new SqlColumnBinder("file");

    protected override DirTreeInfo Bind() => new DirTreeInfo()
    {
      SubDirectory = this.m_subDirectoryColumn.GetString((IDataReader) this.Reader, false),
      Depth = this.m_depthColumn.GetInt32((IDataReader) this.Reader),
      File = this.m_fileColumn.GetInt32((IDataReader) this.Reader, 0, 0)
    };
  }
}
