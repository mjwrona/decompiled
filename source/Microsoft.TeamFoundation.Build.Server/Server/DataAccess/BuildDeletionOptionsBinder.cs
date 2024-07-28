// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeletionOptionsBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildDeletionOptionsBinder : 
    BuildObjectBinder<KeyValuePair<DeleteOptions, string>>
  {
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder deleteOptions = new SqlColumnBinder("DeleteOptions");

    protected override KeyValuePair<DeleteOptions, string> Bind()
    {
      string artifactUriFromString = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false);
      return new KeyValuePair<DeleteOptions, string>(this.deleteOptions.GetDeleteOptions(this.Reader), artifactUriFromString);
    }
  }
}
