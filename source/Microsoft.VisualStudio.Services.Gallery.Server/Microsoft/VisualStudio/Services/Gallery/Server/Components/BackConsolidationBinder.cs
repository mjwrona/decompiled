// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.BackConsolidationBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class BackConsolidationBinder : ObjectBinder<BackConsolidationMappingEntry>
  {
    protected SqlColumnBinder sourceExtensionId = new SqlColumnBinder("SourceExtensionId");
    protected SqlColumnBinder sourceExtensionVsixId = new SqlColumnBinder("SourceExtensionVsixId");
    protected SqlColumnBinder targetExtensionId = new SqlColumnBinder("TargetExtensionId");
    protected SqlColumnBinder targetExtensionVsixId = new SqlColumnBinder("TargetExtensionVsixId");

    protected override BackConsolidationMappingEntry Bind() => new BackConsolidationMappingEntry()
    {
      SourceExtensionId = this.sourceExtensionId.GetGuid((IDataReader) this.Reader),
      SourceExtensionVsixId = this.sourceExtensionVsixId.GetString((IDataReader) this.Reader, false),
      TargetExtensionId = this.targetExtensionId.GetGuid((IDataReader) this.Reader),
      TargetExtensionVsixId = this.targetExtensionVsixId.GetString((IDataReader) this.Reader, false)
    };
  }
}
