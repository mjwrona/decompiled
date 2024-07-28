// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.EntitlementProcessor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.Licensing.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class EntitlementProcessor
  {
    private readonly IVssRequestContext m_collectionContext;
    private readonly ILicensingRepository m_licensingRepository;
    private readonly IInternalPlatformEntitlementService m_entitlementService;
    private readonly IdentityService m_identityService;
    private readonly PlatformAccountMembershipService m_membershipService;
    private readonly IOfferSubscriptionService m_meteringService;
    private readonly AzCommerceService m_azCommService;
    private ICollection<UserLicense> m_userLicenses;
    private readonly SortedDictionary<AccountLicense, EntitlementProcessor.LicenseUsage> m_counts = new SortedDictionary<AccountLicense, EntitlementProcessor.LicenseUsage>((IComparer<AccountLicense>) EntitlementProcessor.AccountLicenseAvailabilityComparer.Instance);
    private readonly Dictionary<Guid, License> m_pendingAssignments = new Dictionary<Guid, License>();
    private Dictionary<Guid, AccountEntitlement> m_existingEntitlements = new Dictionary<Guid, AccountEntitlement>();
    private Dictionary<Guid, AccountUser> m_users = new Dictionary<Guid, AccountUser>();
    private readonly Guid m_collectionId;
    private Guid m_collectionOwner;
    private bool m_isCollectionEnabled;
    private EntitlementProcessor.RequestedData m_loadedData;
    private EntitlementProcessor.RequestedData m_pendingData;
    private bool m_allowReconcileToReenable;
    private const string s_area = "EntitlementProcessor";
    private const string s_layer = "BussinessLogic";

    public EntitlementProcessor(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        throw LicensingTrace.Log.EntitlementProcessor_WrongHostType(requestContext, new UnexpectedHostTypeException(requestContext.ServiceHost.HostType));
      this.m_collectionContext = requestContext.IsSystemContext ? requestContext : throw LicensingTrace.Log.EntitlementProcessor_WrongContext(requestContext, new InvalidOperationException(LicensingResources.LicensingOperationFailed()));
      this.m_collectionId = requestContext.ServiceHost.InstanceId;
      this.m_licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(requestContext);
      this.m_entitlementService = requestContext.GetService<IInternalPlatformEntitlementService>();
      this.m_membershipService = requestContext.GetService<PlatformAccountMembershipService>();
      this.m_meteringService = requestContext.GetService<IOfferSubscriptionService>();
      this.m_azCommService = requestContext.GetService<AzCommerceService>();
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    public virtual void Reconcile()
    {
      using (LicensingTrace.Log.EntitlementProcessor_ReconcileEnterLeave(this.m_collectionContext))
      {
        this.Reset();
        try
        {
          this.m_allowReconcileToReenable = true;
          this.OnDemand(EntitlementProcessor.RequestedData.Reconcile);
        }
        finally
        {
          this.m_allowReconcileToReenable = false;
        }
      }
    }

    public virtual bool Assign(
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      return this.Assign(userId, license, origin, assignmentSource, out AccountEntitlement _);
    }

    public virtual bool Assign(
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      out AccountEntitlement entitlement)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForNull<License>(license, nameof (license));
      entitlement = (AccountEntitlement) null;
      using (LicensingTrace.Log.EntitlementProcessor_AssignEnterLeave(this.m_collectionContext))
      {
        if (!this.IsCollectionValid())
        {
          LicensingTrace.Log.EntitlementProcessor_AssignCollectionInvalid(this.m_collectionContext);
          return false;
        }
        if (license == (License) AccountLicense.EarlyAdopter)
          return false;
        if (license == License.None)
          this.Revoke(userId, out entitlement);
        LicensingTrace.Log.EntitlementProcessor_AttemptAssignLicenseToUser(this.m_collectionContext, license, userId);
        AccountUser validUser = this.GetValidUser(userId, true);
        EntitlementProcessor.ProcessAssignDecision decision = this.TryProcessAssign(validUser, license, assignmentSource);
        LicensingTrace.Log.EntitlementProcessor_ReachedAssignmentDecision(this.m_collectionContext, (object) decision);
        switch (decision)
        {
          case EntitlementProcessor.ProcessAssignDecision.Assign:
            this.ProcessAssign(validUser, license);
            LicensedIdentity licensedIdentity1 = this.ReadLicensedIdentity(validUser.UserId);
            this.EnsureUserInCollectionWithLicense(validUser, license, licensedIdentity1);
            entitlement = this.AssignCommon(validUser, license, origin, assignmentSource);
            this.TryEnableUser(validUser);
            return true;
          case EntitlementProcessor.ProcessAssignDecision.AlreadyHasLicense:
            this.TryEnableUser(validUser);
            entitlement = this.m_existingEntitlements[validUser.UserId];
            return true;
          case EntitlementProcessor.ProcessAssignDecision.OverwriteAssignmentSource:
            LicensedIdentity licensedIdentity2 = this.ReadLicensedIdentity(validUser.UserId);
            this.EnsureUserInCollectionWithLicense(validUser, license, licensedIdentity2);
            entitlement = this.AssignCommon(validUser, license, origin, assignmentSource);
            this.TryEnableUser(validUser);
            return true;
          default:
            return false;
        }
      }
    }

    public virtual License AssignAvailable(Guid userId) => this.AssignAvailable(userId, false);

    public virtual License AssignAvailable(Guid userId, bool overwrite)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      using (LicensingTrace.Log.EntitlementProcessor_AssignAvailableEnterLeave(this.m_collectionContext))
      {
        if (this.IsCollectionValid())
          return this.AssignAvailableCore(this.GetValidUser(userId, true), overwrite);
        LicensingTrace.Log.EntitlementProcessor_AssignAvailableCollectionInvalid(this.m_collectionContext);
        return License.None;
      }
    }

    public virtual bool Revoke(Guid userId) => this.Revoke(userId, out AccountEntitlement _);

    public virtual bool Revoke(Guid userId, out AccountEntitlement entitlement)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      entitlement = (AccountEntitlement) null;
      using (LicensingTrace.Log.EntitlementProcessor_RevokeEnterLeave(this.m_collectionContext))
      {
        if (!this.IsCollectionValid())
        {
          LicensingTrace.Log.EntitlementProcessor_RevokeCollectionInvalid(this.m_collectionContext);
          return false;
        }
        AccountUser validUser = this.GetValidUser(userId, true);
        int num = this.TryProcessRevoke(validUser) ? 1 : 0;
        if (num != 0)
          entitlement = this.RevokeCommon(validUser);
        return num != 0;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual AccountUser GetUserCore(Guid userId) => this.m_membershipService.GetAccountUser(this.m_collectionContext, userId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual void AddUserCore(
      AccountUser user,
      License license,
      LicensedIdentity licensedIdentity)
    {
      this.m_membershipService.AddUserToAccount(this.m_collectionContext, user, license, licensedIdentity);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual void UpdateUserCore(AccountUser user) => this.m_membershipService.UpdateUserInAccount(this.m_collectionContext, user);

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual AccountEntitlement AssignCore(
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      return this.m_entitlementService.AssignAccountEntitlementInternal(this.m_collectionContext, userId, license, origin, assignmentSource);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual AccountEntitlement GetExsitingEntitlementCore(AccountUser user) => this.m_entitlementService.GetAccountEntitlementForAccountUserInternal(this.m_collectionContext, user.UserId, true, false, false).Item1;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual IEnumerable<IOfferSubscription> GetMeteringResourcesCore()
    {
      IEnumerable<IOfferSubscription> offerSubscriptions = this.m_meteringService.GetOfferSubscriptions(this.m_collectionContext);
      CommonUtil.ValidateOfferSubscriptions(this.m_collectionContext, offerSubscriptions);
      return (IEnumerable<IOfferSubscription>) offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (m => m.OfferMeter.IsLicenseCategory())).ToList<IOfferSubscription>();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual Guid GetCollectionOwner() => CollectionHelper.GetCollectionOwner(this.m_collectionContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool IsCollectionEnabled() => CollectionHelper.IsCollectionEnabled(this.m_collectionContext);

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual IEnumerable<UserLicense> GetUserLicensesCore() => (IEnumerable<UserLicense>) this.m_userLicenses ?? (IEnumerable<UserLicense>) (this.m_userLicenses = (ICollection<UserLicense>) this.m_licensingRepository.GetEntitlements(this.m_collectionContext));

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual IEnumerable<AccountUser> GetUsersCore() => (IEnumerable<AccountUser>) this.GetUserLicensesCore().Where<UserLicense>((Func<UserLicense, bool>) (x => x.Status != AccountUserStatus.Deleted)).Select<UserLicense, AccountUser>((Func<UserLicense, AccountUser>) (x => (AccountUser) x)).ToList<AccountUser>();

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual IEnumerable<AccountEntitlement> GetEntitlementsCore() => (IEnumerable<AccountEntitlement>) this.GetUserLicensesCore().Select<UserLicense, AccountEntitlement>((Func<UserLicense, AccountEntitlement>) (x => (AccountEntitlement) x)).ToList<AccountEntitlement>();

    internal virtual License GetMsdnLicenseCore(AccountUser user) => this.m_entitlementService.GetMsdnLicenseForUser(this.m_collectionContext, user.UserId);

    internal virtual License CheckForEarlyAdopterLicense()
    {
      License license = (License) null;
      if (this.m_entitlementService.IsInternalHost(this.m_collectionContext))
        license = (License) AccountLicense.EarlyAdopter;
      return license;
    }

    private void Process()
    {
      using (LicensingTrace.Log.EntitlementProcessor_ProcessEnterLeave(this.m_collectionContext))
      {
        this.m_pendingAssignments.Clear();
        if (!this.IsCollectionValid())
        {
          LicensingTrace.Log.EntitlementProcessor_ProcessCollectionInvalid(this.m_collectionContext);
        }
        else
        {
          this.OnDemand(EntitlementProcessor.RequestedData.Collection | EntitlementProcessor.RequestedData.CollectionUsers | EntitlementProcessor.RequestedData.CollectionEntitlements | EntitlementProcessor.RequestedData.MeteredUsage);
          foreach (var data in this.m_existingEntitlements.Values.Join((IEnumerable<AccountUser>) this.m_users.Values, (Func<AccountEntitlement, Guid>) (entitlement => entitlement.UserId), (Func<AccountUser, Guid>) (user => user.UserId), (entitlement, user) => new
          {
            entitlement = entitlement,
            user = user
          }).Where(_param1 => _param1.user.IsLicensable() && this.IsValidEntitlement(_param1.entitlement)).OrderByDescending(_param1 => _param1.entitlement.UserId == this.m_collectionOwner).ThenBy(_param1 => _param1.entitlement.AssignmentDate).Select(_param1 => new
          {
            License = _param1.entitlement.License,
            AssignmentSource = _param1.entitlement.AssignmentSource,
            User = _param1.user
          }))
          {
            AccountUser user = data.User;
            AssignmentSource assignmentSource = data.AssignmentSource;
            License license = data.License;
            switch (this.TryProcessAssign(user, license, assignmentSource))
            {
              case EntitlementProcessor.ProcessAssignDecision.Assign:
              case EntitlementProcessor.ProcessAssignDecision.AlreadyHasLicense:
                if (!user.IsDisabled() || this.m_allowReconcileToReenable)
                {
                  this.ProcessAssign(user, license);
                  this.TryEnableUser(user);
                  continue;
                }
                continue;
              case EntitlementProcessor.ProcessAssignDecision.LicenseUnavailable:
                if (this.IsOwnerUser(user))
                {
                  LicensingTrace.Log.EntitlementProcessor_ProcessNoLicenseForCollectionOwner(this.m_collectionContext);
                  this.AssignAvailableCore(user, true);
                  continue;
                }
                this.DisableUser(user);
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    private AccountUser GetValidUser(Guid userId, bool ensureUser = false)
    {
      AccountUser user;
      if (!this.m_users.TryGetValue(userId, out user) && !this.HasData(EntitlementProcessor.RequestedData.CollectionUsers))
      {
        user = this.GetUserCore(userId);
        if (user != null)
          this.m_users[user.UserId] = user;
      }
      if (user == null & ensureUser)
        user = new AccountUser(this.m_collectionId, userId)
        {
          UserStatus = AccountUserStatus.Pending
        };
      else if (user == null || !user.IsLicensable())
        throw LicensingTrace.Log.EntitlementProcessor_GetValidCollectionUser_InvalidUserId(this.m_collectionContext, new InvalidLicensingOperation(LicensingResources.InvalidUserId((object) userId)));
      return user;
    }

    private bool IsCollectionValid()
    {
      this.OnDemand(EntitlementProcessor.RequestedData.Collection);
      return this.m_isCollectionEnabled;
    }

    private bool IsOwnerUser(AccountUser user) => this.IsOwnerUser(user.UserId);

    private bool IsOwnerUser(Guid userId)
    {
      this.OnDemand(EntitlementProcessor.RequestedData.Collection);
      return userId == this.m_collectionOwner;
    }

    private bool IsValidEntitlement(AccountEntitlement entitlement) => entitlement.License != License.None;

    private bool TryEnableUser(AccountUser user)
    {
      AccountUserStatus userStatus = user.UserStatus;
      switch (userStatus)
      {
        case AccountUserStatus.Disabled:
          user.UserStatus = AccountUserStatus.Active;
          break;
        case AccountUserStatus.PendingDisabled:
          user.UserStatus = AccountUserStatus.Pending;
          break;
        default:
          return false;
      }
      LicensingTrace.Log.EntitlementProcessor_EnablingUser(this.m_collectionContext, user.UserId, userStatus, user.UserStatus);
      this.UpdateUserCore(user);
      return true;
    }

    private void EnsureUserInCollectionWithLicense(
      AccountUser user,
      License license,
      LicensedIdentity licensedIdentity)
    {
      if (this.m_users.ContainsKey(user.UserId))
        return;
      LicensingTrace.Log.EntitlementProcessor_AddingUser(this.m_collectionContext, user.UserId);
      this.AddUserCore(user, license, licensedIdentity);
      this.m_users[user.UserId] = user;
    }

    private void DisableUser(AccountUser user)
    {
      AccountUserStatus userStatus = user.UserStatus;
      switch (userStatus)
      {
        case AccountUserStatus.Active:
          user.UserStatus = AccountUserStatus.Disabled;
          break;
        case AccountUserStatus.Pending:
          user.UserStatus = AccountUserStatus.PendingDisabled;
          break;
        default:
          return;
      }
      LicensingTrace.Log.EntitlementProcessor_DisablingUser(this.m_collectionContext, user.UserId, userStatus, user.UserStatus);
      this.UpdateUserCore(user);
    }

    private AccountEntitlement AssignCommon(
      AccountUser user,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource)
    {
      LicensingTrace.Log.EntitlementProcessor_AssignLicense(this.m_collectionContext, user.UserId, license);
      AccountEntitlement accountEntitlement = this.AssignCore(user.UserId, license, origin, assignmentSource);
      this.m_existingEntitlements[user.UserId] = accountEntitlement;
      return accountEntitlement;
    }

    private AccountEntitlement RevokeCommon(AccountUser user)
    {
      LicensingTrace.Log.EntitlementProcessor_RevokeLicense(this.m_collectionContext, user.UserId);
      AccountEntitlement accountEntitlement = this.AssignCore(user.UserId, License.None, LicensingOrigin.None, AssignmentSource.Unknown);
      this.m_existingEntitlements.Remove(user.UserId);
      return accountEntitlement;
    }

    private License AssignAvailableCore(AccountUser user, bool overwrite)
    {
      License license1 = (License) null;
      AssignmentSource source1 = AssignmentSource.Unknown;
      AssignmentSource source2 = AssignmentSource.Unknown;
      License license2;
      if (this.UserHasValidLicense(user, out license2, out source2) && license2 != (License) MsdnLicense.Eligible)
      {
        if (!overwrite)
          return license2;
        license1 = license2;
        source1 = source2;
        this.TryProcessRevoke(user);
      }
      License license3 = this.CheckForEarlyAdopterLicense();
      if (License.IsNullOrNone(license3) && this.IsOwnerUser(user))
        license3 = this.GetMsdnLicenseCore(user);
      if (License.IsNullOrNone(license3))
      {
        this.OnDemand(EntitlementProcessor.RequestedData.Reconcile);
        license3 = (License) this.GetLicenseWithAvailableUses();
      }
      if (License.IsNullOrNone(license3) && this.IsOwnerUser(user))
        license3 = (License) this.ReserveLicenseForCollectionOwner();
      if (license3 != (License) null)
      {
        switch (this.TryProcessAssign(user, license3, source2))
        {
          case EntitlementProcessor.ProcessAssignDecision.Assign:
            this.ProcessAssign(user, license3);
            LicensedIdentity licensedIdentity1 = this.ReadLicensedIdentity(user.UserId);
            this.EnsureUserInCollectionWithLicense(user, license3, licensedIdentity1);
            this.AssignCommon(user, license3, LicensingOrigin.None, source2);
            this.TryEnableUser(user);
            break;
          case EntitlementProcessor.ProcessAssignDecision.AlreadyHasLicense:
            this.TryEnableUser(user);
            break;
          case EntitlementProcessor.ProcessAssignDecision.OverwriteAssignmentSource:
            LicensedIdentity licensedIdentity2 = this.ReadLicensedIdentity(user.UserId);
            this.EnsureUserInCollectionWithLicense(user, license3, licensedIdentity2);
            this.AssignCommon(user, license3, LicensingOrigin.None, source2);
            this.TryEnableUser(user);
            break;
          default:
            if (license1 != (License) null)
            {
              int num = (int) this.TryProcessAssign(user, license1, source1);
            }
            LicensingTrace.Log.EntitlementProcessor_NoLicensesAvailable(this.m_collectionContext, user.UserId);
            license3 = (License) null;
            break;
        }
      }
      License license4 = license3;
      return (object) license4 != null ? license4 : License.None;
    }

    private AccountLicense ReserveLicenseForCollectionOwner()
    {
      AccountLicense committedQuantity = this.GetLeastLicenseWithCommittedQuantity();
      if ((License) committedQuantity != (License) null)
      {
        EntitlementProcessor.LicenseUsage usage = this.GetUsage(committedQuantity);
        if (usage.RemainingQuantity <= 0)
        {
          Guid guid = usage.Usages.Last<Guid>();
          this.DisableUser(this.GetValidUser(guid));
          usage.Usages.Remove(guid);
          this.m_pendingAssignments.Remove(guid);
        }
      }
      return committedQuantity;
    }

    private AccountLicense GetLicenseWithAvailableUses()
    {
      this.OnDemand(EntitlementProcessor.RequestedData.MeteredUsage);
      return this.m_counts.Select(entry => new
      {
        entry = entry,
        license = entry.Key
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        usage = _param1.entry.Value
      }).Where(_param1 => this.IsReservableLicense(_param1.\u003C\u003Eh__TransparentIdentifier0.license) && _param1.usage.RemainingQuantity > 0).Select(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier0.license).FirstOrDefault<AccountLicense>();
    }

    private AccountLicense GetLeastLicenseWithCommittedQuantity()
    {
      this.OnDemand(EntitlementProcessor.RequestedData.MeteredUsage);
      return this.m_counts.Select(entry => new
      {
        entry = entry,
        license = entry.Key
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        usage = _param1.entry.Value
      }).Where(_param1 => this.IsReservableLicense(_param1.\u003C\u003Eh__TransparentIdentifier0.license) && _param1.usage.CommittedQuantity > 0).Select(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier0.license).FirstOrDefault<AccountLicense>();
    }

    private bool IsReservableLicense(AccountLicense license) => (License) license == (License) AccountLicense.Express || (License) license == (License) AccountLicense.Professional || (License) license == (License) AccountLicense.Advanced;

    private bool DoesUserHaveLicenseWithSource(
      AccountUser user,
      License license,
      AssignmentSource source)
    {
      License license1;
      AssignmentSource source1;
      return this.UserHasValidLicense(user, out license1, out source1) && license1 == license && source1 == source;
    }

    private bool DoesUserHaveLicense(AccountUser user, License license)
    {
      License license1;
      return this.UserHasValidLicense(user, out license1, out AssignmentSource _) && license1 == license;
    }

    private bool UserHasValidLicense(AccountUser user, out License license)
    {
      License license1;
      int num = this.UserHasValidLicense(user, out license1, out AssignmentSource _) ? 1 : 0;
      license = license1;
      return num != 0;
    }

    private bool UserHasValidLicense(
      AccountUser user,
      out License license,
      out AssignmentSource source)
    {
      license = (License) null;
      source = AssignmentSource.Unknown;
      if (user.IsDisabled())
        return false;
      License license1;
      if (this.m_pendingAssignments.TryGetValue(user.UserId, out license1))
      {
        if (!(license1 != License.None))
          return false;
        license = license1;
        return true;
      }
      if (!this.HasData(EntitlementProcessor.RequestedData.Reconcile) && !this.IsRequestingData(EntitlementProcessor.RequestedData.Reconcile))
      {
        AccountEntitlement exsitingEntitlementCore = this.GetExsitingEntitlementCore(user);
        if (exsitingEntitlementCore != (AccountEntitlement) null)
        {
          this.m_existingEntitlements[user.UserId] = exsitingEntitlementCore;
          if (exsitingEntitlementCore.License != License.None)
          {
            license = exsitingEntitlementCore.License;
            source = exsitingEntitlementCore.AssignmentSource;
            return true;
          }
        }
      }
      return false;
    }

    private EntitlementProcessor.ProcessAssignDecision TryProcessAssign(
      AccountUser user,
      License license,
      AssignmentSource source)
    {
      AssignmentSource source1 = AssignmentSource.Unknown;
      License license1;
      this.UserHasValidLicense(user, out license1, out source1);
      if (license1 == license)
        return source == source1 ? EntitlementProcessor.ProcessAssignDecision.AlreadyHasLicense : EntitlementProcessor.ProcessAssignDecision.OverwriteAssignmentSource;
      AccountLicense license2 = license as AccountLicense;
      if ((License) license2 == (License) null || (License) license2 == (License) AccountLicense.EarlyAdopter || (License) license2 == (License) AccountLicense.Stakeholder)
        return EntitlementProcessor.ProcessAssignDecision.Assign;
      this.OnDemand(EntitlementProcessor.RequestedData.Reconcile);
      EntitlementProcessor.LicenseUsage usage = this.GetUsage(license2);
      return usage == null || usage.RemainingQuantity <= 0 ? EntitlementProcessor.ProcessAssignDecision.LicenseUnavailable : EntitlementProcessor.ProcessAssignDecision.Assign;
    }

    private void ProcessAssign(AccountUser user, License license)
    {
      AccountLicense license1 = license as AccountLicense;
      if ((License) license1 != (License) null)
      {
        EntitlementProcessor.LicenseUsage usage = this.GetUsage(license1);
        if (usage != null)
        {
          if (this.IsOwnerUser(user))
            usage.Usages.Insert(0, user.UserId);
          else
            usage.Usages.Add(user.UserId);
        }
      }
      this.m_pendingAssignments[user.UserId] = license;
    }

    private bool TryProcessRevoke(AccountUser user)
    {
      Guid userId = user.UserId;
      License license1;
      if (!this.UserHasValidLicense(user, out license1))
        return false;
      this.m_pendingAssignments.Remove(userId);
      AccountLicense license2 = license1 as AccountLicense;
      if ((License) license2 != (License) null)
        this.GetUsage(license2)?.Usages.Remove(userId);
      return true;
    }

    private EntitlementProcessor.LicenseUsage GetUsage(AccountLicense license)
    {
      EntitlementProcessor.LicenseUsage licenseUsage;
      return this.m_counts.TryGetValue(license, out licenseUsage) ? licenseUsage : (EntitlementProcessor.LicenseUsage) null;
    }

    private void SetResourceCounts()
    {
      if (!CommerceUtil.IsAzCommV2ApiEnabled(this.m_collectionContext))
      {
        this.SetResourceCountsLegacy();
      }
      else
      {
        foreach (MeterUsage2GetResponse licenseMeterUsage in this.m_azCommService.GetLicenseMeterUsages(this.m_collectionContext))
        {
          AccountLicenseType license;
          if (CommerceUtil.GetMeterIdToAccountLicenseTypeMap().TryGetValue(licenseMeterUsage.MeterId, out license))
            this.SetCommittedQuantity(AccountLicense.GetLicense(license) as AccountLicense, Convert.ToInt32(licenseMeterUsage.MaxQuantity));
        }
      }
    }

    private void SetResourceCountsLegacy()
    {
      foreach (IOfferSubscription offerSubscription in this.GetMeteringResourcesCore())
      {
        AccountLicense collectionLicense = this.MapResourceToCollectionLicense(offerSubscription.OfferMeter.Name);
        if ((License) collectionLicense != (License) null)
          this.SetCommittedQuantity(collectionLicense, offerSubscription.CommittedQuantity);
      }
    }

    private AccountLicense MapResourceToCollectionLicense(string resourceName)
    {
      switch (resourceName)
      {
        case "StandardLicense":
          return AccountLicense.Express;
        case "Test Manager":
          return AccountLicense.Advanced;
        case "ProfessionalLicense":
          return AccountLicense.Professional;
        default:
          return (AccountLicense) null;
      }
    }

    private void SetCommittedQuantity(AccountLicense license, int committedQuantity) => this.m_counts[license] = new EntitlementProcessor.LicenseUsage(committedQuantity);

    private LicensedIdentity ReadLicensedIdentity(Guid userIdentityId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_identityService.ReadIdentityWithFallback(this.m_collectionContext, userIdentityId);
      return identity == null ? new LicensedIdentity() : identity.ToLicensedIdentity();
    }

    private void Reset()
    {
      if (this.m_loadedData == (EntitlementProcessor.RequestedData) 0)
        return;
      this.m_counts.Clear();
      this.m_pendingAssignments.Clear();
      this.m_existingEntitlements.Clear();
      this.m_users.Clear();
      this.m_userLicenses.Clear();
      this.m_collectionOwner = Guid.Empty;
      this.m_isCollectionEnabled = false;
      this.m_loadedData = (EntitlementProcessor.RequestedData) 0;
    }

    private void OnDemand(EntitlementProcessor.RequestedData requestedData)
    {
      requestedData &= ~(this.m_loadedData | this.m_pendingData);
      if ((requestedData & EntitlementProcessor.RequestedData.Collection) != (EntitlementProcessor.RequestedData) 0)
      {
        this.m_pendingData |= EntitlementProcessor.RequestedData.Collection;
        try
        {
          this.m_collectionOwner = this.GetCollectionOwner();
          this.m_isCollectionEnabled = this.IsCollectionEnabled();
          this.m_loadedData |= EntitlementProcessor.RequestedData.Collection;
        }
        finally
        {
          this.m_pendingData &= ~EntitlementProcessor.RequestedData.Collection;
        }
      }
      if ((requestedData & EntitlementProcessor.RequestedData.CollectionUsers) != (EntitlementProcessor.RequestedData) 0)
      {
        this.m_pendingData |= EntitlementProcessor.RequestedData.CollectionUsers;
        try
        {
          this.m_users = this.GetUsersCore().ToDictionary<AccountUser, Guid>((Func<AccountUser, Guid>) (_ => _.UserId));
          this.m_loadedData |= EntitlementProcessor.RequestedData.CollectionUsers;
        }
        finally
        {
          this.m_pendingData &= ~EntitlementProcessor.RequestedData.CollectionUsers;
        }
      }
      if ((requestedData & EntitlementProcessor.RequestedData.CollectionEntitlements) != (EntitlementProcessor.RequestedData) 0)
      {
        this.m_pendingData |= EntitlementProcessor.RequestedData.CollectionEntitlements;
        try
        {
          this.m_existingEntitlements = this.GetEntitlementsCore().ToDictionary<AccountEntitlement, Guid>((Func<AccountEntitlement, Guid>) (_ => _.UserId));
          this.m_loadedData |= EntitlementProcessor.RequestedData.CollectionEntitlements;
        }
        finally
        {
          this.m_pendingData &= ~EntitlementProcessor.RequestedData.CollectionEntitlements;
        }
      }
      if ((requestedData & EntitlementProcessor.RequestedData.MeteredUsage) != (EntitlementProcessor.RequestedData) 0)
      {
        this.m_pendingData |= EntitlementProcessor.RequestedData.MeteredUsage;
        try
        {
          this.SetResourceCounts();
          this.m_loadedData |= EntitlementProcessor.RequestedData.MeteredUsage;
        }
        finally
        {
          this.m_pendingData &= ~EntitlementProcessor.RequestedData.MeteredUsage;
        }
      }
      if ((requestedData & EntitlementProcessor.RequestedData.Reconcile) == (EntitlementProcessor.RequestedData) 0)
        return;
      this.m_pendingData |= EntitlementProcessor.RequestedData.Reconcile;
      try
      {
        this.Process();
        this.m_loadedData |= EntitlementProcessor.RequestedData.Reconcile;
      }
      finally
      {
        this.m_pendingData &= ~EntitlementProcessor.RequestedData.Reconcile;
      }
    }

    private bool HasData(EntitlementProcessor.RequestedData requestedData) => requestedData == (this.m_loadedData & requestedData);

    private bool IsRequestingData(EntitlementProcessor.RequestedData requestedData) => (this.m_pendingData & requestedData) != 0;

    [Flags]
    private enum RequestedData
    {
      Collection = 1,
      CollectionUsers = 2,
      CollectionEntitlements = 4,
      MeteredUsage = 8,
      Reconcile = 16, // 0x00000010
    }

    private enum ProcessAssignDecision
    {
      Assign,
      AlreadyHasLicense,
      LicenseUnavailable,
      OverwriteAssignmentSource,
    }

    private class LicenseUsage
    {
      public LicenseUsage(int committedQuantity)
      {
        this.Usages = new List<Guid>();
        this.CommittedQuantity = committedQuantity;
      }

      public List<Guid> Usages { get; private set; }

      public int CommittedQuantity { get; private set; }

      public int RemainingQuantity => this.CommittedQuantity - this.Usages.Count;
    }

    private sealed class AccountLicenseAvailabilityComparer : IComparer<AccountLicense>
    {
      public static readonly EntitlementProcessor.AccountLicenseAvailabilityComparer Instance = new EntitlementProcessor.AccountLicenseAvailabilityComparer();

      public int Compare(AccountLicense x, AccountLicense y)
      {
        int num = this.GetOrder(x.License).CompareTo(this.GetOrder(y.License));
        return num == 0 ? x.CompareTo(y) : num;
      }

      private int GetOrder(AccountLicenseType type)
      {
        switch (type)
        {
          case AccountLicenseType.Express:
            return 1;
          case AccountLicenseType.Professional:
            return 2;
          case AccountLicenseType.Advanced:
            return 0;
          default:
            return 3;
        }
      }
    }
  }
}
