// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectIdNameColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectIdNameColumns : ObjectBinder<Tuple<Guid, string>>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder nameColumn = new SqlColumnBinder("ProjectName");

    protected override Tuple<Guid, string> Bind()
    {
      string userPath = this.nameColumn.GetString((IDataReader) this.Reader, true);
      if (userPath != null)
        userPath = DBPath.DatabaseToUserPath(userPath);
      return Tuple.Create<Guid, string>(this.idColumn.GetGuid((IDataReader) this.Reader, false), userPath);
    }
  }
}
