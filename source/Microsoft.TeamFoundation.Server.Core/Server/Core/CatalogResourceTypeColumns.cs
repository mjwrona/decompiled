// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogResourceTypeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogResourceTypeColumns : ObjectBinder<CatalogResourceType>
  {
    private SqlColumnBinder identifierColumn = new SqlColumnBinder("Identifier");
    private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");

    protected override CatalogResourceType Bind() => new CatalogResourceType()
    {
      Identifier = this.identifierColumn.GetGuid((IDataReader) this.Reader),
      DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false),
      Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
