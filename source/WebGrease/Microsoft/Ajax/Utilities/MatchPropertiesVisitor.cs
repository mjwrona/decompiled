// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.MatchPropertiesVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class MatchPropertiesVisitor : IVisitor
  {
    private string[] m_parts;
    private bool m_isMatch;
    private int m_index;

    public bool Match(AstNode node, string identifiers)
    {
      this.m_isMatch = false;
      if (node != null && !string.IsNullOrEmpty(identifiers))
      {
        string[] strArray = identifiers.Split('.');
        bool flag = true;
        foreach (string name in strArray)
        {
          if (!JSScanner.IsValidIdentifier(name))
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          this.m_parts = strArray;
          this.m_index = strArray.Length - 1;
          node.Accept((IVisitor) this);
        }
      }
      return this.m_isMatch;
    }

    public void Visit(CallNode node)
    {
      if (node == null || this.m_index <= 0 || !node.InBrackets || node.Arguments == null || node.Arguments.Count != 1 || !(node.Arguments[0] is ConstantWrapper constantWrapper) || constantWrapper.PrimitiveType != PrimitiveType.String || string.CompareOrdinal(constantWrapper.Value.ToString(), this.m_parts[this.m_index--]) != 0)
        return;
      node.Function.Accept((IVisitor) this);
    }

    public void Visit(Member node)
    {
      if (node == null || this.m_index <= 0 || string.CompareOrdinal(node.Name, this.m_parts[this.m_index--]) != 0)
        return;
      node.Root.Accept((IVisitor) this);
    }

    public void Visit(Lookup node)
    {
      if (node == null || this.m_index != 0 || string.CompareOrdinal(node.Name, this.m_parts[0]) != 0 || node.VariableField != null && node.VariableField.FieldType != FieldType.UndefinedGlobal && node.VariableField.FieldType != FieldType.Global)
        return;
      this.m_isMatch = true;
    }

    public virtual void Visit(GroupingOperator node)
    {
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public void Visit(ArrayLiteral node)
    {
    }

    public void Visit(AspNetBlockNode node)
    {
    }

    public void Visit(AstNodeList node)
    {
    }

    public void Visit(BinaryOperator node)
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

    public void Visit(ClassNode node)
    {
    }

    public void Visit(ComprehensionNode node)
    {
    }

    public void Visit(ComprehensionForClause node)
    {
    }

    public void Visit(ComprehensionIfClause node)
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

    public void Visit(Conditional node)
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

    public void Visit(CustomNode node)
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

    public void Visit(FunctionObject node)
    {
    }

    public void Visit(GetterSetter node)
    {
    }

    public void Visit(IfNode node)
    {
    }

    public void Visit(ImportantComment node)
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

    public void Visit(ObjectLiteral node)
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

    public void Visit(RegExpLiteral node)
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

    public void Visit(TemplateLiteral node)
    {
    }

    public void Visit(TemplateLiteralExpression node)
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

    public void Visit(UnaryOperator node)
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
