// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceSemaphoreInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ResourceSemaphoreInfoBinder : ObjectBinder<ResourceSemaphoreInfo>
  {
    private SqlColumnBinder m_resourceSemaphoreId = new SqlColumnBinder("resource_semaphore_id");
    private SqlColumnBinder m_targetMemoryKB = new SqlColumnBinder("target_memory_kb");
    private SqlColumnBinder m_maxTargetMemoryKB = new SqlColumnBinder("max_target_memory_kb");
    private SqlColumnBinder m_totalMemoryKB = new SqlColumnBinder("total_memory_kb");
    private SqlColumnBinder m_availableMemoryKB = new SqlColumnBinder("available_memory_kb");
    private SqlColumnBinder m_grantedMemoryKB = new SqlColumnBinder("granted_memory_kb");
    private SqlColumnBinder m_usedMemoryKB = new SqlColumnBinder("used_memory_kb");
    private SqlColumnBinder m_granteeCount = new SqlColumnBinder("grantee_count");
    private SqlColumnBinder m_waiterCount = new SqlColumnBinder("waiter_count");
    private SqlColumnBinder m_timoutErrorCount = new SqlColumnBinder("timeout_error_count");
    private SqlColumnBinder m_forcedGrantCount = new SqlColumnBinder("forced_grant_count");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("pool_id");

    protected override ResourceSemaphoreInfo Bind() => new ResourceSemaphoreInfo()
    {
      ResourceSemaphoreId = (int) this.m_resourceSemaphoreId.GetInt16((IDataReader) this.Reader),
      TargetMemoryKB = this.m_targetMemoryKB.GetInt64((IDataReader) this.Reader),
      MaxTargetMemoryKB = this.m_maxTargetMemoryKB.GetInt64((IDataReader) this.Reader, 0L),
      TotalMemoryKB = this.m_totalMemoryKB.GetInt64((IDataReader) this.Reader),
      AvailableMemoryKB = this.m_availableMemoryKB.GetInt64((IDataReader) this.Reader),
      GrantedMemoryKB = this.m_grantedMemoryKB.GetInt64((IDataReader) this.Reader),
      UsedMemoryKB = this.m_usedMemoryKB.GetInt64((IDataReader) this.Reader),
      GranteeCount = this.m_granteeCount.GetInt32((IDataReader) this.Reader),
      WaiterCount = this.m_waiterCount.GetInt32((IDataReader) this.Reader),
      TimeoutErrorCount = this.m_timoutErrorCount.GetInt64((IDataReader) this.Reader, 0L),
      ForcedGrantCount = this.m_forcedGrantCount.GetInt64((IDataReader) this.Reader, 0L),
      PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader)
    };
  }
}
