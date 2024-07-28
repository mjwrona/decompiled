// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent3
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Binders;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRecycleBinSqlResourceComponent3 : PackageRecycleBinSqlResourceComponent2
  {
    protected override IEnumerable<Package> ReadPackages()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.IncludePackageDescriptionInMinimalPackageVersion | PackageVersionBindOptions.IncludeDetailsForDeletedVersions | PackageVersionBindOptions.DirectViewSerialization;
        resultCollection.AddBinder<Package>((ObjectBinder<Package>) new PackageBinder((IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)));
        return (IEnumerable<Package>) resultCollection.GetCurrent<Package>().Items;
      }
    }

    protected override IEnumerable<RecycleBinPackageVersion> ReadRecycleBinPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.IncludeDetailsForDeletedVersions | PackageVersionBindOptions.DirectViewSerialization;
        resultCollection.AddBinder<RecycleBinPackageVersion>((ObjectBinder<RecycleBinPackageVersion>) new BindOntoBinder<RecycleBinPackageVersion>((IBindOnto<RecycleBinPackageVersion>) new RecycleBinPackageVersionBinder((IBindOnto<PackageVersion>) new PackageVersionBinder(bindOptions, (IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)))));
        return (IEnumerable<RecycleBinPackageVersion>) resultCollection.GetCurrent<RecycleBinPackageVersion>().Items;
      }
    }
  }
}
