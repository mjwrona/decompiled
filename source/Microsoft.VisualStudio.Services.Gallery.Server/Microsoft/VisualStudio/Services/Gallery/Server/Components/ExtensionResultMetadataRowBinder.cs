// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionResultMetadataRowBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionResultMetadataRowBinder : ObjectBinder<ExtensionResultMetadataRow>
  {
    protected SqlColumnBinder metadataTypeColumn = new SqlColumnBinder("MetadataType");
    protected SqlColumnBinder metadataNameColumn = new SqlColumnBinder("MetadataName");
    protected SqlColumnBinder valueColumn = new SqlColumnBinder("Value");
    protected SqlColumnBinder queryIndexColumn = new SqlColumnBinder("QueryIndex");

    protected override ExtensionResultMetadataRow Bind() => new ExtensionResultMetadataRow()
    {
      QueryIndex = this.queryIndexColumn.GetInt32((IDataReader) this.Reader, 0),
      MetadataType = this.metadataTypeColumn.GetString((IDataReader) this.Reader, true),
      MetadataName = this.metadataNameColumn.GetString((IDataReader) this.Reader, true),
      Value = this.valueColumn.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
