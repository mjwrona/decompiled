// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TemplateFields
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class TemplateFields
  {
    public JObject ContextObject { get; set; }

    public List<TemplateFieldReplacement> Replacements { get; set; }

    public JObject GetObjectWithReplacements(
      object replacementContext,
      Dictionary<string, object> additionalEvaluationData,
      MustacheOptions options)
    {
      JObject withReplacements = this.ContextObject;
      if (this.Replacements != null && this.Replacements.Count > 0)
      {
        withReplacements = new JObject(this.ContextObject);
        foreach (TemplateFieldReplacement replacement in this.Replacements)
        {
          JToken jtoken = !string.IsNullOrEmpty(replacement.ParentSelector) ? withReplacements.SelectToken(replacement.ParentSelector) : (JToken) withReplacements;
          if (jtoken != null)
          {
            string str = replacement.Expression.Evaluate(replacementContext, additionalEvaluationData, (MustacheEvaluationContext) null, (Dictionary<string, MustacheRootExpression>) null, options);
            if (!string.IsNullOrEmpty(replacement.Key) && jtoken is JObject)
              ((JObject) jtoken)[replacement.Key] = (JToken) str;
            else if (replacement.Index >= 0 && jtoken is JArray)
              ((JArray) jtoken)[replacement.Index] = (JToken) str;
          }
        }
      }
      return withReplacements;
    }
  }
}
