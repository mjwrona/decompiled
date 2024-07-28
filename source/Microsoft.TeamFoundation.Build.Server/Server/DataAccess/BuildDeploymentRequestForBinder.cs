// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeploymentRequestForBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildDeploymentRequestForBinder : ObjectBinder<Tuple<string, Guid>>
  {
    private SqlColumnBinder BuildUriColumn = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder RequestForColumn = new SqlColumnBinder("RequestFor");

    internal BuildDeploymentRequestForBinder()
    {
    }

    protected override Tuple<string, Guid> Bind() => new Tuple<string, Guid>(this.BuildUriColumn.GetArtifactUriFromInt64(this.Reader, "Build", false), new Guid(this.RequestForColumn.GetString((IDataReader) this.Reader, false)));
  }
}
