// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheTemplateParser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class MustacheTemplateParser
  {
    private static Dictionary<string, MustacheTemplateHelperWriter> s_defaultHandlebarHelpers = HandleBarBuiltinHelpers.GetHelpers();
    private static Dictionary<string, MustacheTemplateHelperWriter> s_commonHelpers = CommonMustacheHelpers.GetHelpers();
    private Dictionary<string, MustacheTemplateHelperWriter> m_helpers;
    private Dictionary<string, MustacheRootExpression> m_partials;
    private MustacheOptions m_options;

    [Obsolete("Use the RegisterHelper method")]
    public Dictionary<string, MustacheTemplateHelper> Helpers { get; private set; }

    [Obsolete("Use the RegisterHelper method")]
    public Dictionary<string, MustacheTemplateHelper> BlockHelpers { get; private set; }

    [Obsolete("Use the RegisterPartial method")]
    public Dictionary<string, MustacheRootExpression> Partials { get; private set; }

    public MustacheTemplateParser(
      bool useDefaultHandlebarHelpers = true,
      Dictionary<string, string> partials = null)
      : this(useDefaultHandlebarHelpers, true, partials, (MustacheOptions) null)
    {
    }

    public MustacheTemplateParser(bool useDefaultHandlebarHelpers, bool useCommonTemplateHelpers)
      : this(useDefaultHandlebarHelpers, useCommonTemplateHelpers, (Dictionary<string, string>) null, (MustacheOptions) null)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public MustacheTemplateParser(
      bool useDefaultHandlebarHelpers,
      bool useCommonTemplateHelpers,
      Dictionary<string, string> partials,
      MustacheOptions options)
    {
      this.m_options = options;
      this.m_helpers = !useDefaultHandlebarHelpers ? new Dictionary<string, MustacheTemplateHelperWriter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, MustacheTemplateHelperWriter>((IDictionary<string, MustacheTemplateHelperWriter>) MustacheTemplateParser.s_defaultHandlebarHelpers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (useCommonTemplateHelpers)
      {
        foreach (KeyValuePair<string, MustacheTemplateHelperWriter> commonHelper in MustacheTemplateParser.s_commonHelpers)
          this.m_helpers[commonHelper.Key] = commonHelper.Value;
      }
      this.m_partials = new Dictionary<string, MustacheRootExpression>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (partials == null)
        return;
      foreach (KeyValuePair<string, string> partial in partials)
        this.m_partials[partial.Key] = (MustacheRootExpression) this.Parse(partial.Value);
    }

    public void RegisterHelper(string helperName, MustacheTemplateHelperMethod helper) => this.m_helpers[helperName] = (MustacheTemplateHelperWriter) ((expression, writer, context) =>
    {
      object obj = helper(expression, context);
      if (obj == null)
        return;
      writer.Write(obj.ToString());
    });

    public void RegisterHelper(string helperName, MustacheTemplateHelperWriter helper) => this.m_helpers[helperName] = helper;

    public void ParseAndRegisterPartial(string partialName, string partialExpression) => this.m_partials[partialName] = (MustacheRootExpression) this.Parse(partialExpression);

    public void RegisterPartial(string partialName, MustacheRootExpression partialExpression) => this.m_partials[partialName] = partialExpression;

    public string ReplaceValues(string template, object replacementContext) => MustacheExpression.Parse(template, this.m_helpers, this.m_partials, this.m_options).Evaluate(replacementContext, (Dictionary<string, object>) null, (MustacheEvaluationContext) null, (Dictionary<string, MustacheRootExpression>) null, this.m_options);

    public MustacheExpression Parse(string template) => (MustacheExpression) MustacheExpression.Parse(template, this.m_helpers, this.m_partials, this.m_options);
  }
}
