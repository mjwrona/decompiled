// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.LicensesUsageCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing
{
  internal class LicensesUsageCacheService : 
    VssMemoryCacheService<string, List<AccountLicenseUsage>>,
    ILicensesUsageCacheService,
    IVssFrameworkService
  {
    private static readonly TimeSpan CleanUpInternal = TimeSpan.FromMinutes(15.0);

    public LicensesUsageCacheService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, LicensesUsageCacheService.CleanUpInternal)
    {
      this.ExpiryInterval.Value = TimeSpan.FromHours(12.0);
    }
  }
}
