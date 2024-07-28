// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.IAccountLicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  public interface IAccountLicensingHttpClient
  {
    Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      License license,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IDictionary<string, bool>> ComputeExtensionRightsAsync(
      IEnumerable<string> extensionIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task DeleteEntitlementAsync(Guid userId, object userState = null, CancellationToken cancellationToken = default (CancellationToken));

    Task<AccountEntitlement> GetAccountEntitlementAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      bool createIfNotExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      int top,
      int skip = 0,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccountLicenseUsage>> GetAccountLicensesUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ExtensionRightsResult> GetExtensionRightsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IList<AccountEntitlement>> ObtainAvailableAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<PagedAccountEntitlements> SearchAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<PagedAccountEntitlements> SearchMemberAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task TransferIdentityRightsAsync(
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      bool? validateOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
