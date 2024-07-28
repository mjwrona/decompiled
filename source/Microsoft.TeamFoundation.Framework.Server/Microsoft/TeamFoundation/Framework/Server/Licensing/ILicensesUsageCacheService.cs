// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.ILicensesUsageCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Licensing;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing
{
  [DefaultServiceImplementation(typeof (LicensesUsageCacheService))]
  internal interface ILicensesUsageCacheService : IVssFrameworkService
  {
    bool TryGetValue(
      IVssRequestContext requestContext,
      string key,
      out List<AccountLicenseUsage> value);

    bool Remove(IVssRequestContext requestContext, string key);

    void Set(IVssRequestContext requestContext, string key, List<AccountLicenseUsage> value);
  }
}
