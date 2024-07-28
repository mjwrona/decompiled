// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.IAccountExtensionLicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  public interface IAccountExtensionLicensingHttpClient
  {
    Task<ICollection<ExtensionOperationResult>> AssignExtensionToAllEligibleUsersAsync(
      string extensionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ICollection<ExtensionOperationResult>> AssignExtensionToUsersAsync(
      string extensionId,
      IList<Guid> userIds,
      bool isAutoAssignment = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IDictionary<Guid, IList<ExtensionSource>>> BulkGetExtensionsAssignedToUsersAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IList<Guid>> GetEligibleUsersForExtensionAsync(
      string extensionId,
      ExtensionFilterOptions options = ExtensionFilterOptions.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccountLicenseExtensionUsage>> GetExtensionLicenseUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IDictionary<string, LicensingSource>> GetExtensionsAssignedToUserAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IDictionary<Guid, IList<string>>> GetExtensionsAssignedToUsersBatchAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IDictionary<Guid, ExtensionAssignmentDetails>> GetExtensionStatusForUsersAsync(
      string extensionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task TransferExtensionsForIdentitiesAsync(
      IList<IdentityMapping> identityMapping,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ICollection<ExtensionOperationResult>> UnassignExtensionFromUsersAsync(
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
