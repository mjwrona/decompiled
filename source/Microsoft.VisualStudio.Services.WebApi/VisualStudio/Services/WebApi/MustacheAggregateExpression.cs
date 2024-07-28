// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheAggregateExpression
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public abstract class MustacheAggregateExpression : MustacheExpression
  {
    internal MustacheAggregateExpression ParentExpression { get; set; }

    internal List<MustacheExpression> ChildExpressions { get; set; }

    internal MustacheAggregateExpression()
    {
      this.ChildExpressions = new List<MustacheExpression>();
      this.PartialExpressions = new Dictionary<string, MustacheRootExpression>();
    }

    public override bool IsContextBased => this.ChildExpressions.Any<MustacheExpression>((Func<MustacheExpression, bool>) (child => child is MustacheTemplatedExpression));

    internal override void Evaluate(MustacheEvaluationContext context, MustacheTextWriter writer) => this.EvaluateChildExpressions(context, writer);

    public string EvaluateChildExpressions(MustacheEvaluationContext context)
    {
      using (StringWriter writer1 = new StringWriter())
      {
        MustacheTextWriter mustacheTextWriter = new MustacheTextWriter((TextWriter) writer1, context.Options);
        foreach (MustacheExpression childExpression in this.ChildExpressions)
        {
          context.AssertCancellation();
          MustacheEvaluationContext context1 = context;
          MustacheTextWriter writer2 = mustacheTextWriter;
          childExpression.Evaluate(context1, writer2);
        }
        return writer1.ToString();
      }
    }

    public void EvaluateChildExpressions(
      MustacheEvaluationContext context,
      MustacheTextWriter writer)
    {
      foreach (MustacheExpression childExpression in this.ChildExpressions)
      {
        context.AssertCancellation();
        MustacheEvaluationContext context1 = context;
        MustacheTextWriter writer1 = writer;
        childExpression.Evaluate(context1, writer1);
      }
    }

    public JToken GetCurrentJToken(string selector, MustacheEvaluationContext context)
    {
      if (string.IsNullOrEmpty(selector) || context.ReplacementObject == null)
        return (JToken) null;
      JToken currentJtoken = context.ReplacementToken;
      if (selector.StartsWith("@root"))
      {
        bool flag = false;
        if (selector.Length == "@root".Length)
        {
          flag = true;
          selector = (string) null;
        }
        else if (selector["@root".Length] == '.' || selector["@root".Length] == '/')
        {
          flag = true;
          selector = selector.Substring("@root".Length + 1);
        }
        if (flag)
        {
          while (context.ParentContext != null)
          {
            context = context.ParentContext;
            currentJtoken = context.ReplacementToken;
          }
        }
      }
      else
      {
        string str;
        char[] chArray;
        for (; selector.StartsWith("..") && context.ParentContext != null; selector = str.TrimStart(chArray))
        {
          context = context.ParentContext;
          currentJtoken = context.ReplacementToken;
          str = selector.Substring(2);
          chArray = new char[1]{ '/' };
        }
        if (string.Equals(selector, ".") || string.Equals(selector, "this"))
        {
          selector = (string) null;
        }
        else
        {
          if (string.Equals(selector, "@index"))
            return (JToken) context.CurrentIndex.ToString();
          if (string.Equals(selector, "@key"))
            return (JToken) context.CurrentKey;
          if (string.Equals(selector, "@first"))
            return (JToken) (context.CurrentIndex == 0);
          if (string.Equals(selector, "@last"))
            return (JToken) (context.CurrentIndex == context.ParentItemsCount - 1);
          if (selector.StartsWith("./"))
            selector = selector.Substring("./".Length);
          else if (selector.StartsWith("this"))
          {
            if (selector.StartsWith("this/"))
              selector = selector.Substring("this/".Length);
            else if (selector.StartsWith("this."))
              selector = selector.Substring("this.".Length);
          }
        }
      }
      if (!string.IsNullOrEmpty(selector))
      {
        if (currentJtoken != null)
        {
          try
          {
            currentJtoken = currentJtoken.SelectToken(selector);
          }
          catch (JsonException ex)
          {
            currentJtoken = (JToken) null;
          }
        }
      }
      return currentJtoken;
    }
  }
}
