// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.OrganizationalRoot
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class OrganizationalRoot : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.OrganizationalRoot;
    private static readonly Type[] KnownChildTypes = new Type[3]
    {
      typeof (ReportingConfiguration),
      typeof (ApplicationInstance),
      typeof (SharePointWebApplication)
    };
    private ICollection<ApplicationInstance> m_ApplicationInstances;
    private ICollection<SharePointWebApplication> m_SharePointWebApplications;
    private object m_ReportingConfiguration;

    protected override void Reset()
    {
      base.Reset();
      this.m_ReportingConfiguration = (object) null;
      this.m_ApplicationInstances = (ICollection<ApplicationInstance>) null;
      this.m_SharePointWebApplications = (ICollection<SharePointWebApplication>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_ApplicationInstances = this.PreloadChildren<ApplicationInstance>(bulkData);
      this.m_SharePointWebApplications = this.PreloadChildren<SharePointWebApplication>(bulkData);
      this.PreloadChild<ReportingConfiguration>(bulkData, ref this.m_ReportingConfiguration);
    }

    public ReportingConfiguration ReportingConfiguration => this.GetChild<ReportingConfiguration>(ref this.m_ReportingConfiguration);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<ApplicationInstance> ApplicationInstances => this.GetChildCollection<ApplicationInstance>(ref this.m_ApplicationInstances);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<SharePointWebApplication> SharePointWebApplications => this.GetChildCollection<SharePointWebApplication>(ref this.m_SharePointWebApplications);

    public ApplicationInstance ApplicationInstance => this.ApplicationInstances.FirstOrDefault<ApplicationInstance>();

    public ProjectCollection FindProjectCollectionById(Guid instanceId) => this.QueryChild<ProjectCollection>("InstanceId", (object) instanceId, true);

    public TeamProject FindProjectById(Guid projectId) => this.QueryChild<TeamProject>("ProjectId", (object) projectId, true);

    public ICollection<ProjectPortal> FindProjectPortals(Guid ownedWebIdentifier) => this.QueryChildren<ProjectPortal>("OwnedWebIdentifier", (object) ownedWebIdentifier, true);

    public SharePointWebApplication FindSharePointWebApplication(Uri rootUri) => this.SharePointWebApplications.FirstOrDefault<SharePointWebApplication>((Func<SharePointWebApplication, bool>) (webApp => UriUtility.Equals(rootUri, webApp.RootUrlServiceLocation)));

    public SharePointWebApplication FindSharePointWebApplication(
      Uri rootUri,
      Uri adminUri,
      string defaultRelativePath)
    {
      return this.SharePointWebApplications.FirstOrDefault<SharePointWebApplication>((Func<SharePointWebApplication, bool>) (webApp => UriUtility.Equals(rootUri, webApp.RootUrlServiceLocation) && UriUtility.Equals(adminUri, webApp.AdminUrlServiceLocation) && UriUtility.UrlPathIgnoreSeparatorsComparer.Equals(defaultRelativePath, webApp.DefaultRelativePath)));
    }
  }
}
