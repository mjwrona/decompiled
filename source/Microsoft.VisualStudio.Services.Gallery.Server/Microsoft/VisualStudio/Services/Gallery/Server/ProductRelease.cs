// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProductRelease
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ProductRelease
  {
    private IList<string> mProductSubReleasesId;

    public string Id { get; set; }

    public string Name { get; set; }

    public Version MinVersion { get; set; }

    public Version MaxVersion { get; set; }

    public Product Product { get; set; }

    public IList<string> ProductSubReleasesId => this.mProductSubReleasesId;

    public ProductRelease() => this.mProductSubReleasesId = (IList<string>) new List<string>();

    public ProductRelease(
      string id,
      string name,
      Version minVersion,
      Version maxVersion,
      Product product,
      IList<string> productSubReleaseId)
    {
      this.Id = id;
      this.Name = name;
      this.MinVersion = minVersion;
      this.MaxVersion = maxVersion;
      this.Product = product;
      this.mProductSubReleasesId = (IList<string>) new List<string>();
      if (productSubReleaseId == null)
        return;
      foreach (string str in (IEnumerable<string>) productSubReleaseId)
        this.mProductSubReleasesId.Add(str);
    }
  }
}
