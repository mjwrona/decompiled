// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines.MustacheTemplateEngine
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.Engines
{
  public class MustacheTemplateEngine : ITemplateEngine
  {
    private const string c_uriEscapeMustacheHelper = "uriEscape";
    private const string c_jsonEscapeMustacheHelper = "jsonEscape";
    private const string c_encodedAvatarMustacheHelper = "encodedAvatar";
    private const string c_joinLinesMustacheHelper = "joinLines";
    private const string c_iconUrlMustacheHelper = "iconUrl";
    private const string c_stringReplaceHelper = "stringReplace";
    private static readonly MustacheTemplateParser s_mustacheTemplateParser = new MustacheTemplateParser();

    static MustacheTemplateEngine()
    {
      MustacheTemplateHelperWriter helper;
      if (CommonMustacheHelpers.GetHelpers().TryGetValue("stringReplace", out helper))
        MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("stringReplace", helper);
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("uriEscape", new MustacheTemplateHelperMethod(MustacheTemplateEngine.UriEscapeDataStringHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("jsonEscape", new MustacheTemplateHelperMethod(MustacheTemplateEngine.JsonEscapeDataStringHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("encodedAvatar", new MustacheTemplateHelperMethod(MustacheTemplateEngine.EncodedAvatarHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("joinLines", new MustacheTemplateHelperMethod(MustacheTemplateEngine.JoinLinesHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper("iconUrl", new MustacheTemplateHelperMethod(NotificationTransformHandlebarHelpers.IconCdnUrlHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper(WorkItemMustacheTemplateHelpers.c_emailEscapeMustacheHelper, new MustacheTemplateHelperMethod(WorkItemMustacheTemplateHelpers.EmailEscapeDataStringHelper));
      MustacheTemplateEngine.s_mustacheTemplateParser.RegisterHelper(WorkItemMustacheTemplateHelpers.c_emailEscapeForSubjectMustacheHelper, new MustacheTemplateHelperMethod(WorkItemMustacheTemplateHelpers.EmailEscapeForSubjectHelper));
    }

    private static string UriEscapeDataStringHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (!string.IsNullOrEmpty(expression.Expression) && expression.GetCurrentToken(expression.Expression, context) is JToken currentToken)
      {
        string stringToEscape = currentToken.ToString();
        if (!string.IsNullOrEmpty(stringToEscape))
          return Uri.EscapeDataString(stringToEscape);
      }
      return string.Empty;
    }

    private static string JsonEscapeDataStringHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string str = expression.GetHelperArgument<string>(context, 0);
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (expression.GetHelperArgument<bool>(context, 1))
        str = str.Replace("\n", "  \n");
      return MustacheTemplateEngine.JsonEscaper(str);
    }

    public static string JsonEscaper(string value)
    {
      string str = JsonConvert.ToString(value);
      return str.Length >= 2 && str[0] == '"' && str[str.Length - 1] == '"' ? str.Substring(1, str.Length - 2) : str;
    }

    private static string EncodedAvatarHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string helperArgument = expression.GetHelperArgument<string>(context, 0);
      string str = (string) null;
      Guid result;
      IVssRequestContext vssRequestContext;
      if (!string.IsNullOrEmpty(helperArgument) && Guid.TryParse(helperArgument, out result) && context.AdditionalEvaluationData != null && context.AdditionalEvaluationData.TryGetValue<IVssRequestContext>("IVssRequestContext", out vssRequestContext))
        str = vssRequestContext.GetService<AvatarService>().GetEncodedAvatar(vssRequestContext, result);
      if (string.IsNullOrEmpty(str))
        str = AvatarService.c_defaultEncodedAvatar;
      return str;
    }

    private static string JoinLinesHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0, ", ");
      int helperArgument2 = expression.GetHelperArgument<int>(context, 1, -1);
      string helperArgument3 = expression.GetHelperArgument<string>(context, 2, "...");
      string helperArgument4 = expression.GetHelperArgument<string>(context, 3);
      string[] array = ((IEnumerable<string>) expression.EvaluateChildExpressions(context).Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (segment => !string.IsNullOrWhiteSpace(segment))).Select<string, string>((Func<string, string>) (segment => segment.Trim())).ToArray<string>();
      string str = string.Empty;
      if (array.Length == 0)
      {
        if (!string.IsNullOrWhiteSpace(helperArgument4))
          str = helperArgument4;
      }
      else if (helperArgument2 >= 0 && array.Length > helperArgument2)
      {
        if (!string.IsNullOrWhiteSpace(helperArgument3))
          str = string.Join(helperArgument1, ((IEnumerable<string>) array).Take<string>(helperArgument2).Concat<string>((IEnumerable<string>) new string[1]
          {
            helperArgument3
          }));
        else
          str = string.Join(helperArgument1, ((IEnumerable<string>) array).Take<string>(helperArgument2));
      }
      else
        str = string.Join(helperArgument1, array);
      return str;
    }

    public string ApplyTemplate(
      string template,
      JObject model,
      Dictionary<string, object> additionalEvaluationData = null)
    {
      if (model == null)
        throw new ArgumentNullException(nameof (model));
      return string.IsNullOrWhiteSpace(template) ? template : MustacheTemplateEngine.s_mustacheTemplateParser.Parse(template).Evaluate((object) model, additionalEvaluationData);
    }
  }
}
