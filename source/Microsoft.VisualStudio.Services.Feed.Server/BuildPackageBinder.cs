// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.BuildPackageBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class BuildPackageBinder : ObjectBinder<BuildPackage>
  {
    private SqlColumnBinder packageName = new SqlColumnBinder("PackageName");
    private SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private SqlColumnBinder packageVersion = new SqlColumnBinder("PackageVersion");
    private SqlColumnBinder packageDescription = new SqlColumnBinder("PackageDescription");
    private SqlColumnBinder feedName = new SqlColumnBinder("FeedName");
    private SqlColumnBinder projectId = new SqlColumnBinder("ProjectId");

    protected override BuildPackage Bind() => new BuildPackage()
    {
      PackageName = this.packageName.GetString((IDataReader) this.Reader, true),
      ProtocolType = this.protocolType.GetString((IDataReader) this.Reader, true),
      PackageVersion = this.packageVersion.GetString((IDataReader) this.Reader, true),
      PackageDescription = this.packageDescription.GetString((IDataReader) this.Reader, true),
      FeedName = this.feedName.GetString((IDataReader) this.Reader, true),
      ProjectId = this.projectId.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
