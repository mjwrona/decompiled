// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.NewParensVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal class NewParensVisitor : IVisitor
  {
    private bool m_needsParens;
    private bool m_outerHasNoArguments;

    public static bool NeedsParens(AstNode expression, bool outerHasNoArguments)
    {
      NewParensVisitor newParensVisitor = new NewParensVisitor(outerHasNoArguments);
      expression.Accept((IVisitor) newParensVisitor);
      return newParensVisitor.m_needsParens;
    }

    private NewParensVisitor(bool outerHasNoArguments) => this.m_outerHasNoArguments = outerHasNoArguments;

    public void Visit(ArrayLiteral node)
    {
    }

    public void Visit(AspNetBlockNode node) => this.m_needsParens = true;

    public void Visit(BinaryOperator node) => this.m_needsParens = true;

    public void Visit(BindingIdentifier node)
    {
    }

    public void Visit(CallNode node)
    {
      if (node != null)
      {
        if (node.InBrackets)
          node.Function.Accept((IVisitor) this);
        else if (!node.IsConstructor)
        {
          this.m_needsParens = true;
        }
        else
        {
          if (node.Arguments != null && node.Arguments.Count != 0)
            return;
          this.m_needsParens = !this.m_outerHasNoArguments;
        }
      }
      else
        this.m_needsParens = true;
    }

    public void Visit(ClassNode node)
    {
    }

    public void Visit(ComprehensionNode node)
    {
    }

    public void Visit(ConditionalCompilationComment node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
      {
        child.Accept((IVisitor) this);
        if (this.m_needsParens)
          break;
      }
    }

    public void Visit(ConditionalCompilationElse node)
    {
    }

    public void Visit(ConditionalCompilationElseIf node)
    {
    }

    public void Visit(ConditionalCompilationEnd node)
    {
    }

    public void Visit(ConditionalCompilationIf node)
    {
    }

    public void Visit(ConditionalCompilationOn node)
    {
    }

    public void Visit(ConditionalCompilationSet node)
    {
    }

    public void Visit(Conditional node) => this.m_needsParens = true;

    public void Visit(ConstantWrapper node)
    {
    }

    public void Visit(ConstantWrapperPP node)
    {
    }

    public void Visit(CustomNode node)
    {
    }

    public void Visit(FunctionObject node)
    {
    }

    public virtual void Visit(GroupingOperator node)
    {
    }

    public void Visit(ImportantComment node)
    {
    }

    public void Visit(Lookup node)
    {
    }

    public void Visit(Member node) => node?.Root.Accept((IVisitor) this);

    public void Visit(ObjectLiteral node)
    {
    }

    public void Visit(ParameterDeclaration node)
    {
    }

    public void Visit(RegExpLiteral node)
    {
    }

    public void Visit(TemplateLiteral node)
    {
    }

    public void Visit(ThisLiteral node)
    {
    }

    public void Visit(UnaryOperator node) => this.m_needsParens = true;

    public void Visit(AstNodeList node)
    {
    }

    public void Visit(GetterSetter node)
    {
    }

    public void Visit(ObjectLiteralField node)
    {
    }

    public void Visit(ObjectLiteralProperty node)
    {
    }

    public void Visit(Block node)
    {
    }

    public void Visit(Break node)
    {
    }

    public void Visit(ComprehensionForClause node)
    {
    }

    public void Visit(ComprehensionIfClause node)
    {
    }

    public void Visit(ConstStatement node)
    {
    }

    public void Visit(ContinueNode node)
    {
    }

    public void Visit(DebuggerNode node)
    {
    }

    public void Visit(DirectivePrologue node)
    {
    }

    public void Visit(DoWhile node)
    {
    }

    public void Visit(EmptyStatement node)
    {
    }

    public void Visit(ExportNode node)
    {
    }

    public void Visit(ForIn node)
    {
    }

    public void Visit(ForNode node)
    {
    }

    public void Visit(IfNode node)
    {
    }

    public void Visit(ImportExportSpecifier node)
    {
    }

    public void Visit(ImportNode node)
    {
    }

    public void Visit(InitializerNode node)
    {
    }

    public void Visit(LabeledStatement node)
    {
    }

    public void Visit(LexicalDeclaration node)
    {
    }

    public void Visit(ModuleDeclaration node)
    {
    }

    public void Visit(ReturnNode node)
    {
    }

    public void Visit(Switch node)
    {
    }

    public void Visit(SwitchCase node)
    {
    }

    public void Visit(TemplateLiteralExpression node)
    {
    }

    public void Visit(ThrowNode node)
    {
    }

    public void Visit(TryNode node)
    {
    }

    public void Visit(Var node)
    {
    }

    public void Visit(VariableDeclaration node)
    {
    }

    public void Visit(WhileNode node)
    {
    }

    public void Visit(WithNode node)
    {
    }
  }
}
