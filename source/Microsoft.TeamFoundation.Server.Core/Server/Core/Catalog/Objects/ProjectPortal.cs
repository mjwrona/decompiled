// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ProjectPortal
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
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

    public TeamProject TeamProject
    {
      get
      {
        if (this.m_teamProject == null)
          this.m_teamProject = this.QueryParent<TeamProject>(false);
        return this.m_teamProject;
      }
    }

    public override Uri FullUrl
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

    public bool IsOwnerOfWssSite => this.OwnedWebIdentifier != Guid.Empty;

    public new static class Fields
    {
      public const string FullyQualifiedUrl = "FullyQualifiedUrl";
      public const string OwnedWebIdentifier = "OwnedWebIdentifier";
      public const string ResourceSubType = "ResourceSubType";
    }
  }
}
