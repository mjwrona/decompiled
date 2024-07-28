// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Product
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class Product
  {
    private IList<string> mProductReleasesId;

    public string Id { get; set; }

    public string Name { get; set; }

    public ProductFamily Parent { get; set; }

    public IList<string> ProductReleasesId => this.mProductReleasesId;

    public Product() => this.mProductReleasesId = (IList<string>) new List<string>();

    public Product(string id, string name, ProductFamily parent, IList<string> productReleaseId)
    {
      this.Id = id;
      this.Name = name;
      this.Parent = parent;
      this.mProductReleasesId = (IList<string>) new List<string>();
      if (productReleaseId == null)
        return;
      foreach (string str in (IEnumerable<string>) productReleaseId)
        this.mProductReleasesId.Add(str);
    }
  }
}
