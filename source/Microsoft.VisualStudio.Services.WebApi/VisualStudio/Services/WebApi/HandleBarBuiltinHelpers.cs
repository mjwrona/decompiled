// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HandleBarBuiltinHelpers
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class HandleBarBuiltinHelpers
  {
    internal static Dictionary<string, MustacheTemplateHelperWriter> GetHelpers() => new Dictionary<string, MustacheTemplateHelperWriter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        ">",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C0\u003E__HandlebarPartialHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C0\u003E__HandlebarPartialHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarPartialHelper))
      },
      {
        "with",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C1\u003E__HandlebarBlockWithHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C1\u003E__HandlebarBlockWithHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockWithHelper))
      },
      {
        "if",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C2\u003E__HandlebarBlockIfHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C2\u003E__HandlebarBlockIfHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockIfHelper))
      },
      {
        "else",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C3\u003E__HandlebarBlockUnlessHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C3\u003E__HandlebarBlockUnlessHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockUnlessHelper))
      },
      {
        "unless",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C3\u003E__HandlebarBlockUnlessHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C3\u003E__HandlebarBlockUnlessHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockUnlessHelper))
      },
      {
        "each",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C4\u003E__HandlebarBlockEachHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C4\u003E__HandlebarBlockEachHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockEachHelper))
      },
      {
        "lookup",
        HandleBarBuiltinHelpers.\u003C\u003EO.\u003C5\u003E__HandlebarBlockLookupHelper ?? (HandleBarBuiltinHelpers.\u003C\u003EO.\u003C5\u003E__HandlebarBlockLookupHelper = new MustacheTemplateHelperWriter(HandleBarBuiltinHelpers.HandlebarBlockLookupHelper))
      }
    };

    internal static void HandlebarBlockWithHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      JToken currentJtoken = expression.GetCurrentJToken(expression.Expression, context);
      if (!expression.IsTokenTruthy(currentJtoken))
        return;
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) currentJtoken,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData,
        Options = context.Options
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      expression.EvaluateChildExpressions(context1, writer);
    }

    internal static void HandlebarBlockIfHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      JToken currentJtoken = expression.GetCurrentJToken(expression.Expression, context);
      if (!expression.IsTokenTruthy(currentJtoken))
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
      else if (expression.IsBlockExpression)
        expression.EvaluateChildExpressions(context, writer);
      else
        writer.Write("true");
    }

    internal static void HandlebarBlockUnlessHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      JToken currentJtoken = expression.GetCurrentJToken(expression.Expression, context);
      if (expression.IsTokenTruthy(currentJtoken))
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
      else if (expression.IsBlockExpression)
        expression.EvaluateChildExpressions(context, writer);
      else
        writer.Write("true");
    }

    internal static void HandlebarBlockEachHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      JToken currentJtoken = expression.GetCurrentJToken(expression.Expression, context);
      if (!expression.IsTokenTruthy(currentJtoken))
        return;
      if (currentJtoken.Type == JTokenType.Array)
      {
        MustacheParsingUtil.EvaluateJToken(writer, currentJtoken as JArray, context, expression);
      }
      else
      {
        if (currentJtoken.Type != JTokenType.Object)
          return;
        foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) currentJtoken)
        {
          MustacheEvaluationContext context1 = new MustacheEvaluationContext()
          {
            ReplacementObject = (object) keyValuePair.Value,
            ParentContext = context,
            CurrentKey = keyValuePair.Key,
            PartialExpressions = context.PartialExpressions,
            AdditionalEvaluationData = context.AdditionalEvaluationData,
            Options = context.Options
          };
          MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
          expression.EvaluateChildExpressions(context1, writer);
        }
      }
    }

    internal static void HandlebarBlockLookupHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      if (string.IsNullOrEmpty(expression.Expression))
        return;
      string[] strArray = expression.Expression.Split(' ');
      if (strArray.Length != 2)
        return;
      string currentKey = strArray[1];
      string selector = (string) null;
      if (string.Equals(currentKey, "@index"))
      {
        string str = context.CurrentIndex.ToString();
        selector = string.Format("{0}[{1}]", (object) strArray[0], (object) str);
      }
      else
      {
        if (string.Equals(currentKey, "@key"))
          currentKey = context.CurrentKey;
        if (!string.IsNullOrEmpty(currentKey))
          selector = string.Format("{0}.{1}", (object) strArray[0], (object) currentKey);
      }
      if (string.IsNullOrEmpty(selector))
        return;
      JToken currentJtoken = expression.GetCurrentJToken(selector, context);
      if (currentJtoken == null || currentJtoken.Type == JTokenType.Null)
        return;
      writer.Write(currentJtoken.ToString(), expression.Encode);
    }

    internal static void HandlebarPartialHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string rawHelperArgument1 = expression.GetRawHelperArgument(0);
      if (string.IsNullOrEmpty(rawHelperArgument1))
        throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInvalidPartialReference((object) expression.Expression));
      if (rawHelperArgument1[0].Equals('(') && rawHelperArgument1[rawHelperArgument1.Length - 1].Equals(')'))
      {
        JToken currentJtoken = expression.GetCurrentJToken(rawHelperArgument1.Substring(1, rawHelperArgument1.Length - 2), context);
        if (currentJtoken == null || !currentJtoken.Type.Equals((object) JTokenType.String))
          return;
        rawHelperArgument1 = currentJtoken.ToString();
      }
      MustacheRootExpression expression1;
      context.PartialExpressions.TryGetValue(rawHelperArgument1, out expression1);
      if (expression1 == null)
        return;
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression1);
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ReplacementObject = context.ReplacementObject,
        ParentContext = context.ParentContext,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData,
        Options = context.Options
      };
      string rawHelperArgument2 = expression.GetRawHelperArgument(1);
      if (rawHelperArgument2 != null)
      {
        context1.ReplacementObject = (object) expression.GetCurrentJToken(rawHelperArgument2, context);
        context1.ParentContext = context;
        context1.PartialExpressions = context.PartialExpressions;
      }
      expression1.Evaluate(context1, writer);
    }
  }
}
