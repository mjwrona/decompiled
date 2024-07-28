// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ApplicationInstance
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ApplicationInstance : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.TeamFoundationServerInstance;
    private static readonly Type[] KnownChildTypes = new Type[2]
    {
      typeof (TeamSystemWebAccess),
      typeof (ProjectCollection)
    };
    private ICollection<ProjectCollection> m_ProjectCollections;
    private object m_WebAccessInstance;

    public static ApplicationInstance Register(
      IVssRequestContext requestContext,
      OrganizationalRoot root,
      string displayName)
    {
      ArgumentUtility.CheckForNull<OrganizationalRoot>(root, nameof (root));
      ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
      ApplicationInstance applicationInstance = root.ApplicationInstances.FirstOrDefault<ApplicationInstance>();
      if (applicationInstance == null)
      {
        applicationInstance = root.CreateChild<ApplicationInstance>(displayName);
        applicationInstance.LocationService = root.Context.GetLocationService(requestContext).FindServiceDefinition(requestContext, "LocationService", Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier);
      }
      return applicationInstance;
    }

    protected override void Reset()
    {
      base.Reset();
      this.m_WebAccessInstance = (object) null;
      this.m_ProjectCollections = (ICollection<ProjectCollection>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_ProjectCollections = this.PreloadChildren<ProjectCollection>(bulkData);
      this.PreloadChild<TeamSystemWebAccess>(bulkData, ref this.m_WebAccessInstance);
    }

    public TeamSystemWebAccess WebAccessInstance => this.GetChild<TeamSystemWebAccess>(ref this.m_WebAccessInstance);

    public ServiceDefinition LocationService
    {
      get => this.GetServiceReference("Location");
      set => this.SetServiceRefence("Location", value);
    }

    public Uri LocationServiceLocation => this.LocationAsUrl(this.LocationService);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<ProjectCollection> ProjectCollections => this.GetChildCollection<ProjectCollection>(ref this.m_ProjectCollections);

    public ICollection<TeamWebApplication> WebApplications => this.GetDependencyCollection<TeamWebApplication>("WebApplication");

    public static class Fields
    {
      public const string WebApplications = "WebApplication";
      public const string Location = "Location";
    }
  }
}
