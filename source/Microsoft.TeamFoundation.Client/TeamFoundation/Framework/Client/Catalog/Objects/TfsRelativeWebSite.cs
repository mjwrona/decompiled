// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.TfsRelativeWebSite
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsRelativeWebSite : CatalogObject
  {
    public string RelativePath
    {
      get => this.GetProperty<string>(nameof (RelativePath));
      set => this.SetProperty<string>(nameof (RelativePath), value);
    }

    public CatalogObject ReferencedResource
    {
      get => this.GetDependency<CatalogObject>(nameof (ReferencedResource));
      set => this.SetDependency<CatalogObject>(nameof (ReferencedResource), value);
    }

    protected Uri GetFullyQualifiedUri()
    {
      Uri baseUri = (Uri) null;
      if (this.ReferencedResource != null)
        baseUri = this.ReferencedResource.As<SharePointWebApplication>().RootUrlServiceLocation;
      if (baseUri == (Uri) null)
        return (Uri) null;
      if (!string.IsNullOrEmpty(this.RelativePath))
        baseUri = UriUtility.Combine(baseUri, this.RelativePath, true);
      return new Uri(baseUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    public static class Fields
    {
      public const string RelativePath = "RelativePath";
      public const string ReferencedResource = "ReferencedResource";
    }
  }
}
