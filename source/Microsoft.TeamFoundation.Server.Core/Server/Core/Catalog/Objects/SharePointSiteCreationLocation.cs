// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointSiteCreationLocation
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SharePointSiteCreationLocation : TfsRelativeWebSite
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SharePointSiteCreationLocation;

    public SharePointWebApplication WebApplication
    {
      get => this.ReferencedResource == null ? (SharePointWebApplication) null : this.ReferencedResource.As<SharePointWebApplication>();
      set => this.ReferencedResource = (CatalogObject) value;
    }

    public override Uri FullUrl => this.GetFullyQualifiedUri();

    public string FullyQualifiedUncPath => UriUtility.GetDavUncFromHttpPath(this.FullUrl.AbsoluteUri);
  }
}
