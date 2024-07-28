// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TemplatesService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TemplatesService : ITemplatesService, IVssFrameworkService
  {
    private ITemplatesSource m_templatesSource;

    public TemplatesService()
      : this((ITemplatesSource) new ResourcesTemplatesSource())
    {
    }

    internal TemplatesService(ITemplatesSource templatesSource) => this.m_templatesSource = templatesSource;

    public void ServiceStart(IVssRequestContext context) => context.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public Template GetTemplate(
      IVssRequestContext requestContext,
      string templateId,
      CultureInfo cultureInfo = null)
    {
      using (new Tracer<EventsService>(requestContext, TracePoints.Templates.GetTemplateEnter, TracePoints.Templates.GetTemplateLeave, nameof (GetTemplate)))
        return this.m_templatesSource.GetTemplate(requestContext, templateId, cultureInfo);
    }

    public IEnumerable<Template> GetTemplates(
      IVssRequestContext requestContext,
      CultureInfo cultureInfo = null)
    {
      using (new Tracer<EventsService>(requestContext, TracePoints.Templates.GetTemplatesEnter, TracePoints.Templates.GetTemplatesLeave, nameof (GetTemplates)))
        return this.m_templatesSource.GetTemplates(requestContext, cultureInfo);
    }

    public Template RenderTemplate(
      IVssRequestContext requestContext,
      string templateId,
      IReadOnlyDictionary<string, object> parameters,
      CultureInfo cultureInfo = null)
    {
      using (new Tracer<EventsService>(requestContext, TracePoints.Templates.RenderTemplateEnter, TracePoints.Templates.RenderTemplateLeave, nameof (RenderTemplate)))
      {
        Template template = this.m_templatesSource.GetTemplate(requestContext, templateId, cultureInfo);
        JObject replacementContext = TemplatesService.GetRenderReplacementContext(requestContext, parameters, template);
        MustacheTemplateEngine templateEngine = new MustacheTemplateEngine();
        template.Content = templateEngine.EvaluateTemplate(template.Content, (JToken) replacementContext);
        template.Assets.ForEach<TemplateAsset>((Action<TemplateAsset>) (asset => asset.Content = templateEngine.EvaluateTemplate(asset.Content, (JToken) replacementContext)));
        return template;
      }
    }

    private static JObject GetRenderReplacementContext(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<string, object> parameters,
      Template template)
    {
      JObject replacementContext = JObject.FromObject((object) template.Parameters.Where<TemplateParameterDefinition>((Func<TemplateParameterDefinition, bool>) (p => !string.IsNullOrEmpty(p.DefaultValue))).ToDedupedDictionary<TemplateParameterDefinition, string, string>((Func<TemplateParameterDefinition, string>) (p => p.Name), (Func<TemplateParameterDefinition, string>) (p => p.DefaultValue)));
      if (parameters != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in parameters.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (p => p.Value != null)))
          replacementContext[keyValuePair.Key] = JToken.FromObject(keyValuePair.Value);
      }
      foreach (KeyValuePair<string, JToken> keyValuePair in JObject.FromObject((object) requestContext.ExecutionEnvironment))
        replacementContext[keyValuePair.Key] = keyValuePair.Value;
      return replacementContext;
    }
  }
}
