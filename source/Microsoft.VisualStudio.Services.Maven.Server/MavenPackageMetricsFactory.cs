// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPackageMetricsFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenPackageMetricsFactory : 
    IFactory<MavenArtifactFileRequest, IPackageMetricsServiceFacade>
  {
    private readonly IPackageMetricsServiceFacade metricsService;

    public MavenPackageMetricsFactory(IPackageMetricsServiceFacade metricsService) => this.metricsService = metricsService;

    public IPackageMetricsServiceFacade Get(MavenArtifactFileRequest input)
    {
      int num1 = !input.RequireContent ? 1 : 0;
      bool flag1 = MavenFileNameUtility.IsMetadataOrMetadataChecksumFile(input.FilePath);
      bool flag2 = MavenFileNameUtility.IsChecksumFile(input.FilePath);
      int num2 = flag1 ? 1 : 0;
      return (num1 | num2 | (flag2 ? 1 : 0)) == 0 ? this.metricsService : (IPackageMetricsServiceFacade) NoOpPackageMetricsService.Instance;
    }
  }
}
