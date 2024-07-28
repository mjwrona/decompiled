// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.IAccountEntitlementCache
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  [DefaultServiceImplementation(typeof (AccountEntitlementCache))]
  public interface IAccountEntitlementCache : IVssFrameworkService
  {
    License GetAccountEntitlement(IVssRequestContext requestContext, Guid accountId, Guid userId);

    IEnumerable<License> GetAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<Guid> accountIds,
      Guid userId);

    void InvalidateAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void UpdateAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License license);
  }
}
