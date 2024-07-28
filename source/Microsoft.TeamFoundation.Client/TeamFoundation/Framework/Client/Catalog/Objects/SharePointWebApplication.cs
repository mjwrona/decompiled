// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.SharePointWebApplication
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
  public class SharePointWebApplication : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SharePointWebApplication;

    public string DefaultRelativePath
    {
      get => this.GetProperty<string>(nameof (DefaultRelativePath));
      set => this.SetProperty<string>(nameof (DefaultRelativePath), value);
    }

    public Uri SiteCreationUrl
    {
      get => this.GetProperty<Uri>(nameof (SiteCreationUrl));
      set => this.SetProperty<Uri>(nameof (SiteCreationUrl), value);
    }

    public ServiceDefinition RootUrlService
    {
      get => this.GetServiceReference("RootUrl");
      set => this.SetServiceRefence("RootUrl", value);
    }

    public Uri RootUrlServiceLocation => this.LocationAsUrl(this.RootUrlService);

    public ServiceDefinition AdminUrlService
    {
      get => this.GetServiceReference("AdminUrl");
      set => this.SetServiceRefence("AdminUrl", value);
    }

    public Uri AdminUrlServiceLocation => this.LocationAsUrl(this.AdminUrlService);

    public Uri FullyQualifiedUrl => this.RootUrlServiceLocation;

    public string FullyQualifiedUncPath => UriUtility.GetDavUncFromHttpPath(this.FullyQualifiedUrl.AbsoluteUri);

    public override string ToString() => this.CatalogNode.Resource.DisplayName;

    public static class Fields
    {
      public const string DefaultRelativePath = "DefaultRelativePath";
      public const string SiteCreationUrl = "SiteCreationUrl";
      public const string RootUrl = "RootUrl";
      public const string AdminUrl = "AdminUrl";
    }
  }
}
