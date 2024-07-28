// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryShelvesetsColumns2
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryShelvesetsColumns2 : QueryShelvesetsColumns
  {
    internal SqlColumnBinder shelvesetVersion = new SqlColumnBinder("ShelvesetVersion");

    protected override Shelveset Bind()
    {
      Shelveset shelveset = base.Bind();
      shelveset.Version = this.shelvesetVersion.GetInt32((IDataReader) this.Reader);
      return shelveset;
    }
  }
}
