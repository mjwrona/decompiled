// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.OutboundLinkTypeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class OutboundLinkTypeColumns : ObjectBinder<OutboundLinkType>
  {
    private SqlColumnBinder sourceTypeColumn = new SqlColumnBinder("SourceType");
    private SqlColumnBinder sourceNameColumn = new SqlColumnBinder("SourceArtifactTypeName");
    private SqlColumnBinder linkNameColumn = new SqlColumnBinder("LinkName");
    private SqlColumnBinder sinkTypeColumn = new SqlColumnBinder("SinkType");
    private SqlColumnBinder sinkNameColumn = new SqlColumnBinder("SinkArtifactTypeName");

    protected override OutboundLinkType Bind() => new OutboundLinkType(this.linkNameColumn.GetString((IDataReader) this.Reader, false), this.sinkTypeColumn.GetString((IDataReader) this.Reader, false), this.sinkNameColumn.GetString((IDataReader) this.Reader, false))
    {
      ToolType = this.sourceTypeColumn.GetString((IDataReader) this.Reader, false),
      ArtifactName = this.sourceNameColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
