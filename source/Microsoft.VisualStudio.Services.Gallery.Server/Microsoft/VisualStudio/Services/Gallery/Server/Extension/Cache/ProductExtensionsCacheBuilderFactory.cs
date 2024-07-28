// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.ProductExtensionsCacheBuilderFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class ProductExtensionsCacheBuilderFactory
  {
    public static ProductExtensionsCacheBuilder GetProductExtensionsCacheBuilder(string productType)
    {
      if (string.Equals(productType, "vsts", StringComparison.OrdinalIgnoreCase))
        return (ProductExtensionsCacheBuilder) new VstsExtensionsCacheBuilder();
      if (string.Equals(productType, "vscode", StringComparison.OrdinalIgnoreCase))
        return (ProductExtensionsCacheBuilder) new VsCodeExtensionsCacheBuilder();
      throw new ArgumentException(GalleryResources.InvalidProductToCache((object) productType));
    }
  }
}
