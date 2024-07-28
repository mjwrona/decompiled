// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryOptimizerMemoryGatewaysInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class QueryOptimizerMemoryGatewaysInfoBinder : 
    ObjectBinder<QueryOptimizerMemoryGatewaysInfo>
  {
    private SqlColumnBinder m_poolId = new SqlColumnBinder("pool_id");
    private SqlColumnBinder m_name = new SqlColumnBinder("name");
    private SqlColumnBinder m_maxCount = new SqlColumnBinder("max_count");
    private SqlColumnBinder m_activeCount = new SqlColumnBinder("active_count");
    private SqlColumnBinder m_waiterCount = new SqlColumnBinder("waiter_count");
    private SqlColumnBinder m_thresholdFactor = new SqlColumnBinder("threshold_factor");
    private SqlColumnBinder m_threshold = new SqlColumnBinder("threshold");
    private SqlColumnBinder m_isActive = new SqlColumnBinder("is_active");

    protected override QueryOptimizerMemoryGatewaysInfo Bind() => new QueryOptimizerMemoryGatewaysInfo()
    {
      PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader),
      Name = this.m_name.GetString((IDataReader) this.Reader, true),
      MaxCount = this.m_maxCount.GetInt32((IDataReader) this.Reader),
      ActiveCount = this.m_activeCount.GetInt32((IDataReader) this.Reader),
      WaiterCount = this.m_waiterCount.GetInt32((IDataReader) this.Reader),
      ThresholdFactor = this.m_thresholdFactor.GetInt64((IDataReader) this.Reader),
      Threshold = this.m_threshold.GetInt64((IDataReader) this.Reader),
      IsActive = this.m_isActive.GetBoolean((IDataReader) this.Reader)
    };
  }
}
