// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.CrossProcessLeaseFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class CrossProcessLeaseFactory
  {
    internal static readonly TimeSpan WorkItemTypeReconciliationLeaseTime = TimeSpan.FromMinutes(5.0);

    public static bool TryAcquireLeaseForWorkItemTypeExtensionReconciliation(
      IVssRequestContext requestContext,
      Guid extensionId,
      TimeSpan timeout,
      out ILeaseInfo leaseInfo)
    {
      string leaseName = string.Format("WorkItemTypeExtension.{0}.Reconcile", (object) extensionId);
      return requestContext.GetService<ILeaseService>().TryAcquireLease(requestContext, leaseName, CrossProcessLeaseFactory.WorkItemTypeReconciliationLeaseTime, timeout == Timeout.InfiniteTimeSpan ? TimeSpan.MaxValue : timeout, out leaseInfo);
    }
  }
}
