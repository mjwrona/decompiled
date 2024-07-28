// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.PropertyModel.BackgroundImage
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using WebGrease.Extensions;

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
  internal sealed class BackgroundImage
  {
    internal static readonly string UrlRegEx = "url\\((?<quote>[\"']?)\\s*((hash\\(.*?\\))|(%?([-./\\w_]+)(\\:\\w*)?%?))\\s*\\k<quote>\\)";
    private static readonly Regex MultipleUrlsRegex = new Regex(BackgroundImage.UrlRegEx, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex UrlRegex = new Regex(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^{0}$", new object[1]
    {
      (object) BackgroundImage.UrlRegEx
    }), RegexOptions.IgnoreCase | RegexOptions.Compiled);

    internal BackgroundImage()
    {
    }

    internal BackgroundImage(DeclarationNode declarationNode)
    {
      this.DeclarationNode = declarationNode != null ? declarationNode : throw new ArgumentNullException(nameof (declarationNode));
      ExprNode exprNode = declarationNode.ExprNode;
      this.ParseTerm(exprNode.TermNode);
      exprNode.TermsWithOperators.ForEach<TermWithOperatorNode>(new Action<TermWithOperatorNode>(this.ParseTermWithOperator));
    }

    public DeclarationNode DeclarationNode { get; private set; }

    internal TermNode UrlTermNode { get; private set; }

    internal string Url { get; private set; }

    internal static bool HasMultipleUrls(string text) => !string.IsNullOrWhiteSpace(text) && BackgroundImage.MultipleUrlsRegex.Matches(text).Count > 1 && text.IndexOf("background", StringComparison.OrdinalIgnoreCase) != -1;

    internal static bool TryGetUrl(TermNode termNode, out string url)
    {
      if (termNode != null && !string.IsNullOrWhiteSpace(termNode.StringBasedValue))
      {
        string stringBasedValue = termNode.StringBasedValue;
        Match match = BackgroundImage.UrlRegex.Match(stringBasedValue);
        if (match.Success && match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(url = match.Groups[1].Value))
          return true;
      }
      url = (string) null;
      return false;
    }

    internal bool VerifyBackgroundUrl(
      AstNode parent,
      HashSet<string> imageReferencesToIgnore,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
      out bool shouldIgnore)
    {
      shouldIgnore = false;
      if (string.IsNullOrWhiteSpace(this.Url))
      {
        imageAssemblyAnalysisLog.SafeAdd(parent, this.Url, new FailureReason?(FailureReason.NoUrl));
        return false;
      }
      if (imageReferencesToIgnore != null)
      {
        string str = this.Url.NormalizeUrl();
        if (imageReferencesToIgnore.Contains(str))
        {
          imageAssemblyAnalysisLog.SafeAdd(parent, this.Url, new FailureReason?(FailureReason.IgnoreUrl));
          shouldIgnore = true;
          return false;
        }
      }
      return true;
    }

    internal void ParseTerm(TermNode termNode)
    {
      string url;
      if (termNode == null || !BackgroundImage.TryGetUrl(termNode, out url))
        return;
      this.UrlTermNode = termNode;
      this.Url = url;
    }

    internal void ParseTermWithOperator(TermWithOperatorNode termWithOperatorNode)
    {
      if (termWithOperatorNode == null)
        return;
      this.ParseTerm(termWithOperatorNode.TermNode);
    }

    internal bool UpdateTermForUrl(
      TermNode originalTermNode,
      out TermNode updatedTermNode,
      string updatedUrl)
    {
      if (originalTermNode == this.UrlTermNode)
      {
        updatedUrl = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "url({0})", new object[1]
        {
          (object) updatedUrl
        });
        updatedTermNode = new TermNode(originalTermNode.UnaryOperator, originalTermNode.NumberBasedValue, updatedUrl, originalTermNode.Hexcolor, originalTermNode.FunctionNode, originalTermNode.ImportantComments);
        return true;
      }
      updatedTermNode = originalTermNode;
      return false;
    }

    internal DeclarationNode UpdateBackgroundImageNode(string updatedUrl)
    {
      if (this.DeclarationNode == null)
        return (DeclarationNode) null;
      ExprNode exprNode = this.DeclarationNode.ExprNode;
      TermNode updatedTermNode;
      return this.UpdateTermForUrl(exprNode.TermNode, out updatedTermNode, updatedUrl) ? new DeclarationNode(this.DeclarationNode.Property, new ExprNode(updatedTermNode, exprNode.TermsWithOperators, exprNode.ImportantComments), this.DeclarationNode.Prio, this.DeclarationNode.ImportantComments) : this.DeclarationNode;
    }
  }
}
