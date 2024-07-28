// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticPoolBinder : ObjectBinder<ElasticPool>
  {
    protected override ElasticPool Bind() => new ElasticPool()
    {
      PoolId = ElasticPoolBinder.PoolId.GetInt32((IDataReader) this.Reader),
      ServiceEndpointId = ElasticPoolBinder.ServiceEndpointId.GetGuid((IDataReader) this.Reader),
      ServiceEndpointScope = ElasticPoolBinder.ServiceEndpointScope.GetGuid((IDataReader) this.Reader),
      AzureId = ElasticPoolBinder.AzureId.GetString((IDataReader) this.Reader, false),
      MaxCapacity = ElasticPoolBinder.MaxCapacity.GetInt32((IDataReader) this.Reader),
      DesiredIdle = ElasticPoolBinder.DesiredIdle.GetInt32((IDataReader) this.Reader),
      RecycleAfterEachUse = ElasticPoolBinder.RecycleAfterEachUse.GetBoolean((IDataReader) this.Reader),
      OsType = (OperatingSystemType) ElasticPoolBinder.OsType.GetByte((IDataReader) this.Reader, (byte) 0),
      OfflineSince = ElasticPoolBinder.OfflineSince.GetNullableDateTime((IDataReader) this.Reader),
      State = (ElasticPoolState) ElasticPoolBinder.State.GetByte((IDataReader) this.Reader, (byte) 0),
      DesiredSize = ElasticPoolBinder.DesiredSize.GetInt32((IDataReader) this.Reader),
      SizingAttempts = ElasticPoolBinder.SizingAttempts.GetInt32((IDataReader) this.Reader),
      AgentInteractiveUI = ElasticPoolBinder.AgentInteractiveUI.GetBoolean((IDataReader) this.Reader),
      TimeToLiveMinutes = ElasticPoolBinder.TimeToLiveMinutes.GetInt32((IDataReader) this.Reader),
      MaxSavedNodeCount = 0,
      OrchestrationType = OrchestrationType.Uniform
    };

    protected static SqlColumnBinder PoolId { get; } = new SqlColumnBinder(nameof (PoolId));

    protected static SqlColumnBinder ServiceEndpointId { get; } = new SqlColumnBinder(nameof (ServiceEndpointId));

    protected static SqlColumnBinder ServiceEndpointScope { get; } = new SqlColumnBinder(nameof (ServiceEndpointScope));

    protected static SqlColumnBinder AzureId { get; } = new SqlColumnBinder(nameof (AzureId));

    protected static SqlColumnBinder MaxCapacity { get; } = new SqlColumnBinder(nameof (MaxCapacity));

    protected static SqlColumnBinder DesiredIdle { get; } = new SqlColumnBinder(nameof (DesiredIdle));

    protected static SqlColumnBinder RecycleAfterEachUse { get; } = new SqlColumnBinder(nameof (RecycleAfterEachUse));

    protected static SqlColumnBinder OsType { get; } = new SqlColumnBinder(nameof (OsType));

    protected static SqlColumnBinder OfflineSince { get; } = new SqlColumnBinder(nameof (OfflineSince));

    protected static SqlColumnBinder State { get; } = new SqlColumnBinder(nameof (State));

    protected static SqlColumnBinder DesiredSize { get; } = new SqlColumnBinder(nameof (DesiredSize));

    protected static SqlColumnBinder SizingAttempts { get; } = new SqlColumnBinder(nameof (SizingAttempts));

    protected static SqlColumnBinder AgentInteractiveUI { get; } = new SqlColumnBinder(nameof (AgentInteractiveUI));

    protected static SqlColumnBinder TimeToLiveMinutes { get; } = new SqlColumnBinder(nameof (TimeToLiveMinutes));
  }
}
