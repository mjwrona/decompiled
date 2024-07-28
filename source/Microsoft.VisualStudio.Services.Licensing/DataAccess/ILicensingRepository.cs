// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ILicensingRepository
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal interface ILicensingRepository
  {
    void Initialize(IVssRequestContext requestContext);

    IList<Guid> GetScopes(IVssRequestContext requestContext);

    void CreateScope(IVssRequestContext requestContext, Guid scopeId);

    void DeleteScope(IVssRequestContext requestContext, Guid scopeId);

    void GetScopeSnapshot(
      IVssRequestContext requestContext,
      Guid scopeId,
      out List<UserLicense> userLicenses,
      out List<UserLicense> previousUserLicenses,
      out List<UserExtensionLicense> userExtensionLicenses);

    void ImportScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses);

    UserLicense GetEntitlement(IVssRequestContext requestContext, Guid userId);

    List<UserLicense> GetPreviousUserEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds);

    IList<UserLicense> GetEntitlements(IVssRequestContext requestContext, IList<Guid> userIds);

    IList<UserLicense> GetEntitlements(IVssRequestContext requestContext);

    IList<UserLicense> GetEntitlements(IVssRequestContext requestContext, int top, int skip);

    IPagedList<UserLicense> GetFilteredEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort);

    void DeleteEntitlement(IVssRequestContext requestContext, Guid userId);

    UserLicense AssignEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      LicensedIdentity licensedIdentity);

    IList<AccountLicenseCount> GetUserLicenseDistribution(IVssRequestContext requestContext);

    IList<UserLicenseCount> GetUserLicenseUsage(IVssRequestContext requestContext);

    void TransferUserLicenses(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap);

    int GetLicenseCount(IVssRequestContext requestContext);

    int GetLicenseCount(IVssRequestContext requestContext, LicensingSource source, int license);

    UserLicenseCosmosSerializableDocument UpsertUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      bool optimisticConcurrency = false);

    UserLicenseCosmosSerializableDocument UpdateUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool optimisticConcurrency = false);

    void DeleteUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      bool optimisticConcurrency = false);

    UserLicenseCosmosSerializableDocument GetUserDocument(
      IVssRequestContext requestContext,
      Guid userIds);

    UserLicenseCosmosSerializableDocument GetUserDocumentByIdAndPreviousId(
      IVssRequestContext requestContext,
      Guid enterpriseStorageKey);

    IPagedList<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext,
      string continuation);

    IEnumerable<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext);

    IEnumerable<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext,
      IEnumerable<Guid> userIds);

    void ConvertToCollectionLevelDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      Guid collectionLevelId);

    void AddUser(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus status,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      License license,
      LicensedIdentity licensedIdentity);

    void UpdateUserStatus(IVssRequestContext requestContext, Guid userId, AccountUserStatus status);

    void RemoveUser(IVssRequestContext requestContext, Guid userId);

    void UpdateUserLastAccessed(
      IVssRequestContext requestContext,
      Guid userId,
      DateTimeOffset lastAccessed);

    void AssignExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource);

    void UnassignExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId,
      LicensingSource source);

    int UpdateExtensionLicenses(
      IVssRequestContext requestContext,
      Guid userId,
      IList<string> extensions,
      LicensingSource source,
      AssignmentSource assignmentSource);

    IDictionary<Guid, IList<ExtensionSource>> GetExtensions(
      IVssRequestContext requestContext,
      IList<Guid> userIds);

    IList<UserExtensionLicense> GetExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId);

    IList<UserExtensionLicense> GetExtensionLicenses(
      IVssRequestContext requestContext,
      Guid userId,
      UserExtensionLicenseStatus status);

    IList<UserExtensionLicense> GetExtensionLicenses(IVssRequestContext requestContext);

    IList<AccountExtensionCount> GetExtensionQuantities(
      IVssRequestContext requestContext,
      UserExtensionLicenseStatus status);

    int GetExtensionQuantities(IVssRequestContext requestContext, string extensionId);

    void TransferUserExtensionLicenses(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap);
  }
}
