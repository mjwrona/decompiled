// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FolderNameColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FolderNameColumns : ObjectBinder<Tuple<string, string>>
  {
    private SqlColumnBinder m_dataFolderPath = new SqlColumnBinder("DataFolderPath");
    private SqlColumnBinder m_logFolderPath = new SqlColumnBinder("LogFolderPath");

    protected override Tuple<string, string> Bind() => new Tuple<string, string>(this.m_dataFolderPath.GetString((IDataReader) this.Reader, false), this.m_logFolderPath.GetString((IDataReader) this.Reader, false));
  }
}
