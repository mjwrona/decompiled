// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.TagBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class TagBinder : ObjectBinder<TagItem>
  {
    protected SqlColumnBinder ReferenceId = new SqlColumnBinder(nameof (ReferenceId));
    protected SqlColumnBinder TagTypeColumn = new SqlColumnBinder("TagType");
    protected SqlColumnBinder TagNameColumn = new SqlColumnBinder("TagName");
    protected SqlColumnBinder CommentColumn = new SqlColumnBinder("Comment");
    protected SqlColumnBinder IdColumn = new SqlColumnBinder("Id");

    protected override TagItem Bind() => new TagItem()
    {
      ReferenceId = this.ReferenceId.GetGuid((IDataReader) this.Reader),
      TagType = (TagType) this.TagTypeColumn.GetByte((IDataReader) this.Reader),
      TagName = this.TagNameColumn.GetString((IDataReader) this.Reader, true),
      Comment = this.CommentColumn.GetString((IDataReader) this.Reader, true),
      Id = this.IdColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
