// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.ResponseSelectorFactory
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public static class ResponseSelectorFactory
  {
    private const string XPath = "xpath:";
    private const string JsonPath = "jsonpath:";
    private const string None = "none";
    private const string PlainText = "plaintext";

    public static ResponseSelector GetResponseSelector(
      string selector,
      string keySelector,
      string resultTemplate,
      JToken replacementContext,
      string callbackContextTemplate,
      string callbackRequiredTemplate)
    {
      if (selector.ToLower().StartsWith("xpath:", StringComparison.OrdinalIgnoreCase))
        return (ResponseSelector) new XPathResponseSelector(selector.Substring("xpath:".Length), resultTemplate, replacementContext);
      if (selector.ToLower().StartsWith("jsonpath:", StringComparison.OrdinalIgnoreCase))
      {
        string keySelector1 = (string) null;
        if (!string.IsNullOrWhiteSpace(keySelector))
          keySelector1 = keySelector.Substring("jsonpath:".Length);
        return (ResponseSelector) new JsonPathResponseSelector(selector.Substring("jsonpath:".Length), keySelector1, resultTemplate, callbackContextTemplate, callbackRequiredTemplate, replacementContext);
      }
      if (selector.ToLower().Equals("none", StringComparison.OrdinalIgnoreCase))
        return (ResponseSelector) new NoneResponseSelector();
      if (selector.ToLower().Equals("plaintext", StringComparison.OrdinalIgnoreCase))
        return (ResponseSelector) new PlainTextResponseSelector();
      throw new NotSupportedException(Resources.InvalidSelectorType((object) selector));
    }
  }
}
