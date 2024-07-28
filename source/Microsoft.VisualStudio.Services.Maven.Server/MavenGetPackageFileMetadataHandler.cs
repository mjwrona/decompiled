// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageFileMetadataHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageFileMetadataHandler : 
    IAsyncHandler<
    #nullable disable
    MavenFileRequest, MavenPackageFileResponse>,
    IHaveInputType<MavenFileRequest>,
    IHaveOutputType<MavenPackageFileResponse>
  {
    private readonly IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry> metadataService;
    private readonly IMavenPluginMetadataStore pluginStore;
    private readonly IFeatureFlagService featureFlagService;

    public MavenGetPackageFileMetadataHandler(
      IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry> metadataService,
      IMavenPluginMetadataStore pluginStore,
      IFeatureFlagService featureFlagService)
    {
      this.metadataService = metadataService;
      this.pluginStore = pluginStore;
      this.featureFlagService = featureFlagService;
    }

    public async Task<MavenPackageFileResponse> Handle(MavenFileRequest input)
    {
      MavenPackageFileResponse packageFileResponse1 = new MavenPackageFileResponse();
      packageFileResponse1.FileRequest = input;
      packageFileResponse1.FileName = input.FilePath.FileName;
      MavenPackageFileResponse packageFileResponse2 = packageFileResponse1;
      packageFileResponse2.Content = await this.GetPackageMetadataAsync(input, (IMavenMetadataFilePath) input.FilePath);
      return packageFileResponse1;
    }

    private async Task<Stream> GetPackageMetadataAsync(
      MavenFileRequest request,
      IMavenMetadataFilePath metadataFilePath)
    {
      IMavenXml metadataXmlAsync = await this.GetMetadataXmlAsync((IFeedRequest) request, metadataFilePath);
      if (!request.RequireContent)
        return (Stream) null;
      bool omitXmlDeclaration = this.featureFlagService.IsEnabled("Packaging.Maven.OmitXmlDeclaration");
      byte[] buffer = metadataXmlAsync.GetBytes(omitXmlDeclaration);
      MavenHashAlgorithmInfo algorithm;
      if (MavenHashAlgorithmInfo.TryGet(metadataFilePath.Extension, out algorithm))
        buffer = MavenChecksumUtility.ComputeChecksum(buffer, algorithm.Id);
      return (Stream) new MemoryStream(buffer);
    }

    private Task<IMavenXml> GetMetadataXmlAsync(
      IFeedRequest feedRequest,
      IMavenMetadataFilePath metadataFilePath)
    {
      Guid feedId = feedRequest.Feed.Id;
      if (metadataFilePath is IMavenVersionLevelMetadataFilePath filePath)
        return GetVersionMetadataXmlAsync(filePath);
      if (metadataFilePath is IMavenGroupIdLevelMetadataFilePath || metadataFilePath is IMavenArtifactIdLevelMetadataFilePath)
        return GetGroupOrArtifactMetadataXmlAsync();
      throw new UnrecognizedMavenFilePathException(metadataFilePath.FileName + " " + metadataFilePath.Extension);

      async Task<IMavenXml> GetVersionMetadataXmlAsync(IMavenVersionLevelMetadataFilePath filePath)
      {
        MavenPackageIdentity packageIdentity = new MavenPackageIdentity(filePath.PackageName, filePath.PackageVersion);
        PackageRequest<MavenPackageIdentity> packageRequest = new PackageRequest<MavenPackageIdentity>(feedRequest, packageIdentity);
        IMavenXml metadataXmlAsync = (IMavenXml) new MavenSnapshotMetadata(packageIdentity, (await this.GetMetadataEntryAsync((IPackageRequest<MavenPackageIdentity>) packageRequest)).ThrowIfNotActive(feedRequest.Feed));
        packageIdentity = (MavenPackageIdentity) null;
        return metadataXmlAsync;
      }

      async Task<IMavenXml> GetGroupOrArtifactMetadataXmlAsync()
      {
        MavenPluginList pluginsMetadata = (MavenPluginList) null;
        if (metadataFilePath is IMavenGroupIdLevelMetadataFilePath metadataFilePath1)
          pluginsMetadata = await this.pluginStore.GetPluginListAsync(feedRequest.Feed, metadataFilePath1.GroupId);
        if (metadataFilePath is IMavenArtifactIdLevelMetadataFilePath metadataFilePath2)
        {
          MavenPackageName packageName = metadataFilePath2.PackageName;
          List<IMavenMetadataEntry> versionStatesAsync = await this.metadataService.GetPackageVersionStatesAsync(new PackageNameQuery<IMavenMetadataEntry>((IPackageNameRequest) new PackageNameRequest<MavenPackageName>(feedRequest, packageName)));
          if (!versionStatesAsync.IsNullOrEmpty<IMavenMetadataEntry>())
          {
            IEnumerable<IMavenMetadataEntry> mavenMetadataEntries = versionStatesAsync.Where<IMavenMetadataEntry>((Func<IMavenMetadataEntry, bool>) (e => !e.IsDeleted()));
            MavenArtifactMetadata metadataXmlAsync = !mavenMetadataEntries.IsNullOrEmpty<IMavenMetadataEntry>() ? new MavenArtifactMetadata(packageName, mavenMetadataEntries) : throw ExceptionHelper.PackageNotFound(Resources.Error_FileNotFound((object) metadataFilePath.FileName, (object) feedId.ToString("B")));
            metadataXmlAsync.Plugins = pluginsMetadata;
            return (IMavenXml) metadataXmlAsync;
          }
          packageName = (MavenPackageName) null;
        }
        return pluginsMetadata != null ? (IMavenXml) pluginsMetadata : throw ExceptionHelper.PackageNotFound(Resources.Error_FileNotFound((object) metadataFilePath.FileName, (object) feedId.ToString("B")));
      }
    }

    private async Task<IMavenMetadataEntry> GetMetadataEntryAsync(
      IPackageRequest<MavenPackageIdentity> packageRequest)
    {
      return await this.metadataService.GetPackageVersionStateAsync(packageRequest) ?? throw ExceptionHelper.PackageNotFound((IPackageIdentity) packageRequest.PackageId, packageRequest.Feed);
    }
  }
}
