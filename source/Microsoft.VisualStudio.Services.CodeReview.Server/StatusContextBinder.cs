// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.StatusContextBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class StatusContextBinder : ObjectBinder<StatusContext>
  {
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder m_genreColumn = new SqlColumnBinder("Genre");

    protected override StatusContext Bind() => new StatusContext()
    {
      Name = this.m_nameColumn.GetString((IDataReader) this.Reader, false),
      Genre = this.m_genreColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
