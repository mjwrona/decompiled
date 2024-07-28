// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TextTemplating.ServerMustacheTemplateUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.TextTemplating
{
  public static class ServerMustacheTemplateUtil
  {
    public static void EvaluateServerExpression(
      IVssRequestContext requestContext,
      MustacheExpression expression,
      TextWriter writer,
      object replacementObject,
      MustacheOptions options = null,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      if (additionalEvaluationData == null)
        additionalEvaluationData = new Dictionary<string, object>();
      additionalEvaluationData[nameof (requestContext)] = (object) requestContext;
      expression.Evaluate(writer, replacementObject, options, additionalEvaluationData);
    }
  }
}
