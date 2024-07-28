// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TextTemplating.ServerMustacheTemplateExpression
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.TextTemplating
{
  internal class ServerMustacheTemplateExpression : IServerMustacheTemplateExpression
  {
    private MustacheExpression m_expression;

    internal ServerMustacheTemplateExpression(MustacheExpression expression) => this.m_expression = expression;

    public void Evaluate(
      IVssRequestContext requestContext,
      TextWriter writer,
      object replacementObject,
      MustacheOptions options = null,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      ServerMustacheTemplateUtil.EvaluateServerExpression(requestContext, this.m_expression, writer, replacementObject, options, additionalEvaluationData);
    }
  }
}
