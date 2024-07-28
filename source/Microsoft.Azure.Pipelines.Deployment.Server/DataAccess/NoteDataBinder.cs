// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.NoteDataBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class NoteDataBinder : ObjectBinder<NoteData>
  {
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");
    private SqlColumnBinder m_kind = new SqlColumnBinder("Kind");
    private SqlColumnBinder m_fileId = new SqlColumnBinder("FileId");

    protected override NoteData Bind() => new NoteData()
    {
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      ScopeId = this.m_scopeId.GetGuid((IDataReader) this.Reader, false),
      Kind = (NoteKind) this.m_kind.GetByte((IDataReader) this.Reader),
      FileId = this.m_fileId.GetNullableInt32((IDataReader) this.Reader)
    };
  }
}
