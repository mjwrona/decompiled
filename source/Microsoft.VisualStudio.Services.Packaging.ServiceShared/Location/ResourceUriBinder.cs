// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location.ResourceUriBinder
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location
{
  public class ResourceUriBinder : IResourceUriBinder
  {
    public string Area { get; }

    public string ResourceName { get; }

    public string RootPath { get; }

    public string RelativePathTemplate { get; }

    public bool AppendUnusedAsQueryParams { get; }

    public bool RequireExplicitRouteParams { get; }

    public ResourceUriBinder(
      string area,
      string resourceName,
      string rootPath,
      string relativePathTemplate,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams)
    {
      this.Area = area;
      this.ResourceName = resourceName;
      this.RootPath = rootPath;
      this.RelativePathTemplate = relativePathTemplate;
      this.AppendUnusedAsQueryParams = appendUnusedAsQueryParams;
      this.RequireExplicitRouteParams = requireExplicitRouteParams;
    }

    public Uri Bind(object routeValues) => new Uri(TFCommonUtil.CombinePaths(this.RootPath, VssHttpUriUtility.ReplaceRouteValues(this.RelativePathTemplate, VssHttpUriUtility.ToRouteDictionary(routeValues, this.Area, this.ResourceName), appendUnusedAsQueryParams: this.AppendUnusedAsQueryParams, requireExplicitRouteParams: this.RequireExplicitRouteParams)));
  }
}
