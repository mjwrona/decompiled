// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ProductCategoryBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ProductCategoryBinder : ObjectBinder<ProductCategoryRow>
  {
    protected SqlColumnBinder categoryIdColumn = new SqlColumnBinder("CategoryId");
    protected SqlColumnBinder productColumn = new SqlColumnBinder("Product");

    protected override ProductCategoryRow Bind() => new ProductCategoryRow()
    {
      CategoryId = this.categoryIdColumn.GetInt32((IDataReader) this.Reader),
      Product = this.productColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
