// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostLeaseHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class HostLeaseHelper
  {
    private static readonly RegistryQuery s_leaseTime = new RegistryQuery("/Configuration/HostSyncService/LeaseTime");
    private static readonly RegistryQuery s_leaseAcquisitionTimeout = new RegistryQuery("/Configuration/HostSyncService/LeaseAcquisitionTimeout");

    public static IDisposable AcquireHostLease(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      string leaseName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Host-{0}", (object) hostId.ToString("D"));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, in HostLeaseHelper.s_leaseTime, 300);
      int num2 = service.GetValue<int>(requestContext, in HostLeaseHelper.s_leaseAcquisitionTimeout, 300);
      return (IDisposable) requestContext.GetService<ILeaseService>().AcquireLease(requestContext, leaseName, TimeSpan.FromSeconds((double) num1), TimeSpan.FromSeconds((double) num2));
    }
  }
}
