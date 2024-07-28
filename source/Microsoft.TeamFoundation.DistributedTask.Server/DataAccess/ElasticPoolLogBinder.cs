// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolLogBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticPoolLogBinder : ObjectBinder<ElasticPoolLog>
  {
    protected override ElasticPoolLog Bind() => new ElasticPoolLog()
    {
      Id = ElasticPoolLogBinder.LogId.GetInt64((IDataReader) this.Reader),
      PoolId = ElasticPoolLogBinder.PoolId.GetInt32((IDataReader) this.Reader),
      Level = (LogLevel) ElasticPoolLogBinder.Level.GetByte((IDataReader) this.Reader, (byte) 0),
      Operation = (OperationType) ElasticPoolLogBinder.Operation.GetByte((IDataReader) this.Reader, (byte) 0),
      Message = ElasticPoolLogBinder.Message.GetString((IDataReader) this.Reader, false),
      Timestamp = ElasticPoolLogBinder.Timestamp.GetDateTime((IDataReader) this.Reader)
    };

    protected static SqlColumnBinder PoolId { get; } = new SqlColumnBinder(nameof (PoolId));

    protected static SqlColumnBinder LogId { get; } = new SqlColumnBinder(nameof (LogId));

    protected static SqlColumnBinder Level { get; } = new SqlColumnBinder(nameof (Level));

    protected static SqlColumnBinder Operation { get; } = new SqlColumnBinder(nameof (Operation));

    protected static SqlColumnBinder Message { get; } = new SqlColumnBinder(nameof (Message));

    protected static SqlColumnBinder Timestamp { get; } = new SqlColumnBinder(nameof (Timestamp));
  }
}
