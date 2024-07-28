// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.ServiceHostOwnershipBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class ServiceHostOwnershipBinder : BuildObjectBinder<KeyValuePair<Guid, Guid>>
  {
    private SqlColumnBinder oldSessionId = new SqlColumnBinder("OldSessionId");
    private SqlColumnBinder newSessionId = new SqlColumnBinder("NewSessionId");

    protected override KeyValuePair<Guid, Guid> Bind() => new KeyValuePair<Guid, Guid>(this.oldSessionId.GetGuid((IDataReader) this.Reader, true), this.newSessionId.GetGuid((IDataReader) this.Reader, true));
  }
}
