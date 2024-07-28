// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProductFamily
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ProductFamily
  {
    private IList<string> mProductsId;

    public string Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public IList<string> ProductsId => this.mProductsId;

    public ProductFamily() => this.mProductsId = (IList<string>) new List<string>();

    public ProductFamily(string id, string name, string displayName, IList<string> productsId)
    {
      this.Id = id;
      this.Name = name;
      this.DisplayName = displayName;
      this.mProductsId = (IList<string>) new List<string>();
      if (productsId == null)
        return;
      foreach (string str in (IEnumerable<string>) productsId)
        this.mProductsId.Add(str);
    }
  }
}
