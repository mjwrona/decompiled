// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData.V2FeedPackageModelInfo
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.OData.Query;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using System;
using System.Threading;
using System.Web.Http.OData.Builder;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData
{
  public class V2FeedPackageModelInfo
  {
    private static Lazy<V2FeedPackageModelInfo> instanceLazy = new Lazy<V2FeedPackageModelInfo>((Func<V2FeedPackageModelInfo>) (() => new V2FeedPackageModelInfo(V2FeedPackageModelInfo.GetEdmModel())), LazyThreadSafetyMode.PublicationOnly);

    public static V2FeedPackageModelInfo Instance => V2FeedPackageModelInfo.instanceLazy.Value;

    private static IEdmModel GetEdmModel()
    {
      ODataConventionModelBuilder conventionModelBuilder = new ODataConventionModelBuilder();
      conventionModelBuilder.EntitySet<V2FeedPackage>("Packages");
      return conventionModelBuilder.GetEdmModel();
    }

    private V2FeedPackageModelInfo(IEdmModel edmModel)
    {
      this.EdmModel = edmModel;
      this.V2FeedPackageType = (IEdmEntityType) edmModel.FindType(typeof (V2FeedPackage).FullName);
      this.IdProperty = this.V2FeedPackageType.FindProperty("Id");
      this.VersionProperty = this.V2FeedPackageType.FindProperty("Version");
      this.NormalizedVersionProperty = this.V2FeedPackageType.FindProperty("NormalizedVersion");
      this.IsLatestVersionProperty = this.V2FeedPackageType.FindProperty("IsLatestVersion");
      this.IsAbsoluteLatestVersionProperty = this.V2FeedPackageType.FindProperty("IsAbsoluteLatestVersion");
      this.TitleProperty = this.V2FeedPackageType.FindProperty("Title");
      this.DownloadCountProperty = this.V2FeedPackageType.FindProperty("DownloadCount");
      this.ODataUriParser = new ODataUriParser(this.EdmModel, new Uri("https://example.com"));
    }

    public ODataUriParser ODataUriParser { get; }

    public IEdmModel EdmModel { get; }

    public IEdmEntityType V2FeedPackageType { get; }

    public IEdmProperty IdProperty { get; }

    public IEdmProperty NormalizedVersionProperty { get; }

    public IEdmProperty VersionProperty { get; }

    public IEdmProperty IsLatestVersionProperty { get; }

    public IEdmProperty IsAbsoluteLatestVersionProperty { get; }

    public IEdmProperty TitleProperty { get; }

    public IEdmProperty DownloadCountProperty { get; }
  }
}
