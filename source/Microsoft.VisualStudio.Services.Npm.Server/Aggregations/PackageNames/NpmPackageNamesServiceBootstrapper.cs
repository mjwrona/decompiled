// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames.NpmPackageNamesServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames
{
  public class NpmPackageNamesServiceBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmPackageNamesServiceBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>> Bootstrap()
    {
      StreamingPackageNamesDocumentJsonConverter<NpmPackageName> documentJsonConverter = new StreamingPackageNamesDocumentJsonConverter<NpmPackageName>((Func<string, NpmPackageName>) (s => new NpmPackageName(s)));
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      JsonSerializer<IReadOnlyList<PackageNameEntry<NpmPackageName>>> serializer = new JsonSerializer<IReadOnlyList<PackageNameEntry<NpmPackageName>>>(settings);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      return (IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>>) ByFuncInputFactory.For<ContainerAddress, PackageNamesByBlobService<NpmPackageName, NpmPackageIdentity>>((Func<ContainerAddress, PackageNamesByBlobService<NpmPackageName, NpmPackageIdentity>>) (containerAddress => new PackageNamesByBlobService<NpmPackageName, NpmPackageIdentity>(blobServiceFactory, containerAddress, (ISerializer<IReadOnlyList<PackageNameEntry<NpmPackageName>>>) serializer, tracerService)));
    }
  }
}
