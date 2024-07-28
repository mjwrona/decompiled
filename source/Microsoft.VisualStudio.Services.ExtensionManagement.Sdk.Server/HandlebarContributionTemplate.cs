// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.HandlebarContributionTemplate
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.TextTemplating;
using System.IO;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class HandlebarContributionTemplate : IContributionTemplate
  {
    private IServerMustacheTemplateExpression m_compiledExpression;

    public string TemplateName { get; }

    public string ContentType { get; }

    public HandlebarContributionTemplate(
      IVssRequestContext requestContext,
      string templateName,
      string contentType,
      StreamReader templateStream)
    {
      this.TemplateName = templateName;
      this.ContentType = contentType;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_compiledExpression = vssRequestContext.GetService<IMustacheTemplateParserService>().ParseServerTemplate(vssRequestContext, templateStream.ReadToEnd());
    }

    public void Render(
      IVssRequestContext requestContext,
      TextWriter textWriter,
      object templateContext = null)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "ContributionTemplate.Render", this.TemplateName))
      {
        this.m_compiledExpression.Evaluate(requestContext, textWriter, templateContext);
        textWriter.Flush();
      }
    }
  }
}
