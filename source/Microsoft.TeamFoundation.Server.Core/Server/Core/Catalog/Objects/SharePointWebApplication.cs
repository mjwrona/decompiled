// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharePointWebApplication : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SharePointWebApplication;
    private ICollection<ProjectPortal> m_dependentProjectPortals;

    public string DefaultRelativePath
    {
      get => this.GetProperty<string>(nameof (DefaultRelativePath));
      set => this.SetProperty<string>(nameof (DefaultRelativePath), value);
    }

    public Uri SiteCreationUrl
    {
      get => this.GetProperty<Uri>(nameof (SiteCreationUrl));
      set => this.SetProperty<Uri>(nameof (SiteCreationUrl), value);
    }

    public ServiceDefinition RootUrlService
    {
      get => this.GetServiceReference("RootUrl");
      set => this.SetServiceRefence("RootUrl", value);
    }

    public Uri RootUrlServiceLocation => this.LocationAsUrl(this.RootUrlService);

    public ServiceDefinition AdminUrlService
    {
      get => this.GetServiceReference("AdminUrl");
      set => this.SetServiceRefence("AdminUrl", value);
    }

    public Uri AdminUrlServiceLocation => this.LocationAsUrl(this.AdminUrlService);

    protected override void OnRefresh()
    {
      base.OnRefresh();
      this.m_dependentProjectPortals = (ICollection<ProjectPortal>) null;
    }

    public string RootUrlServiceLocationString
    {
      get
      {
        Uri urlServiceLocation = this.RootUrlServiceLocation;
        return !(urlServiceLocation == (Uri) null) ? urlServiceLocation.ToString() : string.Empty;
      }
    }

    internal void ResetSiteCreationUrl() => this.SiteCreationUrl = (Uri) null;

    public static SharePointWebApplication Register(OrganizationalRoot root, string displayName)
    {
      ArgumentUtility.CheckForNull<OrganizationalRoot>(root, nameof (root));
      ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
      return root.SharePointWebApplications.FirstOrDefault<SharePointWebApplication>((Func<SharePointWebApplication, bool>) (w => VssStringComparer.Url.Equals(w.DisplayName, displayName))) ?? root.CreateChild<SharePointWebApplication>(displayName);
    }

    public ICollection<ProjectPortal> DependentProjectPortals => this.GetDependentsCollection<ProjectPortal>(ref this.m_dependentProjectPortals);

    public ICollection<CatalogObject> AllDependents => this.QueryDependents<CatalogObject>(true, true);

    public Uri FullyQualifiedUrl => this.RootUrlServiceLocation;

    public string FullyQualifiedUncPath => UriUtility.GetDavUncFromHttpPath(this.FullyQualifiedUrl.AbsoluteUri);

    public void ResetServiceDefinitions(
      IVssRequestContext requestContext,
      string adminUrl,
      string rootUrl)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(adminUrl, nameof (adminUrl));
      ArgumentUtility.CheckStringForNullOrEmpty(rootUrl, nameof (rootUrl));
      ServiceDefinition adminUrlService = this.AdminUrlService;
      this.UpdateOrCreateSharePointService(ref adminUrlService, "WssAdminUrl", adminUrl, requestContext);
      this.AdminUrlService = adminUrlService;
      ServiceDefinition rootUrlService = this.RootUrlService;
      this.UpdateOrCreateSharePointService(ref rootUrlService, "WssRootUrl", rootUrl, requestContext);
      this.RootUrlService = rootUrlService;
    }

    private void UpdateOrCreateSharePointService(
      ref ServiceDefinition serviceDef,
      string serviceType,
      string location,
      IVssRequestContext requestContext)
    {
      if (serviceDef == null)
      {
        string displayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) FrameworkResources.SharePointWebApplication(), (object) serviceType);
        serviceDef = new ServiceDefinition(serviceType, Guid.NewGuid(), displayName, (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, string.Empty, "Wss");
      }
      else
        serviceDef.LocationMappings.Clear();
      serviceDef.AddLocationMapping(this.Context.PublicAccessMapping, location);
    }

    public ICollection<TfsRelativeWebSite> FindSharePointSites(Uri siteUrl)
    {
      ICollection<TfsRelativeWebSite> sharePointSites = (ICollection<TfsRelativeWebSite>) new HashSet<TfsRelativeWebSite>();
      foreach (CatalogObject queryDependent in (IEnumerable<CatalogObject>) this.QueryDependents<CatalogObject>(true, true))
      {
        TfsRelativeWebSite tfsRelativeWebSite1 = (TfsRelativeWebSite) queryDependent.As<ProjectPortal>();
        if (tfsRelativeWebSite1 == null)
        {
          ProcessGuidanceSite processGuidanceSite = queryDependent.As<ProcessGuidanceSite>();
          tfsRelativeWebSite1 = processGuidanceSite != null ? (TfsRelativeWebSite) processGuidanceSite : (TfsRelativeWebSite) queryDependent.As<SharePointSiteCreationLocation>();
        }
        TfsRelativeWebSite tfsRelativeWebSite2 = tfsRelativeWebSite1;
        if (tfsRelativeWebSite2 != null)
        {
          Uri fullUrl = tfsRelativeWebSite2.FullUrl;
          if (VssStringComparer.ServerUrl.Equals((object) siteUrl, (object) fullUrl))
            sharePointSites.Add(tfsRelativeWebSite2);
        }
      }
      return sharePointSites;
    }

    public static class Fields
    {
      public const string DefaultRelativePath = "DefaultRelativePath";
      public const string SiteCreationUrl = "SiteCreationUrl";
      public const string RootUrl = "RootUrl";
      public const string AdminUrl = "AdminUrl";
    }
  }
}
