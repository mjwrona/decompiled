// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TextTemplating.MustacheTemplateParserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TextTemplating
{
  internal class MustacheTemplateParserService : IMustacheTemplateParserService, IVssFrameworkService
  {
    private IEnumerable<IMustacheTemplateHelper> m_registeredPlugins;
    private IEnumerable<IServerMustacheTemplateHelper> m_registeredServerPlugins;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_registeredPlugins = (IEnumerable<IMustacheTemplateHelper>) systemRequestContext.GetExtensions<IMustacheTemplateHelper>(ExtensionLifetime.Service, throwOnError: true);
      this.m_registeredServerPlugins = (IEnumerable<IServerMustacheTemplateHelper>) systemRequestContext.GetExtensions<IServerMustacheTemplateHelper>(ExtensionLifetime.Service, throwOnError: true);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public MustacheTemplateParser GetTemplateParser(
      IVssRequestContext requestContext,
      bool usePluginHelpers = true,
      bool useDefaultHandlebarHelpers = true,
      bool useCommonTemplateHelpers = true)
    {
      MustacheTemplateParser parser = new MustacheTemplateParser(useDefaultHandlebarHelpers, useCommonTemplateHelpers);
      if (usePluginHelpers)
        this.AddPluginHelpers(parser, true, true);
      return parser;
    }

    public IServerMustacheTemplateExpression ParseServerTemplate(
      IVssRequestContext requestContext,
      string templateExpression,
      bool usePluginHelpers = true,
      bool useDefaultHandlebarHelpers = true,
      bool useCommonTemplateHelpers = true)
    {
      return (IServerMustacheTemplateExpression) new ServerMustacheTemplateExpression(this.GetTemplateParser(requestContext, usePluginHelpers, useDefaultHandlebarHelpers, useCommonTemplateHelpers).Parse(templateExpression));
    }

    private void AddPluginHelpers(
      MustacheTemplateParser parser,
      bool useServerPlugins,
      bool useBasePlugins)
    {
      foreach (IMustacheTemplateHelper registeredPlugin in this.m_registeredPlugins)
        parser.RegisterHelper(registeredPlugin.Name, new MustacheTemplateHelperMethod(registeredPlugin.Evaluate));
      foreach (IServerMustacheTemplateHelper registeredServerPlugin in this.m_registeredServerPlugins)
      {
        IServerMustacheTemplateHelper plugin = registeredServerPlugin;
        parser.RegisterHelper(plugin.Name, (MustacheTemplateHelperWriter) ((expression, writer, context) =>
        {
          IVssRequestContext requestContext = (IVssRequestContext) null;
          MustacheEvaluationContext evaluationContext = context;
          while (evaluationContext.ParentContext != null)
            evaluationContext = evaluationContext.ParentContext;
          object obj;
          if (evaluationContext.AdditionalEvaluationData != null && evaluationContext.AdditionalEvaluationData.TryGetValue("requestContext", out obj))
            requestContext = obj as IVssRequestContext;
          using (PerformanceTimer.StartMeasure(requestContext, "TemplatePlugin", plugin.Name))
            plugin.Evaluate(requestContext, expression, writer, context);
        }));
      }
    }
  }
}
