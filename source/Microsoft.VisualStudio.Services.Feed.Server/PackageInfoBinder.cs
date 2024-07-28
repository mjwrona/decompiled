// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageInfoBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageInfoBinder : ObjectBinder<IPackageInfo>
  {
    private SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder normalizedPackageName = new SqlColumnBinder("NormalizedPackageName");
    private SqlColumnBinder packageName = new SqlColumnBinder("PackageName");

    protected override IPackageInfo Bind() => (IPackageInfo) new PackageInfo()
    {
      ProtocolType = this.protocolType.GetString((IDataReader) this.Reader, false),
      Id = this.packageId.GetGuid((IDataReader) this.Reader, false),
      NormalizedName = this.normalizedPackageName.GetString((IDataReader) this.Reader, false),
      Name = this.packageName.GetString((IDataReader) this.Reader, false)
    };
  }
}
