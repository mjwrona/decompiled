// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.BindingsVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class BindingsVisitor : IVisitor
  {
    private IList<BindingIdentifier> m_bindings;
    private IList<Lookup> m_lookups;

    private BindingsVisitor()
    {
      this.m_bindings = (IList<BindingIdentifier>) new List<BindingIdentifier>();
      this.m_lookups = (IList<Lookup>) new List<Lookup>();
    }

    public static IList<BindingIdentifier> Bindings(AstNode node)
    {
      BindingsVisitor bindingsVisitor = new BindingsVisitor();
      node?.Accept((IVisitor) bindingsVisitor);
      return bindingsVisitor.m_bindings;
    }

    public static IList<Lookup> References(AstNode node)
    {
      BindingsVisitor bindingsVisitor = new BindingsVisitor();
      node?.Accept((IVisitor) bindingsVisitor);
      return bindingsVisitor.m_lookups;
    }

    public void Visit(ArrayLiteral node) => node.IfNotNull<ArrayLiteral>((Action<ArrayLiteral>) (n => AjaxMinExtensions.ForEach<AstNode>(n.Elements, (Action<AstNode>) (e => e.Accept((IVisitor) this)))));

    public void Visit(AstNodeList node) => node.IfNotNull<AstNodeList>((Action<AstNodeList>) (n => n.Children.ForEach<AstNode>((Action<AstNode>) (i => i.Accept((IVisitor) this)))));

    public void Visit(BindingIdentifier node) => node.IfNotNull<BindingIdentifier>((Action<BindingIdentifier>) (n => this.m_bindings.Add(n)));

    public void Visit(ClassNode node)
    {
      if (node == null || node.Binding == null)
        return;
      node.Binding.Accept((IVisitor) this);
    }

    public void Visit(ConstantWrapper node)
    {
      if (node == null || node.Value == Missing.Value)
        return;
      BindingsVisitor.ReportError((AstNode) node);
    }

    public void Visit(ConstStatement node) => node.IfNotNull<ConstStatement>((Action<ConstStatement>) (n => n.Children.ForEach<AstNode>((Action<AstNode>) (v => v.Accept((IVisitor) this)))));

    public void Visit(CustomNode node)
    {
    }

    public void Visit(ExportNode node)
    {
      if (node == null)
        return;
      foreach (AstNode astNode in (ImportExportStatement) node)
        astNode.Accept((IVisitor) this);
    }

    public void Visit(FunctionObject node)
    {
      if (node == null || node.Binding == null)
        return;
      node.Binding.Accept((IVisitor) this);
    }

    public void Visit(InitializerNode node) => node.IfNotNull<InitializerNode>((Action<InitializerNode>) (n => n.Binding.IfNotNull<AstNode>((Action<AstNode>) (v => v.Accept((IVisitor) this)))));

    public void Visit(ImportExportSpecifier node)
    {
      if (node == null || !(node.LocalIdentifier is BindingIdentifier localIdentifier))
        return;
      this.m_bindings.Add(localIdentifier);
    }

    public void Visit(ImportNode node)
    {
      if (node == null)
        return;
      foreach (AstNode astNode in (ImportExportStatement) node)
        astNode.Accept((IVisitor) this);
    }

    public void Visit(LexicalDeclaration node) => node.IfNotNull<LexicalDeclaration>((Action<LexicalDeclaration>) (n => n.Children.ForEach<AstNode>((Action<AstNode>) (v => v.Accept((IVisitor) this)))));

    public void Visit(Lookup node) => node.IfNotNull<Lookup>((Action<Lookup>) (n => this.m_lookups.Add(n)));

    public void Visit(ModuleDeclaration node)
    {
      if (node == null || node.Binding == null)
        return;
      this.m_bindings.Add(node.Binding);
    }

    public void Visit(ObjectLiteral node) => node.IfNotNull<ObjectLiteral>((Action<ObjectLiteral>) (n => AjaxMinExtensions.ForEach<AstNode>(n.Properties, (Action<AstNode>) (p => p.Accept((IVisitor) this)))));

    public void Visit(ObjectLiteralProperty node) => node.IfNotNull<ObjectLiteralProperty>((Action<ObjectLiteralProperty>) (n => n.Value.IfNotNull<AstNode>((Action<AstNode>) (v => v.Accept((IVisitor) this)))));

    public void Visit(ParameterDeclaration node) => node.IfNotNull<ParameterDeclaration>((Action<ParameterDeclaration>) (n => n.Binding.IfNotNull<AstNode>((Action<AstNode>) (b => b.Accept((IVisitor) this)))));

    public void Visit(Var node) => node.IfNotNull<Var>((Action<Var>) (n => n.Children.ForEach<AstNode>((Action<AstNode>) (v => v.Accept((IVisitor) this)))));

    public void Visit(VariableDeclaration node) => node.IfNotNull<VariableDeclaration>((Action<VariableDeclaration>) (n => n.Binding.IfNotNull<AstNode>((Action<AstNode>) (b => b.Accept((IVisitor) this)))));

    private static void ReportError(AstNode node) => node.IfNotNull<AstNode>((Action<AstNode>) (n => n.Context.IfNotNull<Context>((Action<Context>) (c => c.HandleError(JSError.BadBindingSyntax, true)))));

    public void Visit(AspNetBlockNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(BinaryOperator node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(Block node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(Break node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(CallNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ComprehensionNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ComprehensionForClause node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ComprehensionIfClause node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationComment node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationElse node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationElseIf node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationEnd node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationIf node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationOn node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConditionalCompilationSet node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(Conditional node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ConstantWrapperPP node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ContinueNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(DebuggerNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(DirectivePrologue node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(DoWhile node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(EmptyStatement node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ForIn node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ForNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(GetterSetter node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(GroupingOperator node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(IfNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ImportantComment node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(LabeledStatement node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(Member node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ObjectLiteralField node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(RegExpLiteral node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ReturnNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(Switch node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(SwitchCase node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(TemplateLiteral node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(TemplateLiteralExpression node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ThisLiteral node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(ThrowNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(TryNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(UnaryOperator node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(WhileNode node) => BindingsVisitor.ReportError((AstNode) node);

    public void Visit(WithNode node) => BindingsVisitor.ReportError((AstNode) node);
  }
}
