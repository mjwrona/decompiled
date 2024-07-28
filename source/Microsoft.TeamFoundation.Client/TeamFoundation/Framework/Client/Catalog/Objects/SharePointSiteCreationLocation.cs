// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.SharePointSiteCreationLocation
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharePointSiteCreationLocation : TfsRelativeWebSite
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SharePointSiteCreationLocation;
    private Uri m_fullyQualifiedUrl;

    protected override void OnRefresh()
    {
      base.OnRefresh();
      this.m_fullyQualifiedUrl = (Uri) null;
    }

    public Uri FullyQualifiedUrl
    {
      get
      {
        if (this.m_fullyQualifiedUrl == (Uri) null)
          this.m_fullyQualifiedUrl = this.GetFullyQualifiedUri();
        return this.m_fullyQualifiedUrl;
      }
    }

    public Uri AdminUrl
    {
      get
      {
        if (this.ReferencedResource != null)
        {
          SharePointWebApplication pointWebApplication = this.ReferencedResource.As<SharePointWebApplication>();
          if (pointWebApplication != null)
            return pointWebApplication.AdminUrlServiceLocation;
        }
        return (Uri) null;
      }
    }

    public string FullyQualifiedUncPath => UriUtility.GetDavUncFromHttpPath(this.FullyQualifiedUrl.AbsoluteUri);
  }
}
