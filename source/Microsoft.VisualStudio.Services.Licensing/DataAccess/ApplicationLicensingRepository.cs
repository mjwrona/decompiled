// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingRepository
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.Azure.Documents;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingRepository : ILicensingRepository
  {
    private Guid m_serviceHostId;
    private const string s_area = "ApplicationLicensing";
    private const string s_layer = "ApplicationLicensingRepository";
    private const string s_FeatureFlagAddUserWithNoneLicense = "VisualStudio.LicensingService.UseNoneLicenseWhenAddingUser";
    private const string s_FeatureFlagConvertStorageKeys = "VisualStudio.Services.Licensing.ConvertStorageKeys";

    public void Initialize(IVssRequestContext requestContext)
    {
      this.ValidateRequest(requestContext);
      this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
    }

    public IList<Guid> GetScopes(IVssRequestContext requestContext)
    {
      this.ValidateRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetScopes();
    }

    public void CreateScope(IVssRequestContext requestContext, Guid scopeId)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.CreateScope(scopeId);
    }

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeId)
    {
      this.ValidateRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
      {
        List<UserLicense> list = licensingComponent.GetUserLicenses(scopeId).ToList<UserLicense>();
        List<UserLicense> userLicenses = (list != null ? list.Where<UserLicense>((Func<UserLicense, bool>) (lic => lic.AccountId == scopeId)).ToList<UserLicense>() : (List<UserLicense>) null) ?? new List<UserLicense>();
        LicensingEvent licensingEvent = new LicensingEvent()
        {
          EventData = (ILicensingEventData) CommandDataFactory.CreateDeleteLicensesCommandData((IList<UserLicense>) userLicenses)
        };
        licensingComponent.DeleteScope(scopeId, (ILicensingEvent) licensingEvent);
      }
    }

    public void ImportScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses)
    {
      this.ValidateRequest(requestContext);
      LicensingEvent licensingEvent = new LicensingEvent()
      {
        EventData = (ILicensingEventData) CommandDataFactory.CreateImportLicensesCommandData((IList<UserLicense>) (userLicenses ?? new List<UserLicense>()))
      };
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.ImportScope(scopeId, userLicenses, previousUserLicenses, userExtensionLicenses, (ILicensingEvent) licensingEvent);
    }

    public void GetScopeSnapshot(
      IVssRequestContext requestContext,
      Guid scopeId,
      out List<UserLicense> userLicenses,
      out List<UserLicense> previousUserLicenses,
      out List<UserExtensionLicense> userExtLicenses)
    {
      this.ValidateRequest(requestContext);
      using (ApplicationLicensingComponent component = requestContext.CreateComponent<ApplicationLicensingComponent>())
      {
        userLicenses = component.GetUserLicenses(scopeId).ToList<UserLicense>();
        previousUserLicenses = component.GetPreviousUserLicenses(scopeId);
      }
      using (ApplicationExtensionLicensingComponent component = requestContext.CreateComponent<ApplicationExtensionLicensingComponent>())
        userExtLicenses = component.GetUserExtensionLicenses(scopeId).ToList<UserExtensionLicense>();
    }

    public UserLicense GetEntitlement(IVssRequestContext requestContext, Guid userId)
    {
      this.ValidateCollectionRequest(requestContext);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.ConvertStorageKeys"))
        return this.GetEntitlementWithConversion(requestContext, userId);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicense(instanceId, userId);
    }

    public List<UserLicense> GetPreviousUserEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetPreviousUserLicenses(instanceId, userIds);
    }

    public IList<UserLicense> GetEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenses(instanceId, userIds);
    }

    public IList<UserLicense> GetEntitlements(IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      IList<UserLicense> userLicenses;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        userLicenses = licensingComponent.GetUserLicenses(instanceId);
      foreach (UserLicense row in (IEnumerable<UserLicense>) userLicenses)
        ApplicationLicensingRepository.MergeUltimateAndPremiumSku(requestContext, row);
      return userLicenses;
    }

    public IList<UserLicense> GetEntitlements(IVssRequestContext requestContext, int top, int skip)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      IList<UserLicense> userLicenses;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        userLicenses = licensingComponent.GetUserLicenses(instanceId, top, skip);
      foreach (UserLicense row in (IEnumerable<UserLicense>) userLicenses)
        ApplicationLicensingRepository.MergeUltimateAndPremiumSku(requestContext, row);
      return userLicenses;
    }

    public IPagedList<UserLicense> GetFilteredEntitlements(
      IVssRequestContext requestContext,
      string continuation,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      int searchConfiguration = new LicensingConfigurationRegistryManager().GetAccountEntitlementSearchConfiguration(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetFilteredUserLicenses(continuation, searchConfiguration, filter, sort);
    }

    private static UserLicense MergeUltimateAndPremiumSku(
      IVssRequestContext requestContext,
      UserLicense row)
    {
      if (row.Source == LicensingSource.Msdn && (row.License == 5 || row.License == 6))
        row.License = 7;
      return row;
    }

    public void DeleteEntitlement(IVssRequestContext requestContext, Guid userId) => this.RemoveUser(requestContext, userId);

    public UserLicense AssignEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      LicensedIdentity licensedIdentity)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      userId = this.NormalizeUserId(requestContext, userId);
      LicensingEvent licensingEvent = new LicensingEvent()
      {
        EventData = (ILicensingEventData) CommandDataFactory.CreateSetLicenseCommandData(userId, license, statusIfAbsent, assignmentSource)
      };
      UserLicense userLicense;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        userLicense = licensingComponent.SetUserLicense(instanceId, userId, license.Source, license.GetLicenseAsInt32(), origin, assignmentSource, statusIfAbsent, (ILicensingEvent) licensingEvent, licensedIdentity);
      requestContext.GetExtension<ILicensingAccountHandler>()?.PublishUserLicenseChangedEvent(requestContext.RootContext, userId, license);
      return userLicense;
    }

    public IList<AccountLicenseCount> GetUserLicenseDistribution(IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicensesDistribution(instanceId) ?? (IList<AccountLicenseCount>) new List<AccountLicenseCount>();
    }

    public IList<UserLicenseCount> GetUserLicenseUsage(IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseUsage(instanceId) ?? (IList<UserLicenseCount>) new List<UserLicenseCount>();
    }

    public void UpdateUserLastAccessed(
      IVssRequestContext requestContext,
      Guid userId,
      DateTimeOffset lastAccessed)
    {
      this.ValidateCollectionRequest(requestContext);
      userId = this.NormalizeUserId(requestContext, userId);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.UpdateUserLastAccessed(instanceId, userId, lastAccessed);
    }

    public void AddUser(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus status,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      License license,
      LicensedIdentity licensedIdentity)
    {
      this.ValidateCollectionRequest(requestContext);
      this.AddUserToLicensedUsersGroup(requestContext, userId, status);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      userId = this.NormalizeUserId(requestContext, userId);
      license = requestContext.IsFeatureEnabled("VisualStudio.LicensingService.UseNoneLicenseWhenAddingUser") ? License.None : license;
      LicensingEvent licensingEvent = new LicensingEvent()
      {
        EventData = (ILicensingEventData) CommandDataFactory.CreateAddUserCommandData(userId, status, license, assignmentSource)
      };
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.AddUser(instanceId, userId, status, license, assignmentSource, origin, (ILicensingEvent) licensingEvent, licensedIdentity);
      requestContext.GetExtension<ILicensingAccountHandler>().PublishUserStatusChangedEvent(requestContext, userId, status, AccountMembershipKind.Added);
      IVssRequestContext deployment = requestContext.ToDeployment();
      deployment.GetService<ITeamFoundationSqlNotificationService>().SendNotification(deployment, SqlNotificationEventClasses.UserAddedToAccount, userId.ToString());
    }

    public void UpdateUserStatus(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus status)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      userId = this.NormalizeUserId(requestContext, userId);
      if (status == AccountUserStatus.Deleted)
      {
        this.RemoveUser(requestContext, userId);
      }
      else
      {
        LicensingEvent licensingEvent = new LicensingEvent()
        {
          EventData = (ILicensingEventData) CommandDataFactory.CreateUpdateUserCommandData(userId, status, AssignmentSource.None)
        };
        using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
          licensingComponent.UpdateUserStatus(instanceId, userId, status, (ILicensingEvent) licensingEvent);
        this.AddUserToLicensedUsersGroup(requestContext, userId, status);
        requestContext.GetExtension<ILicensingAccountHandler>().PublishUserStatusChangedEvent(requestContext.RootContext, userId, status, AccountMembershipKind.Updated);
      }
    }

    public void RemoveUser(IVssRequestContext requestContext, Guid userId)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      userId = this.NormalizeUserId(requestContext, userId);
      LicensingEvent licensingEvent = new LicensingEvent()
      {
        EventData = (ILicensingEventData) CommandDataFactory.CreateRemoveUserCommandData(userId)
      };
      try
      {
        using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
          licensingComponent.DeleteUserLicense(instanceId, userId, (ILicensingEvent) licensingEvent);
      }
      catch (AccountUserNotFoundException ex)
      {
      }
      this.RemoveUserFromLicensedUsersGroup(requestContext, userId);
      requestContext.GetExtension<ILicensingAccountHandler>().PublishUserStatusChangedEvent(requestContext.RootContext, userId, AccountUserStatus.Deleted, AccountMembershipKind.Removed);
      IVssRequestContext deployment = requestContext.ToDeployment();
      deployment.GetService<ITeamFoundationSqlNotificationService>().SendNotification(deployment, SqlNotificationEventClasses.UserRemovedFromAccount, userId.ToString());
    }

    public void AssignExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      Guid currentCollectionId = this.GetCurrentCollectionId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        licensingComponent.AssignExtensionLicenseToUserBatch(extensionScopeId, (IEnumerable<Guid>) userIds, extensionId, source, assignmentSource, currentCollectionId);
      ILicensingAccountHandler extension = requestContext.GetExtension<ILicensingAccountHandler>();
      foreach (Guid userId in (IEnumerable<Guid>) userIds)
        extension?.PublishUserExtensionChangedEvent(requestContext.RootContext, userId, extensionId);
    }

    public IDictionary<Guid, IList<ExtensionSource>> GetExtensions(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.GetExtensionsForUsersBatch(extensionScopeId, userIds);
    }

    public IList<UserExtensionLicense> GetExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.FilterUsersWithExtensionBatch(extensionScopeId, (IEnumerable<Guid>) userIds, extensionId);
    }

    public IList<UserExtensionLicense> GetExtensionLicenses(
      IVssRequestContext requestContext,
      Guid userId,
      UserExtensionLicenseStatus status)
    {
      this.ValidateRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.GetUserExtensionLicenses(extensionScopeId, userId, status);
    }

    public IList<UserExtensionLicense> GetExtensionLicenses(IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.GetUserExtensionLicenses(extensionScopeId);
    }

    public int UpdateExtensionLicenses(
      IVssRequestContext requestContext,
      Guid userId,
      IList<string> extensions,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      Guid currentCollectionId = this.GetCurrentCollectionId(requestContext);
      int userBatchWithCount;
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        userBatchWithCount = licensingComponent.UpdateExtensionsAssignedToUserBatchWithCount(extensionScopeId, userId, (IEnumerable<string>) extensions, source, assignmentSource, currentCollectionId);
      ILicensingAccountHandler extension1 = requestContext.GetExtension<ILicensingAccountHandler>();
      foreach (string extension2 in (IEnumerable<string>) extensions)
        extension1?.PublishUserExtensionChangedEvent(requestContext.RootContext, userId, extension2);
      return userBatchWithCount;
    }

    public void UnassignExtensionLicenses(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId,
      LicensingSource source)
    {
      this.ValidateRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        licensingComponent.UpdateUserStatusBatch(extensionScopeId, (IEnumerable<Guid>) userIds, extensionId, UserExtensionLicenseStatus.NotActive, source, AssignmentSource.None, Guid.Empty);
    }

    public IList<AccountExtensionCount> GetExtensionQuantities(
      IVssRequestContext requestContext,
      UserExtensionLicenseStatus status)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.GetAccountExtensionCount(extensionScopeId, UserExtensionLicenseStatus.Active);
    }

    public int GetExtensionQuantities(IVssRequestContext requestContext, string extensionId)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        return licensingComponent.GetExtensionUsageCountInAccount(extensionScopeId, extensionId);
    }

    public void TransferUserLicenses(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.TransferUserLicenses(extensionScopeId, userIdTransferMap);
    }

    public void TransferUserExtensionLicenses(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      this.ValidateCollectionRequest(requestContext);
      Guid extensionScopeId = ApplicationLicensingRepository.GetExtensionScopeId(requestContext);
      using (IExtensionLicensingComponent licensingComponent = ApplicationLicensingRepository.CreateExtensionLicensingComponent(requestContext))
        licensingComponent.TransferUserExtensionLicenses(extensionScopeId, userIdTransferMap);
    }

    public UserLicenseCosmosSerializableDocument UpsertUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      bool optimisticConcurrency = false)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.UpsertUserLicenseCosmosDocument(document, optimisticConcurrency);
    }

    public UserLicenseCosmosSerializableDocument UpdateUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool optimisticConcurrency = false)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.UpdateUserLicenseCosmosDocument(document, documentId, optimisticConcurrency);
    }

    public void DeleteUserDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      bool optimisticConcurrency = false)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        licensingComponent.DeleteUserLicenseCosmosDocument(document, optimisticConcurrency);
    }

    public UserLicenseCosmosSerializableDocument GetUserDocument(
      IVssRequestContext requestContext,
      Guid userId)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCosmosDocument(userId);
    }

    public UserLicenseCosmosSerializableDocument GetUserDocumentByIdAndPreviousId(
      IVssRequestContext requestContext,
      Guid enterpriseStorageKey)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCosmosDocumentByIdAndPreviousId(enterpriseStorageKey);
    }

    public IEnumerable<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext,
      IEnumerable<Guid> userIds)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCosmosDocuments(userIds);
    }

    public IEnumerable<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCosmosDocuments();
    }

    public IPagedList<UserLicenseCosmosSerializableDocument> GetUserDocuments(
      IVssRequestContext requestContext,
      string continuation)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetPagedUserLicenseCosmosDocuments(continuation);
    }

    public int GetLicenseCount(IVssRequestContext requestContext)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCount();
    }

    public int GetLicenseCount(
      IVssRequestContext requestContext,
      LicensingSource source,
      int license)
    {
      this.ValidateCollectionRequest(requestContext);
      using (ILicensingComponent licensingComponent = ApplicationLicensingRepository.CreateLicensingComponent(requestContext))
        return licensingComponent.GetUserLicenseCount(source, license);
    }

    private Guid NormalizeUserId(IVssRequestContext requestContext, Guid userId)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.ConvertStorageKeys"))
        return userId;
      UserLicense entitlementWithConversion = this.GetEntitlementWithConversion(requestContext, userId);
      return entitlementWithConversion != null ? entitlementWithConversion.UserId : Microsoft.VisualStudio.Services.Licensing.ValidationHelper.ValidateCollectionLevelStorageKey(requestContext, userId, true).Id;
    }

    private static Guid GetExtensionScopeId(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? requestContext.ServiceHost.OrganizationServiceHost.InstanceId : requestContext.ServiceHost.InstanceId;

    private void ValidateCollectionRequest(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) nameof (ApplicationLicensingRepository), (object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private void ValidateRequest(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && !requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      }
      else if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) && !requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      if (this.m_serviceHostId != Guid.Empty && !this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) nameof (ApplicationLicensingRepository), (object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private Guid GetCurrentCollectionId(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) ? requestContext.ServiceHost.CollectionServiceHost.InstanceId : Guid.Empty;

    private static ILicensingComponent CreateLicensingComponent(IVssRequestContext requestContext)
    {
      IVssRequestContext componentRequestContext = requestContext.ToLicensingComponentRequestContext();
      return !requestContext.UseCosmosDb() ? (ILicensingComponent) componentRequestContext.CreateComponent<ApplicationLicensingComponent>() : CosmosDBLicensingStore.CreateLicensingComponent(componentRequestContext);
    }

    private static IExtensionLicensingComponent CreateExtensionLicensingComponent(
      IVssRequestContext requestContext)
    {
      IVssRequestContext componentRequestContext = requestContext.ToLicensingComponentRequestContext();
      return !requestContext.UseCosmosDb() ? (IExtensionLicensingComponent) componentRequestContext.CreateComponent<ApplicationExtensionLicensingComponent>() : CosmosDBLicensingStore.CreateExtensionLicensingComponent(componentRequestContext);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      Guid userIdentityId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IdentityService>().ReadIdentityWithFallback(vssRequestContext, userIdentityId, includeRestrictedVisibility: true) ?? throw new IdentityNotFoundException(userIdentityId);
    }

    private void AddUserToLicensedUsersGroup(
      IVssRequestContext requestContext,
      Guid userId,
      AccountUserStatus status)
    {
      if (status == AccountUserStatus.Deleted)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity = ApplicationLicensingRepository.ReadIdentity(requestContext, userId);
      this.AddUserToLicensedUsersGroup(requestContext, identity);
    }

    private void AddUserToLicensedUsersGroup(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.ValidateCollectionRequest(requestContext);
      try
      {
        requestContext.GetService<IdentityService>().AddMemberToGroup(requestContext.Elevate(), GroupWellKnownIdentityDescriptors.LicensedUsersGroup, identity.Descriptor);
      }
      catch (IdentityAccountNameAlreadyInUseException ex)
      {
        requestContext.Trace(5001313, TraceLevel.Warning, "ApplicationLicensing", nameof (ApplicationLicensingRepository), "Unable to add identity '{0}' into Licensed Users group", (object) identity.Descriptor.ToString());
        requestContext.TraceException(5001314, TraceLevel.Warning, "ApplicationLicensing", nameof (ApplicationLicensingRepository), (Exception) ex);
      }
    }

    private void RemoveUserFromLicensedUsersGroup(
      IVssRequestContext requestContext,
      Guid userIdentityId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = ApplicationLicensingRepository.ReadIdentity(requestContext, userIdentityId);
      this.RemoveUserFromLicensedUsersGroup(requestContext, identity.Descriptor);
    }

    private void RemoveUserFromLicensedUsersGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      this.ValidateCollectionRequest(requestContext);
      requestContext.GetService<IdentityService>().RemoveMemberFromGroup(requestContext.Elevate(), GroupWellKnownIdentityDescriptors.LicensedUsersGroup, identityDescriptor);
    }

    private static Guid ConvertToCollectionStorageKey(
      IVssRequestContext requestContext,
      Guid userId)
    {
      return (Microsoft.VisualStudio.Services.Licensing.ValidationHelper.ValidateCollectionLevelStorageKey(requestContext, userId) ?? throw new ArgumentException(string.Format("Could not compute collection level key for {0}", (object) userId), nameof (userId))).Id;
    }

    private static Guid ConvertToEnterpriseStorageKey(
      IVssRequestContext requestContext,
      Guid userId)
    {
      return (requestContext.GetService<IdentityService>().ReadIdentityWithFallback(requestContext, userId) ?? throw new ArgumentException(string.Format("Could not find identity with storage key {0}", (object) userId), nameof (userId))).EnterpriseStorageKey(requestContext);
    }

    private UserLicense GetEntitlementWithConversion(IVssRequestContext requestContext, Guid userId)
    {
      if (!requestContext.IsLps())
        return this.GetEntitlement(requestContext, userId);
      Guid collectionStorageKey = ApplicationLicensingRepository.ConvertToCollectionStorageKey(requestContext, userId);
      UserLicenseCosmosSerializableDocument document1 = this.GetUserDocument(requestContext, collectionStorageKey);
      if (document1 != null)
      {
        if (!document1.Document.IsCollectionLevel)
        {
          document1.Document.IsCollectionLevel = true;
          document1 = this.UpdateUserDocument(requestContext, document1, (string) null, false);
        }
        return document1.Document.License;
      }
      Guid enterpriseStorageKey = ApplicationLicensingRepository.ConvertToEnterpriseStorageKey(requestContext, userId);
      UserLicenseCosmosSerializableDocument document2 = this.GetUserDocument(requestContext, enterpriseStorageKey) ?? this.GetUserDocumentByIdAndPreviousId(requestContext, enterpriseStorageKey);
      if (document2 != null)
        this.ConvertToCollectionLevelDocument(requestContext, document2, collectionStorageKey);
      return document2?.Document?.License;
    }

    public void ConvertToCollectionLevelDocument(
      IVssRequestContext requestContext,
      UserLicenseCosmosSerializableDocument document,
      Guid collectionLevelId)
    {
      string id = ((Resource) document).Id;
      if (document.Document.UserId != collectionLevelId)
        document = document.Transfer(collectionLevelId);
      document.Document.IsCollectionLevel = true;
      this.UpdateUserDocument(requestContext, document, id, false);
    }
  }
}
