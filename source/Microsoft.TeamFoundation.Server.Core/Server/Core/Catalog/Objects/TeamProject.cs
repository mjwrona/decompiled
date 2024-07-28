// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.TeamProject
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamProject : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.TeamProject;
    private static readonly Type[] KnownChildTypes = new Type[3]
    {
      typeof (ProcessGuidanceSite),
      typeof (ReportingFolder),
      typeof (ProjectPortal)
    };
    private object m_Guidance;
    private object m_ReportFolder;
    private object m_Portal;
    private ProjectCollection m_projectCollection;
    private string m_displayState = string.Empty;

    protected override void Reset()
    {
      base.Reset();
      this.m_Guidance = (object) null;
      this.m_ReportFolder = (object) null;
      this.m_Portal = (object) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.PreloadChild<ProcessGuidanceSite>(bulkData, ref this.m_Guidance);
      this.PreloadChild<ReportingFolder>(bulkData, ref this.m_ReportFolder);
      this.PreloadChild<ProjectPortal>(bulkData, ref this.m_Portal);
    }

    public string ProjectName
    {
      get => this.GetProperty<string>(nameof (ProjectName));
      set => this.SetProperty<string>(nameof (ProjectName), value);
    }

    public Uri ProjectUri
    {
      get => this.GetProperty<Uri>(nameof (ProjectUri));
      set => this.SetProperty<Uri>(nameof (ProjectUri), value);
    }

    public Guid ProjectId
    {
      get => this.GetProperty<Guid>(nameof (ProjectId));
      set => this.SetProperty<Guid>(nameof (ProjectId), value);
    }

    public ProjectState ProjectState
    {
      get => this.GetProperty<ProjectState>(nameof (ProjectState));
      set => this.SetProperty<ProjectState>(nameof (ProjectState), value);
    }

    public ProcessGuidanceSite Guidance => this.GetChild<ProcessGuidanceSite>(ref this.m_Guidance);

    public ReportingFolder ReportFolder => this.GetChild<ReportingFolder>(ref this.m_ReportFolder);

    public ProjectPortal Portal => this.GetChild<ProjectPortal>(ref this.m_Portal);

    public bool PortalIsSharePoint => this.Portal != null && this.Portal.ResourceSubType == ProjectPortalType.WssSite;

    public bool IsOwnerOfSharePointPortal => this.PortalIsSharePoint && this.Portal.OwnedWebIdentifier != Guid.Empty;

    public ProjectPortal AddProjectPortal()
    {
      if (this.m_Portal == CatalogObject.NullObject || this.m_Portal == null)
        this.m_Portal = (object) this.CreateChild<ProjectPortal>();
      return this.m_Portal as ProjectPortal;
    }

    public ProcessGuidanceSite AddProcessGuidanceSite()
    {
      if (this.m_Guidance == CatalogObject.NullObject || this.m_Guidance == null)
        this.m_Guidance = (object) this.CreateChild<ProcessGuidanceSite>();
      return this.m_Guidance as ProcessGuidanceSite;
    }

    public ReportingFolder AddReportingFolder(ReportingFolder referencedParent, string itemPath)
    {
      if (this.m_ReportFolder == CatalogObject.NullObject || this.m_ReportFolder == null)
        this.m_ReportFolder = (object) this.CreateChild<ReportingFolder>();
      ReportingFolder reportFolder = this.m_ReportFolder as ReportingFolder;
      reportFolder.ItemPath = itemPath;
      reportFolder.ReferencedResource = (CatalogObject) referencedParent;
      return reportFolder;
    }

    public string DisplayState
    {
      get => this.m_displayState;
      set
      {
        this.m_displayState = value;
        this.NotifyPropertyChanged(nameof (DisplayState));
      }
    }

    public ProjectCollection ProjectCollection
    {
      get
      {
        if (this.m_projectCollection == null)
          this.m_projectCollection = this.QueryParent<ProjectCollection>(false);
        return this.m_projectCollection;
      }
    }

    public static class Fields
    {
      public const string ProjectName = "ProjectName";
      public const string ProjectUri = "ProjectUri";
      public const string ProjectId = "ProjectId";
      public const string ProjectState = "ProjectState";
    }
  }
}
