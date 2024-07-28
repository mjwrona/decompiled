// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ResourcesTemplatesSource
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class ResourcesTemplatesSource : ITemplatesSource
  {
    private readonly IResourceTemplateCreator m_pipelinesTemplateCreator;

    public ResourcesTemplatesSource()
      : this((IResourceTemplateCreator) new PipelinesResourceTemplateCreator())
    {
    }

    internal ResourcesTemplatesSource(IResourceTemplateCreator pipelinesTemplateCreator) => this.m_pipelinesTemplateCreator = pipelinesTemplateCreator;

    public Template GetTemplate(
      IVssRequestContext requestContext,
      string templateId,
      CultureInfo cultureInfo = null)
    {
      if (!this.GetTemplateIds().Contains<string>(templateId, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
        throw new TemplateNotFoundException(PipelinesResources.TemplateNotFound((object) templateId));
      return this.CreateTemplateModel(requestContext, templateId, cultureInfo);
    }

    public IEnumerable<Template> GetTemplates(
      IVssRequestContext requestContext,
      CultureInfo cultureInfo = null)
    {
      HashSet<string> hashSet = this.GetTemplateIds().ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!requestContext.IsFeatureEnabled("Pipelines.EnableConfiglessTemplates"))
      {
        hashSet.Remove(TemplateIds.Pipelines.DockerContainerToAcr);
        hashSet.Remove(TemplateIds.Pipelines.DockerContainerToAks);
        hashSet.Remove(TemplateIds.Pipelines.DockerContainerFunctionApp);
        hashSet.Remove(TemplateIds.Pipelines.DockerContainerWebapp);
      }
      return hashSet.Select<string, Template>((Func<string, Template>) (resourceName => this.CreateTemplateModel(requestContext, resourceName, cultureInfo)));
    }

    private Template CreateTemplateModel(
      IVssRequestContext requestContext,
      string templateName,
      CultureInfo cultureInfo)
    {
      return this.m_pipelinesTemplateCreator.CreateTemplateModel(requestContext, templateName, cultureInfo);
    }

    private IEnumerable<string> GetTemplateIds() => this.m_pipelinesTemplateCreator.GetTemplateIds();
  }
}
