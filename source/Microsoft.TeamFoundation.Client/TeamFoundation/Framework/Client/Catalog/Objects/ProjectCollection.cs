// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ProjectCollection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
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

    public TfsTeamProjectCollection TfsServer => TfsTeamProjectCollectionFactory.GetTeamProjectCollection(this.LocationServiceLocation);

    public static class Fields
    {
      public const string InstanceId = "InstanceId";
      public const string Location = "Location";
    }
  }
}
