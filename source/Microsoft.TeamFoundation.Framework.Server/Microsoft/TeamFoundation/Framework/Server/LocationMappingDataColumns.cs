// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationMappingDataColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationMappingDataColumns : ObjectBinder<LocationMappingData>
  {
    private SqlColumnBinder serviceTypeColumn = new SqlColumnBinder("ServiceType");
    private SqlColumnBinder serviceIdentifierColumn = new SqlColumnBinder("ServiceIdentifier");
    private SqlColumnBinder accessMappingMonikerColumn = new SqlColumnBinder("AccessMappingMoniker");
    private SqlColumnBinder locationColumn = new SqlColumnBinder("Location");

    protected override LocationMappingData Bind() => new LocationMappingData(this.serviceTypeColumn.GetString((IDataReader) this.Reader, false), this.serviceIdentifierColumn.GetGuid((IDataReader) this.Reader), this.accessMappingMonikerColumn.GetString((IDataReader) this.Reader, false), this.locationColumn.GetString((IDataReader) this.Reader, false));
  }
}
