// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.SharedResourceBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class SharedResourceBinder : BuildObjectBinder<SharedResource>
  {
    private SqlColumnBinder resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder resourceName = new SqlColumnBinder("ResourceName");
    private SqlColumnBinder lockedBy = new SqlColumnBinder("LockedBy");
    private SqlColumnBinder lockedByBuild = new SqlColumnBinder("LockedByBuild");
    private SqlColumnBinder lockedOn = new SqlColumnBinder("LockedOn");

    protected override SharedResource Bind() => new SharedResource()
    {
      Id = this.resourceId.GetInt32((IDataReader) this.Reader),
      Name = this.resourceName.GetString((IDataReader) this.Reader, false),
      LockedBy = this.lockedBy.GetString((IDataReader) this.Reader, false),
      LockedByBuild = this.lockedByBuild.GetArtifactUriFromInt32(this.Reader, "Build", true),
      LockedOn = this.lockedOn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
