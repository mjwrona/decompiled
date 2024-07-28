// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ProjectCollection
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProjectCollection : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ProjectCollection;
    private static readonly Type[] KnownChildTypes = new Type[3]
    {
      typeof (SharePointSiteCreationLocation),
      typeof (ReportingFolder),
      typeof (TeamProject)
    };
    private ICollection<TeamProject> m_Projects;
    private object m_LocationForNewWssSites;
    private object m_ReportFolder;

    protected override void Reset()
    {
      base.Reset();
      this.m_LocationForNewWssSites = (object) null;
      this.m_ReportFolder = (object) null;
      this.m_Projects = (ICollection<TeamProject>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_Projects = this.PreloadChildren<TeamProject>(bulkData);
      this.PreloadChild<SharePointSiteCreationLocation>(bulkData, ref this.m_LocationForNewWssSites);
      this.PreloadChild<ReportingFolder>(bulkData, ref this.m_ReportFolder);
    }

    public Guid InstanceId
    {
      get => this.GetProperty<Guid>(nameof (InstanceId));
      set => this.SetProperty<Guid>(nameof (InstanceId), value);
    }

    public SharePointSiteCreationLocation LocationForNewWssSites => this.GetChild<SharePointSiteCreationLocation>(ref this.m_LocationForNewWssSites);

    public ReportingFolder ReportFolder => this.GetChild<ReportingFolder>(ref this.m_ReportFolder);

    public ApplicationInstance ApplicationInstance => this.GetParent<ApplicationInstance>();

    public ServiceDefinition LocationService
    {
      get => this.GetServiceReference("Location");
      set => this.SetServiceRefence("Location", value);
    }

    public Uri LocationServiceLocation => this.LocationAsUrl(this.LocationService);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<TeamProject> Projects => this.GetChildCollection<TeamProject>(ref this.m_Projects);

    public TeamProject FindProjectById(Guid projectId) => this.QueryChild<TeamProject>("ProjectId", (object) projectId, false);

    public TeamProject FindProjectByName(string projectName) => this.QueryChild<TeamProject>("ProjectName", (object) projectName, false);

    public TeamProject FindProjectByUri(Uri projectUri) => this.QueryChild<TeamProject>("ProjectUri", (object) projectUri, false);

    public ICollection<SharePointWebApplication> GetReferencedSharePointWebApplications()
    {
      ICollection<SharePointWebApplication> collection = (ICollection<SharePointWebApplication>) new HashSet<SharePointWebApplication>((IEqualityComparer<SharePointWebApplication>) new CatalogObjectEqualityComparer());
      ProjectCollection.AppendSharePointWebApplication((TfsRelativeWebSite) this.LocationForNewWssSites, collection);
      foreach (TeamProject project in (IEnumerable<TeamProject>) this.Projects)
      {
        ProjectCollection.AppendSharePointWebApplication((TfsRelativeWebSite) project.Portal, collection);
        ProjectCollection.AppendSharePointWebApplication((TfsRelativeWebSite) project.Guidance, collection);
      }
      return collection;
    }

    private static void AppendSharePointWebApplication(
      TfsRelativeWebSite item,
      ICollection<SharePointWebApplication> collection)
    {
      if (item == null || item.ReferencedResource == null)
        return;
      SharePointWebApplication pointWebApplication = item.ReferencedResource.As<SharePointWebApplication>();
      if (pointWebApplication == null)
        return;
      collection.Add(pointWebApplication);
    }

    public SharePointSiteCreationLocation AddSiteCreationLocation()
    {
      if (this.m_LocationForNewWssSites == CatalogObject.NullObject || this.m_LocationForNewWssSites == null)
        this.m_LocationForNewWssSites = (object) this.CreateChild<SharePointSiteCreationLocation>();
      return this.m_LocationForNewWssSites as SharePointSiteCreationLocation;
    }

    public ReportingFolder AddReportingFolder(ReportingServer reportingServer, string itemPath)
    {
      if (this.m_ReportFolder == CatalogObject.NullObject || this.m_ReportFolder == null)
        this.m_ReportFolder = (object) this.CreateChild<ReportingFolder>();
      ReportingFolder reportFolder = this.m_ReportFolder as ReportingFolder;
      reportFolder.ItemPath = itemPath;
      reportFolder.ReferencedResource = (CatalogObject) reportingServer;
      return reportFolder;
    }

    public static class Fields
    {
      public const string InstanceId = "InstanceId";
      public const string Location = "Location";
    }
  }
}
