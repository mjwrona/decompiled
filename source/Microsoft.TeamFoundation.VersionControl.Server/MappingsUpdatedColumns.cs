// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MappingsUpdatedColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class MappingsUpdatedColumns : VersionControlObjectBinder<KeyValuePair<bool, DateTime>>
  {
    private SqlColumnBinder updated = new SqlColumnBinder("Updated");
    private SqlColumnBinder lastMappingsUpdate = new SqlColumnBinder("LastMappingsUpdate");

    protected override KeyValuePair<bool, DateTime> Bind() => new KeyValuePair<bool, DateTime>(this.updated.GetBoolean((IDataReader) this.Reader), this.lastMappingsUpdate.GetDateTime((IDataReader) this.Reader));
  }
}
