// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Converters.MavenFileRequestToMavenArtifactFileRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Maven.Server.Converters
{
  public class MavenFileRequestToMavenArtifactFileRequestConverter : 
    IConverter<MavenFileRequest, MavenArtifactFileRequest>,
    IHaveInputType<MavenFileRequest>,
    IHaveOutputType<MavenArtifactFileRequest>
  {
    public MavenArtifactFileRequest Convert(MavenFileRequest input) => new MavenArtifactFileRequest((IFeedRequest) input, input.FilePath as IMavenArtifactFilePath, input.RequireContent, input.StreamContent);
  }
}
