// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.AstNodeExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Linq;
using System.Text;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Extensions
{
  public static class AstNodeExtensions
  {
    public static string PrettyPrint(this AstNode node) => node != null ? PrintVisitor.Print(node, true) : string.Empty;

    public static string MinifyPrint(this AstNode node) => node != null ? PrintVisitor.Print(node, false) : string.Empty;

    public static string PrintSelector(this MediaNode node) => "@media " + string.Join(",", node.MediaQueries.Select<MediaQueryNode, string>((Func<MediaQueryNode, string>) (mq => mq.MinifyPrint())));

    internal static string PrintSelector(this RulesetNode rulesetNode)
    {
      if (rulesetNode == null)
        return string.Empty;
      StringBuilder rulesetBuilder = new StringBuilder();
      rulesetNode.SelectorsGroupNode.SelectorNodes.ForEach<SelectorNode>((Action<SelectorNode, bool>) ((selector, last) =>
      {
        rulesetBuilder.Append(selector.MinifyPrint());
        if (last)
          return;
        rulesetBuilder.Append(',');
      }));
      return rulesetBuilder.ToString();
    }
  }
}
