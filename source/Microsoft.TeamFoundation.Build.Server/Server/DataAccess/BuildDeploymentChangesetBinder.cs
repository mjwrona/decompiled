// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeploymentChangesetBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildDeploymentChangesetBinder : 
    ObjectBinder<Tuple<string, ChangesetDisplayInformation>>
  {
    private SqlColumnBinder BuildUriColumn = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder ChangesetIdColumn = new SqlColumnBinder("ChangesetId");
    private SqlColumnBinder CheckedInByColumn = new SqlColumnBinder("CheckedInBy");

    internal BuildDeploymentChangesetBinder()
    {
    }

    protected override Tuple<string, ChangesetDisplayInformation> Bind() => new Tuple<string, ChangesetDisplayInformation>(this.BuildUriColumn.GetArtifactUriFromInt64(this.Reader, "Build", false), new ChangesetDisplayInformation(this.ChangesetIdColumn.GetInt32((IDataReader) this.Reader), this.CheckedInByColumn.GetString((IDataReader) this.Reader, false)));
  }
}
