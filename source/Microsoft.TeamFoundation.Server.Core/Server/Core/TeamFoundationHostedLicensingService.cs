// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationHostedLicensingService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationHostedLicensingService : TeamFoundationLicensingServiceBase
  {
    private static readonly IReadOnlyDictionary<VisualStudioOnlineServiceLevel, Guid> s_licenseLevelToLicenseId = (IReadOnlyDictionary<VisualStudioOnlineServiceLevel, Guid>) new Dictionary<VisualStudioOnlineServiceLevel, Guid>()
    {
      {
        VisualStudioOnlineServiceLevel.Express,
        HostedLicenseName.Express
      },
      {
        VisualStudioOnlineServiceLevel.Stakeholder,
        HostedLicenseName.Stakeholder
      },
      {
        VisualStudioOnlineServiceLevel.Advanced,
        HostedLicenseName.Advanced
      },
      {
        VisualStudioOnlineServiceLevel.AdvancedPlus,
        HostedLicenseName.AdvancedPlus
      }
    };
    protected new static readonly string s_area = "TeamFoundationLicensing";
    protected new static readonly string s_layer = nameof (TeamFoundationHostedLicensingService);

    public override bool IsFeatureSupported(
      IVssRequestContext requestContext,
      Guid featureId,
      IdentityDescriptor userContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(featureId, nameof (featureId));
      if (userContext == (IdentityDescriptor) null)
        return false;
      if (IdentityDescriptorComparer.Instance.Equals(userContext, requestContext.ServiceHost.SystemDescriptor()))
        return true;
      try
      {
        this.EnsureSettingsLoaded(requestContext);
        if (this.m_previewFeatures.Contains(featureId))
          return true;
        ILicenseType licenseForUserInternal = this.GetLicenseForUserInternal(requestContext, userContext);
        if (licenseForUserInternal == null)
          throw new LicenseNotAvailableException(Resources.LicenseNotFound((object) userContext));
        ILicenseType licenseType;
        return ((IEnumerable<Guid>) licenseForUserInternal.Features).Contains<Guid>(featureId) || licenseForUserInternal.Id != HostedLicenseName.Stakeholder && TeamFoundationHostedLicensingService.IsAccountInTrial(requestContext) || licenseForUserInternal.Id == HostedLicenseName.Stakeholder && StakeholderLicensingHelper.IsBuildAndReleaseEnabledForStakeholders(requestContext) && this.m_licenseToLicenseType.TryGetValue(HostedLicenseName.Limited, out licenseType) && ((IEnumerable<Guid>) licenseType.Features).Contains<Guid>(featureId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(66501, TeamFoundationHostedLicensingService.s_area, TeamFoundationHostedLicensingService.s_layer, ex);
      }
      return false;
    }

    public override bool IsFeatureInAdvertisingMode(
      IVssRequestContext requestContext,
      Guid featureId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      bool flag = this.IsFeatureSupported(requestContext, featureId);
      if (!flag && requestContext.UserContext != (IdentityDescriptor) null)
      {
        ILicenseType licenseForUserInternal = this.GetLicenseForUserInternal(requestContext, requestContext.UserContext);
        if (licenseForUserInternal == null)
          throw new LicenseNotAvailableException(Resources.LicenseNotFound((object) requestContext.UserContext));
        if (licenseForUserInternal.Id == HostedLicenseName.Stakeholder)
          return false;
      }
      return !flag;
    }

    public override ILicenseType[] GetLicensesForUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(userDescriptor, nameof (userDescriptor));
      try
      {
        this.EnsureSettingsLoaded(requestContext);
        ILicenseType licenseType = this.LocalizeLicenseType(this.GetLicenseForUserInternal(requestContext, userDescriptor) ?? throw new LicenseNotAvailableException(Resources.LicenseNotFound((object) requestContext.UserContext)));
        this.TraceLicenseType(requestContext, licenseType);
        return new ILicenseType[1]{ licenseType };
      }
      catch (Exception ex)
      {
        requestContext.TraceException(66502, TeamFoundationHostedLicensingService.s_area, TeamFoundationHostedLicensingService.s_layer, ex);
        throw;
      }
    }

    private ILicenseType TransformToLicenseType(
      IVssRequestContext requestContext,
      VisualStudioOnlineServiceLevel entitledServiceLevel)
    {
      ILicenseType licenseType1 = this.GetDefaultLicense(requestContext);
      ILicenseType licenseType2;
      if ((entitledServiceLevel == VisualStudioOnlineServiceLevel.Stakeholder || entitledServiceLevel == VisualStudioOnlineServiceLevel.Express || entitledServiceLevel == VisualStudioOnlineServiceLevel.Advanced || entitledServiceLevel == VisualStudioOnlineServiceLevel.AdvancedPlus) && this.m_licenseToLicenseType.TryGetValue(TeamFoundationHostedLicensingService.s_licenseLevelToLicenseId[entitledServiceLevel], out licenseType2))
        licenseType1 = licenseType2;
      return licenseType1;
    }

    private ILicenseType GetDefaultLicense(IVssRequestContext requestContext) => !this.m_licenseToLicenseType.ContainsKey(HostedLicenseName.Stakeholder) || !TeamFoundationHostedLicensingService.IsStakeholderFeatureEnabled(requestContext) ? this.m_defaultLicense : this.m_licenseToLicenseType[HostedLicenseName.Stakeholder];

    private ILicenseType GetLicenseForUserInternal(
      IVssRequestContext requestContext,
      IdentityDescriptor userContext)
    {
      VisualStudioOnlineServiceLevel entitledServiceLevel = VisualStudioOnlineServiceLevel.None;
      if (requestContext.IsAnonymous() || requestContext.IsPublicUser())
        return this.TransformToLicenseType(requestContext, VisualStudioOnlineServiceLevel.Express);
      if (IdentityDescriptorComparer.Instance.Equals(userContext, requestContext.UserContext))
        entitledServiceLevel = requestContext.GetAccountEntitlement(requestContext.GetUserId())?.Rights?.Level.GetValueOrDefault();
      if (entitledServiceLevel == VisualStudioOnlineServiceLevel.None)
        entitledServiceLevel = TeamFoundationHostedLicensingService.GetServiceLevel(requestContext, userContext);
      if (entitledServiceLevel == VisualStudioOnlineServiceLevel.None && TeamFoundationHostedLicensingService.IsStakeholderFeatureEnabled(requestContext))
        entitledServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;
      return this.TransformToLicenseType(requestContext, entitledServiceLevel);
    }

    private static VisualStudioOnlineServiceLevel GetServiceLevel(
      IVssRequestContext requestContext,
      IdentityDescriptor userContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return VisualStudioOnlineServiceLevel.None;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.ReadIdentity(userContext);
      if (identity == null)
        throw new IdentityNotFoundException(string.Format("Couldn`t find identity: {0} in account: {1}.", (object) userContext, (object) requestContext.ServiceHost));
      VisualStudioOnlineServiceLevel valueOrDefault = requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlement(requestContext, identity.Id)?.Rights?.Level.GetValueOrDefault();
      requestContext.Trace(66508, TraceLevel.Warning, TeamFoundationHostedLicensingService.s_area, TeamFoundationHostedLicensingService.s_layer, "Made an call to get license of user '{0}' while current user context is '{1}'. Retrieved account rights '{2}'.", (object) userContext, (object) requestContext.UserContext, (object) valueOrDefault);
      return valueOrDefault;
    }

    private static bool IsAccountInTrial(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      Collection collection = (Collection) null;
      try
      {
        collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext.Elevate(), (IEnumerable<string>) new string[2]
        {
          "SystemProperty.TrialStartDate",
          "SystemProperty.TrialEndDate"
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(66503, TeamFoundationHostedLicensingService.s_area, TeamFoundationHostedLicensingService.s_layer, ex);
      }
      if (collection == null)
        return false;
      object obj1;
      collection.Properties.TryGetValue("SystemProperty.TrialStartDate", out obj1);
      object obj2;
      collection.Properties.TryGetValue("SystemProperty.TrialEndDate", out obj2);
      if (!(obj1 is DateTime) || !(obj2 is DateTime))
        return false;
      DateTime utcNow = DateTime.UtcNow;
      return (DateTime) obj1 <= utcNow && utcNow <= (DateTime) obj2;
    }

    private void TraceLicenseType(IVssRequestContext requestContext, ILicenseType licenseType) => requestContext.TraceConditionally(66509, TraceLevel.Info, TeamFoundationHostedLicensingService.s_area, TeamFoundationHostedLicensingService.s_layer, (Func<string>) (() => string.Format("License name: {0}, description {1}, is default {2}, features: {3}", (object) licenseType.Name, (object) licenseType.Description, (object) licenseType.IsDefault, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) licenseType.Features))));

    private static bool IsStakeholderFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(LicensingFeatureFlags.StakeholderFeatureFlagName);
  }
}
