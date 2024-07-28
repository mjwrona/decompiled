// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogServiceReferenceColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogServiceReferenceColumns : ObjectBinder<CatalogServiceReference>
  {
    private SqlColumnBinder resourceIdentifierColumn = new SqlColumnBinder("ResourceIdentifier");
    private SqlColumnBinder associationKeyColumn = new SqlColumnBinder("AssociationKey");
    private SqlColumnBinder serviceTypeColumn = new SqlColumnBinder("ServiceType");
    private SqlColumnBinder serviceIdentifierColumn = new SqlColumnBinder("ServiceIdentifier");

    protected override CatalogServiceReference Bind() => new CatalogServiceReference()
    {
      ResourceIdentifier = this.resourceIdentifierColumn.GetGuid((IDataReader) this.Reader),
      AssociationKey = this.associationKeyColumn.GetString((IDataReader) this.Reader, false),
      ServiceType = this.serviceTypeColumn.GetString((IDataReader) this.Reader, true),
      ServiceIdentifier = this.serviceIdentifierColumn.GetGuid((IDataReader) this.Reader)
    };
  }
}
