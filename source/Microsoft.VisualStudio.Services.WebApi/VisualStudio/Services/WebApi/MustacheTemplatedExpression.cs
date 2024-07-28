// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheTemplatedExpression
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class MustacheTemplatedExpression : MustacheAggregateExpression
  {
    private IList<object> m_helperArguments;

    internal MustacheRootExpression RootExpression { get; set; }

    public string Expression { get; protected set; }

    public string HelperName { get; set; }

    public IList<object> HelperArguments
    {
      get
      {
        if (this.m_helperArguments == null)
          this.m_helperArguments = this.ParseArgumentExpressions();
        return this.m_helperArguments;
      }
    }

    public bool Encode { get; set; }

    public bool IsBlockExpression { get; set; }

    public bool IsNegativeExpression { get; set; }

    public bool IsElseBlock { get; set; }

    public MustacheTemplatedExpression ElseSourceExpression { get; set; }

    public bool IsComment { get; set; }

    public MustacheTemplatedExpression(
      string expression,
      MustacheAggregateExpression parentExpression,
      MustacheRootExpression rootExpression,
      bool isBlockExpression = false,
      bool isInvertedExpression = false,
      bool isElseBlock = false,
      bool encode = false)
    {
      this.RootExpression = rootExpression;
      this.ParentExpression = parentExpression;
      this.Expression = expression;
      this.IsBlockExpression = isBlockExpression;
      this.IsNegativeExpression = isInvertedExpression;
      this.IsElseBlock = isElseBlock;
      this.Encode = encode;
      if (string.IsNullOrEmpty(expression))
        return;
      if (!this.IsBlockExpression && expression.StartsWith("!"))
      {
        this.IsComment = true;
      }
      else
      {
        int length = expression.IndexOf(' ');
        if (length >= 0)
        {
          this.HelperName = expression.Substring(0, length);
          this.Expression = expression.Substring(length + 1);
          if (this.RootExpression.TemplateHelpers == null || !this.RootExpression.TemplateHelpers.ContainsKey(this.HelperName))
            throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateMissingHelper((object) this.HelperName, (object) expression));
        }
        else
        {
          if (expression.StartsWith(".") || expression.StartsWith("this/") || expression.StartsWith("this.") || this.RootExpression.TemplateHelpers == null || !this.RootExpression.TemplateHelpers.ContainsKey(expression))
            return;
          this.HelperName = expression;
          this.Expression = string.Empty;
        }
      }
    }

    public override bool IsContextBased => true;

    public object GetCurrentToken(string selector, MustacheEvaluationContext context) => (object) this.GetCurrentJToken(selector, context);

    public bool IsTokenTruthy(JToken token)
    {
      if (token == null || token.Type == JTokenType.Null)
        return false;
      switch (token.Type)
      {
        case JTokenType.Object:
          return token.Value<object>() != null;
        case JTokenType.Array:
          return token.AsJEnumerable().Any<JToken>();
        case JTokenType.Integer:
        case JTokenType.Float:
          return token.Value<double>() != 0.0;
        case JTokenType.String:
          return !string.IsNullOrEmpty(token.Value<string>());
        case JTokenType.Boolean:
          return token.Value<bool>();
        default:
          return true;
      }
    }

    internal override void Evaluate(MustacheEvaluationContext context, MustacheTextWriter writer)
    {
      int resultLength = writer.ResultLength;
      this.EvaluateInternal(context, writer);
      context.StoreExpressionTruthiness(this, writer.ResultLength > resultLength);
    }

    private void EvaluateInternal(MustacheEvaluationContext context, MustacheTextWriter writer)
    {
      if (this.IsComment)
        return;
      if (this.ElseSourceExpression != null)
      {
        bool? nullable = context.WasExpressionEvaluatedAsTruthy(this.ElseSourceExpression);
        if (!nullable.HasValue)
          nullable = new bool?(!string.IsNullOrEmpty(this.ElseSourceExpression.Evaluate((object) context)));
        if (nullable.Value)
          return;
        base.Evaluate(context, writer);
      }
      else if (!string.IsNullOrEmpty(this.HelperName))
      {
        MustacheTemplateHelperWriter templateHelperWriter = (MustacheTemplateHelperWriter) null;
        if (this.RootExpression.TemplateHelpers != null)
          this.RootExpression.TemplateHelpers.TryGetValue(this.HelperName, out templateHelperWriter);
        if (templateHelperWriter == null)
          return;
        templateHelperWriter(this, writer, context);
      }
      else
      {
        JToken currentJtoken = this.GetCurrentJToken(this.Expression, context);
        bool flag = this.IsTokenTruthy(currentJtoken);
        if (this.IsNegativeExpression)
        {
          if (flag)
            return;
          base.Evaluate(context, writer);
        }
        else if (!this.IsBlockExpression)
        {
          if (currentJtoken == null || currentJtoken.Type == JTokenType.Null)
            return;
          string str = currentJtoken.ToString();
          writer.Write(str, this.Encode);
        }
        else
        {
          if (!flag)
            return;
          if (currentJtoken.Type == JTokenType.Array)
          {
            MustacheParsingUtil.EvaluateJToken(writer, currentJtoken as JArray, context, this);
          }
          else
          {
            if (currentJtoken.Type == JTokenType.Object)
            {
              context = new MustacheEvaluationContext()
              {
                ReplacementObject = (object) currentJtoken,
                ParentContext = context,
                PartialExpressions = context.PartialExpressions,
                AdditionalEvaluationData = context.AdditionalEvaluationData,
                Options = context.Options
              };
              MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) this);
            }
            base.Evaluate(context, writer);
          }
        }
      }
    }

    private IList<object> ParseArgumentExpressions()
    {
      List<object> argumentExpressions = new List<object>();
      if (!string.IsNullOrEmpty(this.Expression))
      {
        for (int index = 0; index < this.Expression.Length; ++index)
        {
          char c1 = this.Expression[index];
          switch (c1)
          {
            case '"':
            case '\'':
              StringBuilder stringBuilder1 = new StringBuilder();
              ++index;
              while (index < this.Expression.Length)
              {
                char ch1 = this.Expression[index];
                if ((int) ch1 != (int) c1)
                {
                  if (ch1 == '\\')
                  {
                    ++index;
                    char ch2 = MustacheParsingUtil.SafeCharAt(this.Expression, index);
                    switch (ch2)
                    {
                      case '"':
                      case '\'':
                      case '\\':
                        stringBuilder1.Append(ch2);
                        break;
                      default:
                        throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInvalidEscapedStringLiteral((object) stringBuilder1.ToString(), (object) this.Expression));
                    }
                  }
                  else
                    stringBuilder1.Append(ch1);
                  ++index;
                  if (index == this.Expression.Length)
                    throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateUnterminatedStringLiteral((object) (c1.ToString() + stringBuilder1.ToString()), (object) this.Expression));
                }
                else
                  break;
              }
              argumentExpressions.Add((object) stringBuilder1.ToString());
              break;
            default:
              if (char.IsDigit(c1) || c1 == '-')
              {
                StringBuilder stringBuilder2 = new StringBuilder().Append(c1);
                for (++index; index < this.Expression.Length; ++index)
                {
                  char c2 = this.Expression[index];
                  if (!char.IsDigit(c2))
                  {
                    switch (c2)
                    {
                      case ' ':
                        goto label_21;
                      case '.':
                        break;
                      default:
                        index -= stringBuilder2.Length;
                        stringBuilder2 = (StringBuilder) null;
                        goto label_21;
                    }
                  }
                  stringBuilder2.Append(c2);
                }
label_21:
                if (stringBuilder2 != null && !stringBuilder2.ToString().Equals("-"))
                {
                  string s = stringBuilder2.ToString();
                  try
                  {
                    if (s.IndexOf('.') > -1)
                    {
                      argumentExpressions.Add((object) double.Parse(s));
                      break;
                    }
                    argumentExpressions.Add((object) int.Parse(s));
                    break;
                  }
                  catch (Exception ex)
                  {
                    throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInvalidNumericLiteral((object) s, (object) this.Expression));
                  }
                }
              }
              else
              {
                if (MustacheParsingUtil.IsConstantAt(this.Expression, index, "null"))
                {
                  argumentExpressions.Add((object) null);
                  index += 4;
                  break;
                }
                if (MustacheParsingUtil.IsConstantAt(this.Expression, index, bool.TrueString))
                {
                  argumentExpressions.Add((object) true);
                  index += bool.TrueString.Length;
                  break;
                }
                if (MustacheParsingUtil.IsConstantAt(this.Expression, index, bool.FalseString))
                {
                  argumentExpressions.Add((object) false);
                  index += bool.FalseString.Length;
                  break;
                }
                if (c1 == ' ')
                  break;
              }
              StringBuilder stringBuilder3 = new StringBuilder().Append(c1);
              for (++index; index < this.Expression.Length; ++index)
              {
                char ch = this.Expression[index];
                if (ch != ' ')
                  stringBuilder3.Append(ch);
                else
                  break;
              }
              MustacheTemplatedExpression templatedExpression = new MustacheTemplatedExpression(stringBuilder3.ToString(), this.ParentExpression, this.RootExpression);
              argumentExpressions.Add((object) templatedExpression);
              break;
          }
        }
      }
      return (IList<object>) argumentExpressions;
    }

    public string GetRawHelperArgument(int index)
    {
      IList<object> helperArguments = this.HelperArguments;
      if (helperArguments.Count > index)
      {
        object obj = helperArguments[index];
        if (obj != null)
          return obj is MustacheTemplatedExpression templatedExpression ? templatedExpression.Expression : obj.ToString();
      }
      return (string) null;
    }

    public T GetHelperArgument<T>(MustacheEvaluationContext context, int index, T defaultValue = null)
    {
      helperArgument = defaultValue;
      IList<object> helperArguments = this.HelperArguments;
      if (helperArguments != null && index >= 0 && index < helperArguments.Count)
      {
        object obj = helperArguments[index];
        switch (obj)
        {
          case null:
          case T helperArgument:
            break;
          case MustacheTemplatedExpression templatedExpression:
            try
            {
              JToken currentJtoken = this.GetCurrentJToken(templatedExpression.Expression, context);
              if (currentJtoken != null)
              {
                helperArgument = currentJtoken.Value<T>();
                break;
              }
              break;
            }
            catch (Exception ex)
            {
              break;
            }
          default:
            try
            {
              TypeConverter converter1 = TypeDescriptor.GetConverter(typeof (T));
              if (converter1.CanConvertFrom(obj.GetType()))
              {
                helperArgument = (T) converter1.ConvertFrom(obj);
                break;
              }
              TypeConverter converter2 = TypeDescriptor.GetConverter(obj.GetType());
              if (converter2.CanConvertTo(typeof (T)))
              {
                helperArgument = (T) converter2.ConvertTo(obj, typeof (T));
                break;
              }
              break;
            }
            catch (Exception ex)
            {
              break;
            }
        }
      }
      return helperArgument;
    }

    internal static bool IsHelperArgumentTruthy(MustacheEvaluationContext context, object argValue)
    {
      switch (argValue)
      {
        case MustacheTemplatedExpression templatedExpression:
          JToken currentJtoken = templatedExpression.GetCurrentJToken(templatedExpression.Expression, context);
          return templatedExpression.IsTokenTruthy(currentJtoken);
        case double num1:
          return num1 != 0.0;
        case int num2:
          return num2 != 0;
        case bool flag:
          return flag;
        case string str:
          return !string.IsNullOrEmpty(str);
        default:
          return false;
      }
    }
  }
}
