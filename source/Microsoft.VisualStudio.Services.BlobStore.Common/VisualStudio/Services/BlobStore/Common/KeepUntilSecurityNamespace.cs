// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.KeepUntilSecurityNamespace
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class KeepUntilSecurityNamespace
  {
    public static readonly Guid NamespaceId = new Guid("72e679c6-6073-4c9f-9693-548476b7c02e");
    public static readonly string Token = "MaxKeepUntilSpan";

    public static KeepUntilSecurityNamespace.Permissions GetPermissionForDays(double requestedDays)
    {
      if (requestedDays <= 7.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep7Days;
      if (requestedDays <= 31.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep31Days;
      if (requestedDays <= 365.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep365Days;
      if (requestedDays <= 730.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep2Years;
      if (requestedDays <= 1095.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep3Years;
      if (requestedDays <= 1460.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep4Years;
      if (requestedDays <= 1825.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep5Years;
      if (requestedDays <= 2190.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep6Years;
      if (requestedDays <= 2555.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep7Years;
      if (requestedDays <= 2920.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep8Years;
      if (requestedDays <= 3285.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep9Years;
      if (requestedDays <= 3650.0)
        return KeepUntilSecurityNamespace.Permissions.MaxKeep10Years;
      return requestedDays <= 5475.0 ? KeepUntilSecurityNamespace.Permissions.MaxKeep15Years : KeepUntilSecurityNamespace.Permissions.MaxKeepUnlimited;
    }

    [Flags]
    public enum Permissions
    {
      MaxKeep7Days = 1,
      MaxKeep31Days = 2,
      MaxKeep365Days = 4,
      MaxKeep2Years = 8,
      MaxKeep3Years = 16, // 0x00000010
      MaxKeep4Years = 32, // 0x00000020
      MaxKeep5Years = 64, // 0x00000040
      MaxKeep6Years = 128, // 0x00000080
      MaxKeep7Years = 256, // 0x00000100
      MaxKeep8Years = 512, // 0x00000200
      MaxKeep9Years = 1024, // 0x00000400
      MaxKeep10Years = 2048, // 0x00000800
      MaxKeep15Years = 4096, // 0x00001000
      MaxKeepUnlimited = 268435456, // 0x10000000
    }
  }
}
