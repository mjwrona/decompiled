// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionCategoryBinder2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionCategoryBinder2 : ObjectBinder<ExtensionCategory>
  {
    protected SqlColumnBinder categoryIdColumn = new SqlColumnBinder("CategoryId");
    protected SqlColumnBinder internalNameColumn = new SqlColumnBinder("Internalname");
    protected SqlColumnBinder parentIdColumn = new SqlColumnBinder("ParentId");
    protected SqlColumnBinder migratedIdColumn = new SqlColumnBinder("MigratedId");

    protected override ExtensionCategory Bind() => new ExtensionCategory()
    {
      CategoryId = this.categoryIdColumn.GetInt32((IDataReader) this.Reader),
      CategoryName = this.internalNameColumn.GetString((IDataReader) this.Reader, false),
      ParentId = this.parentIdColumn.GetInt32((IDataReader) this.Reader, 0),
      MigratedId = this.migratedIdColumn.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
