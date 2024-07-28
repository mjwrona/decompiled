// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PhpPipelineFragmentProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class PhpPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      PhpPipelineFragmentProvider fragmentProvider = this;
      yield return fragmentProvider.GetTemplate(requestContext, TemplateIds.Php);
      yield return fragmentProvider.GetTemplate(requestContext, TemplateIds.Pipelines.PhpWebAppToLinuxOnAzure);
    }

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => string.Equals(detectedBuildFramework.Id, PhpBuildFrameworkDetector.Id, StringComparison.OrdinalIgnoreCase);
  }
}
