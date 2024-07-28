// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionLcidBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionLcidBinder : ObjectBinder<ExtensionLcidRow>
  {
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder lcidColumn = new SqlColumnBinder("Lcid");

    protected override ExtensionLcidRow Bind() => new ExtensionLcidRow()
    {
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
      Lcid = this.lcidColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
