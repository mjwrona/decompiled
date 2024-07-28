// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelineFragmentProviderBase
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public abstract class PipelineFragmentProviderBase : IPipelineFragmentProvider
  {
    public IReadOnlyList<Template> GetTemplates(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      return !this.IsSupportedFramework(detectedBuildFramework) ? (IReadOnlyList<Template>) new Template[0] : (IReadOnlyList<Template>) this.GetTemplatesInternal(requestContext, repositoryContext, detectedBuildFramework).ToList<Template>();
    }

    protected abstract IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework);

    protected abstract bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework);

    protected Template GetTemplate(IVssRequestContext requestContext, string templateId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITemplatesService>().GetTemplate(vssRequestContext, templateId);
    }

    protected IEnumerable<string> FilterTemplateIdsToDialect(IEnumerable<string> templateIds) => templateIds.Except<string>((IEnumerable<string>) TemplateIds.Workflow.Ids, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
