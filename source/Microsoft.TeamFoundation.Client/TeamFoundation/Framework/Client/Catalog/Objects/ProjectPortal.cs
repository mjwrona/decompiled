// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ProjectPortal
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProjectPortal : TfsRelativeWebSite
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ProjectPortal;
    private TeamProject m_teamProject;

    public Uri FullyQualifiedUrl
    {
      get => this.GetProperty<Uri>(nameof (FullyQualifiedUrl));
      set => this.SetProperty<Uri>(nameof (FullyQualifiedUrl), value);
    }

    public Guid OwnedWebIdentifier
    {
      get => this.GetProperty<Guid>(nameof (OwnedWebIdentifier));
      set => this.SetProperty<Guid>(nameof (OwnedWebIdentifier), value);
    }

    public ProjectPortalType ResourceSubType
    {
      get => this.GetProperty<ProjectPortalType>(nameof (ResourceSubType));
      set => this.SetProperty<ProjectPortalType>(nameof (ResourceSubType), value);
    }

    protected override void OnRefresh()
    {
      base.OnRefresh();
      this.m_teamProject = (TeamProject) null;
    }

    protected internal override void Initialize()
    {
      if (!string.IsNullOrEmpty(this.CatalogNode.FullPath))
        return;
      this.ResourceSubType = ProjectPortalType.WssSite;
      this.OwnedWebIdentifier = Guid.Empty;
      this.FullyQualifiedUrl = (Uri) null;
    }

    public TeamProject TeamProject
    {
      get
      {
        if (this.m_teamProject == null)
          this.m_teamProject = this.QueryParent<TeamProject>(false);
        return this.m_teamProject;
      }
    }

    public Uri AdminUrl
    {
      get
      {
        if (this.ResourceSubType == ProjectPortalType.WssSite && this.ReferencedResource != null)
        {
          SharePointWebApplication pointWebApplication = this.ReferencedResource.As<SharePointWebApplication>();
          if (pointWebApplication != null)
            return pointWebApplication.AdminUrlServiceLocation;
        }
        return (Uri) null;
      }
    }

    public Uri FullUrl
    {
      get
      {
        switch (this.ResourceSubType)
        {
          case ProjectPortalType.WssSite:
            return this.GetFullyQualifiedUri();
          case ProjectPortalType.WebSite:
            return this.FullyQualifiedUrl;
          default:
            return (Uri) null;
        }
      }
    }

    public string FullyQualifiedUncPath => this.ResourceSubType == ProjectPortalType.WssSite ? UriUtility.GetDavUncFromHttpPath(this.FullyQualifiedUrl.AbsoluteUri) : (string) null;

    public string ToOwnershipString() => TFCommonResources.EntityModel_PortalOwnerInfo((object) this.TeamProject.ProjectName, (object) this.TeamProject.Collection.DisplayName);

    public static string FormatOwnershipString(ICollection<ProjectPortal> list) => string.Join(";", list.Select<ProjectPortal, string>((Func<ProjectPortal, string>) (p => p.ToOwnershipString())).ToArray<string>());

    public bool IsOwnerOfWssSite => this.OwnedWebIdentifier != Guid.Empty;

    public bool HasAdminUrl => this.ResourceSubType == ProjectPortalType.WssSite;

    public void ResetValues()
    {
      this.ResourceSubType = ProjectPortalType.WssSite;
      this.ReferencedResource = (CatalogObject) null;
      this.RelativePath = (string) null;
      this.FullyQualifiedUrl = (Uri) null;
      this.OwnedWebIdentifier = Guid.Empty;
    }

    public new static class Fields
    {
      public const string FullyQualifiedUrl = "FullyQualifiedUrl";
      public const string OwnedWebIdentifier = "OwnedWebIdentifier";
      public const string ResourceSubType = "ResourceSubType";
    }
  }
}
