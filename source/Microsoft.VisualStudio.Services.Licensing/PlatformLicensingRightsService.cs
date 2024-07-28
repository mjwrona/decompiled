// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.PlatformLicensingRightsService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class PlatformLicensingRightsService : ILicensingRightsService, IVssFrameworkService
  {
    private const string c_disableTraceLicensingFeature = "VisualStudio.Services.Licensing.TraceTransferUserLicensesEvent.Disable";
    private const string c_shortCircuitLicenseTransferOnError = "VisualStudio.Services.Licensing.ShortCircuitTransferOnError";
    private const string c_area = "LicensingService";
    private const string c_layer = "PlatformLicensingRightsService";
    private const string s_userEntitlementsBatchMaxSizeRegistryKey = "/Service/Licensing/AccountEntitlement/UserEntitlementsBatchMaxSize";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void PreValidateTransferIdentityRights(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.ShortCircuitTransferOnError"))
        return;
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> masterIdToIdentityMap;
      IDictionary<Guid, AccountEntitlement> userIdToLicenseMap;
      PlatformLicensingRightsService.GenerateAccountEntitlementsMap(requestContext, userIdTransferMap, out masterIdToIdentityMap, out userIdToLicenseMap);
      foreach (KeyValuePair<Guid, Guid> userIdTransfer in userIdTransferMap)
        PlatformLicensingRightsService.GetLicenseAndIdentities(requestContext, userIdTransfer, masterIdToIdentityMap, userIdToLicenseMap, true);
    }

    public void TransferIdentityRights(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      try
      {
        if (userIdTransferMap.IsNullOrEmpty<KeyValuePair<Guid, Guid>>())
          return;
        IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> masterIdToIdentityMap;
        IDictionary<Guid, AccountEntitlement> userIdToLicenseMap;
        PlatformLicensingRightsService.GenerateAccountEntitlementsMap(requestContext, userIdTransferMap, out masterIdToIdentityMap, out userIdToLicenseMap);
        ILicensingRepository licensingRepository = LicensingRepositoryFactory<ApplicationLicensingRepository>.Create(requestContext);
        licensingRepository.TransferUserLicenses(requestContext, userIdTransferMap);
        PlatformLicensingRightsService.TraceTransferUserLicensesEvent(requestContext, userIdTransferMap, masterIdToIdentityMap, userIdToLicenseMap);
        try
        {
          licensingRepository.TransferUserExtensionLicenses(requestContext, userIdTransferMap);
        }
        catch (NotImplementedException ex1)
        {
          try
          {
            List<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMapping = new List<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>();
            foreach (KeyValuePair<Guid, Guid> userIdTransfer in userIdTransferMap)
            {
              (Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity, Microsoft.VisualStudio.Services.Identity.Identity targetIdentity, AccountEntitlement sourceAccountEntitlement) licenseAndIdentities = PlatformLicensingRightsService.GetLicenseAndIdentities(requestContext, userIdTransfer, masterIdToIdentityMap, userIdToLicenseMap);
              identityMapping.Add(new Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>(licenseAndIdentities.sourceIdentity, licenseAndIdentities.targetIdentity));
            }
            requestContext.GetService<IExtensionEntitlementService>().TransferExtensionsForIdentities(requestContext, (IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMapping);
          }
          catch (Exception ex2)
          {
            requestContext.TraceException(5001245, "LicensingService", nameof (PlatformLicensingRightsService), ex2);
            IEnumerable<string> identityMapStr = userIdTransferMap.Select<KeyValuePair<Guid, Guid>, string>((Func<KeyValuePair<Guid, Guid>, string>) (x => string.Format("source {0}, target {1}", (object) x.Key, (object) x.Value)));
            requestContext.TraceConditionally(5001247, TraceLevel.Error, "LicensingService", nameof (PlatformLicensingRightsService), (Func<string>) (() => string.Format("Transferring extension rights failed! {0}", (object) identityMapStr)));
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5001260, "LicensingService", nameof (PlatformLicensingRightsService), ex);
        throw new FailedToTransferLicensesException(LicensingResources.FailedToTransferLicensesException(), ex);
      }
    }

    private static void GenerateAccountEntitlementsMap(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      out IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> masterIdToIdentityMap,
      out IDictionary<Guid, AccountEntitlement> userIdToLicenseMap)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.TraceTransferUserLicensesEvent.Disable"))
      {
        masterIdToIdentityMap = (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
        userIdToLicenseMap = (IDictionary<Guid, AccountEntitlement>) new Dictionary<Guid, AccountEntitlement>();
      }
      else
      {
        masterIdToIdentityMap = PlatformLicensingRightsService.ReadIdentitiesByMasterId(requestContext, (IList<Guid>) userIdTransferMap.SelectMany<KeyValuePair<Guid, Guid>, Guid>((Func<KeyValuePair<Guid, Guid>, IEnumerable<Guid>>) (x => (IEnumerable<Guid>) new List<Guid>()
        {
          x.Key,
          x.Value
        })).ToList<Guid>());
        userIdToLicenseMap = PlatformLicensingRightsService.ReadSourceAccountEntitlements(requestContext, (IList<Guid>) userIdTransferMap.Select<KeyValuePair<Guid, Guid>, Guid>((Func<KeyValuePair<Guid, Guid>, Guid>) (x => x.Key)).ToList<Guid>());
      }
    }

    private static IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByMasterId(
      IVssRequestContext requestContext,
      IList<Guid> masterIds)
    {
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> list;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) vssRequestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(vssRequestContext, (IEnumerable<Guid>) masterIds).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(5001319, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Exception in ReadIdentitiesByMasterId : '{0}'.", (object) ex));
        return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) dictionary;
      }
      if (!list.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        foreach (Guid masterId1 in (IEnumerable<Guid>) masterIds)
        {
          Guid masterId = masterId1;
          if (!dictionary.ContainsKey(masterId))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.EnterpriseStorageKey(requestContext) == masterId));
            if (identity == null)
              requestContext.TraceAlways(5001311, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Identity not found for master id '{0}'.", (object) masterId));
            else
              dictionary.Add(masterId, identity);
          }
          else
            requestContext.TraceAlways(5001312, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Duplicate identity found in the dictionary for masterId '{0}.", (object) masterId));
        }
      }
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) dictionary;
    }

    private static IDictionary<Guid, AccountEntitlement> ReadSourceAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> sourceMasterIds)
    {
      Dictionary<Guid, AccountEntitlement> dictionary = new Dictionary<Guid, AccountEntitlement>();
      ILicensingEntitlementService service = requestContext.GetService<ILicensingEntitlementService>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int batchSize = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/UserEntitlementsBatchMaxSize", 100);
      foreach (IList<Guid> userIds in sourceMasterIds.Batch<Guid>(batchSize))
      {
        IList<AccountEntitlement> accountEntitlements;
        try
        {
          accountEntitlements = service.GetAccountEntitlements(requestContext, userIds, true);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(5001320, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Exception in GetAccountEntitlements : '{0}'.", (object) ex));
          continue;
        }
        if (!accountEntitlements.IsNullOrEmpty<AccountEntitlement>())
        {
          foreach (Guid guid in (IEnumerable<Guid>) userIds)
          {
            Guid sourceMasterId = guid;
            if (!dictionary.ContainsKey(sourceMasterId))
            {
              AccountEntitlement accountEntitlement = accountEntitlements.FirstOrDefault<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x != (AccountEntitlement) null && x.UserId == sourceMasterId));
              if (accountEntitlement == (AccountEntitlement) null)
                requestContext.TraceAlways(5001310, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Account entitlement not found for master id '{0}'.", (object) sourceMasterId));
              else
                dictionary.Add(sourceMasterId, accountEntitlement);
            }
            else
              requestContext.TraceAlways(5001315, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), string.Format("Duplicate account entitlement found in the dictionary for master id '{0}'.", (object) sourceMasterId));
          }
        }
      }
      return (IDictionary<Guid, AccountEntitlement>) dictionary;
    }

    private static void TraceTransferUserLicensesEvent(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> masterIdToIdentityMap,
      IDictionary<Guid, AccountEntitlement> userIdToLicenseMap)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.TraceTransferUserLicensesEvent.Disable"))
        return;
      foreach (KeyValuePair<Guid, Guid> userIdTransfer in userIdTransferMap)
      {
        (Microsoft.VisualStudio.Services.Identity.Identity identity1, Microsoft.VisualStudio.Services.Identity.Identity identity2, AccountEntitlement accountEntitlement) = PlatformLicensingRightsService.GetLicenseAndIdentities(requestContext, userIdTransfer, masterIdToIdentityMap, userIdToLicenseMap);
        if (identity1 != null && identity2 != null && accountEntitlement != (AccountEntitlement) null && requestContext.LicensingTracingEnabled())
        {
          PlatformLicensingRightsService.TraceLicensingEventHelper(requestContext, identity1, accountEntitlement, AccountUserStatus.Deleted, string.Format("As part of transferring user license from user '{0}' to user '{1}', deleting license of user '{2}'.", (object) userIdTransfer.Key, (object) userIdTransfer.Value, (object) userIdTransfer.Key));
          PlatformLicensingRightsService.TraceLicensingEventHelper(requestContext, identity2, accountEntitlement, accountEntitlement.UserStatus, string.Format("As part of transferring user license from user '{0}' to user '{1}', assigning license to user '{2}'.", (object) userIdTransfer.Key, (object) userIdTransfer.Value, (object) userIdTransfer.Value));
        }
      }
    }

    private static (Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity, Microsoft.VisualStudio.Services.Identity.Identity targetIdentity, AccountEntitlement sourceAccountEntitlement) GetLicenseAndIdentities(
      IVssRequestContext requestContext,
      KeyValuePair<Guid, Guid> userIdPair,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> masterIdToIdentityMap,
      IDictionary<Guid, AccountEntitlement> userIdToLicenseMap,
      bool throwOnError = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      AccountEntitlement accountEntitlement = (AccountEntitlement) null;
      if (!masterIdToIdentityMap.TryGetValue(userIdPair.Key, out identity1))
      {
        string str = string.Format("Identity not found in the dictionary '{0}'.", (object) userIdPair.Key);
        requestContext.TraceAlways(5001316, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), str);
        if (throwOnError)
          throw new TransferUserLicenseException(str);
      }
      if (!masterIdToIdentityMap.TryGetValue(userIdPair.Value, out identity2))
      {
        string str = string.Format("Identity not found in the dictionary '{0}'.", (object) userIdPair.Value);
        requestContext.TraceAlways(5001317, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), str);
        if (throwOnError)
          throw new TransferUserLicenseException(str);
      }
      if (!userIdToLicenseMap.TryGetValue(userIdPair.Key, out accountEntitlement))
      {
        string str = string.Format("Account entitlement not found in the dictionary for source identity '{0}'.", (object) userIdPair.Key);
        requestContext.TraceAlways(5001318, TraceLevel.Warning, "LicensingService", nameof (PlatformLicensingRightsService), str);
        if (throwOnError && string.Equals(identity1.Descriptor.IdentityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase))
          throw new TransferUserLicenseException(str);
      }
      return (identity1, identity2, accountEntitlement);
    }

    private static void TraceLicensingEventHelper(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      AccountEntitlement accountEntitlement,
      AccountUserStatus accountUserStatus,
      string message)
    {
      Guid instanceId1 = requestContext.ServiceHost.InstanceId;
      Guid instanceId2 = requestContext.ServiceHost.ParentServiceHost.InstanceId;
      int hostType = (int) requestContext.ServiceHost.HostType;
      Guid userID = identity.EnterpriseStorageKey(requestContext);
      Guid userCUID = identity.Cuid();
      int userStatus = (int) accountUserStatus;
      int? licenseSourceId = new int?((int) accountEntitlement.License.Source);
      int? licenseId = new int?(accountEntitlement.License.GetLicenseAsInt32());
      string userStatusName = accountUserStatus.ToString();
      string licenseSourceName = accountEntitlement.License.Source.ToString();
      string licenseName = accountEntitlement.License.ToString();
      DateTimeOffset assignmentDate = accountEntitlement.AssignmentDate;
      DateTime dateTime1 = assignmentDate.DateTime;
      assignmentDate = accountEntitlement.AssignmentDate;
      DateTime dateTime2 = assignmentDate.DateTime;
      DateTime utcNow = DateTime.UtcNow;
      string changeType = message;
      TeamFoundationTracingService.TraceAccountUserLicensingChanges(instanceId1, instanceId2, (TeamFoundationHostType) hostType, userID, userCUID, userStatus, licenseSourceId, licenseId, userStatusName, licenseSourceName, licenseName, dateTime1, dateTime2, utcNow, changeType);
    }
  }
}
