// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingEntitlementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (FrameworkLicensingEntitlementService))]
  public interface ILicensingEntitlementService : IVssFrameworkService
  {
    IList<AccountEntitlement> GetAccountEntitlements(IVssRequestContext requestContext);

    IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      int top,
      int skip = 0);

    IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      bool bypassCache = false);

    PagedAccountEntitlements SearchAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      string filter,
      string orderBy,
      bool includeServicePrincipals = false);

    PagedAccountEntitlements SearchMemberAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      string filter,
      string orderBy);

    AccountEntitlement GetAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      bool bypassCache = false,
      bool determineRights = true,
      bool createIfNotExists = true);

    IList<AccountEntitlement> ObtainAvailableAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds);

    AccountEntitlement AssignAvailableAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin = LicensingOrigin.None);

    AccountEntitlement AssignAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin = LicensingOrigin.None);

    IList<AccountLicenseUsage> GetLicensesUsage(IVssRequestContext requestContext, bool bypassCache = false);

    void DeleteAccountEntitlement(IVssRequestContext requestContext, Guid userId);
  }
}
