// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformAccountMembershipService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PlatformAccountMembershipService : IAccountMembershipService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private ILicensingRepository m_licensingRepository;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
      this.m_licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(requestContext);
      IList<Guid> scopes = this.m_licensingRepository.GetScopes(requestContext);
      if (!scopes.IsNullOrEmpty<Guid>() && scopes.Contains(this.m_serviceHostId))
        return;
      this.m_licensingRepository.CreateScope(requestContext, this.m_serviceHostId);
    }

    public virtual IEnumerable<AccountUser> GetAccountUsers(IVssRequestContext requestContext)
    {
      this.ValidateCollectionContext(requestContext);
      PlatformAccountMembershipService.CheckPermission(requestContext, 1);
      return (IEnumerable<AccountUser>) this.m_licensingRepository.GetEntitlements(requestContext).Where<UserLicense>((Func<UserLicense, bool>) (x => x.Status != AccountUserStatus.Deleted)).Select<UserLicense, AccountUser>((Func<UserLicense, AccountUser>) (x => (AccountUser) x)).ToList<AccountUser>();
    }

    public virtual void AddUserToAccount(
      IVssRequestContext requestContext,
      AccountUser accountUser,
      LicensedIdentity licensedIdentity)
    {
      this.AddUserToAccount(requestContext, accountUser, License.None, LicensingOrigin.None, AssignmentSource.Unknown, licensedIdentity);
    }

    public virtual void AddUserToAccount(
      IVssRequestContext requestContext,
      AccountUser accountUser,
      License license,
      LicensedIdentity licensedIdentity)
    {
      this.AddUserToAccount(requestContext, accountUser, license, LicensingOrigin.None, AssignmentSource.Unknown, licensedIdentity);
    }

    internal virtual void AddUserToAccount(
      IVssRequestContext requestContext,
      AccountUser accountUser,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      LicensedIdentity licensedIdentity)
    {
      this.ValidateCollectionContext(requestContext);
      PlatformAccountMembershipService.ValidateAccountUserParameterForAddOperation(accountUser, requestContext.ServiceHost.InstanceId);
      PlatformAccountMembershipService.CheckPermission(requestContext, 2);
      this.m_licensingRepository.AddUser(requestContext, accountUser.UserId, accountUser.UserStatus, origin, assignmentSource, license, licensedIdentity);
    }

    public virtual void UpdateUserInAccount(
      IVssRequestContext requestContext,
      AccountUser accountUser)
    {
      this.ValidateCollectionContext(requestContext);
      PlatformAccountMembershipService.ValidateAccountUserParameter(accountUser);
      PlatformAccountMembershipService.CheckPermission(requestContext, 4);
      this.m_licensingRepository.UpdateUserStatus(requestContext, accountUser.UserId, accountUser.UserStatus);
      requestContext.GetExtension<ILicensingAccountHandler>().PublishAccountMembershipEvent(requestContext, accountUser, AccountMembershipKind.Updated);
      PlatformAccountMembershipService.TraceUpdate(requestContext, accountUser);
      if (accountUser.UserStatus != AccountUserStatus.Disabled && accountUser.UserStatus != AccountUserStatus.Deleted && accountUser.UserStatus != AccountUserStatus.PendingDisabled && accountUser.UserStatus != AccountUserStatus.Expired)
        return;
      requestContext.GetService<PlatformExtensionEntitlementService>().UnassignmentExtensionOnUserRemove(requestContext, accountUser.UserId);
    }

    public virtual void RemoveUserFromAccount(
      IVssRequestContext requestContext,
      AccountUser accountUser)
    {
      this.ValidateCollectionContext(requestContext);
      PlatformAccountMembershipService.ValidateAccountUserParameter(accountUser);
      PlatformAccountMembershipService.CheckPermission(requestContext, 8);
      this.m_licensingRepository.RemoveUser(requestContext, accountUser.UserId);
      requestContext.GetExtension<ILicensingAccountHandler>().PublishAccountMembershipEvent(requestContext, accountUser, AccountMembershipKind.Removed);
      requestContext.GetService<PlatformExtensionEntitlementService>().UnassignmentExtensionOnUserRemove(requestContext, accountUser.UserId);
    }

    internal virtual AccountUser GetAccountUser(
      IVssRequestContext requestContext,
      Guid userIdentityId)
    {
      ArgumentUtility.CheckForEmptyGuid(userIdentityId, nameof (userIdentityId));
      this.ValidateCollectionContext(requestContext);
      PlatformAccountMembershipService.CheckPermission(requestContext, 1);
      PlatformAccountMembershipService.ReadIdentity(requestContext, userIdentityId);
      return (AccountUser) this.m_licensingRepository.GetEntitlement(requestContext, userIdentityId);
    }

    private static void TraceUpdate(IVssRequestContext requestContext, AccountUser accountUser)
    {
      AccountEntitlement accountEntitlement = requestContext.GetService<IInternalPlatformEntitlementService>().GetAccountEntitlementForAccountUserInternal(requestContext, accountUser.UserId, true, false, false).Item1;
      string licenseSourceName = (string) null;
      string licenseName = (string) null;
      int? licenseId = new int?();
      int? licenseSourceId = new int?();
      if (accountEntitlement != (AccountEntitlement) null)
      {
        licenseSourceName = accountEntitlement.License.Source.ToString();
        licenseName = License.GetLicense(accountEntitlement.License.Source, accountEntitlement.License.GetLicenseAsInt32()).ToString();
        licenseId = new int?(accountEntitlement.License.GetLicenseAsInt32());
        licenseSourceId = new int?((int) accountEntitlement.License.Source);
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        accountUser.UserId
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      Guid userCUID = identity != null ? identity.Cuid() : Guid.Empty;
      if (!requestContext.LicensingTracingEnabled())
        return;
      TeamFoundationTracingService.TraceAccountUserLicensingChanges(accountUser.AccountId, requestContext.ServiceHost.ParentServiceHost.InstanceId, requestContext.ServiceHost.HostType, accountUser.UserId, userCUID, (int) accountUser.UserStatus, licenseSourceId, licenseId, accountUser.UserStatus.ToString(), licenseSourceName, licenseName, DateTime.UtcNow, accountUser.CreationDate.DateTime, DateTime.UtcNow, "Updated");
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      Guid userIdentityId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IdentityService>().ReadIdentityWithFallback(vssRequestContext, userIdentityId) ?? throw new IdentityNotFoundException(userIdentityId);
    }

    private void ValidateCollectionContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) "PlatformAccountMembership", (object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static void ValidateAccountUserParameter(AccountUser accountUser)
    {
      ArgumentUtility.CheckForNull<AccountUser>(accountUser, nameof (accountUser));
      ArgumentUtility.CheckForEmptyGuid(accountUser.AccountId, "accountUser.AccountId");
      ArgumentUtility.CheckForEmptyGuid(accountUser.UserId, "accountUser.UserId");
    }

    private static void ValidateAccountUserParameterForAddOperation(
      AccountUser accountUser,
      Guid accountId)
    {
      PlatformAccountMembershipService.ValidateAccountUserParameter(accountUser);
      if (accountUser.UserStatus == AccountUserStatus.Deleted)
        throw new ArgumentException(string.Format("User cannot be added with status {0}", (object) accountUser.UserStatus), nameof (accountUser));
    }

    private static void CheckPermission(
      IVssRequestContext requestContext,
      int requestedLicensingPermissions)
    {
      IVssRequestContext permissionRequestContext = requestContext.ToLicensingPermissionRequestContext();
      permissionRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(permissionRequestContext, LicensingSecurity.NamespaceId).CheckPermission(permissionRequestContext, LicensingSecurity.MembershipsToken, requestedLicensingPermissions);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
