// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserAccessedHostBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class UserAccessedHostBinder : ObjectBinder<Tuple<Guid, int>>
  {
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder PreviouslyInactiveSecondsColumn = new SqlColumnBinder("PreviouslyInactiveSeconds");

    protected override Tuple<Guid, int> Bind() => new Tuple<Guid, int>(this.HostIdColumn.GetGuid((IDataReader) this.Reader), this.PreviouslyInactiveSecondsColumn.GetInt32((IDataReader) this.Reader));
  }
}
