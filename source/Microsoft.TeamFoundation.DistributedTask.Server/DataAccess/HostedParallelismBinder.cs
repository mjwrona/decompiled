// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.HostedParallelismBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class HostedParallelismBinder : ObjectBinder<HostedParallelism>
  {
    private SqlColumnBinder m_level = new SqlColumnBinder("Level");
    private SqlColumnBinder m_source = new SqlColumnBinder("Source");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_revision = new SqlColumnBinder("Revision");
    private SqlColumnBinder m_creationDate = new SqlColumnBinder("CreationDate");
    private SqlColumnBinder m_lastUpdated = new SqlColumnBinder("LastUpdated");

    protected override HostedParallelism Bind() => new HostedParallelism()
    {
      Level = (HostedParallelismLevel) this.m_level.GetByte((IDataReader) this.Reader),
      Source = (HostedParallelismSource) this.m_source.GetByte((IDataReader) this.Reader),
      Description = this.m_description.GetString((IDataReader) this.Reader, true),
      Revision = this.m_revision.GetInt32((IDataReader) this.Reader),
      CreationDate = this.m_creationDate.GetDateTime((IDataReader) this.Reader),
      LastUpdated = this.m_lastUpdated.GetDateTime((IDataReader) this.Reader)
    };
  }
}
