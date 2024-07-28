// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.WikiInlineFieldCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Markdig.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class WikiInlineFieldCorrector : TermCorrector
  {
    private static readonly IReadOnlyDictionary<string, string> s_inlineFieldMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "url",
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "contentLinks", (object) "lower"))
      },
      {
        "title",
        "fileNames"
      },
      {
        "organization",
        "collectionName"
      },
      {
        "project",
        "projectName"
      }
    };

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if (termExpression.IsOfType("*"))
        return (IExpression) termExpression;
      TermExpression termExpression1 = termExpression;
      string str;
      if (WikiInlineFieldCorrector.s_inlineFieldMap.TryGetValue(termExpression.Type, out str))
      {
        termExpression1.Type = str;
        if (this.IsExpressionUrlType(termExpression1.Type))
        {
          string escapedUrl = this.GetEscapedUrl(termExpression.Value);
          termExpression1.Value = this.GetMarkDigEncodedUrl(escapedUrl);
        }
      }
      else
        termExpression1 = new TermExpression("*", Operator.Matches, termExpression.Type + ":" + termExpression.Value);
      return (IExpression) termExpression1;
    }

    private string GetMarkDigEncodedUrl(string searchUrl) => MarkdownTextExtracter.GetEncodedUrl(searchUrl) ?? searchUrl;

    private string GetEscapedUrl(string searchUrl)
    {
      if (string.IsNullOrEmpty(searchUrl))
        return searchUrl;
      StringBuilder stringBuilder = new StringBuilder(searchUrl.Length);
      foreach (char c in searchUrl)
      {
        string str = HtmlHelper.EscapeUrlCharacter(c);
        stringBuilder.Append(str ?? c.ToString());
      }
      return stringBuilder.ToString();
    }

    private bool IsExpressionUrlType(string type) => WikiInlineFieldCorrector.s_inlineFieldMap["url"].Equals(type);
  }
}
