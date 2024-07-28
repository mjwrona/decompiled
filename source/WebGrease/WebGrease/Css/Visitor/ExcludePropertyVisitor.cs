// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ExcludePropertyVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public sealed class ExcludePropertyVisitor : NodeTransformVisitor
  {
    private const string ExcludedSubstring = "Exclude";

    public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
      if (declarationNode == null)
        throw new ArgumentNullException(nameof (declarationNode));
      return !declarationNode.MinifyPrint().Contains("Exclude") ? (AstNode) declarationNode : (AstNode) null;
    }
  }
}
