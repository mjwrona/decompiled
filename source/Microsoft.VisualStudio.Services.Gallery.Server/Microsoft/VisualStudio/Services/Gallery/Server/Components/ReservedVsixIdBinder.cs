// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ReservedVsixIdBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ReservedVsixIdBinder : ObjectBinder<ReservedVsixId>
  {
    protected SqlColumnBinder vsixIdColumn = new SqlColumnBinder("VsixId");
    protected SqlColumnBinder purposeColumn = new SqlColumnBinder("Purpose");
    protected SqlColumnBinder useridColumn = new SqlColumnBinder("UserId");

    protected override ReservedVsixId Bind() => new ReservedVsixId()
    {
      VsixId = this.vsixIdColumn.GetString((IDataReader) this.Reader, false),
      Purpose = (ReservedVsixIdPurposeType) this.purposeColumn.GetInt32((IDataReader) this.Reader),
      UserId = this.useridColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
