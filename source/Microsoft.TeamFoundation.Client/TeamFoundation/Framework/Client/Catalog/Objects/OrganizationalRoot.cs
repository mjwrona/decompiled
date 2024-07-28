// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.OrganizationalRoot
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
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

    public TeamProject FindProjectByUri(Uri projectUri) => this.QueryChild<TeamProject>("ProjectUri", (object) projectUri, true);

    public ICollection<ProjectPortal> FindProjectPortals(Guid ownedWebIdentifier) => this.QueryChildren<ProjectPortal>("OwnedWebIdentifier", (object) ownedWebIdentifier, true);
  }
}
