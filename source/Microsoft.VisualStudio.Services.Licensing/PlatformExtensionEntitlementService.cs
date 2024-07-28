// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformExtensionEntitlementService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class PlatformExtensionEntitlementService : 
    IExtensionEntitlementService,
    IVssFrameworkService,
    IPlatformExtensionEntitlementService
  {
    private static IDictionary<Tuple<ExtensionAssignmentStatus, ExtensionOperation>, Func<string, string>> s_warningMessageMap = PlatformExtensionEntitlementService.CreateWarningMessageMap();
    private Dictionary<Guid, string> collectionNameCache = new Dictionary<Guid, string>();
    private const string s_area = "LicensingService";
    private ILicensingRepository m_licensingRepository;
    private Guid m_serviceHostId;
    private const string s_layer = "PlatformExtensionEntitlementService";
    private const string s_GetMinimunRequiredAccessLevelForExtension = "VisualStudio.Services.Licensing.GetMinimunRequiredAccessLevelForExtension";
    private const string s_EnableGetExtensionsForUsersBatch = "VisualStudio.LicensingService.EnableGetExtensionsForUsersBatch";
    private const string s_HidePackageManagementAsExtension = "AzureDevOps.Services.Licensing.HidePackageManagementAsExtension";
    private const string s_PackageManagementGalleryId = "ms.feed";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && !systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      }
      else
        systemRequestContext.CheckProjectCollectionRequestContext();
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.m_licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual ICollection<ExtensionOperationResult> UnassignExtensionFromUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source)
    {
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        throw new NotSupportedException();
      return this.UnassignExtensionFromUsersInternal(requestContext, extensionId, userIds, source).ToExtensionOperationResults();
    }

    public virtual ICollection<ExtensionOperationResult> AssignExtensionToUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      bool autoAssignment = false)
    {
      requestContext.TraceEnter(1034220, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) extensionId,
        (object) userIds
      }, nameof (AssignExtensionToUsers));
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        throw new NotSupportedException();
      if (!userIds.Any<Guid>())
        return (ICollection<ExtensionOperationResult>) new List<ExtensionOperationResult>();
      if (autoAssignment && userIds.Count != 1)
        throw new ArgumentException("Can auto assign only a single user id.", nameof (userIds));
      ICollection<ExtensionOperationResult> operationResults = this.AssignExtensionToUsersInternal(requestContext, extensionId, userIds, false, autoAssignment, AssignmentSource.Unknown).ToExtensionOperationResults();
      requestContext.TraceLeave(1034229, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (AssignExtensionToUsers));
      return operationResults;
    }

    public virtual ICollection<ExtensionOperationResult> AssignExtensionToAllEligibleUsers(
      IVssRequestContext requestContext,
      string extensionId)
    {
      requestContext.TraceEnter(1034230, "LicensingService", nameof (PlatformExtensionEntitlementService), extensionId);
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      ICollection<ExtensionOperationResult> allEligibleUsers = !ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext) ? this.AssignExtensionToUsersInternal(requestContext, extensionId, (IList<Guid>) null, true, false, AssignmentSource.Unknown).ToExtensionOperationResults() : throw new NotSupportedException();
      requestContext.TraceLeave(1034239, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (AssignExtensionToAllEligibleUsers));
      return allEligibleUsers;
    }

    public virtual IDictionary<string, LicensingSource> GetExtensionsAssignedToUser(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.TraceEnter(1034240, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[1]
      {
        (object) userId
      }, nameof (GetExtensionsAssignedToUser));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
          return (IDictionary<string, LicensingSource>) new Dictionary<string, LicensingSource>();
        this.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
        License license = (License) null;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          license = this.GetHostedUserEntitlements(requestContext, (IList<Guid>) new List<Guid>()
          {
            userId
          }).FirstOrDefault<AccountEntitlement>()?.License;
        else if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          license = this.GetOnPremisesUserEntitlements(requestContext);
        requestContext.Trace(1034246, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("License for {0} was determined to be {1}", (object) userId, (object) license));
        return this.GetExtensionsAssignedToUserUnchecked(requestContext, userId, license);
      }
      finally
      {
        requestContext.TraceLeave(1034249, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionsAssignedToUser));
      }
    }

    public IDictionary<Guid, IList<ExtensionSource>> GetExtensionsAssignedToUsers(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      requestContext.TraceEnter(1034250, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[1]
      {
        (object) userIds
      }, nameof (GetExtensionsAssignedToUsers));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.LicensingService.EnableGetExtensionsForUsersBatch") || ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
          return (IDictionary<Guid, IList<ExtensionSource>>) new Dictionary<Guid, IList<ExtensionSource>>();
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dedupedDictionary = requestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(requestContext, (IEnumerable<Guid>) userIds).ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.EnterpriseStorageKey(requestContext)), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity));
        IDictionary<Guid, IList<ExtensionSource>> extensions = this.m_licensingRepository.GetExtensions(requestContext, (IList<Guid>) dedupedDictionary.Keys.ToList<Guid>());
        Dictionary<Guid, IList<ExtensionSource>> extensionsAssignedToUsers = new Dictionary<Guid, IList<ExtensionSource>>();
        foreach (Guid key in (IEnumerable<Guid>) extensions.Keys)
          extensionsAssignedToUsers.Add(dedupedDictionary.ContainsKey(key) ? dedupedDictionary[key].Id : key, extensions[key]);
        return (IDictionary<Guid, IList<ExtensionSource>>) extensionsAssignedToUsers;
      }
      finally
      {
        requestContext.TraceLeave(1034247, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionsAssignedToUsers));
      }
    }

    public virtual IEnumerable<AccountLicenseExtensionUsage> GetExtensionLicenseUsage(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1034240, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionLicenseUsage));
      this.ValidateRequestContext(requestContext);
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        return (IEnumerable<AccountLicenseExtensionUsage>) new List<AccountLicenseExtensionUsage>();
      this.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      requestContext.Trace(1034241, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("GetExtensionsAssignedToAccount for account with id {0}", (object) instanceId));
      IList<AccountExtensionCount> accountExtensionCounts = this.GetStoredAccountExtensionCounts(requestContext);
      Dictionary<string, int> msdnExtensionMap = accountExtensionCounts.Where<AccountExtensionCount>((Func<AccountExtensionCount, bool>) (ext => ext.Source == LicensingSource.Msdn)).ToDictionary<AccountExtensionCount, string, int>((Func<AccountExtensionCount, string>) (x => x.ExtensionId), (Func<AccountExtensionCount, int>) (x => x.Assigned));
      Dictionary<string, int> accountExtensionMap = accountExtensionCounts.Where<AccountExtensionCount>((Func<AccountExtensionCount, bool>) (ext => ext.Source == LicensingSource.Account)).ToDictionary<AccountExtensionCount, string, int>((Func<AccountExtensionCount, string>) (x => x.ExtensionId), (Func<AccountExtensionCount, int>) (x => x.Assigned));
      IEnumerable<IOfferSubscription> offerSubscriptions = requestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(requestContext);
      CommonUtil.ValidateOfferSubscriptions(requestContext, offerSubscriptions);
      requestContext.Trace(1034256, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Available extensions returned from Commerce: " + string.Join(", ", offerSubscriptions.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      IEnumerable<IOfferSubscription> list = (IEnumerable<IOfferSubscription>) offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.IsExtensionCategory() && o.OfferMeter.BillingState == MeterBillingState.Paid && !o.IsPreview && o.OfferMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.Express && o.OfferMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.Stakeholder)).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.GalleryId != "ms.feed" || !requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.HidePackageManagementAsExtension"))).ToList<IOfferSubscription>();
      requestContext.Trace(1034257, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Filtered extensions: " + string.Join(", ", list.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      IEnumerable<AccountLicenseExtensionUsage> extensionLicenseUsage = list.Select<IOfferSubscription, AccountLicenseExtensionUsage>((Func<IOfferSubscription, AccountLicenseExtensionUsage>) (info => new AccountLicenseExtensionUsage()
      {
        ExtensionName = info.OfferMeter.Name,
        ExtensionId = info.OfferMeter.GalleryId,
        UsedCount = accountExtensionMap.ContainsKey(info.OfferMeter.GalleryId) ? accountExtensionMap[info.OfferMeter.GalleryId] : 0,
        MsdnUsedCount = msdnExtensionMap.ContainsKey(info.OfferMeter.GalleryId) ? msdnExtensionMap[info.OfferMeter.GalleryId] : 0,
        ProvisionedCount = info.CommittedQuantity,
        IncludedQuantity = info.IncludedQuantity,
        IsTrial = info.IsTrialOrPreview && !info.IsPreview,
        RemainingTrialDays = (int) info.OfferMeter.TrialDays,
        MinimumLicenseRequired = info.OfferMeter.MinimumRequiredAccessLevel,
        TrialExpiryDate = info.TrialExpiryDate
      }));
      requestContext.TraceLeave(1034249, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionLicenseUsage));
      return extensionLicenseUsage;
    }

    public virtual IList<Guid> GetEligibleUsersForExtension(
      IVssRequestContext requestContext,
      string extensionId,
      ExtensionFilterOptions options)
    {
      requestContext.TraceEnter(1034250, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) extensionId,
        (object) options
      }, nameof (GetEligibleUsersForExtension));
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        return (IList<Guid>) new List<Guid>();
      this.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      List<Guid> usersForExtension = new List<Guid>();
      IDictionary<Guid, ExtensionAssignmentDetails> extensionStatusForUsers = this.GetExtensionStatusForUsers(requestContext, extensionId);
      if ((options & ExtensionFilterOptions.None) != ~ExtensionFilterOptions.All)
      {
        requestContext.Trace(1034251, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetEligibleUsersForExtension: filter list of users for filter None");
        usersForExtension.AddRange((IEnumerable<Guid>) extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.NotAssigned || x.Value.AssignmentStatus == ExtensionAssignmentStatus.TrialAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key)).ToList<Guid>());
      }
      if ((options & ExtensionFilterOptions.AccountAssignment) != ~ExtensionFilterOptions.All || (options & ExtensionFilterOptions.ImplicitAssignment) != ~ExtensionFilterOptions.All)
      {
        requestContext.Trace(1034252, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetEligibleUsersForExtension: filter list of users for filter AccountAssignment");
        usersForExtension.AddRange((IEnumerable<Guid>) extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment || x.Value.AssignmentStatus == ExtensionAssignmentStatus.RoamingAccountAssignment || x.Value.AssignmentStatus == ExtensionAssignmentStatus.ImplicitAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key)).ToList<Guid>());
      }
      if ((options & ExtensionFilterOptions.Bundle) != ~ExtensionFilterOptions.All)
      {
        requestContext.Trace(1034253, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetEligibleUsersForExtension: filter list of users for filter Bundle");
        usersForExtension.AddRange((IEnumerable<Guid>) extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.BundleAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key)).ToList<Guid>());
      }
      requestContext.TraceLeave(1034259, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetEligibleUsersForExtension));
      return (IList<Guid>) usersForExtension;
    }

    public void TransferExtensionsForIdentities(
      IVssRequestContext requestContext,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMapping)
    {
      this.TransferExtensionsForIdentities(requestContext, identityMapping.Select<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<Guid, Guid>>((Func<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<Guid, Guid>>) (t => new KeyValuePair<Guid, Guid>(t.Item1.EnterpriseStorageKey(requestContext), t.Item2.EnterpriseStorageKey(requestContext)))));
    }

    private void TransferExtensionsForIdentities(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      requestContext.TraceEnter(1034350, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[1]
      {
        (object) userIdTransferMap
      }, nameof (TransferExtensionsForIdentities));
      this.ValidateRequestContext(requestContext);
      Dictionary<string, ExtensionAssignmentModel>.KeyCollection keys = this.GetExtensionsData(requestContext).Keys;
      requestContext.Trace(1034351, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "TransferExtensionsForIdentities: Transferring identities for extensions: {0}", keys == null ? (object) string.Empty : (object) string.Join(",", (IEnumerable<string>) keys));
      foreach (string extensionId in keys)
      {
        requestContext.Trace(1034352, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "TransferExtensionsForIdentities: Calling to the DB component to get users for extension {0}", (object) extensionId);
        IList<Guid> list1 = (IList<Guid>) userIdTransferMap.Select<KeyValuePair<Guid, Guid>, Guid>((Func<KeyValuePair<Guid, Guid>, Guid>) (_ => _.Key)).ToList<Guid>();
        IList<UserExtensionLicense> extensionLicenses = this.m_licensingRepository.GetExtensionLicenses(requestContext, list1, extensionId);
        foreach (LicensingSource licensingSource in (IEnumerable<LicensingSource>) new List<LicensingSource>()
        {
          LicensingSource.Account,
          LicensingSource.Msdn
        })
        {
          LicensingSource source = licensingSource;
          Dictionary<AssignmentSource, List<UserExtensionLicense>> dictionary = extensionLicenses.Where<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (_ => _.Source == source)).GroupBy<UserExtensionLicense, AssignmentSource>((Func<UserExtensionLicense, AssignmentSource>) (_ => _.AssignmentSource)).ToDictionary<IGrouping<AssignmentSource, UserExtensionLicense>, AssignmentSource, List<UserExtensionLicense>>((Func<IGrouping<AssignmentSource, UserExtensionLicense>, AssignmentSource>) (t => t.Key), (Func<IGrouping<AssignmentSource, UserExtensionLicense>, List<UserExtensionLicense>>) (t => t.ToList<UserExtensionLicense>()));
          foreach (AssignmentSource key in dictionary.Keys)
          {
            List<Guid> list2 = dictionary[key].Select<UserExtensionLicense, Guid>((Func<UserExtensionLicense, Guid>) (_ => _.UserId)).ToList<Guid>();
            if (list2.Any<Guid>())
            {
              IEnumerable<\u003C\u003Ef__AnonymousType11<Guid>> datas = list2.Join(userIdTransferMap, (Func<Guid, Guid>) (user => user), (Func<KeyValuePair<Guid, Guid>, Guid>) (map => map.Key), (user, map) => new
              {
                StorageKey = map.Value
              });
              requestContext.Trace(1034354, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "TransferExtensionsForIdentities: assigning extension {0} to userIds {0}", (object) extensionId, (object) string.Join(",", datas));
              this.AssignExtensionToUsersUnchecked(requestContext, (IList<Guid>) datas.Select(_ => _.StorageKey).ToList<Guid>(), extensionId, source, key);
              requestContext.Trace(1034353, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "TransferExtensionsForIdentities: unassigning extension {0} from userIds {0}", (object) extensionId, (object) string.Join<Guid>(",", (IEnumerable<Guid>) list2));
              this.UnassignExtensionFromUsersUnchecked(requestContext, extensionId, (IList<Guid>) list2, source);
            }
          }
        }
      }
      requestContext.TraceLeave(1034359, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (TransferExtensionsForIdentities));
    }

    public virtual void EvaluateExtensionAssignmentsOnAccessLevelChange(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License license)
    {
      requestContext.TraceEnter(1034280, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) identity,
        (object) license
      }, nameof (EvaluateExtensionAssignmentsOnAccessLevelChange));
      try
      {
        if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
          return;
        Guid userId = identity.EnterpriseStorageKey(requestContext);
        List<KeyValuePair<string, LicensingSource>> extensions = this.GetStoredUserExtensions(requestContext, userId).ToList<KeyValuePair<string, LicensingSource>>();
        requestContext.TraceConditionally(1034281, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), (Func<string>) (() => "EvaluateExtensionAssignmentsOnAccessLevelChange: user: " + identity.DisplayName + ", extensions: " + string.Join<KeyValuePair<string, LicensingSource>>(",", (IEnumerable<KeyValuePair<string, LicensingSource>>) extensions.ToArray())));
        foreach (KeyValuePair<string, LicensingSource> keyValuePair in extensions)
        {
          KeyValuePair<string, LicensingSource> extension = keyValuePair;
          int includedQuantity = 0;
          VisualStudioOnlineServiceLevel minLevel;
          bool isFirstParty;
          this.GetExtensionData(requestContext, extension.Key, out minLevel, out ExtensionAssignmentModel _, out isFirstParty, out includedQuantity);
          if (!this.UsageRightsMatch(minLevel, license) || extension.Value == LicensingSource.Msdn || requestContext.ExecutionEnvironment.IsOnPremisesDeployment & isFirstParty && license.Source == LicensingSource.Msdn)
          {
            requestContext.TraceConditionally(1034282, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), (Func<string>) (() => string.Format("EvaluateExtensionAssignmentsOnAccessLevelChange: Unassigning extension {0} from user {1} due to change of access level to {2}", (object) extension, (object) identity.DisplayName, (object) license)));
            IVssRequestContext requestContext1 = requestContext;
            string key = extension.Key;
            List<Guid> userIds = new List<Guid>();
            userIds.Add(userId);
            int source = (int) extension.Value;
            this.UnassignExtensionFromUsersUnchecked(requestContext1, key, (IList<Guid>) userIds, (LicensingSource) source);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034288, "LicensingService", nameof (PlatformExtensionEntitlementService), ex);
        throw;
      }
      requestContext.TraceLeave(1034289, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (EvaluateExtensionAssignmentsOnAccessLevelChange));
    }

    public ICollection<ExtensionOperationResultInternal> AssignExtensionToUsersInternal(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      bool assignToAllEligibleUsers,
      bool isAutoAssignment,
      AssignmentSource assignmentSource)
    {
      requestContext.TraceEnter(1034260, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[3]
      {
        (object) extensionId,
        (object) userIds,
        (object) assignToAllEligibleUsers
      }, nameof (AssignExtensionToUsersInternal));
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      this.ValidateAssignmentSource(assignmentSource);
      userIds = !ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext) ? this.GetMasterFromLocalId(requestContext, userIds) : throw new NotSupportedException();
      this.CheckPermission(requestContext, 16, LicensingSecurity.AccountEntitlementsToken);
      List<ExtensionOperationResultInternal> usersInternal = new List<ExtensionOperationResultInternal>();
      requestContext.Trace(1034262, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "AssignExtensionToUsersInternal: Acquired ExtensionEntitlement lock");
      requestContext.Trace(1034263, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "AssignExtensionToUsersInternal: Acquired AccountEntitlement lock");
      IOfferSubscription subscriptionByExtensionId = this.GetOfferSubscriptionByExtensionId(requestContext, extensionId);
      if (isAutoAssignment && !subscriptionByExtensionId.AutoAssignOnAccess)
      {
        Guid userId = userIds.First<Guid>();
        requestContext.Trace(1034273, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("{0} is not setup for Auto extension assignment. {1} could not be assigned a extension license.", (object) extensionId, (object) userId));
        return (ICollection<ExtensionOperationResultInternal>) new List<ExtensionOperationResultInternal>()
        {
          PlatformExtensionEntitlementService.CreateExtensionOperationResultWarning(requestContext.ServiceHost.InstanceId, userId, extensionId, "Extension is not setup for auto assignment", ExtensionOperation.Assign, ExtensionAssignmentStatus.NotEligible)
        };
      }
      IList<Guid> guidList;
      if (!assignToAllEligibleUsers)
      {
        requestContext.Trace(1034264, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "AssignExtensionToUsersInternal: Generate OperationFailure for the users that cannot be assigned");
        Dictionary<Guid, License> userEntitlements = this.GetUserEntitlements(requestContext, userIds);
        IDictionary<Guid, ExtensionAssignmentDetails> extensionStatusForUsers = this.GetExtensionStatusForUsers(requestContext, extensionId, userEntitlements);
        if (isAutoAssignment && extensionStatusForUsers.First<KeyValuePair<Guid, ExtensionAssignmentDetails>>().Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment)
          return (ICollection<ExtensionOperationResultInternal>) new List<ExtensionOperationResultInternal>();
        usersInternal.AddRange((IEnumerable<ExtensionOperationResultInternal>) this.CreateOperationResultSetForFailedOperationExtension(requestContext, extensionId, userIds, extensionStatusForUsers, ExtensionOperation.Assign));
        guidList = (IList<Guid>) extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.NotAssigned || x.Value.AssignmentStatus == ExtensionAssignmentStatus.TrialAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key)).Intersect<Guid>((IEnumerable<Guid>) userIds).ToList<Guid>();
      }
      else
      {
        requestContext.Trace(1034265, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "AssignExtensionToUsersInternal: assign extension to all eligible users");
        guidList = this.GetEligibleUsersForExtension(requestContext, extensionId, ExtensionFilterOptions.None);
      }
      if (isAutoAssignment && !guidList.Any<Guid>())
        return (ICollection<ExtensionOperationResultInternal>) usersInternal;
      if (this.IsQuantityCheckRequired(requestContext, extensionId))
      {
        if (subscriptionByExtensionId == null)
          throw new LicenseNotAvailableException(LicensingResources.ExtensionLicenseNotAvailableException((object) guidList.Count, (object) extensionId));
        int extensionUsage = this.GetExtensionUsage(requestContext, extensionId);
        this.CheckLicenseQuantity(requestContext, subscriptionByExtensionId.OfferMeter.Name, subscriptionByExtensionId.CommittedQuantity - extensionUsage, guidList.Count);
      }
      this.AssignExtensionToUsersUnchecked(requestContext, guidList, extensionId, LicensingSource.Account, assignmentSource);
      requestContext.TraceLeave(1034269, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (AssignExtensionToUsersInternal));
      return (ICollection<ExtensionOperationResultInternal>) usersInternal;
    }

    public ICollection<ExtensionOperationResultInternal> UnassignExtensionFromUsersInternal(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source)
    {
      requestContext.TraceEnter(1034210, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[3]
      {
        (object) extensionId,
        (object) userIds,
        (object) source
      }, nameof (UnassignExtensionFromUsersInternal));
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      ArgumentUtility.CheckForNull<IList<Guid>>(userIds, nameof (userIds));
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        throw new NotSupportedException();
      if (!userIds.Any<Guid>())
        return (ICollection<ExtensionOperationResultInternal>) new List<ExtensionOperationResultInternal>();
      this.CheckPermission(requestContext, 4, LicensingSecurity.AccountEntitlementsToken);
      List<ExtensionOperationResultInternal> operationResultInternalList = new List<ExtensionOperationResultInternal>();
      userIds = this.GetMasterFromLocalId(requestContext, userIds);
      Dictionary<Guid, License> userEntitlements = this.GetUserEntitlements(requestContext, userIds);
      IDictionary<Guid, ExtensionAssignmentDetails> extensionStatusForUsers = this.GetExtensionStatusForUsers(requestContext, extensionId, userEntitlements);
      operationResultInternalList.AddRange((IEnumerable<ExtensionOperationResultInternal>) this.CreateOperationResultSetForFailedOperationExtension(requestContext, extensionId, userIds, extensionStatusForUsers, ExtensionOperation.Unassign));
      List<Guid> list = extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key)).Intersect<Guid>((IEnumerable<Guid>) userIds).ToList<Guid>();
      if (list.Any<Guid>())
        this.UnassignExtensionFromUsersUnchecked(requestContext, extensionId, (IList<Guid>) list, source);
      requestContext.TraceLeave(1034219, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (UnassignExtensionFromUsersInternal));
      return (ICollection<ExtensionOperationResultInternal>) operationResultInternalList;
    }

    internal IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<AccountEntitlement> accountEntitlements)
    {
      Dictionary<Guid, License> licenseDictionary = accountEntitlements.ToLicenseDictionary();
      return this.GetExtensionStatusForUsers(requestContext, extensionId, licenseDictionary);
    }

    internal void SynchronizeMsdnExtensions(
      IVssRequestContext requestContext,
      List<MsdnEntitlement> msdnEntitlements,
      Guid userId)
    {
      requestContext.TraceEnter(1034270, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) msdnEntitlements,
        (object) userId
      }, nameof (SynchronizeMsdnExtensions));
      ICollection<string> msdnExtensions = MsdnLicensingAdapter.TranslateMsdnEntitlementsToMsdnExtensions(msdnEntitlements);
      requestContext.Trace(1034271, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("SynchronizeMsdnExtensions: user {0}, new extensions are {1}", (object) userId, (object) string.Join(",", msdnExtensions.ToArray<string>())));
      requestContext.Trace(1034272, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "SynchronizeMsdnExtensions: Calling to the DB component");
      if (this.m_licensingRepository.UpdateExtensionLicenses(requestContext, userId, (IList<string>) msdnExtensions.ToList<string>(), LicensingSource.Msdn, AssignmentSource.None) > 0)
        this.InvalidateExtensionEntitlementCache(requestContext, (IList<Guid>) new List<Guid>()
        {
          userId
        });
      requestContext.TraceLeave(1034279, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (SynchronizeMsdnExtensions));
    }

    internal void ReconcileExtensionUsage(
      IVssRequestContext requestContext,
      string extensionId,
      int newQuantity)
    {
      this.ValidateExtensionId(extensionId);
      requestContext.TraceEnter(1034330, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) extensionId,
        (object) newQuantity
      }, nameof (ReconcileExtensionUsage));
      List<AccountEntitlement> list1 = requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(requestContext.Elevate()).Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x.UserStatus == AccountUserStatus.Active || x.UserStatus == AccountUserStatus.Pending)).ToList<AccountEntitlement>();
      IEnumerable<Guid> guids = this.GetExtensionStatusForUsers(requestContext, extensionId, (IList<AccountEntitlement>) list1).Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (_ => _.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment)).Select<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (_ => _.Key));
      if (guids.Count<Guid>() > newQuantity)
      {
        List<Guid> list2 = list1.Join(guids, (Func<AccountEntitlement, Guid>) (entitlement => entitlement.UserId), (Func<Guid, Guid>) (user => user), (entitlement, user) => new
        {
          UserId = entitlement.UserId,
          LastAccessedDate = entitlement.LastAccessedDate
        }).OrderByDescending(_ => _.LastAccessedDate).ThenByDescending(_ => _.UserId).ToList().GetRange(newQuantity, guids.Count<Guid>() - newQuantity).Select(_ => _.UserId).ToList<Guid>();
        requestContext.Trace(1034332, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "ReconcileExtensionUsage: unassign extension " + extensionId + " from users: " + string.Join<Guid>(",", (IEnumerable<Guid>) list2.ToArray()));
        this.UnassignExtensionFromUsersUnchecked(requestContext, extensionId, (IList<Guid>) list2, LicensingSource.Account);
      }
      requestContext.TraceLeave(1034339, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (ReconcileExtensionUsage));
    }

    internal virtual void UnassignmentExtensionOnUserRemove(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.TraceEnter(1034290, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[1]
      {
        (object) userId
      }, nameof (UnassignmentExtensionOnUserRemove));
      try
      {
        Dictionary<string, LicensingSource> storedUserExtensions = this.GetStoredUserExtensions(requestContext, userId);
        requestContext.Trace(1034291, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("UnassignmentExtensionOnUserRemove: user: {0}, extensions: {1}", (object) userId, (object) string.Join<KeyValuePair<string, LicensingSource>>(",", (IEnumerable<KeyValuePair<string, LicensingSource>>) storedUserExtensions.ToArray<KeyValuePair<string, LicensingSource>>())));
        foreach (KeyValuePair<string, LicensingSource> keyValuePair in storedUserExtensions)
        {
          requestContext.Trace(1034292, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("UnassignmentExtensionOnUserRemove: Unassigning extension {0} from user {1} because the user is removed from account", (object) keyValuePair, (object) userId));
          IVssRequestContext requestContext1 = requestContext;
          string key = keyValuePair.Key;
          List<Guid> userIds = new List<Guid>();
          userIds.Add(userId);
          int source = (int) keyValuePair.Value;
          this.UnassignExtensionFromUsersUnchecked(requestContext1, key, (IList<Guid>) userIds, (LicensingSource) source);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034298, "LicensingService", nameof (PlatformExtensionEntitlementService), ex);
        throw;
      }
      requestContext.TraceLeave(1034299, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (UnassignmentExtensionOnUserRemove));
    }

    internal IOfferSubscription GetOfferSubscriptionByExtensionId(
      IVssRequestContext requestContext,
      string extensionId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IEnumerable<IOfferSubscription> offerSubscriptions = vssRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(vssRequestContext);
      CommonUtil.ValidateOfferSubscriptions(requestContext, offerSubscriptions);
      requestContext.Trace(1034258, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Available offer subscriptions returned from Commerce: " + string.Join(", ", offerSubscriptions.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      List<IOfferSubscription> list = offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.GalleryId == extensionId)).ToList<IOfferSubscription>();
      requestContext.Trace(1034259, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Filtered subscriptions: " + string.Join(", ", offerSubscriptions.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      return list.Any<IOfferSubscription>() ? list.First<IOfferSubscription>() : (IOfferSubscription) null;
    }

    internal IDictionary<string, LicensingSource> GetExtensionsAssignedToUserUnchecked(
      IVssRequestContext requestContext,
      Guid userId,
      License license)
    {
      Guid userId1 = ValidationHelper.ValidateIdentityId(requestContext, userId, false).EnterpriseStorageKey(requestContext);
      Dictionary<string, LicensingSource> storedUserExtensions = this.GetStoredUserExtensions(requestContext, userId1);
      Dictionary<string, LicensingSource> offerSubscriptions = this.GetExtensionAssignmentBasedOnOfferSubscriptions(requestContext, storedUserExtensions, userId1);
      Dictionary<string, LicensingSource> onUserAccessLevel = this.GetExtensionAssignmentBasedOnUserAccessLevel(requestContext, offerSubscriptions, license, userId1);
      return (IDictionary<string, LicensingSource>) this.FilterAllowableExtensions(requestContext, onUserAccessLevel, license);
    }

    internal Dictionary<string, LicensingSource> GetExtensionAssignmentBasedOnUserAccessLevel(
      IVssRequestContext requestContext,
      Dictionary<string, LicensingSource> userExtensionAssignment,
      License license,
      Guid userId)
    {
      foreach (IOfferMeter offerMeter in requestContext.GetService<IOfferMeterService>().GetOfferMeters(requestContext))
      {
        if (offerMeter.GalleryId != null && offerMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.None && this.UsageRightsMatch(LicensingExtensionEntitlementHelper.MapToVisualStudioOnlineServiceLevel(offerMeter.IncludedInLicenseLevel), license) && !userExtensionAssignment.ContainsKey(offerMeter.GalleryId))
        {
          requestContext.Trace(1034248, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("User {0} was assigned {1} due to license {2}", (object) userId, (object) offerMeter.GalleryId, (object) license));
          userExtensionAssignment.TryAdd<string, LicensingSource>(offerMeter.GalleryId, LicensingSource.Account);
        }
      }
      return userExtensionAssignment;
    }

    internal virtual Dictionary<Guid, License> GetUserEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetHostedUserEntitlements(requestContext, userIds).ToLicenseDictionary();
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? requestContext.GetExtension<ILegacyLicensingHandler>().GetLicensesForUsers(requestContext, (IEnumerable<Guid>) userIds) : (Dictionary<Guid, License>) null;
    }

    private IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      IVssRequestContext requestContext,
      string extensionId,
      Dictionary<Guid, License> accountEntitlements)
    {
      requestContext.TraceEnter(1034340, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[2]
      {
        (object) extensionId,
        (object) accountEntitlements
      }, nameof (GetExtensionStatusForUsers));
      Dictionary<Guid, ExtensionAssignmentDetails> dictionary = new Dictionary<Guid, ExtensionAssignmentDetails>();
      int includedQuantity = 0;
      VisualStudioOnlineServiceLevel minLevel;
      ExtensionAssignmentModel assignmentModel;
      bool isFirstParty;
      this.GetExtensionData(requestContext, extensionId, out minLevel, out assignmentModel, out isFirstParty, out includedQuantity);
      bool flag = this.IsSubscriptionOnTrial(requestContext, extensionId);
      List<Guid> list = accountEntitlements.Keys.ToList<Guid>();
      requestContext.Trace(1034312, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetExtensionStatusForUsers: Calling to the DB component to get users for extension");
      IList<UserExtensionLicense> source = this.m_licensingRepository.GetExtensionLicenses(requestContext, (IList<Guid>) list, extensionId) ?? (IList<UserExtensionLicense>) new List<UserExtensionLicense>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in requestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(requestContext, (IEnumerable<Guid>) list))
      {
        if (!ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor) && !IdentityHelper.IsAnonymousPrincipal(identity.Descriptor))
        {
          Guid storageKey = identity.EnterpriseStorageKey(requestContext);
          UserExtensionLicense extensionLicense = source.FirstOrDefault<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (_ => _.UserId == storageKey));
          if (extensionLicense != null)
          {
            if (isFirstParty && extensionLicense.Source == LicensingSource.Msdn)
              dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.BundleAssignment));
            else if (extensionLicense.Source == LicensingSource.Account)
            {
              Guid currentCollectionId = this.GetCurrentCollectionId(requestContext);
              if ((!(currentCollectionId != Guid.Empty) ? 0 : (extensionLicense.CollectionId != currentCollectionId ? 1 : 0)) != 0)
              {
                string collectionName = this.GetCollectionName(requestContext, extensionLicense.CollectionId);
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.RoamingAccountAssignment, collectionName));
              }
              else
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.AccountAssignment));
            }
          }
          else if (!accountEntitlements.ContainsKey(storageKey))
          {
            requestContext.TraceAlways(1034319, TraceLevel.Error, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetExtensionStatusForUsers: AccountEntitlements - Key Not Found Exception key: {0} and User Id: {1}", (object) storageKey, (object) storageKey);
          }
          else
          {
            TeamFoundationExecutionEnvironment executionEnvironment;
            if (isFirstParty)
            {
              executionEnvironment = requestContext.ExecutionEnvironment;
              if (executionEnvironment.IsOnPremisesDeployment && accountEntitlements[storageKey] != (License) null && accountEntitlements[storageKey].Source == LicensingSource.Msdn)
              {
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.BundleAssignment));
                continue;
              }
            }
            if (isFirstParty)
            {
              executionEnvironment = requestContext.ExecutionEnvironment;
              if (executionEnvironment.IsHostedDeployment && accountEntitlements[storageKey] != (License) null && (accountEntitlements[storageKey] == (License) AccountLicense.EarlyAdopter || accountEntitlements[storageKey] == (License) AccountLicense.Advanced || accountEntitlements[storageKey] == (License) MsdnLicense.Enterprise))
              {
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.BundleAssignment));
                continue;
              }
            }
            if (this.UsageRightsMatch(minLevel, accountEntitlements[storageKey]))
            {
              if (assignmentModel == ExtensionAssignmentModel.ImplicitAssignment)
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.ImplicitAssignment));
              else if (flag)
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.TrialAssignment));
              else
                dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.NotAssigned));
            }
            else if (accountEntitlements[storageKey] != (License) null && accountEntitlements[storageKey].Source == LicensingSource.Msdn)
              dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.PendingValidation));
            else
              dictionary.TryAdd<Guid, ExtensionAssignmentDetails>(storageKey, new ExtensionAssignmentDetails(ExtensionAssignmentStatus.NotEligible));
          }
        }
      }
      requestContext.TraceLeave(1034319, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionStatusForUsers));
      return (IDictionary<Guid, ExtensionAssignmentDetails>) dictionary;
    }

    public virtual IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      IVssRequestContext requestContext,
      string extensionId)
    {
      requestContext.TraceEnter(1034340, "LicensingService", nameof (PlatformExtensionEntitlementService), extensionId);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ValidateRequestContext(requestContext);
      this.ValidateExtensionId(extensionId);
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
        return (IDictionary<Guid, ExtensionAssignmentDetails>) new Dictionary<Guid, ExtensionAssignmentDetails>();
      this.CheckPermission(requestContext, 1, LicensingSecurity.AccountEntitlementsToken);
      Dictionary<Guid, License> accountEntitlements = this.GetAllAccountEntitlements(requestContext);
      requestContext.TraceLeave(1034349, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (GetExtensionStatusForUsers));
      return this.GetExtensionStatusForUsers(requestContext, extensionId, accountEntitlements);
    }

    private Dictionary<string, LicensingSource> FilterAllowableExtensions(
      IVssRequestContext requestContext,
      Dictionary<string, LicensingSource> userExtensionsAssignment,
      License license)
    {
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, LicensingSource> keyValuePair in userExtensionsAssignment)
      {
        VisualStudioOnlineServiceLevel requiredAccessLevel = this.GetMinimumRequiredAccessLevel(requestContext, keyValuePair.Key);
        if (!this.UsageRightsMatch(requiredAccessLevel, license))
        {
          requestContext.Trace(1034342, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("Filtering out extension {0}. License {1} was insufficient for minimum access level {2}", (object) keyValuePair.Key, (object) license, (object) requiredAccessLevel));
          stringList.Add(keyValuePair.Key);
        }
      }
      foreach (string key in stringList)
        userExtensionsAssignment.Remove(key);
      return userExtensionsAssignment;
    }

    private License GetOnPremisesUserEntitlements(IVssRequestContext collectionContext) => collectionContext.GetExtension<ILegacyLicensingHandler>().GetLicenseForUser(collectionContext);

    private Dictionary<Guid, License> GetAllAccountEntitlements(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetHostedAllAccountEntitlements(requestContext);
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? this.GetOnPremisesEntitlements(requestContext) : (Dictionary<Guid, License>) null;
    }

    private Dictionary<Guid, License> GetHostedAllAccountEntitlements(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(requestContext.Elevate()).Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x.UserStatus == AccountUserStatus.Active || x.UserStatus == AccountUserStatus.Pending)).ToList<AccountEntitlement>().ToLicenseDictionary();
    }

    private Dictionary<Guid, License> GetOnPremisesEntitlements(IVssRequestContext collectionContext) => collectionContext.GetExtension<ILegacyLicensingHandler>().GetLicensesForAllActiveUsers(collectionContext);

    private void UnassignExtensionFromUsersUnchecked(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source)
    {
      requestContext.TraceEnter(1034320, "LicensingService", nameof (PlatformExtensionEntitlementService), new object[3]
      {
        (object) extensionId,
        (object) userIds,
        (object) source
      }, nameof (UnassignExtensionFromUsersUnchecked));
      try
      {
        requestContext.Trace(1034321, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "UnassignExtensionFromUsersUnchecked: Calling to the DB component");
        this.m_licensingRepository.UnassignExtensionLicenses(requestContext, userIds, extensionId, source);
        this.InvalidateExtensionEntitlementCache(requestContext, userIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034328, "LicensingService", nameof (PlatformExtensionEntitlementService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1034329, "LicensingService", nameof (PlatformExtensionEntitlementService), nameof (UnassignExtensionFromUsersUnchecked));
      }
    }

    private Dictionary<string, LicensingSource> GetStoredUserExtensions(
      IVssRequestContext requestContext,
      Guid userId)
    {
      requestContext.Trace(1034242, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetExtensionAssignedToUserUnchecked: Calling to the DB component");
      IList<UserExtensionLicense> extensionLicenses = this.m_licensingRepository.GetExtensionLicenses(requestContext, userId, UserExtensionLicenseStatus.Active);
      Dictionary<string, LicensingSource> dictionary = new Dictionary<string, LicensingSource>();
      if (extensionLicenses != null)
      {
        foreach (UserExtensionLicense extensionLicense in (IEnumerable<UserExtensionLicense>) extensionLicenses)
        {
          LicensingSource licensingSource;
          if (!dictionary.TryGetValue(extensionLicense.ExtensionId, out licensingSource))
            dictionary.TryAdd<string, LicensingSource>(extensionLicense.ExtensionId, extensionLicense.Source);
          else
            requestContext.Trace(1034243, TraceLevel.Error, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetExtensionAssignedToUserUnchecked: duplicate extension assignment for user: {0}, assignment exists from source {1}", (object) userId, (object) licensingSource);
        }
      }
      return dictionary;
    }

    private IList<AccountExtensionCount> GetStoredAccountExtensionCounts(
      IVssRequestContext requestContext)
    {
      requestContext.Trace(1034245, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetStoredAccountExtensionCounts: Calling to the DB component");
      return this.m_licensingRepository.GetExtensionQuantities(requestContext, UserExtensionLicenseStatus.Active);
    }

    internal Dictionary<string, LicensingSource> GetExtensionAssignmentBasedOnOfferSubscriptions(
      IVssRequestContext requestContext,
      Dictionary<string, LicensingSource> userExtensionAssignment,
      Guid userId)
    {
      IEnumerable<IOfferSubscription> offerSubscriptions = requestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(requestContext);
      CommonUtil.ValidateOfferSubscriptions(requestContext, offerSubscriptions);
      requestContext.Trace(1034254, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Available extensions returned from Commerce: " + string.Join(", ", offerSubscriptions.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      IEnumerable<IOfferSubscription> source = offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => (o.OfferMeter.IsExtensionCategory() || o.OfferMeter.Name == "Test Manager") && !userExtensionAssignment.ContainsKey(o.OfferMeter.GalleryId)));
      requestContext.Trace(1034255, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "Filtered extensions: " + string.Join(", ", source.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (e => string.Format("{0} | {1} | {2}", (object) e.OfferMeter?.Name, (object) e.IncludedQuantity, (object) e.CommittedQuantity)))));
      bool? nullable = new bool?();
      foreach (IOfferSubscription offerSubscription in source)
      {
        requestContext.Trace(1034244, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("User {0} checking assignment status for {1}", (object) userId, (object) offerSubscription.OfferMeter.GalleryId));
        if (offerSubscription.OfferMeter.AssignmentModel == OfferMeterAssignmentModel.Implicit && offerSubscription.IsPaidBillingEnabled && offerSubscription.CommittedQuantity > 0)
        {
          requestContext.Trace(1034244, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("User {0} has {1} implicitly", (object) userId, (object) offerSubscription.OfferMeter.GalleryId));
          userExtensionAssignment.TryAdd<string, LicensingSource>(offerSubscription.OfferMeter.GalleryId, LicensingSource.Account);
        }
        else
        {
          if (offerSubscription.IsTrialOrPreview)
          {
            DateTime? trialExpiryDate = offerSubscription.TrialExpiryDate;
            if (trialExpiryDate.HasValue)
            {
              trialExpiryDate = offerSubscription.TrialExpiryDate;
              DateTime utcNow = DateTime.UtcNow;
              if ((trialExpiryDate.HasValue ? (trialExpiryDate.GetValueOrDefault() > utcNow ? 1 : 0) : 0) != 0 && !offerSubscription.IsPreview)
              {
                requestContext.Trace(1034244, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "{0} is in trial because extension is in trial and its expiry date is less than utc now based on IsTrialOrPreview {1}, Trial Expiry Date {2} and UTC Now {3}", (object) offerSubscription.OfferMeter.GalleryId, (object) offerSubscription.IsTrialOrPreview, (object) offerSubscription.TrialExpiryDate, (object) DateTime.UtcNow);
                userExtensionAssignment.TryAdd<string, LicensingSource>(offerSubscription.OfferMeter.GalleryId, LicensingSource.Trial);
                continue;
              }
            }
          }
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && offerSubscription.OfferMeter.IsFirstParty)
          {
            if (!nullable.HasValue)
              nullable = new bool?(requestContext.GetExtension<ILegacyLicensingHandler>().IsEnterpriseUser(requestContext, userId));
            requestContext.Trace(1034244, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("User {0} isEnterpriseUser: {1}", (object) userId, (object) nullable));
            if (nullable.HasValue && nullable.Value)
              userExtensionAssignment.TryAdd<string, LicensingSource>(offerSubscription.OfferMeter.GalleryId, LicensingSource.Msdn);
          }
        }
      }
      return userExtensionAssignment;
    }

    private IList<Guid> GetMasterFromLocalId(
      IVssRequestContext collectionContext,
      IList<Guid> localIds)
    {
      return collectionContext.ExecutionEnvironment.IsOnPremisesDeployment ? (IList<Guid>) collectionContext.GetExtension<IOnPremisesLicensingIdentityHandler>().GetMasterFromLocalId(collectionContext, (IEnumerable<Guid>) localIds).Values.ToList<Guid>() : (IList<Guid>) PlatformExtensionEntitlementService.ReadIdentities(collectionContext, localIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (m => m != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (m =>
      {
        Guid guid = m.EnterpriseStorageKey(collectionContext);
        return !(guid != Guid.Empty) ? m.Id : guid;
      })).ToList<Guid>();
    }

    private void AssignExtensionToUsersUnchecked(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      if (!userIds.Any<Guid>())
        return;
      try
      {
        requestContext.Trace(1034266, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "AssignExtensionToUsersUnchecked: Making a call to DB component");
        this.m_licensingRepository.AssignExtensionLicenses(requestContext, userIds, extensionId, source, assignmentSource);
        this.InvalidateExtensionEntitlementCache(requestContext, userIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034268, "LicensingService", nameof (PlatformExtensionEntitlementService), ex);
        throw;
      }
    }

    private IList<AccountEntitlement> GetHostedUserEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      IList<AccountEntitlement> userEntitlements = (IList<AccountEntitlement>) new List<AccountEntitlement>();
      foreach (Guid userId in (IEnumerable<Guid>) userIds)
      {
        AccountEntitlement accountEntitlement = requestContext.GetService<IInternalPlatformEntitlementService>().GetAccountEntitlementForAccountUserInternal(requestContext, userId, false, false, false).Item1;
        if (accountEntitlement != (AccountEntitlement) null && (accountEntitlement.UserStatus == AccountUserStatus.Active || accountEntitlement.UserStatus == AccountUserStatus.Pending))
          userEntitlements.Add(accountEntitlement);
      }
      return userEntitlements;
    }

    private void CheckLicenseQuantity(
      IVssRequestContext requestContext,
      string meterName,
      int available,
      int needed)
    {
      if (available < needed)
      {
        requestContext.Trace(1034371, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("{0}: Not enough licenses available for extension {1}, {2} licenses available while {3} needed", (object) nameof (CheckLicenseQuantity), (object) meterName, (object) available, (object) needed));
        throw new LicenseNotAvailableException(LicensingResources.ExtensionLicenseNotAvailableException((object) (needed - available), (object) meterName));
      }
      requestContext.Trace(1034374, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), string.Format("{0}: There are enough licenses available for extension {1}, {2} licenses available while {3} needed", (object) nameof (CheckLicenseQuantity), (object) meterName, (object) available, (object) needed));
    }

    private bool UsageRightsMatch(
      VisualStudioOnlineServiceLevel minimumRequiredAccessLevel,
      License license)
    {
      return !(license == (License) null) && LicensingExtensionEntitlementHelper.GetAccessLevelOrder(PlatformAccountEntitlementService.ExtractVisualStudioOnlineServiceLevel(license)) >= LicensingExtensionEntitlementHelper.GetAccessLevelOrder(minimumRequiredAccessLevel);
    }

    private int GetExtensionUsage(IVssRequestContext requestContext, string extensionId) => this.m_licensingRepository.GetExtensionQuantities(requestContext, extensionId);

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) "PlatformExtensionEntitlement", (object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private void ValidateExtensionId(string extensionId) => ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionId, nameof (extensionId));

    private void ValidateAssignmentSource(AssignmentSource assignmentSource)
    {
      if (assignmentSource == AssignmentSource.None)
        throw new ArgumentException(string.Format("AssignmentSource '{0}' is not supported", (object) assignmentSource));
    }

    internal Dictionary<string, ExtensionAssignmentModel> GetExtensionsData(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<IOfferMeter> offerMeters = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Where<IOfferMeter>((Func<IOfferMeter, bool>) (x => x.IsExtensionCategory()));
      Dictionary<string, ExtensionAssignmentModel> dictionary = new Dictionary<string, ExtensionAssignmentModel>();
      foreach (IOfferMeter offerMeter in offerMeters)
        dictionary.TryAdd<string, ExtensionAssignmentModel>(offerMeter.GalleryId, offerMeter.AssignmentModel == OfferMeterAssignmentModel.Implicit ? ExtensionAssignmentModel.ImplicitAssignment : ExtensionAssignmentModel.ExplicitAssignment);
      return dictionary;
    }

    private VisualStudioOnlineServiceLevel GetMinimumRequiredAccessLevel(
      IVssRequestContext requestContext,
      string extensionId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, extensionId);
      if (offerMeter == null)
        return VisualStudioOnlineServiceLevel.Express;
      requestContext.Trace(1034361, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetMinimumRequiredAccessLevel: returning {0} access level for {1} extensionId", (object) offerMeter.MinimumRequiredAccessLevel.ToString(), (object) extensionId);
      return LicensingExtensionEntitlementHelper.MapToVisualStudioOnlineServiceLevel(offerMeter.MinimumRequiredAccessLevel);
    }

    private void GetExtensionData(
      IVssRequestContext requestContext,
      string extensionId,
      out VisualStudioOnlineServiceLevel minLevel,
      out ExtensionAssignmentModel assignmentModel,
      out bool isFirstParty,
      out int includedQuantity)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, extensionId);
      if (offerMeter != null)
      {
        assignmentModel = offerMeter.AssignmentModel == OfferMeterAssignmentModel.Implicit ? ExtensionAssignmentModel.ImplicitAssignment : ExtensionAssignmentModel.ExplicitAssignment;
        isFirstParty = offerMeter.IsFirstParty;
        minLevel = (VisualStudioOnlineServiceLevel) offerMeter.MinimumRequiredAccessLevel;
        includedQuantity = offerMeter.IncludedQuantity;
        requestContext.Trace(1034381, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "GetExtensionData: returning {0} assignment model for extension {1}", (object) assignmentModel.ToString(), (object) extensionId);
      }
      else
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          isFirstParty = false;
        }
        else
        {
          bool? nullable = this.IsConnectedOnPremisesServer(requestContext);
          if (nullable.HasValue && !nullable.Value)
          {
            IOnPremiseOfflineExtensionHandler extension = requestContext.GetExtension<IOnPremiseOfflineExtensionHandler>();
            isFirstParty = extension.IsFirstPartyExtension(requestContext, extensionId);
          }
          else
            isFirstParty = false;
        }
        assignmentModel = ExtensionAssignmentModel.ExplicitAssignment;
        minLevel = VisualStudioOnlineServiceLevel.Express;
        includedQuantity = 0;
      }
    }

    private bool CheckIfExtensionIsFirstParty(IVssRequestContext requestContext, string extensionId)
    {
      IOfferMeter offerMeter = requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext.To(TeamFoundationHostType.Deployment), extensionId);
      if (offerMeter != null)
      {
        requestContext.Trace(1034267, TraceLevel.Info, "LicensingService", nameof (PlatformExtensionEntitlementService), "CheckIfExtensionIsFirstParty: returning {0} for {1} extensionId", (object) offerMeter.IsFirstParty, (object) extensionId);
        return offerMeter.IsFirstParty;
      }
      bool? nullable = this.IsConnectedOnPremisesServer(requestContext);
      return nullable.HasValue && !nullable.Value;
    }

    private bool IsSubscriptionOnTrial(IVssRequestContext requestContext, string extensionId)
    {
      bool? nullable = this.IsConnectedOnPremisesServer(requestContext);
      if (nullable.HasValue && !nullable.Value)
        return false;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IOfferSubscription offerSubscription = vssRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(vssRequestContext, extensionId);
      return offerSubscription != null && offerSubscription.IsTrialOrPreview && !offerSubscription.IsPreview && !offerSubscription.IsPurchasedDuringTrial;
    }

    private ICollection<ExtensionOperationResultInternal> CreateOperationResultSetForFailedOperationExtension(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      IDictionary<Guid, ExtensionAssignmentDetails> userExtensionAssignmentStatus,
      ExtensionOperation operation)
    {
      List<ExtensionOperationResultInternal> operationExtension = new List<ExtensionOperationResultInternal>();
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in PlatformExtensionEntitlementService.ReadIdentities(requestContext, userIds))
      {
        Guid guid = readIdentity.EnterpriseStorageKey(requestContext);
        ExtensionAssignmentDetails assignmentDetails;
        Func<string, string> func;
        if (userExtensionAssignmentStatus.TryGetValue(guid, out assignmentDetails) && PlatformExtensionEntitlementService.s_warningMessageMap.TryGetValue(new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(assignmentDetails.AssignmentStatus, operation), out func))
        {
          ExtensionOperationResultInternal operationResultWarning = PlatformExtensionEntitlementService.CreateExtensionOperationResultWarning(instanceId, guid, extensionId, func(readIdentity.DisplayName), operation, assignmentDetails.AssignmentStatus);
          operationExtension.Add(operationResultWarning);
        }
      }
      return (ICollection<ExtensionOperationResultInternal>) operationExtension;
    }

    private static IDictionary<Tuple<ExtensionAssignmentStatus, ExtensionOperation>, Func<string, string>> CreateWarningMessageMap() => (IDictionary<Tuple<ExtensionAssignmentStatus, ExtensionOperation>, Func<string, string>>) new Dictionary<Tuple<ExtensionAssignmentStatus, ExtensionOperation>, Func<string, string>>()
    {
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.AccountAssignment, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C0\u003E__ExtensionAccountAssignmentAlreadyExistsWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C0\u003E__ExtensionAccountAssignmentAlreadyExistsWarning = new Func<string, string>(LicensingResources.ExtensionAccountAssignmentAlreadyExistsWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.BundleAssignment, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C1\u003E__ExtensionAlreadyExistsThroughBoundleWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C1\u003E__ExtensionAlreadyExistsThroughBoundleWarning = new Func<string, string>(LicensingResources.ExtensionAlreadyExistsThroughBoundleWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.NotEligible, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C2\u003E__UserNotEligibleForExtension ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C2\u003E__UserNotEligibleForExtension = new Func<string, string>(LicensingResources.UserNotEligibleForExtension))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.PendingValidation, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C3\u003E__UserPendingValidation ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C3\u003E__UserPendingValidation = new Func<string, string>(LicensingResources.UserPendingValidation))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.ImplicitAssignment, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C4\u003E__UserImplicitAssignment ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C4\u003E__UserImplicitAssignment = new Func<string, string>(LicensingResources.UserImplicitAssignment))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.RoamingAccountAssignment, ExtensionOperation.Assign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C5\u003E__InvalidRoamingAssignment ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C5\u003E__InvalidRoamingAssignment = new Func<string, string>(LicensingResources.InvalidRoamingAssignment))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.BundleAssignment, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C6\u003E__ExtensionBundleUnassignmentWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C6\u003E__ExtensionBundleUnassignmentWarning = new Func<string, string>(LicensingResources.ExtensionBundleUnassignmentWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.NotAssigned, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning = new Func<string, string>(LicensingResources.ExtensionAccountAssignmentDoesNotExistWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.NotEligible, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning = new Func<string, string>(LicensingResources.ExtensionAccountAssignmentDoesNotExistWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.PendingValidation, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C7\u003E__ExtensionAccountAssignmentDoesNotExistWarning = new Func<string, string>(LicensingResources.ExtensionAccountAssignmentDoesNotExistWarning))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.ImplicitAssignment, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C8\u003E__ImplicitAssignmentExtensionUnassignment ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C8\u003E__ImplicitAssignmentExtensionUnassignment = new Func<string, string>(LicensingResources.ImplicitAssignmentExtensionUnassignment))
      },
      {
        new Tuple<ExtensionAssignmentStatus, ExtensionOperation>(ExtensionAssignmentStatus.RoamingAccountAssignment, ExtensionOperation.Unassign),
        PlatformExtensionEntitlementService.\u003C\u003EO.\u003C9\u003E__InvalidRoamingUnassignment ?? (PlatformExtensionEntitlementService.\u003C\u003EO.\u003C9\u003E__InvalidRoamingUnassignment = new Func<string, string>(LicensingResources.InvalidRoamingUnassignment))
      }
    };

    private static ExtensionOperationResultInternal CreateExtensionOperationResultWarning(
      Guid accountId,
      Guid userId,
      string extensionId,
      string message,
      ExtensionOperation operation,
      ExtensionAssignmentStatus status)
    {
      return new ExtensionOperationResultInternal(accountId, userId, extensionId, operation, status, OperationResult.Warning, message);
    }

    private static List<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> userIdentityIds)
    {
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, userIdentityIds, QueryMembership.None, (IEnumerable<string>) null).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private void CheckPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      string token)
    {
      IVssRequestContext vssRequestContext = requestContext;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsSps())
        vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, LicensingSecurity.NamespaceId).CheckPermission(vssRequestContext, token, requestedPermissions, false);
    }

    private void InvalidateExtensionEntitlementCache(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      List<Microsoft.VisualStudio.Services.Identity.Identity> source = PlatformExtensionEntitlementService.ReadIdentities(requestContext, userIds);
      requestContext.GetService<IExtensionRightsCacheService>().InvalidateCacheAll(requestContext, (IList<Guid>) source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (_ => _.Id)).ToList<Guid>());
    }

    internal Guid GetCurrentCollectionId(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) ? requestContext.ServiceHost.CollectionServiceHost.InstanceId : Guid.Empty;

    internal string GetCollectionName(IVssRequestContext requestContext, Guid collectionId)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !(collectionId != Guid.Empty) || !requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        return string.Empty;
      if (this.collectionNameCache.ContainsKey(collectionId))
        return this.collectionNameCache[collectionId];
      TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, collectionId);
      string collectionName = serviceHostProperties != null ? serviceHostProperties.Name : string.Empty;
      this.collectionNameCache[collectionId] = collectionName;
      return collectionName;
    }

    private bool IsQuantityCheckRequired(IVssRequestContext requestContext, string extensionId) => !requestContext.ExecutionEnvironment.Flags.HasFlag((Enum) ExecutionEnvironmentFlags.OnPremisesDeployment) || !this.CheckIfExtensionIsFirstParty(requestContext, extensionId);

    private bool? IsConnectedOnPremisesServer(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? new bool?(!requestContext.GetService<ICloudConnectedService>().GetConnectedAccountId(requestContext).Equals(Guid.Empty)) : new bool?();
  }
}
