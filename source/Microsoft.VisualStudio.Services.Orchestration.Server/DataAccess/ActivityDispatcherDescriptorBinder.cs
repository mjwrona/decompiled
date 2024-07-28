// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.ActivityDispatcherDescriptorBinder
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal class ActivityDispatcherDescriptorBinder : ObjectBinder<ActivityDispatcherDescriptor>
  {
    private SqlColumnBinder m_hubName = new SqlColumnBinder("HubName");
    private SqlColumnBinder m_dispatcherType = new SqlColumnBinder("DispatcherType");
    private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");

    protected override ActivityDispatcherDescriptor Bind()
    {
      ActivityDispatcherDescriptor dispatcherDescriptor = new ActivityDispatcherDescriptor();
      dispatcherDescriptor.HubName = this.m_hubName.GetString((IDataReader) this.Reader, false);
      dispatcherDescriptor.Type = this.m_dispatcherType.GetString((IDataReader) this.Reader, false);
      dispatcherDescriptor.JobId = this.m_jobId.GetGuid((IDataReader) this.Reader, false);
      return dispatcherDescriptor;
    }
  }
}
