// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.RequiresSeparatorVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public class RequiresSeparatorVisitor : IVisitor
  {
    private CodeSettings m_settings;

    public bool DoesRequire { get; private set; }

    public RequiresSeparatorVisitor(CodeSettings settings)
    {
      this.DoesRequire = true;
      this.m_settings = settings ?? new CodeSettings();
    }

    public bool Query(AstNode node)
    {
      this.DoesRequire = node != null;
      node.IfNotNull<AstNode>((Action<AstNode>) (n => n.Accept((IVisitor) this)));
      return this.DoesRequire;
    }

    public void Visit(ArrayLiteral node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(AspNetBlockNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = node.IsTerminatedByExplicitSemicolon;
    }

    public void Visit(AstNodeList node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(BindingIdentifier node)
    {
    }

    public void Visit(Block node)
    {
      if (node == null)
        return;
      if (node.ForceBraces || node.Count > 1)
        this.DoesRequire = false;
      else if (node.Count == 0)
        this.DoesRequire = true;
      else if (node[0] == null)
        this.DoesRequire = true;
      else
        node[0].Accept((IVisitor) this);
    }

    public void Visit(Break node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(CallNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ClassNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ComprehensionNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ComprehensionForClause node)
    {
    }

    public void Visit(ComprehensionIfClause node)
    {
    }

    public void Visit(ConditionalCompilationComment node)
    {
      if (node == null)
        return;
      if (node.Statements.IfNotNull<Block, bool>((Func<Block, bool>) (s => s.Count > 0)))
        node.Statements[node.Statements.Count - 1].Accept((IVisitor) this);
      else
        this.DoesRequire = true;
    }

    public void Visit(ConditionalCompilationElse node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ConditionalCompilationElseIf node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ConditionalCompilationEnd node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ConditionalCompilationIf node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ConditionalCompilationOn node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ConditionalCompilationSet node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(Conditional node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ConstantWrapperPP node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ConstStatement node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ContinueNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(CustomNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = node.RequiresSeparator;
    }

    public void Visit(DebuggerNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(DirectivePrologue node)
    {
      if (node == null)
        return;
      this.DoesRequire = !node.IsRedundant;
    }

    public void Visit(DoWhile node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(EmptyStatement node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ExportNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
      if (node.IsDefault || node.Count != 1 || !(node[0] is FunctionObject) && !(node[0] is ClassNode))
        return;
      this.DoesRequire = false;
    }

    public void Visit(ForIn node)
    {
      if (node == null)
        return;
      if (node.Body == null || node.Body.Count == 0)
        this.DoesRequire = false;
      else
        node.Body.Accept((IVisitor) this);
    }

    public void Visit(ForNode node)
    {
      if (node == null)
        return;
      if (node.Body == null)
        this.DoesRequire = false;
      else
        node.Body.Accept((IVisitor) this);
    }

    public void Visit(FunctionObject node)
    {
      if (node == null)
        return;
      if (node.FunctionType == FunctionType.ArrowFunction && node.Body.IfNotNull<Block, bool>((Func<Block, bool>) (b => b.Count == 1 && !(b[0] is ReturnNode))))
        node.Body[0].Accept((IVisitor) this);
      else
        this.DoesRequire = false;
    }

    public void Visit(GetterSetter node)
    {
    }

    public void Visit(GroupingOperator node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(IfNode node)
    {
      if (node == null)
        return;
      if (node.FalseBlock != null && node.FalseBlock.Count > 0)
        node.FalseBlock.Accept((IVisitor) this);
      else if (node.TrueBlock != null && node.TrueBlock.Count > 0)
        node.TrueBlock.Accept((IVisitor) this);
      else
        this.DoesRequire = false;
    }

    public void Visit(ImportantComment node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(ImportExportSpecifier node)
    {
    }

    public void Visit(ImportNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(InitializerNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(LabeledStatement node)
    {
      if (node == null)
        return;
      if (node.Statement != null)
        node.Statement.Accept((IVisitor) this);
      else
        this.DoesRequire = false;
    }

    public void Visit(LexicalDeclaration node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(Lookup node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(Member node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ModuleDeclaration node)
    {
      if (node == null)
        return;
      this.DoesRequire = node.Binding != null;
    }

    public void Visit(ObjectLiteral node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
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

    public void Visit(RegExpLiteral node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ReturnNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(Switch node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(SwitchCase node)
    {
      if (node == null)
        return;
      if (node.Statements == null || node.Statements.Count == 0)
        this.DoesRequire = false;
      else
        node.Statements[node.Statements.Count - 1].Accept((IVisitor) this);
    }

    public void Visit(TemplateLiteral node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(TemplateLiteralExpression node)
    {
    }

    public void Visit(ThisLiteral node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(ThrowNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = !this.m_settings.MacSafariQuirks;
    }

    public void Visit(TryNode node)
    {
      if (node == null)
        return;
      this.DoesRequire = false;
    }

    public void Visit(Var node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(VariableDeclaration node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      this.DoesRequire = true;
    }

    public void Visit(WhileNode node)
    {
      if (node == null)
        return;
      if (node.Body == null || node.Body.Count == 0)
        this.DoesRequire = false;
      else
        node.Body.Accept((IVisitor) this);
    }

    public void Visit(WithNode node)
    {
      if (node == null)
        return;
      if (node.Body == null || node.Body.Count == 0)
        this.DoesRequire = false;
      else
        node.Body.Accept((IVisitor) this);
    }
  }
}
