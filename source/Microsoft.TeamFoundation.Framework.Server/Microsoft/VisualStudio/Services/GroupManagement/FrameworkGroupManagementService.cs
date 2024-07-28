// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupManagement.FrameworkGroupManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.GroupManagement
{
  public class FrameworkGroupManagementService : IGroupManagementService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckHostedDeployment();
      this.ServiceHostId = systemRequestContext.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsMember(IVssRequestContext requestContext, string member)
    {
      requestContext.TraceEnter(10027110, "GroupManagement", nameof (FrameworkGroupManagementService), nameof (IsMember));
      try
      {
        requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
        return true;
      }
      finally
      {
        requestContext.TraceLeave(10027119, "GroupManagement", nameof (FrameworkGroupManagementService), nameof (IsMember));
      }
    }

    private Guid ServiceHostId { get; set; }
  }
}
