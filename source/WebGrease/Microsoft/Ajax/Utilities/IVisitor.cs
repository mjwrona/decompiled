// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.IVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public interface IVisitor
  {
    void Visit(ArrayLiteral node);

    void Visit(AspNetBlockNode node);

    void Visit(AstNodeList node);

    void Visit(BinaryOperator node);

    void Visit(BindingIdentifier node);

    void Visit(Block node);

    void Visit(Break node);

    void Visit(CallNode node);

    void Visit(ClassNode node);

    void Visit(ComprehensionNode node);

    void Visit(ComprehensionForClause node);

    void Visit(ComprehensionIfClause node);

    void Visit(ConditionalCompilationComment node);

    void Visit(ConditionalCompilationElse node);

    void Visit(ConditionalCompilationElseIf node);

    void Visit(ConditionalCompilationEnd node);

    void Visit(ConditionalCompilationIf node);

    void Visit(ConditionalCompilationOn node);

    void Visit(ConditionalCompilationSet node);

    void Visit(Conditional node);

    void Visit(ConstantWrapper node);

    void Visit(ConstantWrapperPP node);

    void Visit(ConstStatement node);

    void Visit(ContinueNode node);

    void Visit(CustomNode node);

    void Visit(DebuggerNode node);

    void Visit(DirectivePrologue node);

    void Visit(DoWhile node);

    void Visit(EmptyStatement node);

    void Visit(ExportNode node);

    void Visit(ForIn node);

    void Visit(ForNode node);

    void Visit(FunctionObject node);

    void Visit(GetterSetter node);

    void Visit(GroupingOperator node);

    void Visit(IfNode node);

    void Visit(ImportantComment node);

    void Visit(ImportExportSpecifier node);

    void Visit(ImportNode node);

    void Visit(InitializerNode node);

    void Visit(LabeledStatement node);

    void Visit(LexicalDeclaration node);

    void Visit(Lookup node);

    void Visit(Member node);

    void Visit(ModuleDeclaration node);

    void Visit(ObjectLiteral node);

    void Visit(ObjectLiteralField node);

    void Visit(ObjectLiteralProperty node);

    void Visit(ParameterDeclaration node);

    void Visit(RegExpLiteral node);

    void Visit(ReturnNode node);

    void Visit(Switch node);

    void Visit(SwitchCase node);

    void Visit(TemplateLiteral node);

    void Visit(TemplateLiteralExpression node);

    void Visit(ThisLiteral node);

    void Visit(ThrowNode node);

    void Visit(TryNode node);

    void Visit(Var node);

    void Visit(VariableDeclaration node);

    void Visit(UnaryOperator node);

    void Visit(WhileNode node);

    void Visit(WithNode node);
  }
}
