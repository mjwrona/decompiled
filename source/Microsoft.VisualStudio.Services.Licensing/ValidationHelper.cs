// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ValidationHelper
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class ValidationHelper
  {
    private const string s_area = "VisualStudio.Services.Licensing.ValidationHelper";
    private const string s_layer = "BusinessLogic";

    public static Microsoft.VisualStudio.Services.Identity.Identity ValidateCollectionLevelStorageKey(
      IVssRequestContext requestContext,
      Guid storageKey,
      bool materializeIfNotFound = false)
    {
      try
      {
        requestContext.CheckProjectCollectionRequestContext();
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          storageKey
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
        {
          if (materializeIfNotFound)
          {
            requestContext.Trace(1030280, TraceLevel.Error, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), string.Format("Materialization error: {0} is not materialized in this collection. The original caller should have materialized this user before calling Licensing", (object) storageKey));
            identity = service.ReadIdentityWithFallback(requestContext, storageKey);
            if (identity != null)
            {
              identity.Id = IdentityHelper.MaterializeUser(requestContext, (IVssIdentity) identity, nameof (ValidateCollectionLevelStorageKey));
              requestContext.Trace(1030281, TraceLevel.Error, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), string.Format("Materialized {0} from requested storage key ({1})", (object) identity.Id, (object) storageKey));
            }
            else
            {
              requestContext.Trace(1030282, TraceLevel.Error, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), string.Format("Storage key, {0}, could not be materialized because it could not be found at any level", (object) storageKey));
              return (Microsoft.VisualStudio.Services.Identity.Identity) null;
            }
          }
          else
          {
            requestContext.Trace(1030283, TraceLevel.Error, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), string.Format("Invalid Storage Key: {0}. This identity will not be materialized. Returning null identity", (object) storageKey));
            return (Microsoft.VisualStudio.Services.Identity.Identity) null;
          }
        }
        if (identity != null && identity.Id != storageKey)
          requestContext.Trace(1030283, TraceLevel.Error, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), string.Format("Invalid Storage Key: {0}, was not a collection level key. The correct key was {1}", (object) storageKey, (object) identity.Id));
        return identity;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030284, nameof (ValidationHelper), nameof (ValidateCollectionLevelStorageKey), ex);
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ValidateIdentityId(
      IVssRequestContext requestContext,
      Guid userId,
      bool shouldValidateUserIdentity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity accountIdentity = requestContext.GetService<IdentityService>().ReadIdentityWithFallback(requestContext, userId);
      if (accountIdentity == null)
      {
        if (IdentityIdChecker.IsStorageKey(userId))
          throw new InvalidLicensingOperation(LicensingResources.UserNotFound((object) userId));
        throw new ArgumentException(string.Format("UserId {0} is not a valid storage key", (object) userId), nameof (userId));
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.UpgradeHistoricalIdentity(requestContext, accountIdentity);
      ValidationHelper.ValidateIdentity(requestContext, identity, shouldValidateUserIdentity);
      return identity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ValidateIdentity(
      IVssRequestContext requestContext,
      SubjectDescriptor identityDescriptor,
      bool shouldValidateUserIdentity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = ValidationHelper.UpgradeHistoricalIdentity(requestContext, requestContext.GetService<IdentityService>().ReadIdentityWithFallback(requestContext, identityDescriptor) ?? throw new InvalidLicensingOperation(LicensingResources.UserNotFound((object) identityDescriptor)));
      ValidationHelper.ValidateIdentity(requestContext, identity, shouldValidateUserIdentity);
      return identity;
    }

    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> ValidateIdentities(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> identityDescriptors,
      bool shouldValidateUserIdentity)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = requestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(requestContext, identityDescriptors).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (list == null)
        throw new InvalidLicensingOperation(LicensingResources.UserNotFound((object) identityDescriptors));
      list.ForEach((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (identity => ValidationHelper.ValidateIdentity(requestContext, identity, shouldValidateUserIdentity)));
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list;
    }

    internal static void ValidateLicenseIsAssignable(License license)
    {
      bool flag;
      switch (license.Source)
      {
        case LicensingSource.None:
          flag = license == License.None;
          break;
        case LicensingSource.Account:
        case LicensingSource.Msdn:
          flag = true;
          break;
        case LicensingSource.Auto:
          flag = ValidationHelper.IsSupportedAutoLicense(license);
          break;
        default:
          flag = false;
          break;
      }
      if (!flag)
        throw new InvalidLicensingOperation(LicensingResources.RequestedLicenseNotAvailable((object) license));
    }

    internal static bool IsSupportedAutoLicense(License license)
    {
      License.AutoLicense autoLicense = license as License.AutoLicense;
      return (License) autoLicense == (License) null || (License) autoLicense == License.Auto || (License) autoLicense == License.AutoLicense.Msdn;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity UpgradeHistoricalIdentity(
      IVssRequestContext accountContext,
      Microsoft.VisualStudio.Services.Identity.Identity accountIdentity)
    {
      if (accountContext.IsSps() && accountIdentity.IsImported && !accountContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableUpgradeHistoricalIdentitiesOnLicensing"))
      {
        Guid id = accountIdentity.Id;
        accountContext.Trace(1030234, TraceLevel.Info, "VisualStudio.Services.Licensing.ValidationHelper", "BusinessLogic", "Attempting to upgrade historical identity to claims identity for licensing; identityIdToBeFixed = {0}", (object) id);
        Microsoft.VisualStudio.Services.Identity.Identity claimsIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (accountContext.GetService<IPlatformIdentityServiceInternal>().TryUpgradeHistoricalIdentityToClaimsIdentity(accountContext, id, out claimsIdentity))
        {
          accountContext.TraceSerializedConditionally(1030235, TraceLevel.Info, "VisualStudio.Services.Licensing.ValidationHelper", "BusinessLogic", "Successfully upgraded historical identity to claims identity for licensing; identityIdToBeFixed = {0}, claimsIdentity = {1}", (object) id, (object) claimsIdentity);
          return claimsIdentity;
        }
        accountContext.Trace(1030236, TraceLevel.Error, "VisualStudio.Services.Licensing.ValidationHelper", "BusinessLogic", "Failed to upgrade historical identity to claims identity for licensing; identityIdToBeFixed = {0}", (object) id);
      }
      return accountIdentity;
    }

    private static void ValidateIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool shouldValidateUserIdentity)
    {
      if (IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal))
        throw new InvalidLicensingOperation(LicensingResources.InvalidUserId((object) identity.ToString()));
      if (shouldValidateUserIdentity && IdentityHelper.IsServiceIdentity(requestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) identity))
        throw new InvalidLicensingOperation(LicensingResources.InvaliduserIdServiceIdentity((object) identity.ToString()));
      if (shouldValidateUserIdentity && IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity))
        throw new InvalidLicensingOperation(LicensingResources.InvaliduserIdServiceIdentity((object) identity.ToString()));
      if (identity.IsContainer)
        throw new InvalidLicensingOperation(LicensingResources.InvaliduserIdGroupIdentity((object) identity.ToString()));
      if (shouldValidateUserIdentity && !identity.IsClaims && !identity.IsBindPending && !identity.IsAADServicePrincipal)
        throw new InvalidLicensingOperation(LicensingResources.InvalidIdentityType((object) identity.ToString()));
    }
  }
}
