// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.DeniedBillingMeters
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class DeniedBillingMeters
  {
    public static HashSet<Guid> DeniedMeterGuidsForPurchase = new HashSet<Guid>()
    {
      new Guid("DAF52501-330A-4A7A-A88A-CF85ED40988F"),
      new Guid("c240c688-f075-4691-99f2-7cdaef88e4b1")
    };
    public static HashSet<Guid> AdditionalMeterGuidsForPurchase = new HashSet<Guid>()
    {
      new Guid("B40291F6-F450-429B-A21F-0BC6711787AC"),
      new Guid("EE6736CF-57FD-443B-9A89-9B9810953C65")
    };
  }
}
