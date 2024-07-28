// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal interface ILicensingComponent : IDisposable
  {
    void CreateScope(Guid scopeId);

    IList<Guid> GetScopes();

    List<UserLicense> GetPreviousUserLicenses(Guid scopeId, IList<Guid> userIds);

    UserLicense GetUserLicense(Guid scopeId, Guid userId);

    UserLicense SetUserLicense(
      Guid scopeId,
      Guid userId,
      LicensingSource source,
      int license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity);

    void DeleteUserLicense(Guid scopeId, Guid userId, ILicensingEvent licensingEvent);

    IList<UserLicense> GetUserLicenses(Guid scopeId);

    IList<UserLicense> GetUserLicenses(Guid scopeId, IList<Guid> userIds);

    IList<UserLicense> GetUserLicenses(Guid scopeId, int top, int skip);

    IPagedList<UserLicense> GetUserLicenses(Guid scopeId, string continuationToken);

    IPagedList<UserLicense> GetFilteredUserLicenses(
      string continuationToken,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort);

    void UpdateUserLastAccessed(Guid scopeId, Guid userId, DateTimeOffset lastAccessedDate);

    IList<AccountLicenseCount> GetUserLicensesDistribution(Guid scopeId);

    IList<UserLicenseCount> GetUserLicenseUsage(Guid scopeId);

    int GetUserLicenseCount();

    int GetUserLicenseCount(LicensingSource source, int license);

    void ImportScope(
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses,
      ILicensingEvent licensingEvent);

    void DeleteScope(Guid scopeId, ILicensingEvent licensingEvent);

    void AddUser(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSourceIfAbsent,
      LicensingOrigin originIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity);

    void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      ILicensingEvent licensingEvent);

    void TransferUserLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap);

    UserLicenseCosmosSerializableDocument UpdateUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool withOptimisticConcurrency = false);

    UserLicenseCosmosSerializableDocument UpsertUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false);

    void DeleteUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false);

    UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocument(Guid userId);

    UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocumentByIdAndPreviousId(
      Guid enterpriseStorageKey);

    IPagedList<UserLicenseCosmosSerializableDocument> GetPagedUserLicenseCosmosDocuments(
      string continuation);

    IPagedList<UserLicenseCosmosSerializableDocument> GetFilteredPagedUserLicenseCosmosDocuments(
      string continuation,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort);

    IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments();

    IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments(
      IEnumerable<Guid> userIds);
  }
}
