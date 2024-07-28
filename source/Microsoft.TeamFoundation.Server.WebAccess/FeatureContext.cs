// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FeatureContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class FeatureContext
  {
    private IVssRequestContext m_requestContext;
    private ITeamFoundationLicensingService m_licenseService;
    private IDictionary<Guid, bool> m_featureAvailableCache = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
    private IDictionary<Guid, bool> m_featureInAdvertisingCache = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
    private IDictionary<Guid, bool> m_featureLicensedCache = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
    internal const string s_featureName = "VisualStudio.PremiumFeatures.Licensing";

    protected FeatureContext()
    {
    }

    public FeatureContext(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_licenseService = this.m_requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>();
    }

    public bool AreStandardFeaturesAvailable => this.IsFeatureAvailable(LicenseFeatures.StandardFeaturesId);

    public bool IsAdminFeatureAvailable => this.IsFeatureAvailable(LicenseFeatures.AdminId);

    public bool AreAllFeaturesAvailable => FeatureContext.PreCheck(this.m_requestContext, (Func<bool>) (() =>
    {
      ILicenseType[] licensesForUser = this.m_licenseService.GetLicensesForUser(this.m_requestContext, this.m_requestContext.GetUserIdentity().Descriptor);
      ISet<Guid> myFeatures = (ISet<Guid>) new HashSet<Guid>(((IEnumerable<ILicenseType>) licensesForUser).SelectMany<ILicenseType, Guid>((Func<ILicenseType, IEnumerable<Guid>>) (l => (IEnumerable<Guid>) l.Features)));
      ISet<Guid> previewFeatures = (ISet<Guid>) new HashSet<Guid>((IEnumerable<Guid>) this.m_licenseService.GetLicenseFeaturesInPreview(this.m_requestContext));
      return !((IEnumerable<ILicenseType>) this.m_licenseService.GetAllLicenses(this.m_requestContext)).Except<ILicenseType>((IEnumerable<ILicenseType>) licensesForUser).SelectMany<ILicenseType, Guid>((Func<ILicenseType, IEnumerable<Guid>>) (l => (IEnumerable<Guid>) l.Features)).Where<Guid>((Func<Guid, bool>) (f => !myFeatures.Contains(f) && !previewFeatures.Contains(f))).Any<Guid>();
    }));

    public bool AreMultipleLicensesAvailable => this.m_licenseService.GetAllLicenses(this.m_requestContext).Length > 1;

    public virtual bool IsFeatureAvailable(Guid featureId)
    {
      bool flag1 = FeatureContext.PreCheck(this.m_requestContext, new Guid?(featureId), true, (Func<bool>) (() =>
      {
        bool flag2;
        if (this.m_featureAvailableCache.TryGetValue(featureId, out flag2))
        {
          if (this.m_requestContext.IsTracing(520065, TraceLevel.Verbose, "WebAccess", TfsTraceLayers.Controller))
            this.m_requestContext.Trace(520065, TraceLevel.Verbose, "WebAccess", TfsTraceLayers.Controller, "Feature availability result from m_featureAvailableCache: feature: {0}  result: {1}, request context: {2}, service host is {3}", (object) featureId, (object) flag2, (object) this.m_requestContext.UserContext.Serialize<IdentityDescriptor>(), (object) this.m_requestContext.ServiceHost);
          return flag2;
        }
        bool flag3 = this.m_licenseService.IsFeatureSupported(this.m_requestContext, featureId);
        this.m_featureAvailableCache[featureId] = flag3;
        return flag3;
      }));
      this.m_requestContext.Trace(520025, TraceLevel.Verbose, "WebAccess", TfsTraceLayers.Controller, "IsFeatureSupported: feature: {0}  result: {1}", (object) featureId, (object) flag1);
      return flag1;
    }

    public ILicenseFeature GetLicenseFeature(Guid featureId) => this.m_licenseService.GetLicenseFeature(this.m_requestContext, featureId);

    public bool IsFeatureLicensed(Guid featureId)
    {
      bool flag1 = FeatureContext.PreCheck(this.m_requestContext, new Guid?(featureId), true, (Func<bool>) (() =>
      {
        bool flag2 = false;
        if (!this.m_featureLicensedCache.TryGetValue(featureId, out flag2))
        {
          flag2 = this.IsFeatureLicensedInternal(featureId);
          this.m_featureLicensedCache[featureId] = flag2;
        }
        return flag2;
      }));
      this.m_requestContext.Trace(520026, TraceLevel.Verbose, "WebAccess", TfsTraceLayers.Controller, "IsFeatureLicensed: feature: {0}  result: {1}", (object) featureId, (object) flag1);
      return flag1;
    }

    private bool IsFeatureLicensedInternal(Guid featureId)
    {
      if (this.m_requestContext.UserContext == (IdentityDescriptor) null)
        return false;
      ILicenseType[] licensesForUser = this.m_licenseService.GetLicensesForUser(this.m_requestContext, this.m_requestContext.UserContext);
      return licensesForUser != null && ((IEnumerable<ILicenseType>) licensesForUser).Where<ILicenseType>((Func<ILicenseType, bool>) (license => license.Features != null)).SelectMany<ILicenseType, Guid>((Func<ILicenseType, IEnumerable<Guid>>) (license => (IEnumerable<Guid>) license.Features)).Any<Guid>((Func<Guid, bool>) (licensedFeature => licensedFeature == featureId));
    }

    public virtual FeatureMode GetFeatureMode(Guid featureId)
    {
      if (this.IsFeatureLicensed(featureId))
        return FeatureMode.Licensed;
      if (this.IsFeatureAvailable(featureId))
        return FeatureMode.Trial;
      return this.IsFeatureInAdvertisingMode(featureId) ? FeatureMode.Advertising : FeatureMode.Off;
    }

    public bool IsFeatureInAdvertisingMode(Guid featureId) => FeatureContext.PreCheck(this.m_requestContext, new Guid?(featureId), false, (Func<bool>) (() =>
    {
      bool flag = false;
      if (!this.m_featureInAdvertisingCache.TryGetValue(featureId, out flag))
      {
        flag = this.m_licenseService.IsFeatureInAdvertisingMode(this.m_requestContext, featureId);
        this.m_featureInAdvertisingCache[featureId] = flag;
      }
      return flag;
    }));

    private static bool PreCheck(IVssRequestContext requestContext, Func<bool> guardedCode) => FeatureContext.PreCheck(requestContext, new Guid?(), true, guardedCode);

    private static bool PreCheck(
      IVssRequestContext requestContext,
      Guid? featureId,
      bool returnValueIfPrecheckSucceeds,
      Func<bool> guardedCode)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled("VisualStudio.PremiumFeatures.Licensing") || featureId.HasValue && (object.Equals((object) featureId.Value, (object) LicenseFeatures.NoneRequiredId) || object.Equals((object) featureId.Value, (object) LicenseFeatures.ViewMyWorkItemsId)) ? returnValueIfPrecheckSucceeds : guardedCode();
    }
  }
}
