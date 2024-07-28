// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.CommonMustacheHelpers
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class CommonMustacheHelpers
  {
    public static Dictionary<string, MustacheTemplateHelperWriter> GetHelpers() => new Dictionary<string, MustacheTemplateHelperWriter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "equals",
        CommonMustacheHelpers.\u003C\u003EO.\u003C0\u003E__EqualsHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C0\u003E__EqualsHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.EqualsHelper))
      },
      {
        "notEquals",
        CommonMustacheHelpers.\u003C\u003EO.\u003C1\u003E__NotEqualsHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C1\u003E__NotEqualsHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.NotEqualsHelper))
      },
      {
        "contains",
        CommonMustacheHelpers.\u003C\u003EO.\u003C2\u003E__StringContainsHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C2\u003E__StringContainsHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringContainsHelper))
      },
      {
        "stringLeft",
        CommonMustacheHelpers.\u003C\u003EO.\u003C3\u003E__StringLeftHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C3\u003E__StringLeftHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringLeftHelper))
      },
      {
        "stringRight",
        CommonMustacheHelpers.\u003C\u003EO.\u003C4\u003E__StringRightHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C4\u003E__StringRightHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringRightHelper))
      },
      {
        "arrayLength",
        CommonMustacheHelpers.\u003C\u003EO.\u003C5\u003E__ArrayLengthHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C5\u003E__ArrayLengthHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.ArrayLengthHelper))
      },
      {
        "date",
        CommonMustacheHelpers.\u003C\u003EO.\u003C6\u003E__DateHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C6\u003E__DateHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.DateHelper))
      },
      {
        "stringFormat",
        CommonMustacheHelpers.\u003C\u003EO.\u003C7\u003E__StringFormatHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C7\u003E__StringFormatHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringFormatHelper))
      },
      {
        "stringPadLeft",
        CommonMustacheHelpers.\u003C\u003EO.\u003C8\u003E__StringPadHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C8\u003E__StringPadHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringPadHelper))
      },
      {
        "stringPadRight",
        CommonMustacheHelpers.\u003C\u003EO.\u003C8\u003E__StringPadHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C8\u003E__StringPadHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringPadHelper))
      },
      {
        "stringReplace",
        CommonMustacheHelpers.\u003C\u003EO.\u003C9\u003E__StringReplaceHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C9\u003E__StringReplaceHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringReplaceHelper))
      },
      {
        "stringLower",
        CommonMustacheHelpers.\u003C\u003EO.\u003C10\u003E__StringLowerHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C10\u003E__StringLowerHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringLowerHelper))
      },
      {
        "and",
        CommonMustacheHelpers.\u003C\u003EO.\u003C11\u003E__LogicalAndHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C11\u003E__LogicalAndHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.LogicalAndHelper))
      },
      {
        "or",
        CommonMustacheHelpers.\u003C\u003EO.\u003C12\u003E__LogicalOrHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C12\u003E__LogicalOrHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.LogicalOrHelper))
      },
      {
        "stringContains",
        CommonMustacheHelpers.\u003C\u003EO.\u003C2\u003E__StringContainsHelper ?? (CommonMustacheHelpers.\u003C\u003EO.\u003C2\u003E__StringContainsHelper = new MustacheTemplateHelperWriter(CommonMustacheHelpers.StringContainsHelper))
      }
    };

    public static void EqualsHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      string helperArgument2 = expression.GetHelperArgument<string>(context, 1);
      bool helperArgument3 = expression.GetHelperArgument<bool>(context, 2);
      string b = helperArgument2;
      int comparisonType = helperArgument3 ? 5 : 4;
      if (string.Equals(helperArgument1, b, (StringComparison) comparisonType))
      {
        if (expression.IsBlockExpression)
          expression.EvaluateChildExpressions(context, writer);
        else
          writer.Write("true");
      }
      else
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
    }

    public static void NotEqualsHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      string helperArgument2 = expression.GetHelperArgument<string>(context, 1);
      bool helperArgument3 = expression.GetHelperArgument<bool>(context, 2);
      string b = helperArgument2;
      int comparisonType = helperArgument3 ? 5 : 4;
      if (string.Equals(helperArgument1, b, (StringComparison) comparisonType))
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

    public static void StringContainsHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0, string.Empty);
      string helperArgument2 = expression.GetHelperArgument<string>(context, 1, string.Empty);
      if (helperArgument1 != null && helperArgument2 != null)
      {
        StringComparison comparisonType = expression.GetHelperArgument<bool>(context, 2) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (helperArgument1.IndexOf(helperArgument2, comparisonType) >= 0)
        {
          if (expression.IsBlockExpression)
          {
            expression.EvaluateChildExpressions(context, writer);
            return;
          }
          writer.Write("true");
          return;
        }
      }
      if (expression.IsBlockExpression)
        return;
      writer.Write("false");
    }

    public static void StringLeftHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      if (expression == null || context == null)
        return;
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      int helperArgument2 = expression.GetHelperArgument<int>(context, 1);
      if (helperArgument2 <= 0)
        return;
      string str = helperArgument1;
      if (helperArgument1.Length > helperArgument2)
      {
        string helperArgument3 = expression.GetHelperArgument<string>(context, 2);
        str = helperArgument1.Substring(0, helperArgument2) + (helperArgument3 ?? string.Empty);
      }
      writer.Write(str, expression.Encode);
    }

    public static void StringRightHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      if (expression == null || context == null)
        return;
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      int helperArgument2 = expression.GetHelperArgument<int>(context, 1);
      if (helperArgument2 <= 0)
        return;
      string str = helperArgument1;
      if (helperArgument1.Length > helperArgument2)
        str = helperArgument1.Substring(helperArgument1.Length - helperArgument2, helperArgument2);
      writer.Write(str, expression.Encode);
    }

    public static void ArrayLengthHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      try
      {
        if (expression != null)
        {
          if (context != null)
          {
            string rawHelperArgument = expression.GetRawHelperArgument(0);
            if (!string.IsNullOrEmpty(rawHelperArgument))
            {
              JToken currentJtoken = expression.GetCurrentJToken(rawHelperArgument, context);
              if (currentJtoken != null)
              {
                writer.Write(currentJtoken.Children().Count<JToken>().ToString());
                return;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      writer.Write("0");
    }

    public static void DateHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      try
      {
        string helperArgument = expression.GetHelperArgument<string>(context, 0);
        if (string.IsNullOrEmpty(helperArgument))
          return;
        DateTime dateTime = DateTime.Parse(helperArgument);
        writer.Write(dateTime.ToString("d"));
      }
      catch (Exception ex)
      {
      }
    }

    public static void StringPadHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string str;
      int helperArgument;
      char paddingChar;
      if (expression.IsBlockExpression)
      {
        str = expression.EvaluateChildExpressions(context);
        helperArgument = expression.GetHelperArgument<int>(context, 0);
        paddingChar = expression.GetHelperArgument<string>(context, 1, " ")[0];
      }
      else
      {
        str = expression.GetHelperArgument<string>(context, 0);
        helperArgument = expression.GetHelperArgument<int>(context, 1);
        paddingChar = expression.GetHelperArgument<string>(context, 2, " ")[0];
      }
      if (string.Equals(expression.HelperName, "stringPadLeft", StringComparison.OrdinalIgnoreCase))
        writer.Write(str.PadLeft(helperArgument, paddingChar));
      else
        writer.Write(str.PadRight(helperArgument, paddingChar));
    }

    public static void StringReplaceHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string str;
      string helperArgument1;
      string helperArgument2;
      if (expression.IsBlockExpression)
      {
        str = expression.EvaluateChildExpressions(context);
        helperArgument1 = expression.GetHelperArgument<string>(context, 0);
        helperArgument2 = expression.GetHelperArgument<string>(context, 1);
      }
      else
      {
        str = expression.GetHelperArgument<string>(context, 0);
        helperArgument1 = expression.GetHelperArgument<string>(context, 1);
        helperArgument2 = expression.GetHelperArgument<string>(context, 2);
      }
      if (!string.IsNullOrWhiteSpace(str))
        writer.Write(str.Replace(helperArgument1, helperArgument2));
      else
        writer.Write("");
    }

    public static void StringLowerHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      if (expression == null || context == null)
        return;
      string helperArgument = expression.GetHelperArgument<string>(context, 0);
      writer.Write(helperArgument.ToLower());
    }

    public static void StringFormatHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      try
      {
        if (expression == null || context == null)
          return;
        int count = expression.HelperArguments.Count;
        if (count <= 0)
          return;
        string helperArgument = expression.GetHelperArgument<string>(context, 0);
        if (count == 1)
        {
          writer.Write(helperArgument, expression.Encode);
        }
        else
        {
          string[] strArray = new string[count - 1];
          for (int index = 1; index < count; ++index)
            strArray[index - 1] = expression.GetHelperArgument<string>(context, index);
          string str = string.Format(helperArgument, (object[]) strArray);
          writer.Write(str, expression.Encode);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void LogicalAndHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      CommonMustacheHelpers.WriteTruthyOutput(expression, writer, context, expression.HelperArguments != null && expression.HelperArguments.Count > 0 && expression.HelperArguments.All<object>((Func<object, bool>) (a => MustacheTemplatedExpression.IsHelperArgumentTruthy(context, a))));
    }

    public static void LogicalOrHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      CommonMustacheHelpers.WriteTruthyOutput(expression, writer, context, expression.HelperArguments != null && expression.HelperArguments.Count > 0 && expression.HelperArguments.Any<object>((Func<object, bool>) (a => MustacheTemplatedExpression.IsHelperArgumentTruthy(context, a))));
    }

    private static void WriteTruthyOutput(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context,
      bool isTruthy)
    {
      if (isTruthy)
      {
        if (expression.IsBlockExpression)
          expression.EvaluateChildExpressions(context, writer);
        else
          writer.Write("true");
      }
      else
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
    }
  }
}
