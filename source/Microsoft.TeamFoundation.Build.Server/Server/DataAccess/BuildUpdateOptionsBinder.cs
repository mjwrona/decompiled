// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildUpdateOptionsBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildUpdateOptionsBinder : BuildObjectBinder<BuildUpdateOptions>
  {
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder updatedFields = new SqlColumnBinder("UpdatedFields");

    protected override BuildUpdateOptions Bind() => new BuildUpdateOptions()
    {
      Uri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false),
      Fields = this.updatedFields.GetBuildUpdateFlags(this.Reader)
    };
  }
}
