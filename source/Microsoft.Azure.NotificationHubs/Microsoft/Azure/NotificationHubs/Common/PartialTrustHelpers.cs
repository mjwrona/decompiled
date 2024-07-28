// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.PartialTrustHelpers
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class PartialTrustHelpers
  {
    [SecurityCritical]
    private static Type aptca;

    internal static bool ShouldFlowSecurityContext
    {
      [SecurityCritical] get => !AppDomain.CurrentDomain.IsHomogenous && SecurityManager.CurrentThreadRequiresSecurityContextCapture();
    }

    [SecurityCritical]
    internal static bool IsInFullTrust()
    {
      if (AppDomain.CurrentDomain.IsHomogenous)
        return AppDomain.CurrentDomain.IsFullyTrusted;
      if (!SecurityManager.CurrentThreadRequiresSecurityContextCapture())
        return true;
      try
      {
        PartialTrustHelpers.DemandForFullTrust();
        return true;
      }
      catch (SecurityException ex)
      {
        return false;
      }
    }

    [SecurityCritical]
    internal static bool UnsafeIsInFullTrust() => AppDomain.CurrentDomain.IsHomogenous ? AppDomain.CurrentDomain.IsFullyTrusted : !SecurityManager.CurrentThreadRequiresSecurityContextCapture();

    [SecurityCritical]
    internal static SecurityContext CaptureSecurityContextNoIdentityFlow()
    {
      if (SecurityContext.IsWindowsIdentityFlowSuppressed())
        return SecurityContext.Capture();
      using (SecurityContext.SuppressFlowWindowsIdentity())
        return SecurityContext.Capture();
    }

    [SecurityCritical]
    internal static bool IsTypeAptca(Type type)
    {
      Assembly assembly = type.Assembly;
      return PartialTrustHelpers.IsAssemblyAptca(assembly) || !PartialTrustHelpers.IsAssemblySigned(assembly);
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    private static void DemandForFullTrust()
    {
    }

    [SecurityCritical]
    private static bool IsAssemblyAptca(Assembly assembly)
    {
      if (PartialTrustHelpers.aptca == (Type) null)
        PartialTrustHelpers.aptca = typeof (AllowPartiallyTrustedCallersAttribute);
      return assembly.GetCustomAttributes(PartialTrustHelpers.aptca, false).Length != 0;
    }

    [SecurityCritical]
    [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
    private static bool IsAssemblySigned(Assembly assembly)
    {
      byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
      return publicKeyToken != null & publicKeyToken.Length != 0;
    }

    [SecurityCritical]
    internal static bool CheckAppDomainPermissions(PermissionSet permissions) => AppDomain.CurrentDomain.IsHomogenous && permissions.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);

    [SecurityCritical]
    internal static bool HasEtwPermissions() => PartialTrustHelpers.CheckAppDomainPermissions(new PermissionSet(PermissionState.Unrestricted));
  }
}
