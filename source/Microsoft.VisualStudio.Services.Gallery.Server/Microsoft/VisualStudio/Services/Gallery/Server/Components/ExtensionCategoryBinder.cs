// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionCategoryBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionCategoryBinder : ObjectBinder<ExtensionCategory>
  {
    protected SqlColumnBinder categoryNameColumn = new SqlColumnBinder("CategoryName");
    protected SqlColumnBinder languangeColumn = new SqlColumnBinder("Lang");
    protected SqlColumnBinder categoryIdColumn = new SqlColumnBinder("CategoryId");

    protected override ExtensionCategory Bind() => new ExtensionCategory()
    {
      CategoryName = this.categoryNameColumn.GetString((IDataReader) this.Reader, false),
      Language = this.languangeColumn.GetString((IDataReader) this.Reader, false),
      CategoryId = this.categoryIdColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
