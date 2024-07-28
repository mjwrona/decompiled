// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServiceHostExtensions
  {
    internal static IServiceHostInternal ServiceHostInternal(
      this IVssServiceHost serviceHost,
      bool throwIfNull = true)
    {
      IServiceHostInternal serviceHostInternal = serviceHost as IServiceHostInternal;
      return !(serviceHostInternal == null & throwIfNull) ? serviceHostInternal : throw new InvalidCastException("Attempt to cast IVssServiceHost to IServiceHostInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    internal static IDeploymentServiceHostInternal DeploymentServiceHostInternal(
      this IVssDeploymentServiceHost serviceHost,
      bool throwIfNull = true)
    {
      IDeploymentServiceHostInternal serviceHostInternal = serviceHost as IDeploymentServiceHostInternal;
      return !(serviceHostInternal == null & throwIfNull) ? serviceHostInternal : throw new InvalidCastException("Attempt to cast IVssDeploymentServiceHost to IDeploymentServiceHostInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    internal static void CheckDisposedOrDisposing(this IVssServiceHostControl serviceHost)
    {
      if (serviceHost.IsDisposed)
        throw new ObjectDisposedException("ServiceHost");
      if (serviceHost.IsDisposing)
        throw new ObjectDisposedException("ServiceHost", "The service host is being disposed.");
    }

    internal static void CheckDisposed(this IVssServiceHostControl serviceHost)
    {
      if (serviceHost.IsDisposed)
        throw new ObjectDisposedException("ServiceHost");
    }

    internal static Guid SystemServicePrincipal(this IVssServiceHost serviceHost) => serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().SystemServicePrincipal;

    public static IdentityDescriptor SystemDescriptor(this IVssServiceHost serviceHost) => serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().SystemDescriptor;

    internal static IRequestActor SystemActor(this IVssServiceHost serviceHost) => serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().SystemActor;

    internal static bool IsCreating(this IVssServiceHost serviceHost) => serviceHost.Status == TeamFoundationServiceHostStatus.Stopped && serviceHost.ServiceHostInternal().SubStatus == ServiceHostSubStatus.Creating;

    public static bool IsHostProcessType(
      this IVssServiceHost serviceHost,
      HostProcessType processType)
    {
      return serviceHost.DeploymentServiceHost.ProcessType == processType;
    }
  }
}
