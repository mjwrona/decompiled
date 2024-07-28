// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.StatementStartVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public class StatementStartVisitor : IVisitor
  {
    private bool m_isSafe;

    public bool IsSafe(AstNode node)
    {
      this.m_isSafe = true;
      node.IfNotNull<AstNode>((Action<AstNode>) (n => n.Accept((IVisitor) this)));
      return this.m_isSafe;
    }

    public void Visit(BinaryOperator node)
    {
      if (node == null || node.Operand1 == null)
        return;
      node.Operand1.Accept((IVisitor) this);
    }

    public void Visit(CallNode node)
    {
      if (node == null || node.Function == null)
        return;
      node.Function.Accept((IVisitor) this);
    }

    public void Visit(Conditional node)
    {
      if (node == null || node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public void Visit(Member node)
    {
      if (node == null || node.Root == null)
        return;
      node.Root.Accept((IVisitor) this);
    }

    public void Visit(UnaryOperator node)
    {
      if (node == null || !node.IsPostfix || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public void Visit(ClassNode node) => this.m_isSafe = node.IfNotNull<ClassNode, bool>((Func<ClassNode, bool>) (n => n.ClassType == ClassType.Declaration));

    public void Visit(CustomNode node) => this.m_isSafe = false;

    public void Visit(FunctionObject node) => this.m_isSafe = node.IfNotNull<FunctionObject, bool>((Func<FunctionObject, bool>) (n => n.FunctionType == FunctionType.ArrowFunction));

    public void Visit(ObjectLiteral node) => this.m_isSafe = false;

    public void Visit(ArrayLiteral node)
    {
    }

    public void Visit(AspNetBlockNode node)
    {
    }

    public void Visit(BindingIdentifier node)
    {
    }

    public void Visit(Block node)
    {
    }

    public void Visit(Break node)
    {
    }

    public void Visit(ComprehensionNode node)
    {
    }

    public void Visit(ConditionalCompilationComment node)
    {
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

    public void Visit(ConstantWrapper node)
    {
    }

    public void Visit(ConstantWrapperPP node)
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

    public void Visit(GetterSetter node)
    {
    }

    public void Visit(GroupingOperator node)
    {
    }

    public void Visit(IfNode node)
    {
    }

    public void Visit(ImportantComment node)
    {
    }

    public void Visit(ImportNode node)
    {
    }

    public void Visit(LabeledStatement node)
    {
    }

    public void Visit(LexicalDeclaration node)
    {
    }

    public void Visit(Lookup node)
    {
    }

    public void Visit(ModuleDeclaration node)
    {
    }

    public void Visit(RegExpLiteral node)
    {
    }

    public void Visit(ReturnNode node)
    {
    }

    public void Visit(Switch node)
    {
    }

    public void Visit(TemplateLiteral node)
    {
    }

    public void Visit(ThisLiteral node)
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

    public void Visit(WhileNode node)
    {
    }

    public void Visit(WithNode node)
    {
    }

    public void Visit(AstNodeList node)
    {
    }

    public void Visit(ComprehensionForClause node)
    {
    }

    public void Visit(ComprehensionIfClause node)
    {
    }

    public void Visit(InitializerNode node)
    {
    }

    public void Visit(ImportExportSpecifier node)
    {
    }

    public void Visit(ObjectLiteralField node)
    {
    }

    public void Visit(ObjectLiteralProperty node)
    {
    }

    public void Visit(ParameterDeclaration node)
    {
    }

    public void Visit(SwitchCase node)
    {
    }

    public void Visit(TemplateLiteralExpression node)
    {
    }

    public void Visit(VariableDeclaration node)
    {
    }
  }
}
