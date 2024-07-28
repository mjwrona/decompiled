// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolBinder6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticPoolBinder6 : ElasticPoolBinder5
  {
    protected override ElasticPool Bind()
    {
      ElasticPool elasticPool = base.Bind();
      elasticPool.OrchestrationType = (Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType) ElasticPoolBinder6.OrchestrationType.GetByte((IDataReader) this.Reader, (byte) 0);
      return elasticPool;
    }

    protected static SqlColumnBinder OrchestrationType { get; } = new SqlColumnBinder(nameof (OrchestrationType));
  }
}
