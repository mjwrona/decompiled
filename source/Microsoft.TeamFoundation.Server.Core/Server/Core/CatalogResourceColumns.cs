// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogResourceColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogResourceColumns : ObjectBinder<CatalogResource>
  {
    private SqlColumnBinder identifierColumn = new SqlColumnBinder("Identifier");
    private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder resourceTypeColumn = new SqlColumnBinder("ResourceType");
    private SqlColumnBinder matchedQueryColumn = new SqlColumnBinder("MatchedQuery");
    private SqlColumnBinder propertyIdColumn = new SqlColumnBinder("PropertyId");

    protected override CatalogResource Bind() => new CatalogResource()
    {
      Identifier = this.identifierColumn.GetGuid((IDataReader) this.Reader),
      DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false),
      Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
      ResourceTypeIdentifier = this.resourceTypeColumn.GetGuid((IDataReader) this.Reader),
      MatchedQuery = this.matchedQueryColumn.GetBoolean((IDataReader) this.Reader),
      PropertyId = this.propertyIdColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
