// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionMetadataBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionMetadataBinder : ObjectBinder<ExtensionMetadataRow>
  {
    protected SqlColumnBinder extensionIdIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder keyNameColumn = new SqlColumnBinder("KeyName");
    protected SqlColumnBinder valueColumn = new SqlColumnBinder("Value");

    protected override ExtensionMetadataRow Bind() => new ExtensionMetadataRow()
    {
      ExtensionId = this.extensionIdIdColumn.GetGuid((IDataReader) this.Reader),
      KeyName = this.keyNameColumn.GetString((IDataReader) this.Reader, false),
      Value = this.valueColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
