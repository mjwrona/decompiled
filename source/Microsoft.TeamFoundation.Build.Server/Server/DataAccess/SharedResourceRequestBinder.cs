// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.SharedResourceRequestBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class SharedResourceRequestBinder : BuildObjectBinder<SharedResourceRequest>
  {
    private SqlColumnBinder resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder requestedByBuild = new SqlColumnBinder("RequestedByBuild");
    private SqlColumnBinder requestedOn = new SqlColumnBinder("RequestTime");

    protected override SharedResourceRequest Bind() => new SharedResourceRequest()
    {
      ResourceId = this.resourceId.GetInt32((IDataReader) this.Reader),
      RequestedBy = this.requestedBy.GetString((IDataReader) this.Reader, false),
      RequestedByBuild = this.requestedByBuild.GetArtifactUriFromInt32(this.Reader, "Build", true),
      RequestedOn = this.requestedOn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
