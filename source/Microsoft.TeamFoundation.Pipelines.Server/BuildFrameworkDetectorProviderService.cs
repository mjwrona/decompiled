// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.BuildFrameworkDetectorProviderService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class BuildFrameworkDetectorProviderService : 
    IBuildFrameworkDetectorProviderService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyList<IBuildFrameworkDetector> GetDetectors(IVssRequestContext requestContext) => (IReadOnlyList<IBuildFrameworkDetector>) new IBuildFrameworkDetector[7]
    {
      (IBuildFrameworkDetector) new DockerBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new NodeJsBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new PythonBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new PhpBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new MsBuildBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new DotNetCoreBuildFrameworkDetector(),
      (IBuildFrameworkDetector) new PowershellBuildFrameworkDetector()
    };

    public IBuildFrameworkDetector GetFallbackDetector(IVssRequestContext requestContext) => (IBuildFrameworkDetector) new FallbackBuildFrameworkDetector();
  }
}
