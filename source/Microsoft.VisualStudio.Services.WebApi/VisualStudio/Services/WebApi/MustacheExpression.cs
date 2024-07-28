// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheExpression
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class MustacheExpression
  {
    public abstract bool IsContextBased { get; }

    public Dictionary<string, MustacheRootExpression> PartialExpressions { get; set; }

    internal abstract void Evaluate(MustacheEvaluationContext context, MustacheTextWriter writer);

    public string Evaluate(
      object replacementObject,
      Dictionary<string, object> additionalEvaluationData = null,
      MustacheEvaluationContext parentContext = null,
      Dictionary<string, MustacheRootExpression> partialExpressions = null)
    {
      return this.Evaluate(replacementObject, additionalEvaluationData, parentContext, partialExpressions, (MustacheOptions) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Evaluate(
      object replacementObject,
      Dictionary<string, object> additionalEvaluationData,
      MustacheEvaluationContext parentContext,
      Dictionary<string, MustacheRootExpression> partialExpressions,
      MustacheOptions options)
    {
      using (StringWriter writer = new StringWriter())
      {
        this.Evaluate((TextWriter) writer, replacementObject, options, additionalEvaluationData, parentContext, partialExpressions);
        return writer.ToString();
      }
    }

    public void Evaluate(
      TextWriter writer,
      object replacementObject,
      MustacheOptions options = null,
      Dictionary<string, object> additionalEvaluationData = null,
      MustacheEvaluationContext parentContext = null,
      Dictionary<string, MustacheRootExpression> partialExpressions = null)
    {
      if (options == null)
        options = new MustacheOptions();
      MustacheTextWriter writer1 = new MustacheTextWriter(writer, options);
      MustacheEvaluationContext context = new MustacheEvaluationContext()
      {
        ReplacementObject = replacementObject,
        PartialExpressions = this.PartialExpressions,
        AdditionalEvaluationData = additionalEvaluationData,
        ParentContext = parentContext,
        Options = options
      };
      for (MustacheEvaluationContext evaluationContext = parentContext; evaluationContext != null; evaluationContext = evaluationContext.ParentContext)
        evaluationContext.Options = options;
      if (partialExpressions != null)
        context.PartialExpressions = partialExpressions;
      context.AssertCancellation();
      this.Evaluate(context, writer1);
    }

    internal static MustacheRootExpression Parse(
      string template,
      Dictionary<string, MustacheTemplateHelperWriter> helpers,
      Dictionary<string, MustacheRootExpression> partials,
      MustacheOptions options,
      int depth = 0)
    {
      if (options == null)
        options = new MustacheOptions();
      MustacheRootExpression mustacheRootExpression1 = new MustacheRootExpression();
      mustacheRootExpression1.TemplateHelpers = helpers;
      mustacheRootExpression1.PartialExpressions = partials == null ? new Dictionary<string, MustacheRootExpression>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : partials;
      MustacheRootExpression rootExpression = mustacheRootExpression1;
      MustacheAggregateExpression aggregateExpression = (MustacheAggregateExpression) rootExpression;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      bool encode = false;
      bool flag2 = false;
      for (int index = 0; index < template.Length; ++index)
      {
        char ch = template[index];
        if (ch == '\\' && MustacheParsingUtil.SafeCharAt(template, index + 1) == '{' && MustacheParsingUtil.SafeCharAt(template, index + 2) == '{' && !flag2)
        {
          stringBuilder.Append("{{");
          index += 2;
        }
        else if (ch == '\\' && MustacheParsingUtil.SafeCharAt(template, index + 1) == '}' && MustacheParsingUtil.SafeCharAt(template, index + 2) == '}' && !flag2)
        {
          stringBuilder.Append("}}");
          index += 2;
        }
        else if (ch == '{' && MustacheParsingUtil.SafeCharAt(template, index + 1) == '{' && !flag2 && MustacheParsingUtil.SafeCharAt(template, index - 1) != '\\')
        {
          if (flag1)
            throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInvalidStartBraces((object) stringBuilder.ToString(), (object) index, (object) template));
          if (stringBuilder.Length > 0)
          {
            MustacheParsingUtil.AssertDepth(options, depth + 1);
            MustacheExpression.AddTextExpression(aggregateExpression, stringBuilder.ToString(), false);
          }
          flag1 = true;
          stringBuilder = new StringBuilder();
          if (MustacheParsingUtil.SafeCharAt(template, index + 2) == '{')
          {
            encode = false;
            index += 2;
          }
          else
          {
            encode = true;
            ++index;
          }
          flag2 = MustacheParsingUtil.SafeCharAt(template, index + 1) == '!' && MustacheParsingUtil.SafeCharAt(template, index + 2) == '-' && MustacheParsingUtil.SafeCharAt(template, index + 3) == '-';
        }
        else if (((ch != '}' ? 0 : (MustacheParsingUtil.SafeCharAt(template, index + 1) == '}' ? 1 : 0)) & (flag1 ? 1 : 0)) != 0 && (!flag2 || MustacheParsingUtil.SafeCharAt(template, index - 1) == '-' && MustacheParsingUtil.SafeCharAt(template, index - 2) == '-'))
        {
          if (encode)
          {
            ++index;
          }
          else
          {
            if (MustacheParsingUtil.SafeCharAt(template, index + 2) != '}')
              throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateBraceCountMismatch((object) stringBuilder.ToString()));
            index += 2;
          }
          string a1 = stringBuilder.ToString();
          if (a1.StartsWith("/"))
          {
            string a2 = a1.Substring(1);
            if (!(aggregateExpression is MustacheTemplatedExpression templatedExpression) || !templatedExpression.IsBlockExpression)
              throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInvalidEndBlock((object) a1));
            if (!string.Equals(a2, templatedExpression.Expression, StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, templatedExpression.HelperName, StringComparison.OrdinalIgnoreCase))
              throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateNonMatchingEndBlock((object) a1, (object) templatedExpression.Expression));
            aggregateExpression = aggregateExpression.ParentExpression;
            --depth;
          }
          else
          {
            bool isBlockExpression = false;
            bool isInvertedExpression = false;
            MustacheTemplatedExpression templatedExpression1 = (MustacheTemplatedExpression) null;
            if (a1.StartsWith("#*inline"))
            {
              if (options.DisableInlinePartials)
                throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateInlinePartialsNotAllowed());
              string key = a1.Substring("#*inline".Length).Trim(' ', '"', '\'');
              int num = template.IndexOf("{{/inline}}", index);
              if (num == -1)
                throw new MustacheExpressionInvalidException(WebApiResources.MissingCloseInlineMessage());
              if (template.Substring(index, num - index).Contains("#*inline"))
                throw new MustacheExpressionInvalidException(WebApiResources.NestedInlinePartialsMessage());
              MustacheParsingUtil.AssertDepth(options, depth + 1);
              MustacheRootExpression mustacheRootExpression2 = MustacheExpression.Parse(template.Substring(index + 1, num - (index + 1)), helpers, (Dictionary<string, MustacheRootExpression>) null, options, depth + 1);
              aggregateExpression.PartialExpressions.Add(key, mustacheRootExpression2);
              index = num + "{{/inline}}".Length - 1;
            }
            else
            {
              if (a1.StartsWith("#"))
              {
                isBlockExpression = true;
                a1 = a1.Substring(1);
              }
              else if (a1.StartsWith("^"))
              {
                isBlockExpression = true;
                isInvertedExpression = true;
                a1 = a1.Substring(1);
              }
              else if (string.Equals(a1, "else", StringComparison.OrdinalIgnoreCase) && aggregateExpression is MustacheTemplatedExpression)
              {
                templatedExpression1 = (MustacheTemplatedExpression) aggregateExpression;
                isBlockExpression = true;
                isInvertedExpression = !templatedExpression1.IsNegativeExpression;
                encode = templatedExpression1.Encode;
                a1 = string.IsNullOrEmpty(templatedExpression1.HelperName) ? templatedExpression1.Expression : templatedExpression1.HelperName + " " + templatedExpression1.Expression;
                aggregateExpression = aggregateExpression.ParentExpression;
                --depth;
              }
              MustacheTemplatedExpression templatedExpression2 = new MustacheTemplatedExpression(a1.Trim(), aggregateExpression, rootExpression, isBlockExpression, isInvertedExpression, templatedExpression1 != null, encode);
              if (templatedExpression1 != null)
                templatedExpression2.ElseSourceExpression = templatedExpression1;
              MustacheParsingUtil.AssertDepth(options, depth + 1);
              aggregateExpression.ChildExpressions.Add((MustacheExpression) templatedExpression2);
              if (isBlockExpression)
              {
                aggregateExpression = (MustacheAggregateExpression) templatedExpression2;
                ++depth;
              }
            }
          }
          flag1 = false;
          flag2 = false;
          encode = false;
          stringBuilder = new StringBuilder();
        }
        else
          stringBuilder.Append(ch);
      }
      if (stringBuilder.Length > 0 & flag1)
        throw new MustacheExpressionInvalidException(WebApiResources.MissingEndingBracesMessage((object) stringBuilder.ToString()));
      if (stringBuilder.Length > 0)
        MustacheParsingUtil.AssertDepth(options, depth + 1);
      MustacheExpression.AddTextExpression(aggregateExpression, stringBuilder.ToString(), true);
      return rootExpression;
    }

    private static void AddTextExpression(
      MustacheAggregateExpression expression,
      string text,
      bool isLastEntry)
    {
      MustacheTextExpression mustacheTextExpression = (MustacheTextExpression) null;
      bool flag = false;
      if (expression is MustacheTemplatedExpression templatedExpression && templatedExpression.IsBlockExpression && templatedExpression.ChildExpressions.Count == 0)
      {
        if (templatedExpression.ElseSourceExpression != null)
        {
          mustacheTextExpression = templatedExpression.ElseSourceExpression.ChildExpressions.LastOrDefault<MustacheExpression>() as MustacheTextExpression;
        }
        else
        {
          if (templatedExpression.ParentExpression.ChildExpressions.Count > 1)
            mustacheTextExpression = templatedExpression.ParentExpression.ChildExpressions[templatedExpression.ParentExpression.ChildExpressions.Count - 2] as MustacheTextExpression;
          if (templatedExpression.ParentExpression is MustacheRootExpression)
            flag = templatedExpression.ParentExpression.ChildExpressions.Count == 1 || templatedExpression.ParentExpression.ChildExpressions.Count == 2 && mustacheTextExpression != null;
        }
      }
      else if (expression.ChildExpressions.Count > 0 && expression.ChildExpressions[expression.ChildExpressions.Count - 1] is MustacheTemplatedExpression childExpression && childExpression.IsBlockExpression && childExpression.ChildExpressions.Count > 0)
        mustacheTextExpression = childExpression.ChildExpressions[childExpression.ChildExpressions.Count - 1] as MustacheTextExpression;
      if (flag || mustacheTextExpression != null)
      {
        int num = -1;
        if (mustacheTextExpression != null)
          num = mustacheTextExpression.Text.LastIndexOf('\n');
        if (mustacheTextExpression == null || flag && num < 0 && string.IsNullOrWhiteSpace(mustacheTextExpression.Text) || num >= 0 && string.IsNullOrWhiteSpace(mustacheTextExpression.Text.Substring(num + 1)))
        {
          int length = text.IndexOf('\n');
          if (length >= 0 && string.IsNullOrWhiteSpace(text.Substring(0, length)) || isLastEntry && string.IsNullOrWhiteSpace(text))
          {
            text = length < 0 ? string.Empty : text.Substring(length + 1);
            if (mustacheTextExpression != null)
              mustacheTextExpression.Text = num >= 0 ? mustacheTextExpression.Text.Substring(0, num + 1) : string.Empty;
          }
        }
      }
      if (text.Length <= 0)
        return;
      expression.ChildExpressions.Add((MustacheExpression) new MustacheTextExpression(text));
    }
  }
}
