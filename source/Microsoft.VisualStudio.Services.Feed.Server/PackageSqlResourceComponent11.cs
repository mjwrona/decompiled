// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent11
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent11 : PackageSqlResourceComponent10
  {
    protected override IEnumerable<Package> ReadPackages(bool includeDescriptions)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = (PackageVersionBindOptions) (4 | (includeDescriptions ? 1 : 0));
        resultCollection.AddBinder<Package>((ObjectBinder<Package>) new PackageBinder((IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)));
        Dictionary<Guid, Package> dictionary = new Dictionary<Guid, Package>();
        foreach (Package package in resultCollection.GetCurrent<Package>())
        {
          if (dictionary.ContainsKey(package.Id))
          {
            (dictionary[package.Id].Versions as List<MinimalPackageVersion>).AddRange(package.Versions);
          }
          else
          {
            dictionary[package.Id] = package;
            package.Versions = (IEnumerable<MinimalPackageVersion>) new List<MinimalPackageVersion>(package.Versions);
          }
        }
        return (IEnumerable<Package>) dictionary.Values;
      }
    }

    protected override IEnumerable<PackageVersion> ReadPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.DirectViewSerialization;
        resultCollection.AddBinder<PackageVersion>((ObjectBinder<PackageVersion>) new BindOntoBinder<PackageVersion>((IBindOnto<PackageVersion>) new PackageVersionBinder(bindOptions, (IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions))));
        return (IEnumerable<PackageVersion>) resultCollection.GetCurrent<PackageVersion>().Items;
      }
    }
  }
}
