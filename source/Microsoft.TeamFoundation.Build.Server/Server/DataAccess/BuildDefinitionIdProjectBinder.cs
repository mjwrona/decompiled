// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionIdProjectBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildDefinitionIdProjectBinder : BuildObjectBinder<Tuple<Guid, int>>
  {
    private SqlColumnBinder id = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder dataspace = new SqlColumnBinder("DataspaceId");

    public BuildDefinitionIdProjectBinder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Tuple<Guid, int> Bind()
    {
      int int32 = this.id.GetInt32((IDataReader) this.Reader);
      return new Tuple<Guid, int>(this.Component.GetDataspaceIdentifier(this.dataspace.GetInt32((IDataReader) this.Reader)), int32);
    }
  }
}
