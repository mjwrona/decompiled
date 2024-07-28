// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines.WorkItemMustacheTemplateHelpers
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines
{
  public static class WorkItemMustacheTemplateHelpers
  {
    public static string c_emailEscapeMustacheHelper = "emailEscape";
    public static string c_emailEscapeForSubjectMustacheHelper = "emailEscapeForSubject";
    public static string c_excludeFieldsFromWorkItemEventHelper = "excludeFieldsFromWorkItemEvent";
    internal static readonly string[] ExcludedFields = new string[8]
    {
      "System.Rev",
      "System.AuthorizedDate",
      "System.RevisedDate",
      "System.Watermark",
      "System.IterationId",
      "System.AreaId",
      "Microsoft.VSTS.Common.StateChangeDate",
      "System.PersonId"
    };

    public static string ExcludeFieldsFromWorktItemEventStringHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (string.IsNullOrEmpty(expression.Expression))
        return string.Empty;
      object currentToken = expression.GetCurrentToken(expression.Expression, context);
      if (!(currentToken is JToken jtoken1))
        jtoken1 = JToken.FromObject(currentToken);
      JArray jarray = new JArray();
      JEnumerable<JToken> fieldList = jtoken1.Children();
      foreach (JToken jtoken2 in fieldList)
      {
        JProperty jproperty = jtoken2 as JProperty;
        string name = jproperty.Name;
        JToken jtoken3 = jproperty.Value;
        if (!((IEnumerable<string>) WorkItemMustacheTemplateHelpers.ExcludedFields).Contains<string>(name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        {
          JObject jobject = WorkItemMustacheTemplateHelpers.ConstructFieldsOutput(name, jtoken3, fieldList);
          if (jobject != null)
            jarray.Add((JToken) jobject);
        }
      }
      return jarray.ToString();
    }

    private static JObject ConstructFieldsOutput(
      string name,
      JToken value,
      JEnumerable<JToken> fieldList)
    {
      if (name.IndexOf("DisplayName") > -1)
        return (JObject) null;
      JObject jobject = new JObject();
      string str = "";
      if (value.Type != JTokenType.Object)
      {
        foreach (JToken field in fieldList)
        {
          if (field is JProperty jproperty && string.Equals(jproperty.Name, name + ".DisplayName", StringComparison.OrdinalIgnoreCase))
            jobject.Add(nameof (name), jproperty.Value);
        }
        if (jobject[nameof (name)] == null)
          jobject.Add(nameof (name), (JToken) name);
        str = WorkItemMustacheTemplateHelpers.EmailEscaperCommonHelper(value.ToString());
      }
      else
      {
        if (value[(object) "displayName"] != null)
          jobject.Add(nameof (name), value[(object) "displayName"]);
        else
          jobject.Add(nameof (name), (JToken) name);
        if (value[(object) "newValue"] != null)
          str = WorkItemMustacheTemplateHelpers.EmailEscaperCommonHelper(value[(object) "newValue"].ToString());
        if (value[(object) "oldValue"] != null && value[(object) "oldValue"].Type != JTokenType.Date)
          str = str + ", *(" + WorkItemMustacheTemplateHelpers.EmailEscaperCommonHelper(value[(object) "oldValue"].ToString()) + ")*";
      }
      jobject.Add(nameof (value), (JToken) str);
      return jobject;
    }

    public static string EmailEscapeForSubjectHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string str1 = (string) null;
      if (!string.IsNullOrEmpty(expression.Expression) && expression.GetCurrentToken(expression.Expression, context) is JToken currentToken)
      {
        string str2 = currentToken.ToString();
        Match match = WorkItemMustacheTemplateHelpers.EmailRegexMatchingHelper(str2.ToString());
        if (match == null)
          str1 = string.Empty;
        else if (match.Success)
        {
          int length = str2.LastIndexOf(' ');
          str1 = str2.Substring(0, length);
        }
        else
          str1 = str2;
      }
      return !string.IsNullOrEmpty(str1) ? MustacheTemplateEngine.JsonEscaper(str1) : string.Empty;
    }

    public static string EmailEscapeDataStringHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string str = (string) null;
      if (!string.IsNullOrEmpty(expression.Expression) && expression.GetCurrentToken(expression.Expression, context) is JToken currentToken)
        str = WorkItemMustacheTemplateHelpers.EmailEscaperCommonHelper(currentToken.ToString());
      return !string.IsNullOrEmpty(str) ? MustacheTemplateEngine.JsonEscaper(str) : string.Empty;
    }

    private static string EmailEscaperCommonHelper(string expressionValue)
    {
      if (expressionValue == null)
        return string.Empty;
      Match match = WorkItemMustacheTemplateHelpers.EmailRegexMatchingHelper(expressionValue);
      if (match == null)
        return string.Empty;
      if (!match.Success)
        return expressionValue;
      int length = expressionValue.LastIndexOf(' ');
      return "[" + expressionValue.Substring(0, length) + "](mailto:" + match.Value + ")";
    }

    private static Match EmailRegexMatchingHelper(string expressionValue)
    {
      if (string.IsNullOrEmpty(expressionValue))
        return (Match) null;
      expressionValue = MustacheTemplateEngine.JsonEscaper(expressionValue);
      return new Regex("([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)", RegexOptions.Compiled).Match(expressionValue);
    }
  }
}
