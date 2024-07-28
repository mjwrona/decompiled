// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.MediaQuery.MediaQueryNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.MediaQuery
{
  public sealed class MediaQueryNode : AstNode
  {
    public MediaQueryNode(
      string onlyText,
      string notText,
      string mediaType,
      ReadOnlyCollection<MediaExpressionNode> mediaExpressions)
    {
      this.OnlyText = onlyText;
      this.NotText = notText;
      this.MediaType = mediaType;
      this.MediaExpressions = mediaExpressions ?? new List<MediaExpressionNode>(0).AsReadOnly();
    }

    public string OnlyText { get; private set; }

    public string NotText { get; private set; }

    public string MediaType { get; private set; }

    public ReadOnlyCollection<MediaExpressionNode> MediaExpressions { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitMediaQueryNode(this);
  }
}
