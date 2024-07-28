// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessMappingColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AccessMappingColumns : ObjectBinder<AccessMapping>
  {
    private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder monikerColumn = new SqlColumnBinder("Moniker");
    private SqlColumnBinder accessPointColumn = new SqlColumnBinder("AccessPoint");

    protected override AccessMapping Bind() => new AccessMapping(this.monikerColumn.GetString((IDataReader) this.Reader, false), this.displayNameColumn.GetString((IDataReader) this.Reader, true), this.accessPointColumn.GetString((IDataReader) this.Reader, false));
  }
}
