// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ReservePropertyIdsBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ReservePropertyIdsBinder15 : VersionControlObjectBinder<PropertyDataspaceIdPair>
  {
    protected SqlColumnBinder startRange = new SqlColumnBinder("StartRange");
    protected SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public ReservePropertyIdsBinder15()
    {
    }

    public ReservePropertyIdsBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override PropertyDataspaceIdPair Bind()
    {
      PropertyDataspaceIdPair propertyDataspaceIdPair = new PropertyDataspaceIdPair();
      propertyDataspaceIdPair.StartRange = this.startRange.GetInt32((IDataReader) this.Reader);
      int int32 = this.dataspaceId.GetInt32((IDataReader) this.Reader, 0);
      propertyDataspaceIdPair.DataspaceId = int32 > 0 ? this.GetDataspaceIdentifier(int32) : Guid.Empty;
      return propertyDataspaceIdPair;
    }
  }
}
