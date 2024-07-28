// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IExtensionLicensingComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal interface IExtensionLicensingComponent : IDisposable
  {
    void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId);

    IList<UserExtensionLicense> FilterUsersWithExtensionBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId);

    IList<UserExtensionLicense> GetUserExtensionLicenses(
      Guid scopeId,
      Guid userId,
      UserExtensionLicenseStatus status);

    IDictionary<Guid, IList<ExtensionSource>> GetExtensionsForUsersBatch(
      Guid scopeId,
      IList<Guid> userIds);

    IList<UserExtensionLicense> GetUserExtensionLicenses(Guid scopeId);

    int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId);

    void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid CollectionId);

    IList<AccountExtensionCount> GetAccountExtensionCount(
      Guid scopeId,
      UserExtensionLicenseStatus status);

    int GetExtensionUsageCountInAccount(Guid scopeId, string extensionId);

    void TransferUserExtensionLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap);
  }
}
