// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.DistTag.DistTagProvider
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.DistTag
{
  public class DistTagProvider : 
    IAsyncHandler<
    #nullable disable
    IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>, IDictionary<string, string>>,
    IHaveInputType<IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>>,
    IHaveOutputType<IDictionary<string, string>>,
    IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>,
    IHaveInputType<IPackageNameRequest<NpmPackageName>>
  {
    private readonly IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService;

    public DistTagProvider(
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public async Task<IDictionary<string, string>> Handle(
      IPackageNameRequest<NpmPackageName> request)
    {
      return await this.Handle((IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>) request.WithData<NpmPackageName, MetadataDocument<INpmMetadataEntry>>((MetadataDocument<INpmMetadataEntry>) null));
    }

    public async Task<IDictionary<string, string>> Handle(
      IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>> request)
    {
      MetadataDocument<INpmMetadataEntry> doc = request.AdditionalData;
      if (doc == null)
        doc = await this.metadataService.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<INpmMetadataEntry>((IPackageNameRequest) request));
      IDictionary<string, string> dictionary = doc != null ? doc.GetMergedDistTags() : (IDictionary<string, string>) null;
      if (dictionary.IsNullOrEmpty<KeyValuePair<string, string>>())
        throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) request.PackageName.FullName, (object) request.Feed.FullyQualifiedName));
      if (request.Feed.View != null)
        dictionary = (IDictionary<string, string>) dictionary.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => d.Key == "latest")).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Key), (Func<KeyValuePair<string, string>, string>) (kv => kv.Value));
      return dictionary;
    }
  }
}
