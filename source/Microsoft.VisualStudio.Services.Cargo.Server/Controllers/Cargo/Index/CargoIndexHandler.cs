// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index.CargoIndexHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index
{
  public class CargoIndexHandler : TracingHandler<IPackageNameRequest<CargoPackageName>, string>
  {
    private readonly IReadMetadataService<CargoPackageIdentity, ICargoMetadataEntry> metadataService;

    public CargoIndexHandler(
      IReadMetadataService<CargoPackageIdentity, ICargoMetadataEntry> metadataService,
      ITracerService tracerService)
      : base(tracerService)
    {
      this.metadataService = metadataService;
    }

    public override async Task<string> Handle(
      IPackageNameRequest<CargoPackageName> request,
      ITracerBlock tracer)
    {
      PackageNameQuery<ICargoMetadataEntry> packageNameQueryRequest = new PackageNameQuery<ICargoMetadataEntry>((IPackageNameRequest) request);
      packageNameQueryRequest.Options = new QueryOptions<ICargoMetadataEntry>().WithFilter((Func<ICargoMetadataEntry, bool>) (x => !x.IsDeleted()));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return CargoIndexHandler.PrepareIndexFileContent((await this.metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest)).Select<ICargoMetadataEntry, CargoIndexVersionRow>(CargoIndexHandler.\u003C\u003EO.\u003C0\u003E__FromMetadataEntry ?? (CargoIndexHandler.\u003C\u003EO.\u003C0\u003E__FromMetadataEntry = new Func<ICargoMetadataEntry, CargoIndexVersionRow>(CargoIndexVersionRow.FromMetadataEntry))));
    }

    public static string PrepareIndexFileContent(IEnumerable<CargoIndexVersionRow> packageVersions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (CargoIndexVersionRow packageVersion in packageVersions)
      {
        stringBuilder.Append(packageVersion.Serialize<CargoIndexVersionRow>());
        stringBuilder.Append("\n");
      }
      return stringBuilder.ToString();
    }
  }
}
