// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationLicensingServiceBase
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class TeamFoundationLicensingServiceBase : 
    ITeamFoundationLicensingService,
    IVssFrameworkService
  {
    internal LicensePackageService m_licensePackageService;
    protected Dictionary<Guid, ILicenseFeature> m_featureToLicenseFeature;
    protected Dictionary<Guid, ILicenseType> m_licenseToLicenseType;
    protected ILicenseType m_defaultLicense;
    protected HashSet<Guid> m_defaultFeatures;
    protected HashSet<Guid> m_previewFeatures;
    protected bool m_settingsLoaded;
    protected object m_settingsLock = new object();
    protected static readonly string s_area = "TeamFoundationLicensing";
    protected static readonly string s_layer = nameof (TeamFoundationLicensingServiceBase);

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_licensePackageService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<LicensePackageService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_licensePackageService.LicensePackageChanged += new EventHandler(this.OnLicensePackageChanged);
      this.EnsureSettingsLoaded(systemRequestContext);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_licensePackageService.LicensePackageChanged -= new EventHandler(this.OnLicensePackageChanged);

    public ILicenseType GetLicenseType(
      IVssRequestContext requestContext,
      Guid licenseTypeId,
      out ILicenseFeature[] licenseFeatures)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      this.CheckReadPermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      ILicenseType licenseType;
      if (this.m_licenseToLicenseType.TryGetValue(licenseTypeId, out licenseType))
      {
        licenseFeatures = new ILicenseFeature[licenseType.Features.Length];
        for (int index = 0; index < licenseType.Features.Length; ++index)
          licenseFeatures[index] = this.LocalizeLicenseFeature(this.m_featureToLicenseFeature[licenseType.Features[index]]);
        return this.LocalizeLicenseType(licenseType);
      }
      requestContext.Trace(66005, TraceLevel.Warning, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "No license type found for licenseType {0}", (object) licenseTypeId);
      throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
    }

    public ILicenseFeature GetLicenseFeature(
      IVssRequestContext requestContext,
      Guid licenseFeatureId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseFeatureId, nameof (licenseFeatureId));
      this.CheckReadPermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      ILicenseFeature licenseFeature;
      if (this.m_featureToLicenseFeature.TryGetValue(licenseFeatureId, out licenseFeature))
        return this.LocalizeLicenseFeature(licenseFeature);
      requestContext.Trace(66005, TraceLevel.Warning, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "No license feature found for licenseFeature {0}", (object) licenseFeatureId);
      throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownFeature((object) licenseFeatureId));
    }

    public Guid[] GetLicenseFeaturesInPreview(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.EnsureSettingsLoaded(requestContext);
      return this.m_previewFeatures.ToArray<Guid>();
    }

    public ILicenseType[] GetAllLicenses(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.CheckReadPermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      return this.m_licenseToLicenseType.Values.Select<ILicenseType, ILicenseType>((Func<ILicenseType, ILicenseType>) (x => this.LocalizeLicenseType(x))).ToArray<ILicenseType>();
    }

    public abstract ILicenseType[] GetLicensesForUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor);

    public abstract bool IsFeatureSupported(
      IVssRequestContext requestContext,
      Guid featureId,
      IdentityDescriptor userContext);

    public virtual bool IsFeatureSupported(IVssRequestContext requestContext, Guid featureId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(featureId, nameof (featureId));
      if (requestContext.IsSystemContext)
        return true;
      return !(requestContext.UserContext == (IdentityDescriptor) null) && this.IsFeatureSupported(requestContext, featureId, requestContext.UserContext);
    }

    public abstract bool IsFeatureInAdvertisingMode(
      IVssRequestContext requestContext,
      Guid featureId);

    protected ILicenseType LocalizeLicenseType(ILicenseType licenseType)
    {
      bool replacedAll;
      return (ILicenseType) new LicenseType()
      {
        Name = StringUtil.ReplaceResources(licenseType.Name, out replacedAll),
        Description = StringUtil.ReplaceResources(licenseType.Description, out replacedAll),
        Id = licenseType.Id,
        IsDefault = licenseType.IsDefault,
        Features = licenseType.Features
      };
    }

    protected ILicenseFeature LocalizeLicenseFeature(ILicenseFeature licenseFeature)
    {
      bool replacedAll;
      return (ILicenseFeature) new LicenseFeature()
      {
        Name = StringUtil.ReplaceResources(licenseFeature.Name, out replacedAll),
        Description = StringUtil.ReplaceResources(licenseFeature.Description, out replacedAll),
        Id = licenseFeature.Id,
        InPreviewMode = licenseFeature.InPreviewMode
      };
    }

    protected void CheckReadPermission(IVssRequestContext requestContext) => this.CheckPermission(requestContext, 1);

    protected void CheckWritePermission(IVssRequestContext requestContext) => this.CheckPermission(requestContext, 2);

    private void CheckPermission(IVssRequestContext requestContext, int requestedPermission) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, requestedPermission, false);

    protected void OnLicensePackageChanged(object sender, EventArgs e)
    {
      lock (this.m_settingsLock)
        this.m_settingsLoaded = false;
    }

    protected virtual void EnsureSettingsLoaded(IVssRequestContext requestContext)
    {
      if (requestContext.IsTracing(66031, TraceLevel.Info, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer))
      {
        requestContext.Trace(66031, TraceLevel.Info, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "Feature to license map: {0}", this.m_featureToLicenseFeature == null ? (object) "null" : (object) this.m_featureToLicenseFeature.Serialize<Dictionary<Guid, ILicenseFeature>>());
        requestContext.Trace(66031, TraceLevel.Info, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "License to license type map: {0}", this.m_licenseToLicenseType == null ? (object) "null" : (object) this.m_licenseToLicenseType.Serialize<Dictionary<Guid, ILicenseType>>());
      }
      if (this.m_settingsLoaded)
        return;
      lock (this.m_settingsLock)
      {
        if (this.m_settingsLoaded)
          return;
        requestContext = requestContext.Elevate();
        Dictionary<Guid, ILicenseFeature> dictionary1 = new Dictionary<Guid, ILicenseFeature>();
        Dictionary<Guid, ILicenseType> dictionary2 = new Dictionary<Guid, ILicenseType>();
        HashSet<Guid> guidSet1 = new HashSet<Guid>();
        HashSet<Guid> guidSet2 = new HashSet<Guid>();
        ILicenseType licenseType1 = (ILicenseType) null;
        try
        {
          ILicensePackage licensePackage = this.m_licensePackageService.GetLicensePackage(requestContext.To(TeamFoundationHostType.Deployment));
          if (licensePackage == null)
          {
            requestContext.Trace(66008, TraceLevel.Warning, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "No license package found");
          }
          else
          {
            requestContext.Trace(66039, TraceLevel.Info, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, "Retrieved license package: {0}", licensePackage == null ? (object) "null" : (object) licensePackage.Serialize<ILicensePackage>());
            dictionary1 = new Dictionary<Guid, ILicenseFeature>();
            foreach (ILicenseFeature feature in licensePackage.Features)
            {
              dictionary1[feature.Id] = feature;
              if (feature.InPreviewMode)
                guidSet2.Add(feature.Id);
            }
            dictionary2 = new Dictionary<Guid, ILicenseType>();
            foreach (ILicenseType licenseType2 in licensePackage.LicenseTypes)
            {
              dictionary2[licenseType2.Id] = licenseType2;
              foreach (Guid feature in licenseType2.Features)
              {
                if (licenseType2.IsDefault)
                  guidSet1.Add(feature);
              }
              if (licenseType2.IsDefault)
                licenseType1 = licenseType2;
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(66040, TeamFoundationLicensingServiceBase.s_area, TeamFoundationLicensingServiceBase.s_layer, ex);
        }
        finally
        {
          this.m_featureToLicenseFeature = dictionary1;
          this.m_licenseToLicenseType = dictionary2;
          this.m_defaultFeatures = guidSet1;
          this.m_previewFeatures = guidSet2;
          this.m_defaultLicense = licenseType1;
          this.m_settingsLoaded = true;
        }
      }
    }
  }
}
