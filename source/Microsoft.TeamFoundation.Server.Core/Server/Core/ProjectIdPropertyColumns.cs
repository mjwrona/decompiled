// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectIdPropertyColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectIdPropertyColumns : ObjectBinder<Tuple<Guid, ProjectProperty>>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("project_id");
    private SqlColumnBinder nameColumn = new SqlColumnBinder("name");
    private SqlColumnBinder valueColumn = new SqlColumnBinder("value");

    protected override Tuple<Guid, ProjectProperty> Bind() => Tuple.Create<Guid, ProjectProperty>(this.idColumn.GetGuid((IDataReader) this.Reader, false), new ProjectProperty()
    {
      Name = this.nameColumn.GetString((IDataReader) this.Reader, false),
      Value = (object) this.valueColumn.GetString((IDataReader) this.Reader, false)
    });
  }
}
