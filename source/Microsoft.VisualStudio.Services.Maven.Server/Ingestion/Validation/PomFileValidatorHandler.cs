// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.Validation.PomFileValidatorHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion.Validation
{
  public class PomFileValidatorHandler : 
    IAsyncHandler<MavenPackageFileInfo>,
    IAsyncHandler<MavenPackageFileInfo, NullResult>,
    IHaveInputType<MavenPackageFileInfo>,
    IHaveOutputType<NullResult>
  {
    private readonly IRegistryService registryService;

    public PomFileValidatorHandler(IRegistryService registryService) => this.registryService = registryService;

    public Task<NullResult> Handle(MavenPackageFileInfo request)
    {
      if (MavenFileNameUtility.IsPomFile(request.FilePath.FileName))
      {
        long num = this.registryService.GetValue<long>((RegistryQuery) "/Configuration/Packaging/Maven/Ingestion/Pom/MaxSize", 524288L);
        Stream stream = request.Stream;
        if (stream.Length > num)
          throw new MavenPomSizeLimitExceededException(Resources.Error_PomFileSizeLimitExceeded((object) num));
        stream.Position = 0L;
        MavenPomUtility.Parse(stream);
        stream.Position = 0L;
      }
      return NullResult.NullTask;
    }
  }
}
