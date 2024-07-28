// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenResolveRawFileRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenResolveRawFileRequestConverter : 
    IConverter<MavenRawFileRequest, MavenFileRequest>,
    IHaveInputType<MavenRawFileRequest>,
    IHaveOutputType<MavenFileRequest>
  {
    private readonly IConverter<IMavenFilePath, object> requestContextAnnotator;
    private readonly IFeatureFlagService featureFlagService;

    public MavenResolveRawFileRequestConverter(
      IConverter<IMavenFilePath, object> requestContextAnnotator,
      IFeatureFlagService featureFlagService)
    {
      this.requestContextAnnotator = requestContextAnnotator;
      this.featureFlagService = featureFlagService;
    }

    public MavenFileRequest Convert(MavenRawFileRequest input)
    {
      bool allowSnapshotLiteralInGroupIdAndArtifactId = this.featureFlagService.IsEnabled("Packaging.Maven.AllowSnapshotLiteralInGroupIdAndArtifactId");
      try
      {
        IMavenFilePath mavenFilePath = MavenFilePath.Parse(input.FilePath, allowSnapshotLiteralInGroupIdAndArtifactId);
        this.requestContextAnnotator.Convert(mavenFilePath);
        return new MavenFileRequest((IFeedRequest) input, mavenFilePath, input.RequireContent, input.StreamContent);
      }
      catch (MavenInvalidFilenameException ex) when (string.IsNullOrEmpty(ex.Filename))
      {
        throw new PackageNotFoundException(Resources.Error_FileNotFound((object) string.Empty, (object) input.Feed));
      }
    }
  }
}
