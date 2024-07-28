// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageDependencyDetailsBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageDependencyDetailsBinder : ObjectBinder<PackageDependencyDetails>
  {
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder packageVersionId = new SqlColumnBinder("PackageVersionId");
    private SqlColumnBinder normalizedPackageName = new SqlColumnBinder("NormalizedPackageName");
    private SqlColumnBinder packageName = new SqlColumnBinder("PackageName");

    protected override PackageDependencyDetails Bind() => new PackageDependencyDetails()
    {
      PackageId = this.packageId.GetGuid((IDataReader) this.Reader, true),
      PackageVersionId = this.packageVersionId.GetGuid((IDataReader) this.Reader, true),
      NormalizedPackageName = this.normalizedPackageName.GetString((IDataReader) this.Reader, true),
      PackageName = this.packageName.GetString((IDataReader) this.Reader, false)
    };
  }
}
