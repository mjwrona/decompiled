// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryTagBinder2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryTagBinder2 : ObjectBinder<GalleryTag2>
  {
    protected SqlColumnBinder referenceIdColumn = new SqlColumnBinder("ReferenceId");
    protected SqlColumnBinder tagNameColumn = new SqlColumnBinder("TagName");
    protected SqlColumnBinder commentColumn = new SqlColumnBinder("Comment");
    protected SqlColumnBinder tempTagNameColumn = new SqlColumnBinder("TempTagName");

    protected override GalleryTag2 Bind() => new GalleryTag2()
    {
      ReferenceId = this.referenceIdColumn.GetGuid((IDataReader) this.Reader),
      TagName = this.tagNameColumn.GetString((IDataReader) this.Reader, false),
      Comment = this.commentColumn.GetString((IDataReader) this.Reader, true),
      TempTagName = this.tempTagNameColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
