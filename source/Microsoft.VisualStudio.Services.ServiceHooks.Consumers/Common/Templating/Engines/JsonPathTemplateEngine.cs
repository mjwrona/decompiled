// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines.JsonPathTemplateEngine
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines
{
  public class JsonPathTemplateEngine : ITemplateEngine
  {
    private const string c_expressionGroupName = "expression";
    private static readonly Regex s_expressionRegex = new Regex("{{\\s*(?<expression>[\\w$\\.@\\[\\]\\*\\:\\(\\)'\"]+)\\s*}}", RegexOptions.Compiled);

    public string ApplyTemplate(
      string template,
      JObject model,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      if (model == null)
        throw new ArgumentNullException(nameof (model));
      return string.IsNullOrWhiteSpace(template) ? template : JsonPathTemplateEngine.s_expressionRegex.Replace(template, (MatchEvaluator) (match =>
      {
        string path = match.Groups["expression"].Value;
        try
        {
          JToken jtoken = model.SelectToken(path, false);
          return jtoken == null ? string.Empty : jtoken.ToString();
        }
        catch (JsonException ex)
        {
          return match.Value;
        }
      }));
    }
  }
}
