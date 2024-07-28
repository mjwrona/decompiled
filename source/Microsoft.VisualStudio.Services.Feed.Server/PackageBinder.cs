// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageBinder : ObjectBinder<Package>
  {
    private SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder normalizedPackageName = new SqlColumnBinder("NormalizedPackageName");
    private SqlColumnBinder packageName = new SqlColumnBinder("PackageName");
    private SqlColumnBinder isCached = new SqlColumnBinder("IsCached");
    private readonly IBindOnto<MinimalPackageVersion> minimalPackageVersionBinder;

    public PackageBinder(
      IBindOnto<MinimalPackageVersion> minimalPackageVersionBinder)
    {
      this.minimalPackageVersionBinder = minimalPackageVersionBinder;
    }

    protected override Package Bind()
    {
      bool flag = this.isCached.ColumnExists((IDataReader) this.Reader) && this.isCached.GetBoolean((IDataReader) this.Reader, false);
      MinimalPackageVersion minimalPackageVersion = new MinimalPackageVersion();
      this.minimalPackageVersionBinder.BindOnto(this.Reader, minimalPackageVersion);
      return new Package()
      {
        ProtocolType = this.protocolType.GetString((IDataReader) this.Reader, true),
        Id = this.packageId.GetGuid((IDataReader) this.Reader, true),
        NormalizedName = this.normalizedPackageName.GetString((IDataReader) this.Reader, true),
        Name = this.packageName.GetString((IDataReader) this.Reader, true),
        IsCached = flag,
        Versions = (IEnumerable<MinimalPackageVersion>) new List<MinimalPackageVersion>()
        {
          minimalPackageVersion
        }
      };
    }
  }
}
