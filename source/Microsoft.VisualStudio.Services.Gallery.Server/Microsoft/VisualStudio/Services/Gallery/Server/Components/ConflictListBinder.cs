// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ConflictListBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ConflictListBinder : ObjectBinder<ConflictListRow>
  {
    protected SqlColumnBinder oldNameColumn = new SqlColumnBinder("OldName");
    protected SqlColumnBinder newNameColumn = new SqlColumnBinder("NewName");

    protected override ConflictListRow Bind() => new ConflictListRow()
    {
      OldName = this.oldNameColumn.GetString((IDataReader) this.Reader, false),
      NewName = this.newNameColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
