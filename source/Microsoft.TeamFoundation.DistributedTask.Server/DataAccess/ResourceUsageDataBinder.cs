// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceUsageDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class ResourceUsageDataBinder : ObjectBinder<ResourceUsageData>
  {
    private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("ResourceType");
    private SqlColumnBinder m_runningRequestsCount = new SqlColumnBinder("RunningRequestsCount");

    protected override ResourceUsageData Bind() => new ResourceUsageData()
    {
      HostId = this.m_hostId.GetGuid((IDataReader) this.Reader),
      ResourceType = this.m_resourceType.GetString((IDataReader) this.Reader, true),
      RunningRequestsCount = this.m_runningRequestsCount.GetInt32((IDataReader) this.Reader)
    };
  }
}
