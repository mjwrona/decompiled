// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.TreeVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class TreeVisitor : IVisitor
  {
    public virtual void Visit(ArrayLiteral node)
    {
      if (node == null || node.Elements == null)
        return;
      node.Elements.Accept((IVisitor) this);
    }

    public virtual void Visit(AspNetBlockNode node)
    {
    }

    public virtual void Visit(AstNodeList node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      if (node.Operand1 != null)
        node.Operand1.Accept((IVisitor) this);
      if (node.Operand2 == null)
        return;
      node.Operand2.Accept((IVisitor) this);
    }

    public virtual void Visit(BindingIdentifier node)
    {
    }

    public virtual void Visit(Block node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(Break node)
    {
    }

    public virtual void Visit(CallNode node)
    {
      if (node == null)
        return;
      if (node.Arguments != null)
        node.Arguments.Accept((IVisitor) this);
      if (node.Function == null)
        return;
      node.Function.Accept((IVisitor) this);
    }

    public virtual void Visit(ClassNode node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Heritage != null)
        node.Heritage.Accept((IVisitor) this);
      if (node.Elements == null)
        return;
      node.Elements.Accept((IVisitor) this);
    }

    public virtual void Visit(ComprehensionNode node)
    {
      if (node == null)
        return;
      if (node.Clauses != null)
        node.Clauses.Accept((IVisitor) this);
      if (node.Expression == null)
        return;
      node.Expression.Accept((IVisitor) this);
    }

    public virtual void Visit(ComprehensionForClause node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Expression == null)
        return;
      node.Expression.Accept((IVisitor) this);
    }

    public virtual void Visit(ComprehensionIfClause node)
    {
      if (node == null || node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public virtual void Visit(ConditionalCompilationComment node)
    {
      if (node == null || node.Statements == null)
        return;
      node.Statements.Accept((IVisitor) this);
    }

    public virtual void Visit(ConditionalCompilationElse node)
    {
    }

    public virtual void Visit(ConditionalCompilationElseIf node)
    {
      if (node == null || node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public virtual void Visit(ConditionalCompilationEnd node)
    {
    }

    public virtual void Visit(ConditionalCompilationIf node)
    {
      if (node == null || node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public virtual void Visit(ConditionalCompilationOn node)
    {
    }

    public virtual void Visit(ConditionalCompilationSet node)
    {
      if (node == null || node.Value == null)
        return;
      node.Value.Accept((IVisitor) this);
    }

    public virtual void Visit(Conditional node)
    {
      if (node == null)
        return;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      if (node.TrueExpression != null)
        node.TrueExpression.Accept((IVisitor) this);
      if (node.FalseExpression == null)
        return;
      node.FalseExpression.Accept((IVisitor) this);
    }

    public virtual void Visit(ConstantWrapper node)
    {
    }

    public virtual void Visit(ConstantWrapperPP node)
    {
    }

    public virtual void Visit(ConstStatement node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(ContinueNode node)
    {
    }

    public virtual void Visit(CustomNode node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(DebuggerNode node)
    {
    }

    public virtual void Visit(DirectivePrologue node)
    {
    }

    public virtual void Visit(DoWhile node)
    {
      if (node == null)
        return;
      if (node.Body != null)
        node.Body.Accept((IVisitor) this);
      if (node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public virtual void Visit(EmptyStatement node)
    {
    }

    public virtual void Visit(ExportNode node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child.Accept((IVisitor) this);
    }

    public virtual void Visit(ForIn node)
    {
      if (node == null)
        return;
      if (node.Variable != null)
        node.Variable.Accept((IVisitor) this);
      if (node.Collection != null)
        node.Collection.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public virtual void Visit(ForNode node)
    {
      if (node == null)
        return;
      if (node.Initializer != null)
        node.Initializer.Accept((IVisitor) this);
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      if (node.Incrementer != null)
        node.Incrementer.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public virtual void Visit(FunctionObject node)
    {
      if (node == null || node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public virtual void Visit(GetterSetter node)
    {
    }

    public virtual void Visit(GroupingOperator node)
    {
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public virtual void Visit(IfNode node)
    {
      if (node == null)
        return;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      if (node.TrueBlock != null)
        node.TrueBlock.Accept((IVisitor) this);
      if (node.FalseBlock == null)
        return;
      node.FalseBlock.Accept((IVisitor) this);
    }

    public virtual void Visit(ImportantComment node)
    {
    }

    public virtual void Visit(ImportExportSpecifier node)
    {
      if (node == null || node.LocalIdentifier == null)
        return;
      node.LocalIdentifier.Accept((IVisitor) this);
    }

    public virtual void Visit(ImportNode node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child.Accept((IVisitor) this);
    }

    public virtual void Visit(InitializerNode node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public virtual void Visit(LabeledStatement node)
    {
      if (node == null || node.Statement == null)
        return;
      node.Statement.Accept((IVisitor) this);
    }

    public virtual void Visit(LexicalDeclaration node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(Lookup node)
    {
    }

    public virtual void Visit(Member node)
    {
      if (node == null || node.Root == null)
        return;
      node.Root.Accept((IVisitor) this);
    }

    public virtual void Visit(ModuleDeclaration node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public virtual void Visit(ObjectLiteral node)
    {
      if (node == null || node.Properties == null)
        return;
      node.Properties.Accept((IVisitor) this);
    }

    public virtual void Visit(ObjectLiteralField node)
    {
    }

    public virtual void Visit(ObjectLiteralProperty node)
    {
      if (node == null)
        return;
      if (node.Name != null)
        node.Name.Accept((IVisitor) this);
      if (node.Value == null)
        return;
      node.Value.Accept((IVisitor) this);
    }

    public virtual void Visit(ParameterDeclaration node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public virtual void Visit(RegExpLiteral node)
    {
    }

    public virtual void Visit(ReturnNode node)
    {
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public virtual void Visit(Switch node)
    {
      if (node == null)
        return;
      if (node.Expression != null)
        node.Expression.Accept((IVisitor) this);
      if (node.Cases == null)
        return;
      node.Cases.Accept((IVisitor) this);
    }

    public virtual void Visit(SwitchCase node)
    {
      if (node == null)
        return;
      if (node.CaseValue != null)
        node.CaseValue.Accept((IVisitor) this);
      if (node.Statements == null)
        return;
      node.Statements.Accept((IVisitor) this);
    }

    public virtual void Visit(TemplateLiteral node)
    {
      if (node == null)
        return;
      if (node.Function != null)
        node.Function.Accept((IVisitor) this);
      if (node.Expressions == null)
        return;
      node.Expressions.Accept((IVisitor) this);
    }

    public virtual void Visit(TemplateLiteralExpression node)
    {
      if (node == null || node.Expression == null)
        return;
      node.Expression.Accept((IVisitor) this);
    }

    public virtual void Visit(ThisLiteral node)
    {
    }

    public virtual void Visit(ThrowNode node)
    {
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public virtual void Visit(TryNode node)
    {
      if (node == null)
        return;
      if (node.TryBlock != null)
        node.TryBlock.Accept((IVisitor) this);
      if (node.CatchParameter != null)
        node.CatchParameter.Accept((IVisitor) this);
      if (node.CatchBlock != null)
        node.CatchBlock.Accept((IVisitor) this);
      if (node.FinallyBlock == null)
        return;
      node.FinallyBlock.Accept((IVisitor) this);
    }

    public virtual void Visit(Var node)
    {
      if (node == null)
        return;
      foreach (AstNode child in node.Children)
        child?.Accept((IVisitor) this);
    }

    public virtual void Visit(VariableDeclaration node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public virtual void Visit(UnaryOperator node)
    {
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public virtual void Visit(WhileNode node)
    {
      if (node == null)
        return;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public virtual void Visit(WithNode node)
    {
      if (node == null)
        return;
      if (node.WithObject != null)
        node.WithObject.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }
  }
}
