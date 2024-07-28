// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheEvaluationContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class MustacheEvaluationContext
  {
    private JToken m_replacementToken;
    private IDictionary<MustacheTemplatedExpression, bool> m_evaluatedExpressionTruthiness;
    private MustacheOptions m_options;

    public object ReplacementObject { get; set; }

    public Dictionary<string, object> AdditionalEvaluationData { get; set; }

    internal JToken ReplacementToken
    {
      get
      {
        if (this.m_replacementToken == null && this.ReplacementObject != null)
        {
          this.m_replacementToken = this.ReplacementObject as JToken;
          if (this.m_replacementToken == null)
            this.m_replacementToken = JToken.FromObject(this.ReplacementObject);
        }
        return this.m_replacementToken;
      }
    }

    public MustacheEvaluationContext ParentContext { get; set; }

    public int CurrentIndex { get; set; }

    public int ParentItemsCount { get; set; }

    public string CurrentKey { get; set; }

    public Dictionary<string, MustacheRootExpression> PartialExpressions { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public MustacheOptions Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new MustacheOptions();
        return this.m_options;
      }
      set => this.m_options = value;
    }

    public static void CombinePartialsDictionaries(
      MustacheEvaluationContext context,
      MustacheAggregateExpression expression)
    {
      Dictionary<string, MustacheRootExpression> dictionary = new Dictionary<string, MustacheRootExpression>();
      foreach (string key in context.PartialExpressions.Keys)
        dictionary.TryAdd<string, MustacheRootExpression>(key, context.PartialExpressions[key]);
      foreach (string key in expression.PartialExpressions.Keys)
        dictionary.TryAdd<string, MustacheRootExpression>(key, expression.PartialExpressions[key]);
      context.PartialExpressions = dictionary;
    }

    internal void AssertCancellation() => this.Options.CancellationToken.ThrowIfCancellationRequested();

    internal bool? WasExpressionEvaluatedAsTruthy(MustacheTemplatedExpression expression)
    {
      bool flag;
      return this.m_evaluatedExpressionTruthiness != null && this.m_evaluatedExpressionTruthiness.TryGetValue(expression, out flag) ? new bool?(flag) : new bool?();
    }

    internal void StoreExpressionTruthiness(MustacheTemplatedExpression expression, bool truthy)
    {
      if (this.m_evaluatedExpressionTruthiness == null)
        this.m_evaluatedExpressionTruthiness = (IDictionary<MustacheTemplatedExpression, bool>) new Dictionary<MustacheTemplatedExpression, bool>();
      this.m_evaluatedExpressionTruthiness[expression] = truthy;
    }
  }
}
