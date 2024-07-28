// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.OutputVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  public class OutputVisitor : IVisitor
  {
    private TextWriter m_outputStream;
    private char m_lastCharacter;
    private bool m_lastCountOdd;
    private bool m_onNewLine;
    private bool m_startOfStatement;
    private bool m_outputCCOn;
    private bool m_doneWithGlobalDirectives;
    private bool m_needsStrictDirective;
    private bool m_noLineBreaks;
    private int m_indentLevel;
    private int m_lineLength;
    private int m_lineCount;
    private Stack<string> m_functionStack = new Stack<string>();
    private int m_segmentStartLine;
    private int m_segmentStartColumn;
    private Func<char, bool> m_addSpaceIfTrue;
    private bool m_noIn;
    private bool m_hasReplacementTokens;
    private CodeSettings m_settings;
    private RequiresSeparatorVisitor m_requiresSeparator;

    private OutputVisitor(TextWriter writer, CodeSettings settings)
    {
      this.m_outputStream = writer;
      this.m_settings = settings ?? new CodeSettings();
      this.m_onNewLine = true;
      this.m_requiresSeparator = new RequiresSeparatorVisitor(this.m_settings);
      this.m_hasReplacementTokens = settings.ReplacementTokens.Count > 0;
    }

    public static void Apply(TextWriter writer, AstNode node, CodeSettings settings)
    {
      if (node == null)
        return;
      OutputVisitor outputVisitor = new OutputVisitor(writer, settings);
      node.Accept((IVisitor) outputVisitor);
      settings.IfNotNull<CodeSettings>((Action<CodeSettings>) (s => s.SymbolsMap.IfNotNull<ISourceMap>((Action<ISourceMap>) (m => m.EndOutputRun(outputVisitor.m_lineCount, outputVisitor.m_lineLength)))));
    }

    public static string Apply(AstNode node, CodeSettings settings)
    {
      if (node == null)
        return string.Empty;
      using (StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        OutputVisitor.Apply((TextWriter) writer, node, settings);
        return writer.ToString();
      }
    }

    public void Visit(ArrayLiteral node)
    {
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      if (node != null)
      {
        object symbol = this.StartSymbol((AstNode) node);
        this.OutputPossibleLineBreak('[');
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
        this.m_startOfStatement = false;
        if (node.Elements.Count > 0)
        {
          this.Indent();
          AstNode node1 = (AstNode) null;
          for (int index = 0; index < node.Elements.Count; ++index)
          {
            if (index > 0)
            {
              this.OutputPossibleLineBreak(',');
              this.MarkSegment((AstNode) node, (string) null, node1.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (e => e.TerminatingContext)));
              if (this.m_settings.OutputMode == OutputMode.MultipleLines)
                this.OutputPossibleLineBreak(' ');
            }
            node1 = node.Elements[index];
            if (node1 != null)
              this.AcceptNodeWithParens(node1, node1.Precedence == OperatorPrecedence.Comma);
          }
          this.Unindent();
        }
        this.Output(']');
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.EndSymbol(symbol);
      }
      this.m_noIn = noIn;
    }

    public void Visit(AspNetBlockNode node)
    {
      if (node == null)
        return;
      this.Output(node.AspNetBlockText);
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
    }

    public void Visit(AstNodeList node)
    {
      if (node == null || node.Count <= 0)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool flag = node.Parent is CommaOperator && node.Parent.Parent is Block && this.m_settings.OutputMode == OutputMode.MultipleLines;
      node[0].Accept((IVisitor) this);
      OutputVisitor.SetContextOutputPosition(node.Context, node[0].Context);
      this.m_startOfStatement = false;
      if (!flag)
        this.Indent();
      for (int index = 1; index < node.Count; ++index)
      {
        this.OutputPossibleLineBreak(',');
        this.MarkSegment((AstNode) node, (string) null, node[index - 1].IfNotNull<AstNode, Context>((Func<AstNode, Context>) (n => n.TerminatingContext)));
        if (flag)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node[index].Accept((IVisitor) this);
      }
      if (!flag)
        this.Unindent();
      this.EndSymbol(symbol);
    }

    public void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (node.OperatorToken == JSToken.Comma)
      {
        if (node.Operand1 != null)
        {
          node.Operand1.Accept((IVisitor) this);
          OutputVisitor.SetContextOutputPosition(node.Context, node.Operand1.Context);
          if (node.Operand2 != null)
          {
            this.OutputPossibleLineBreak(',');
            this.MarkSegment((AstNode) node, (string) null, node.Operand1.TerminatingContext);
            this.m_startOfStatement = false;
            if (node.Parent is Block)
              this.NewLine();
            else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
              this.OutputPossibleLineBreak(' ');
          }
        }
        if (node.Operand2 == null)
          return;
        node.Operand2.Accept((IVisitor) this);
        this.m_startOfStatement = false;
      }
      else
      {
        OperatorPrecedence precedence1 = node.Precedence;
        bool noIn = this.m_noIn;
        if (noIn)
        {
          if (node.OperatorToken == JSToken.In)
          {
            this.OutputPossibleLineBreak('(');
            this.m_noIn = false;
          }
          else
            this.m_noIn = precedence1 <= OperatorPrecedence.Relational;
        }
        if (node.Operand1 != null)
        {
          this.AcceptNodeWithParens(node.Operand1, node.Operand1.Precedence < precedence1);
          OutputVisitor.SetContextOutputPosition(node.Context, node.Operand1.Context);
        }
        this.m_startOfStatement = false;
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        {
          if (node.OperatorToken != JSToken.Comma)
            this.OutputPossibleLineBreak(' ');
          this.Output(OutputVisitor.OperatorString(node.OperatorToken));
          this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
          this.BreakLine(false);
          if (!this.m_onNewLine)
            this.OutputPossibleLineBreak(' ');
        }
        else
        {
          this.Output(OutputVisitor.OperatorString(node.OperatorToken));
          this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
          this.BreakLine(false);
        }
        if (node.OperatorToken == JSToken.Divide)
          this.m_addSpaceIfTrue = (Func<char, bool>) (c => c == '/');
        if (node.Operand2 != null)
        {
          OperatorPrecedence precedence2 = node.Operand2.Precedence;
          bool needsParens = precedence2 < precedence1;
          if (node.Operand2 is BinaryOperator operand2)
          {
            if (precedence1 == precedence2 && precedence1 != OperatorPrecedence.Assignment && precedence1 != OperatorPrecedence.Comma)
            {
              if (node.OperatorToken == operand2.OperatorToken)
              {
                switch (node.OperatorToken)
                {
                  case JSToken.Multiply:
                  case JSToken.BitwiseAnd:
                  case JSToken.BitwiseOr:
                  case JSToken.BitwiseXor:
                  case JSToken.LogicalAnd:
                  case JSToken.LogicalOr:
                    needsParens = false;
                    break;
                  default:
                    needsParens = true;
                    break;
                }
              }
              else
                needsParens = true;
            }
            else
              needsParens = precedence2 < precedence1;
          }
          this.m_noIn = noIn && precedence1 <= OperatorPrecedence.Relational;
          this.AcceptNodeWithParens(node.Operand2, needsParens);
        }
        if (noIn && node.OperatorToken == JSToken.In)
          this.OutputPossibleLineBreak(')');
        this.m_noIn = noIn;
        this.EndSymbol(symbol);
      }
    }

    public void Visit(BindingIdentifier node)
    {
      if (node == null)
        return;
      this.Output(node.VariableField != null ? node.VariableField.ToString() : node.Name);
      this.MarkSegment((AstNode) node, node.Name, node.Context);
      this.SetContextOutputPosition(node.Context);
      node.VariableField.IfNotNull<JSVariableField>((Action<JSVariableField>) (f => this.SetContextOutputPosition(f.OriginalContext)));
    }

    public void Visit(Block node)
    {
      if (node == null)
        return;
      object symbol = node.Parent != null ? this.StartSymbol((AstNode) node) : (object) null;
      bool flag = true;
      if (node.Parent != null)
      {
        AstNode parent1 = node.Parent;
        if (parent1 is FunctionObject && parent1.EnclosingScope.UseStrict && !parent1.EnclosingScope.Parent.UseStrict)
          this.m_needsStrictDirective = true;
        else if (node.Parent is ModuleDeclaration parent2 && parent2.IsImplicit)
        {
          this.m_needsStrictDirective = false;
          flag = false;
        }
        if (flag)
        {
          this.OutputPossibleLineBreak('{');
          this.SetContextOutputPosition(node.Context);
          this.MarkSegment((AstNode) node, (string) null, node.Context);
          this.Indent();
        }
      }
      else
      {
        this.m_needsStrictDirective = node.EnclosingScope.IfNotNull<ActivationObject, bool>((Func<ActivationObject, bool>) (s => s.UseStrict)) && !this.m_doneWithGlobalDirectives;
        flag = false;
      }
      AstNode node1 = (AstNode) null;
      for (int index = 0; index < node.Count; ++index)
      {
        AstNode astNode = node[index];
        if (astNode != null)
        {
          if (node1 != null && this.m_requiresSeparator.Query(node1))
          {
            this.OutputPossibleLineBreak(';');
            this.MarkSegment(node1, (string) null, node1.TerminatingContext);
          }
          if (!(astNode is DirectivePrologue))
          {
            if (this.m_needsStrictDirective)
            {
              this.Output("\"use strict\";");
              this.m_needsStrictDirective = false;
            }
            this.m_doneWithGlobalDirectives = true;
          }
          this.NewLine();
          this.m_startOfStatement = true;
          astNode.Accept((IVisitor) this);
          node1 = astNode;
        }
      }
      if (flag)
      {
        this.Unindent();
        if (node.Count > 0)
          this.NewLine();
        this.OutputPossibleLineBreak('}');
        this.MarkSegment((AstNode) node, (string) null, node.Context);
      }
      else if (node1 != null && this.m_requiresSeparator.Query(node1) && this.m_settings.TermSemicolons)
      {
        this.OutputPossibleLineBreak(';');
        this.MarkSegment(node1, (string) null, node1.TerminatingContext);
      }
      if (symbol == null)
        return;
      this.EndSymbol(symbol);
    }

    public void Visit(Break node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("break");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (!node.Label.IsNullOrWhiteSpace())
      {
        this.m_noLineBreaks = true;
        if (node.LabelInfo.IfNotNull<LabelInfo, bool>((Func<LabelInfo, bool>) (li => !li.MinLabel.IsNullOrWhiteSpace())))
          this.Output(node.LabelInfo.MinLabel);
        else
          this.Output(node.Label);
        this.MarkSegment((AstNode) node, (string) null, node.LabelContext);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(CallNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      if (node.IsConstructor)
      {
        this.Output("new");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
        this.m_startOfStatement = false;
      }
      if (node.Function != null)
      {
        bool needsParens = node.Function.Precedence < node.Precedence;
        if (!needsParens)
        {
          if (node.IsConstructor)
            needsParens = NewParensVisitor.NeedsParens(node.Function, node.Arguments == null || node.Arguments.Count == 0);
          else if (node.Function is CallNode function && function.IsConstructor && (function.Arguments == null || function.Arguments.Count == 0))
            needsParens = true;
        }
        this.AcceptNodeWithParens(node.Function, needsParens);
        if (!node.IsConstructor)
          this.SetContextOutputPosition(node.Context);
      }
      if (!node.IsConstructor || node.Arguments.Count > 0)
      {
        this.OutputPossibleLineBreak(node.InBrackets ? '[' : '(');
        this.MarkSegment((AstNode) node, (string) null, node.Arguments.Context);
        AstNode node1 = (AstNode) null;
        for (int index = 0; index < node.Arguments.Count; ++index)
        {
          if (index > 0)
          {
            this.OutputPossibleLineBreak(',');
            this.MarkSegment((AstNode) node.Arguments, (string) null, node1.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (a => a.TerminatingContext)) ?? node.Arguments.Context);
            if (this.m_settings.OutputMode == OutputMode.MultipleLines)
              this.OutputPossibleLineBreak(' ');
          }
          node1 = node.Arguments[index];
          if (node1 != null)
            this.AcceptNodeWithParens(node1, node1.Precedence <= OperatorPrecedence.Comma);
        }
        this.Output(node.InBrackets ? ']' : ')');
        this.MarkSegment((AstNode) node, (string) null, node.Arguments.Context);
      }
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    public void Visit(ClassNode node)
    {
      if (node == null)
        return;
      bool flag = this.m_startOfStatement && node.ClassType != ClassType.Declaration;
      if (flag)
      {
        this.OutputPossibleLineBreak('(');
        this.m_startOfStatement = false;
      }
      this.Output("class");
      this.MarkSegment((AstNode) node, (string) null, node.ClassContext);
      this.SetContextOutputPosition(node.ClassContext);
      this.m_startOfStatement = false;
      if (node.Binding != null)
      {
        if (node.Binding is BindingIdentifier binding)
        {
          if (node.ClassType != ClassType.Expression || binding.VariableField.IsReferenced || !this.m_settings.RemoveFunctionExpressionNames)
            node.Binding.Accept((IVisitor) this);
        }
        else
          node.Binding.Accept((IVisitor) this);
      }
      if (node.Heritage != null)
      {
        this.Output("extends");
        this.MarkSegment((AstNode) node, (string) null, node.ExtendsContext);
        this.SetContextOutputPosition(node.ExtendsContext);
        node.Heritage.Accept((IVisitor) this);
      }
      node.Elements.IfNotNull<AstNodeList>((Action<AstNodeList>) (e =>
      {
        if (e.Count <= 0)
          return;
        this.NewLine();
        this.Indent();
      }));
      this.OutputPossibleLineBreak('{');
      this.MarkSegment((AstNode) node, (string) null, node.OpenBrace);
      this.SetContextOutputPosition(node.OpenBrace);
      if (node.Elements != null && node.Elements.Count > 0)
      {
        foreach (AstNode element in node.Elements)
        {
          this.NewLine();
          element.Accept((IVisitor) this);
        }
      }
      node.Elements.IfNotNull<AstNodeList>((Action<AstNodeList>) (e =>
      {
        if (e.Count <= 0)
          return;
        this.Unindent();
        this.NewLine();
      }));
      this.OutputPossibleLineBreak('}');
      this.MarkSegment((AstNode) node, (string) null, node.CloseBrace);
      this.SetContextOutputPosition(node.CloseBrace);
      if (!flag)
        return;
      this.OutputPossibleLineBreak(')');
    }

    public void Visit(ComprehensionNode node)
    {
      if (node == null)
        return;
      this.OutputPossibleLineBreak(node.ComprehensionType == ComprehensionType.Generator ? '(' : '[');
      this.MarkSegment((AstNode) node, (string) null, node.OpenDelimiter);
      if (node.MozillaOrdering)
      {
        if (node.Expression != null)
          node.Expression.Accept((IVisitor) this);
        foreach (AstNode clause in node.Clauses)
          clause.Accept((IVisitor) this);
      }
      else
      {
        foreach (AstNode clause in node.Clauses)
          clause.Accept((IVisitor) this);
        if (node.Expression != null)
          node.Expression.Accept((IVisitor) this);
      }
      this.OutputPossibleLineBreak(node.ComprehensionType == ComprehensionType.Generator ? ')' : ']');
      this.MarkSegment((AstNode) node, (string) null, node.CloseDelimiter);
    }

    public void Visit(ComprehensionForClause node)
    {
      if (node == null)
        return;
      this.Output("for");
      this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
      this.OutputPossibleLineBreak('(');
      this.MarkSegment((AstNode) node, (string) null, node.OpenContext);
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      this.Output(node.IsInOperation ? "in" : "of");
      this.MarkSegment((AstNode) node, (string) null, node.OfContext);
      if (node.Expression != null)
        node.Expression.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.MarkSegment((AstNode) node, (string) null, node.CloseContext);
    }

    public void Visit(ComprehensionIfClause node)
    {
      if (node == null)
        return;
      this.Output("if");
      this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
      this.OutputPossibleLineBreak('(');
      this.MarkSegment((AstNode) node, (string) null, node.OpenContext);
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.MarkSegment((AstNode) node, (string) null, node.CloseContext);
    }

    public void Visit(ConditionalCompilationComment node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      int index = 0;
      if (this.m_outputCCOn && this.m_settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryCCOnStatements))
      {
        while (index < node.Statements.Count && node.Statements[index] is ConditionalCompilationOn)
          ++index;
      }
      if (index < node.Statements.Count)
      {
        this.Output("/*");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
        AstNode statement1 = node.Statements[index];
        switch (statement1)
        {
          case ConditionalCompilationStatement _:
          case ConstantWrapperPP _:
            statement1.Accept((IVisitor) this);
            break;
          default:
            this.OutputPossibleLineBreak('@');
            statement1.Accept((IVisitor) this);
            break;
        }
        AstNode node1 = statement1;
        while (++index < node.Statements.Count)
        {
          AstNode statement2 = node.Statements[index];
          if (statement2 != null)
          {
            if (node1 != null && this.m_requiresSeparator.Query(node1))
            {
              this.OutputPossibleLineBreak(';');
              this.MarkSegment(node1, (string) null, node1.TerminatingContext);
            }
            this.NewLine();
            this.m_startOfStatement = true;
            statement2.Accept((IVisitor) this);
            node1 = statement2;
          }
        }
        this.Output("@*/");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationElse node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("@else");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationElseIf node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("@elif(");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationEnd node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("@end");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationIf node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("@if(");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationOn node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (!this.m_outputCCOn || !this.m_settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryCCOnStatements))
      {
        this.m_outputCCOn = true;
        this.Output("@cc_on");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(ConditionalCompilationSet node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("@set");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Output(node.VariableName);
      this.Output('=');
      if (node.Value is BinaryOperator || node.Value is UnaryOperator)
      {
        this.Output('(');
        node.Value.Accept((IVisitor) this);
        this.OutputPossibleLineBreak(')');
      }
      else if (node.Value != null)
        node.Value.Accept((IVisitor) this);
      this.EndSymbol(symbol);
    }

    public void Visit(Conditional node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      if (node.Condition != null)
      {
        this.AcceptNodeWithParens(node.Condition, node.Condition.Precedence < OperatorPrecedence.LogicalOr);
        this.SetContextOutputPosition(node.Context);
      }
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
      {
        this.OutputPossibleLineBreak(' ');
        this.OutputPossibleLineBreak('?');
        this.MarkSegment((AstNode) node, (string) null, node.QuestionContext ?? node.Context);
        this.BreakLine(false);
        if (!this.m_onNewLine)
          this.OutputPossibleLineBreak(' ');
      }
      else
      {
        this.OutputPossibleLineBreak('?');
        this.MarkSegment((AstNode) node, (string) null, node.QuestionContext ?? node.Context);
      }
      this.m_startOfStatement = false;
      if (node.TrueExpression != null)
      {
        this.m_noIn = noIn;
        this.AcceptNodeWithParens(node.TrueExpression, node.TrueExpression.Precedence < OperatorPrecedence.Assignment);
      }
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
      {
        this.OutputPossibleLineBreak(' ');
        this.OutputPossibleLineBreak(':');
        this.MarkSegment((AstNode) node, (string) null, node.ColonContext ?? node.Context);
        this.BreakLine(false);
        if (!this.m_onNewLine)
          this.OutputPossibleLineBreak(' ');
      }
      else
      {
        this.OutputPossibleLineBreak(':');
        this.MarkSegment((AstNode) node, (string) null, node.ColonContext ?? node.Context);
      }
      if (node.FalseExpression != null)
      {
        this.m_noIn = noIn;
        this.AcceptNodeWithParens(node.FalseExpression, node.FalseExpression.Precedence < OperatorPrecedence.Assignment);
      }
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    public void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      switch (node.PrimitiveType)
      {
        case PrimitiveType.Null:
          this.Output("null");
          break;
        case PrimitiveType.Boolean:
          this.Output(node.ToBoolean() ? "true" : "false");
          break;
        case PrimitiveType.Number:
          if (node.Context == null || !node.Context.HasCode || !node.MayHaveIssues && this.m_settings.IsModificationAllowed(TreeModifications.MinifyNumericLiterals))
          {
            this.Output(OutputVisitor.NormalizeNumber(node.ToNumber(), node.Context));
            break;
          }
          this.Output(node.Context.Code);
          break;
        case PrimitiveType.String:
          if (node.Context == null || !node.Context.HasCode)
          {
            this.Output(this.InlineSafeString(OutputVisitor.EscapeString(this.ReplaceTokens(node.Value.ToString()))));
            break;
          }
          if (!this.m_settings.IsModificationAllowed(TreeModifications.MinifyStringLiterals))
          {
            this.Output(this.ReplaceTokens(node.Context.Code));
            break;
          }
          if (node.MayHaveIssues || this.m_settings.AllowEmbeddedAspNetBlocks && node.StringContainsAspNetReplacement)
          {
            this.Output(this.InlineSafeString(this.ReplaceTokens(node.Context.Code)));
            break;
          }
          this.Output(this.InlineSafeString(OutputVisitor.EscapeString(this.ReplaceTokens(node.Value.ToString()))));
          break;
        case PrimitiveType.Other:
          Match match;
          if (this.m_hasReplacementTokens && (match = CommonData.ReplacementToken.Match(node.Value.ToString())).Success && match.Value.Equals(node.Value))
          {
            this.Output(this.GetSyntacticReplacementToken(match));
            break;
          }
          this.Output(node.Value.ToString());
          break;
      }
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    private string ReplaceTokens(string text)
    {
      if (this.m_hasReplacementTokens)
        text = CommonData.ReplacementToken.Replace(text, new MatchEvaluator(this.GetReplacementToken));
      return text;
    }

    private string GetReplacementToken(Match match)
    {
      string str1;
      if (!this.m_settings.ReplacementTokens.TryGetValue(match.Result("${token}"), out str1))
      {
        string str2 = match.Result("${fallback}");
        if (!str2.IsNullOrWhiteSpace())
          this.m_settings.ReplacementFallbacks.TryGetValue(str2, out str1);
      }
      return str1 ?? string.Empty;
    }

    private string GetSyntacticReplacementToken(Match match)
    {
      string str1;
      if (this.m_settings.ReplacementTokens.TryGetValue(match.Result("${token}"), out str1))
      {
        string text = JSON.Validate(str1);
        str1 = text.IsNullOrWhiteSpace() ? this.InlineSafeString(OutputVisitor.EscapeString(str1)) : text;
      }
      else
      {
        string str2 = match.Result("${fallback}");
        if (!str2.IsNullOrWhiteSpace())
          this.m_settings.ReplacementFallbacks.TryGetValue(str2, out str1);
      }
      return str1 ?? string.Empty;
    }

    public void Visit(ConstantWrapperPP node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (node.ForceComments)
        this.Output("/*");
      this.Output(node.VarName);
      this.m_startOfStatement = false;
      this.SetContextOutputPosition(node.Context);
      if (node.ForceComments)
        this.Output("@*/");
      this.EndSymbol(symbol);
    }

    public void Visit(ConstStatement node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("const");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Indent();
      for (int index = 0; index < node.Count; ++index)
      {
        VariableDeclaration variableDeclaration = node[index];
        if (variableDeclaration != null)
        {
          if (index > 0)
          {
            this.OutputPossibleLineBreak(',');
            this.NewLine();
          }
          variableDeclaration.Accept((IVisitor) this);
        }
      }
      this.Unindent();
      this.EndSymbol(symbol);
    }

    public void Visit(ContinueNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("continue");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (!node.Label.IsNullOrWhiteSpace())
      {
        this.m_noLineBreaks = true;
        if (node.LabelInfo.IfNotNull<LabelInfo, bool>((Func<LabelInfo, bool>) (li => !li.MinLabel.IsNullOrWhiteSpace())))
          this.Output(node.LabelInfo.MinLabel);
        else
          this.Output(node.Label);
        this.MarkSegment((AstNode) node, (string) null, node.LabelContext);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(CustomNode node)
    {
      if (node == null)
        return;
      string code = node.ToCode();
      if (code.IsNullOrWhiteSpace())
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output(code);
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.EndSymbol(symbol);
    }

    public void Visit(DebuggerNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("debugger");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.EndSymbol(symbol);
    }

    public void Visit(DirectivePrologue node)
    {
      if (node == null)
        return;
      node.IsRedundant = node.UseStrict && !this.m_needsStrictDirective;
      if (node.IsRedundant)
        return;
      this.Visit((ConstantWrapper) node);
      if (!node.UseStrict)
        return;
      this.m_needsStrictDirective = false;
    }

    public void Visit(DoWhile node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("do");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (node.Body == null || node.Body.Count == 0)
        this.OutputPossibleLineBreak(';');
      else if (node.Body.Count == 1 && !node.Body.EncloseBlock(EncloseBlockType.SingleDoWhile))
      {
        this.Indent();
        this.NewLine();
        this.m_startOfStatement = true;
        node.Body[0].Accept((IVisitor) this);
        if (this.m_requiresSeparator.Query(node.Body[0]) && this.ReplaceableSemicolon())
          this.MarkSegment(node.Body[0], (string) null, node.Body[0].TerminatingContext);
        this.Unindent();
        this.NewLine();
      }
      else
      {
        if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.Body.BraceOnNewLine)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node.Body.Accept((IVisitor) this);
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
      }
      this.Output("while");
      this.MarkSegment((AstNode) node, (string) null, node.WhileContext);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.Output(')');
      this.EndSymbol(symbol);
    }

    public void Visit(EmptyStatement node)
    {
      if (node == null)
        return;
      this.OutputPossibleLineBreak(';');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
    }

    public void Visit(ExportNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("export");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.SetContextOutputPosition(node.KeywordContext);
      this.m_startOfStatement = false;
      if (node.IsDefault)
      {
        this.Output("default");
        if (node.Count > 0)
          node[0].Accept((IVisitor) this);
      }
      else if (node.Count == 1 && (node[0] is Declaration || node[0] is FunctionObject || node[0] is ClassNode))
      {
        node[0].Accept((IVisitor) this);
      }
      else
      {
        if (node.Count == 0)
        {
          this.OutputPossibleLineBreak('*');
          this.SetContextOutputPosition(node.OpenContext);
        }
        else
        {
          this.OutputPossibleLineBreak('{');
          this.SetContextOutputPosition(node.OpenContext);
          bool flag = true;
          foreach (AstNode child in node.Children)
          {
            if (flag)
              flag = false;
            else
              this.OutputPossibleLineBreak(',');
            child.Accept((IVisitor) this);
          }
          this.OutputPossibleLineBreak('}');
          this.SetContextOutputPosition(node.CloseContext);
        }
        if (node.ModuleName != null)
        {
          this.Output("from");
          this.SetContextOutputPosition(node.FromContext);
          this.Output(OutputVisitor.EscapeString(node.ModuleName));
          this.SetContextOutputPosition(node.ModuleContext);
        }
      }
      this.EndSymbol(symbol);
    }

    public void Visit(ForIn node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("for");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Variable != null)
      {
        this.m_noIn = true;
        node.Variable.Accept((IVisitor) this);
        this.m_noIn = false;
      }
      if (node.OperatorContext != null && !node.OperatorContext.Code.IsNullOrWhiteSpace())
        this.Output(node.OperatorContext.Code);
      else
        this.Output("in");
      this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
      if (node.Collection != null)
        node.Collection.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.OutputBlock(node.Body);
      this.EndSymbol(symbol);
    }

    public void Visit(ForNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("for");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Initializer != null)
      {
        this.m_noIn = true;
        node.Initializer.Accept((IVisitor) this);
        this.m_noIn = false;
      }
      this.OutputPossibleLineBreak(';');
      this.MarkSegment((AstNode) node, (string) null, node.Separator1Context ?? node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(';');
      this.MarkSegment((AstNode) node, (string) null, node.Separator2Context ?? node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      if (node.Incrementer != null)
        node.Incrementer.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.OutputBlock(node.Body);
      this.EndSymbol(symbol);
    }

    private void OutputFunctionPrefix(FunctionObject node, string functionName)
    {
      if (node.FunctionType == FunctionType.Method)
      {
        if (!node.IsGenerator)
          return;
        this.Output('*');
        this.MarkSegment((AstNode) node, functionName, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      else if (node.FunctionType == FunctionType.Getter)
      {
        this.Output("get");
        this.MarkSegment((AstNode) node, functionName, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      else if (node.FunctionType == FunctionType.Setter)
      {
        this.Output("set");
        this.MarkSegment((AstNode) node, functionName, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      else
      {
        this.Output("function");
        this.MarkSegment((AstNode) node, functionName, node.Context);
        this.SetContextOutputPosition(node.Context);
        if (!node.IsGenerator)
          return;
        this.Output('*');
      }
    }

    public void Visit(FunctionObject node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      if (node.FunctionType == FunctionType.ArrowFunction)
      {
        this.OutputFunctionArgsAndBody(node);
      }
      else
      {
        bool flag1 = node.IsExpression && this.m_startOfStatement;
        if (flag1)
          this.OutputPossibleLineBreak('(');
        bool flag2 = node.Binding != null && !node.Binding.Name.IsNullOrWhiteSpace() && (node.FunctionType != FunctionType.Expression || node.Binding.VariableField.RefCount > 0 || !this.m_settings.RemoveFunctionExpressionNames || !this.m_settings.IsModificationAllowed(TreeModifications.RemoveFunctionExpressionNames));
        string functionName = flag2 ? node.Binding.Name : node.NameGuess;
        if (node.IsStatic)
          this.Output("static");
        this.OutputFunctionPrefix(node, functionName);
        this.m_startOfStatement = false;
        bool flag3 = true;
        if (node.Binding != null && !node.Binding.Name.IsNullOrWhiteSpace())
        {
          flag3 = false;
          string text = node.Binding.VariableField != null ? node.Binding.VariableField.ToString() : node.Binding.Name;
          if (this.m_settings.SymbolsMap != null)
            this.m_functionStack.Push(text);
          if (flag2)
          {
            if (JSScanner.IsValidIdentifierPart(this.m_lastCharacter))
              this.Output(' ');
            this.Output(text);
            this.MarkSegment((AstNode) node, node.Binding.Name, node.Binding.Context);
            this.SetContextOutputPosition(node.Context);
          }
        }
        if (this.m_settings.SymbolsMap != null && flag3)
        {
          if (node.Parent is BinaryOperator parent && parent.Operand1 is Lookup)
            this.m_functionStack.Push("(anonymous) [{0}]".FormatInvariant((object) parent.Operand1));
          else
            this.m_functionStack.Push("(anonymous)");
        }
        this.OutputFunctionArgsAndBody(node);
        if (flag1)
          this.OutputPossibleLineBreak(')');
      }
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
      if (this.m_settings.SymbolsMap == null)
        return;
      this.m_functionStack.Pop();
    }

    public void Visit(GetterSetter node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output(node.IsGetter ? "get" : "set");
      this.MarkSegment((AstNode) node, node.Value.ToString(), node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Output(node.Value.ToString());
      this.EndSymbol(symbol);
    }

    public virtual void Visit(GroupingOperator node)
    {
      if (node == null)
        return;
      this.Output('(');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (node.Operand != null)
        node.Operand.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
    }

    public void Visit(IfNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("if");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      if (node.TrueBlock != null && node.TrueBlock.ForceBraces)
        this.OutputBlockWithBraces(node.TrueBlock);
      else if (node.TrueBlock == null || node.TrueBlock.Count == 0)
        this.OutputPossibleLineBreak(';');
      else if (node.TrueBlock.Count == 1 && (node.FalseBlock == null || !node.TrueBlock.EncloseBlock(EncloseBlockType.IfWithoutElse) && !node.TrueBlock.EncloseBlock(EncloseBlockType.SingleDoWhile)) && (!this.m_settings.MacSafariQuirks || !(node.TrueBlock[0] is FunctionObject)))
      {
        this.Indent();
        this.NewLine();
        this.m_startOfStatement = true;
        node.TrueBlock[0].Accept((IVisitor) this);
        if (node.TrueBlock[0] is ImportantComment)
          this.OutputPossibleLineBreak(';');
        if (node.FalseBlock != null && node.FalseBlock.Count > 0 && this.m_requiresSeparator.Query(node.TrueBlock[0]) && this.ReplaceableSemicolon())
          this.MarkSegment(node.TrueBlock[0], (string) null, node.TrueBlock[0].TerminatingContext);
        this.Unindent();
      }
      else
        this.OutputBlockWithBraces(node.TrueBlock);
      if (node.FalseBlock != null && (node.FalseBlock.Count > 0 || node.FalseBlock.ForceBraces))
      {
        this.NewLine();
        this.Output("else");
        this.MarkSegment((AstNode) node, (string) null, node.ElseContext);
        if (node.FalseBlock.Count == 1 && !node.FalseBlock.ForceBraces)
        {
          AstNode astNode = node.FalseBlock[0];
          if (astNode is IfNode)
          {
            astNode.Accept((IVisitor) this);
          }
          else
          {
            this.Indent();
            this.NewLine();
            this.m_startOfStatement = true;
            astNode.Accept((IVisitor) this);
            this.Unindent();
          }
        }
        else
          this.OutputBlockWithBraces(node.FalseBlock);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(ImportantComment node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.BreakLine(true);
      node.Context.OutputLine = this.m_lineCount;
      char[] anyOf = new char[4]
      {
        '\n',
        '\r',
        '\u2028',
        '\u2029'
      };
      int startIndex1 = 0;
      int num = node.Comment.IndexOfAny(anyOf, startIndex1);
      if (num < 0)
      {
        this.Output(node.Comment);
      }
      else
      {
        this.Output(node.Comment.Substring(0, num));
        int startIndex2;
        do
        {
          startIndex2 = node.Comment[num] != '\r' || num >= node.Comment.Length - 1 || node.Comment[num + 1] != '\n' ? num + 1 : num + 2;
          this.BreakLine(true);
          num = node.Comment.IndexOfAny(anyOf, startIndex2);
          if (num > startIndex2)
            this.Output(node.Comment.Substring(startIndex2, num - startIndex2));
        }
        while (num >= 0);
        this.Output(node.Comment.Substring(startIndex2));
      }
      this.BreakLine(true);
      this.EndSymbol(symbol);
    }

    public void Visit(ImportExportSpecifier node)
    {
      if (node == null)
        return;
      if (node.Parent is ImportNode)
      {
        if (!node.ExternalName.IsNullOrWhiteSpace())
        {
          this.Output(node.ExternalName);
          this.SetContextOutputPosition(node.Context);
          this.SetContextOutputPosition(node.NameContext);
          this.Output("as");
          this.SetContextOutputPosition(node.AsContext);
        }
        if (node.LocalIdentifier == null)
          return;
        node.LocalIdentifier.Accept((IVisitor) this);
        if (!node.ExternalName.IsNullOrWhiteSpace())
          return;
        this.SetContextOutputPosition(node.Context);
      }
      else
      {
        if (node.LocalIdentifier != null)
        {
          node.LocalIdentifier.Accept((IVisitor) this);
          this.SetContextOutputPosition(node.Context);
        }
        if (node.ExternalName.IsNullOrWhiteSpace())
          return;
        this.Output("as");
        this.SetContextOutputPosition(node.AsContext);
        this.Output(node.ExternalName);
        this.SetContextOutputPosition(node.NameContext);
      }
    }

    public void Visit(ImportNode node)
    {
      if (node == null)
        return;
      this.Output("import");
      this.SetContextOutputPosition(node.Context);
      this.SetContextOutputPosition(node.KeywordContext);
      this.m_startOfStatement = false;
      if (node.Count > 0)
      {
        if (node.Count == 1 && node[0] is BindingIdentifier)
        {
          node[0].Accept((IVisitor) this);
        }
        else
        {
          this.OutputPossibleLineBreak('{');
          this.SetContextOutputPosition(node.OpenContext);
          bool flag = true;
          foreach (AstNode child in node.Children)
          {
            if (flag)
              flag = false;
            else
              this.OutputPossibleLineBreak(',');
            child.Accept((IVisitor) this);
          }
          this.OutputPossibleLineBreak('}');
          this.SetContextOutputPosition(node.CloseContext);
        }
        this.Output("from");
        this.SetContextOutputPosition(node.FromContext);
      }
      if (node.ModuleName == null)
        return;
      this.Output(OutputVisitor.EscapeString(node.ModuleName));
      this.SetContextOutputPosition(node.ModuleContext);
    }

    public void Visit(InitializerNode node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      this.OutputPossibleLineBreak('=');
      this.MarkSegment((AstNode) node, (string) null, node.AssignContext);
      if (node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public void Visit(LabeledStatement node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (!node.Label.IsNullOrWhiteSpace())
      {
        if (node.LabelInfo.IfNotNull<LabelInfo, bool>((Func<LabelInfo, bool>) (li => !li.MinLabel.IsNullOrWhiteSpace())))
          this.Output(node.LabelInfo.MinLabel);
        else
          this.Output(node.Label);
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
        this.OutputPossibleLineBreak(':');
        this.MarkSegment((AstNode) node, (string) null, node.ColonContext);
      }
      if (node.Statement != null)
      {
        this.m_startOfStatement = true;
        node.Statement.Accept((IVisitor) this);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(LexicalDeclaration node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.Output(OutputVisitor.OperatorString(node.StatementToken));
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Indent();
      bool flag = !(node.Parent is ForNode);
      for (int index = 0; index < node.Count; ++index)
      {
        VariableDeclaration variableDeclaration = node[index];
        if (variableDeclaration != null)
        {
          if (index > 0)
          {
            this.OutputPossibleLineBreak(',');
            if (flag)
              this.NewLine();
            else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
              this.OutputPossibleLineBreak(' ');
          }
          this.m_noIn = noIn;
          variableDeclaration.Accept((IVisitor) this);
        }
      }
      this.Unindent();
      this.EndSymbol(symbol);
    }

    public void Visit(Lookup node)
    {
      if (node == null)
        return;
      if (JSScanner.IsValidIdentifierPart(this.m_lastCharacter))
        this.OutputSpaceOrLineBreak();
      object symbol = this.StartSymbol((AstNode) node);
      this.Output(node.VariableField != null ? node.VariableField.ToString() : node.Name);
      this.MarkSegment((AstNode) node, node.Name, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.EndSymbol(symbol);
    }

    public void Visit(Member node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      if (node.Root != null)
      {
        if (node.Root is ConstantWrapper root1 && (root1.IsFiniteNumericLiteral || root1.IsOtherDecimal))
        {
          string text = root1.Context == null || !root1.Context.HasCode || this.m_settings.IsModificationAllowed(TreeModifications.MinifyNumericLiterals) && !root1.MayHaveIssues ? OutputVisitor.NormalizeNumber(root1.ToNumber(), root1.Context) : root1.Context.Code;
          if (text.StartsWith("-", StringComparison.Ordinal))
          {
            this.Output('(');
            this.Output(text);
            this.Output(')');
          }
          else
          {
            this.Output(text);
            if (text.IndexOf('.') < 0 && text.IndexOf("e", StringComparison.OrdinalIgnoreCase) < 0)
            {
              bool flag = !text.StartsWith("0", StringComparison.Ordinal) || text.Length == 1;
              if (!flag && JSScanner.IsDigit(text[1]))
              {
                for (int index = 1; index < text.Length; ++index)
                {
                  if ('7' < text[index])
                  {
                    flag = true;
                    break;
                  }
                }
              }
              if (flag)
                this.Output('.');
            }
          }
        }
        else
        {
          bool needsParens = node.Root.Precedence < node.Precedence;
          if (!needsParens && node.Root is CallNode root && root.IsConstructor && (root.Arguments == null || root.Arguments.Count == 0))
            needsParens = true;
          this.AcceptNodeWithParens(node.Root, needsParens);
        }
        this.SetContextOutputPosition(node.Context);
      }
      this.OutputPossibleLineBreak('.');
      this.MarkSegment((AstNode) node, node.Name, node.NameContext);
      this.Output(node.Name);
      this.m_startOfStatement = false;
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    public void Visit(ModuleDeclaration node)
    {
      if (node == null)
        return;
      if (node.IsImplicit)
      {
        if (node.Body == null)
          return;
        node.Body.Accept((IVisitor) this);
      }
      else
      {
        this.Output("module");
        this.SetContextOutputPosition(node.Context);
        this.SetContextOutputPosition(node.ModuleContext);
        if (node.Binding != null)
        {
          node.Binding.Accept((IVisitor) this);
          this.Output("from");
          this.SetContextOutputPosition(node.FromContext);
          if (node.ModuleName == null)
            return;
          this.Output(OutputVisitor.EscapeString(node.ModuleName));
          this.SetContextOutputPosition(node.ModuleContext);
        }
        else
        {
          this.m_noLineBreaks = true;
          if (node.ModuleName != null)
          {
            this.Output(OutputVisitor.EscapeString(node.ModuleName));
            this.SetContextOutputPosition(node.ModuleContext);
          }
          if (node.Body != null)
            node.Body.Accept((IVisitor) this);
          else
            this.Output("{}");
        }
      }
    }

    public void Visit(ObjectLiteral node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      bool startOfStatement = this.m_startOfStatement;
      if (startOfStatement)
        this.OutputPossibleLineBreak('(');
      this.OutputPossibleLineBreak('{');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Indent();
      int num = node.Properties.IfNotNull<AstNodeList, int>((Func<AstNodeList, int>) (p => p.Count));
      if (num > 1)
        this.NewLine();
      if (node.Properties != null)
        node.Properties.Accept((IVisitor) this);
      this.Unindent();
      if (num > 1)
        this.NewLine();
      this.Output('}');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      if (startOfStatement)
        this.Output(')');
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    public void Visit(ObjectLiteralField node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (this.m_settings.QuoteObjectLiteralProperties)
      {
        if (node.PrimitiveType == PrimitiveType.String)
        {
          this.Visit((ConstantWrapper) node);
        }
        else
        {
          this.Output('"');
          this.Visit((ConstantWrapper) node);
          this.Output('"');
        }
      }
      else if (node.PrimitiveType == PrimitiveType.String)
      {
        string str = node.ToString();
        if (!string.IsNullOrEmpty(str) && JSScanner.IsSafeIdentifier(str) && !JSScanner.IsKeyword(str, node.EnclosingScope.IfNotNull<ActivationObject, bool>((Func<ActivationObject, bool>) (s => s.UseStrict))))
        {
          this.Output(str);
          this.MarkSegment((AstNode) node, (string) null, node.Context);
        }
        else
          this.Visit((ConstantWrapper) node);
      }
      else
        this.Visit((ConstantWrapper) node);
      this.OutputPossibleLineBreak(':');
      this.MarkSegment((AstNode) node, (string) null, node.ColonContext);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.EndSymbol(symbol);
    }

    public void Visit(ObjectLiteralProperty node)
    {
      if (node == null)
        return;
      if (node.Name != null && !(node.Name is GetterSetter))
      {
        node.Name.Accept((IVisitor) this);
        this.SetContextOutputPosition(node.Context);
      }
      if (node.Value == null)
        return;
      this.AcceptNodeWithParens(node.Value, node.Value.Precedence == OperatorPrecedence.Comma);
    }

    public void Visit(ParameterDeclaration node)
    {
      if (node == null)
        return;
      if (node.HasRest)
      {
        this.Output(OutputVisitor.OperatorString(JSToken.RestSpread));
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      node.Binding.IfNotNull<AstNode>((Action<AstNode>) (b => b.Accept((IVisitor) this)));
      if (node.Initializer == null)
        return;
      if (this.m_settings.OutputMode == OutputMode.MultipleLines && this.m_settings.IndentSize > 0)
      {
        this.OutputPossibleLineBreak(' ');
        this.OutputPossibleLineBreak('=');
        this.BreakLine(false);
        if (!this.m_onNewLine)
          this.OutputPossibleLineBreak(' ');
      }
      else
        this.OutputPossibleLineBreak('=');
      this.AcceptNodeWithParens(node.Initializer, node.Initializer.Precedence == OperatorPrecedence.Comma);
    }

    public void Visit(RegExpLiteral node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.m_startOfStatement = false;
      this.Output('/');
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.Output(node.Pattern);
      this.Output('/');
      if (!string.IsNullOrEmpty(node.PatternSwitches))
        this.Output(node.PatternSwitches);
      this.EndSymbol(symbol);
    }

    public void Visit(ReturnNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("return");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (node.Operand != null)
      {
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.Output(' ');
        this.m_noLineBreaks = true;
        this.Indent();
        node.Operand.Accept((IVisitor) this);
        this.Unindent();
      }
      this.EndSymbol(symbol);
    }

    public void Visit(Switch node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("switch");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Expression != null)
        node.Expression.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.BraceOnNewLine)
        this.NewLine();
      else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('{');
      this.MarkSegment((AstNode) node, (string) null, node.BraceContext);
      this.Indent();
      AstNode node1 = (AstNode) null;
      for (int index = 0; index < node.Cases.Count; ++index)
      {
        AstNode astNode = node.Cases[index];
        if (astNode != null)
        {
          if (node1 != null && this.m_requiresSeparator.Query(node1) && this.ReplaceableSemicolon())
            this.MarkSegment(node1, (string) null, node1.TerminatingContext);
          this.NewLine();
          astNode.Accept((IVisitor) this);
          node1 = astNode;
        }
      }
      this.Unindent();
      this.NewLine();
      this.OutputPossibleLineBreak('}');
      this.MarkSegment((AstNode) node, (string) null, node.BraceContext);
      this.EndSymbol(symbol);
    }

    public void Visit(SwitchCase node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      if (node.CaseValue != null)
      {
        this.Output("case");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
        this.m_startOfStatement = false;
        node.CaseValue.Accept((IVisitor) this);
      }
      else
      {
        this.Output("default");
        this.MarkSegment((AstNode) node, (string) null, node.Context);
        this.SetContextOutputPosition(node.Context);
      }
      this.OutputPossibleLineBreak(':');
      this.MarkSegment((AstNode) node, (string) null, node.ColonContext);
      if (node.Statements != null && node.Statements.Count > 0)
      {
        this.Indent();
        AstNode node1 = (AstNode) null;
        for (int index = 0; index < node.Statements.Count; ++index)
        {
          AstNode statement = node.Statements[index];
          if (statement != null)
          {
            if (node1 != null && this.m_requiresSeparator.Query(node1))
            {
              this.OutputPossibleLineBreak(';');
              this.MarkSegment(node1, (string) null, node1.TerminatingContext);
            }
            this.NewLine();
            this.m_startOfStatement = true;
            statement.Accept((IVisitor) this);
            node1 = statement;
          }
        }
        this.Unindent();
      }
      this.EndSymbol(symbol);
    }

    public virtual void Visit(TemplateLiteral node)
    {
      if (node == null)
        return;
      if (node.Function != null)
      {
        node.Function.Accept((IVisitor) this);
        this.m_startOfStatement = false;
      }
      string text = node.Text;
      if (node.TextContext != null && !this.m_settings.IsModificationAllowed(TreeModifications.MinifyStringLiterals))
        text = node.TextContext.Code;
      if (!text.IsNullOrWhiteSpace())
      {
        this.Output(text);
        this.MarkSegment((AstNode) node, (string) null, node.TextContext ?? node.Context);
        this.SetContextOutputPosition(node.TextContext);
        this.m_startOfStatement = false;
      }
      if (node.Expressions == null || node.Expressions.Count <= 0)
        return;
      node.Expressions.ForEach<TemplateLiteralExpression>((Action<TemplateLiteralExpression>) (expr => expr.Accept((IVisitor) this)));
    }

    public virtual void Visit(TemplateLiteralExpression node)
    {
      if (node == null)
        return;
      if (node.Expression != null)
        node.Expression.Accept((IVisitor) this);
      if (node.Text.IsNullOrWhiteSpace())
        return;
      this.Output(node.Text);
      this.MarkSegment((AstNode) node, (string) null, node.TextContext);
      this.SetContextOutputPosition(node.TextContext);
      this.m_startOfStatement = false;
    }

    public void Visit(ThisLiteral node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("this");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.EndSymbol(symbol);
    }

    public void Visit(ThrowNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("throw");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      if (node.Operand != null)
      {
        this.m_noLineBreaks = true;
        node.Operand.Accept((IVisitor) this);
      }
      if (this.m_settings.MacSafariQuirks)
      {
        this.OutputPossibleLineBreak(';');
        this.MarkSegment((AstNode) node, (string) null, node.TerminatingContext);
      }
      this.EndSymbol(symbol);
    }

    public void Visit(TryNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.OutputTryBranch(node);
      bool flag = false;
      if (node.CatchParameter != null)
      {
        flag = true;
        this.OutputCatchBranch(node);
      }
      if (!flag || node.FinallyBlock != null && node.FinallyBlock.Count > 0)
        this.OutputFinallyBranch(node);
      this.EndSymbol(symbol);
    }

    private void OutputTryBranch(TryNode node)
    {
      this.Output("try");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      if (node.TryBlock == null || node.TryBlock.Count == 0)
      {
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        this.Output("{}");
        this.BreakLine(false);
      }
      else
      {
        if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.TryBlock.BraceOnNewLine)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node.TryBlock.Accept((IVisitor) this);
      }
    }

    private void OutputCatchBranch(TryNode node)
    {
      this.NewLine();
      this.Output("catch(");
      node.CatchParameter.IfNotNull<ParameterDeclaration>((Action<ParameterDeclaration>) (p => p.Accept((IVisitor) this)));
      this.OutputPossibleLineBreak(')');
      if (node.CatchBlock == null || node.CatchBlock.Count == 0)
      {
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        this.Output("{}");
        this.BreakLine(false);
      }
      else
      {
        if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.CatchBlock.BraceOnNewLine)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node.CatchBlock.Accept((IVisitor) this);
      }
    }

    private void OutputFinallyBranch(TryNode node)
    {
      this.NewLine();
      this.Output("finally");
      this.MarkSegment((AstNode) node, (string) null, node.FinallyContext);
      if (node.FinallyBlock == null || node.FinallyBlock.Count == 0)
      {
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        this.Output("{}");
        this.BreakLine(false);
      }
      else
      {
        if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.FinallyBlock.BraceOnNewLine)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node.FinallyBlock.Accept((IVisitor) this);
      }
    }

    public void Visit(Var node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.Output("var");
      this.MarkSegment((AstNode) node, (string) null, node.Context);
      this.SetContextOutputPosition(node.Context);
      this.m_startOfStatement = false;
      this.Indent();
      bool flag = !(node.Parent is ForNode);
      for (int index = 0; index < node.Count; ++index)
      {
        VariableDeclaration variableDeclaration = node[index];
        if (variableDeclaration != null)
        {
          if (index > 0)
          {
            this.OutputPossibleLineBreak(',');
            if (flag)
              this.NewLine();
            else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
              this.OutputPossibleLineBreak(' ');
          }
          this.m_noIn = noIn;
          variableDeclaration.Accept((IVisitor) this);
        }
      }
      this.Unindent();
      this.EndSymbol(symbol);
    }

    public void Visit(VariableDeclaration node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      node.Binding.IfNotNull<AstNode>((Action<AstNode>) (b => b.Accept((IVisitor) this)));
      this.m_startOfStatement = false;
      if (node.Initializer != null)
      {
        if (node.IsCCSpecialCase)
        {
          if (!this.m_outputCCOn || node.UseCCOn && !this.m_settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryCCOnStatements))
          {
            this.Output("/*@cc_on=");
            this.m_outputCCOn = true;
          }
          else
            this.Output("/*@=");
        }
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines && this.m_settings.IndentSize > 0)
        {
          this.OutputPossibleLineBreak(' ');
          this.OutputPossibleLineBreak('=');
          this.BreakLine(false);
          if (!this.m_onNewLine)
            this.OutputPossibleLineBreak(' ');
        }
        else
          this.OutputPossibleLineBreak('=');
        this.AcceptNodeWithParens(node.Initializer, node.Initializer.Precedence == OperatorPrecedence.Comma);
        if (node.IsCCSpecialCase)
          this.Output("@*/");
      }
      this.EndSymbol(symbol);
    }

    public void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      bool noIn = this.m_noIn;
      this.m_noIn = false;
      if (node.IsPostfix)
      {
        if (node.Operand != null)
        {
          this.AcceptNodeWithParens(node.Operand, node.Operand.Precedence < node.Precedence);
          OutputVisitor.SetContextOutputPosition(node.Context, node.Operand.Context);
        }
        this.m_noLineBreaks = true;
        this.Output(OutputVisitor.OperatorString(node.OperatorToken));
        this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
        this.m_startOfStatement = false;
      }
      else if (node.OperatorToken == JSToken.RestSpread)
      {
        this.Output(OutputVisitor.OperatorString(JSToken.RestSpread));
        this.MarkSegment((AstNode) node, (string) null, node.OperatorContext ?? node.Context);
        node.Operand.IfNotNull<AstNode>((Action<AstNode>) (o => o.Accept((IVisitor) this)));
      }
      else
      {
        if (node.OperatorInConditionalCompilationComment)
        {
          if (!this.m_outputCCOn || node.ConditionalCommentContainsOn && !this.m_settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryCCOnStatements))
          {
            this.Output("/*@cc_on");
            this.m_outputCCOn = true;
          }
          else
            this.Output("/*@");
          this.Output(OutputVisitor.OperatorString(node.OperatorToken));
          this.MarkSegment((AstNode) node, (string) null, node.OperatorContext);
          this.SetContextOutputPosition(node.Context);
          this.Output("@*/");
        }
        else
        {
          this.Output(OutputVisitor.OperatorString(node.OperatorToken));
          this.MarkSegment((AstNode) node, (string) null, node.OperatorContext ?? node.Context);
          this.SetContextOutputPosition(node.Context);
          if (node.OperatorToken == JSToken.Yield && node.IsDelegator)
            this.Output('*');
        }
        this.m_startOfStatement = false;
        if (node.Operand != null)
          this.AcceptNodeWithParens(node.Operand, node.Operand.Precedence < node.Precedence);
      }
      this.m_noIn = noIn;
      this.EndSymbol(symbol);
    }

    public void Visit(WhileNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("while");
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.OutputBlock(node.Body);
      this.EndSymbol(symbol);
    }

    public void Visit(WithNode node)
    {
      if (node == null)
        return;
      object symbol = this.StartSymbol((AstNode) node);
      this.Output("with");
      this.SetContextOutputPosition(node.Context);
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      this.OutputPossibleLineBreak('(');
      this.m_startOfStatement = false;
      if (node.WithObject != null)
        node.WithObject.Accept((IVisitor) this);
      this.OutputPossibleLineBreak(')');
      this.OutputBlock(node.Body);
      this.EndSymbol(symbol);
    }

    private void Output([Localizable(false)] string text)
    {
      if (string.IsNullOrEmpty(text))
        return;
      this.InsertSpaceIfNeeded(text);
      this.m_segmentStartLine = this.m_lineCount;
      this.m_segmentStartColumn = this.m_lineLength;
      this.m_lineLength += this.WriteToStream(text);
      this.m_noLineBreaks = false;
      this.m_onNewLine = text[text.Length - 1] == '\n' || text[text.Length - 1] == '\r';
      this.SetLastCharState(text);
    }

    private void Output(char ch)
    {
      this.InsertSpaceIfNeeded(ch);
      this.m_segmentStartLine = this.m_lineCount;
      this.m_segmentStartColumn = this.m_lineLength;
      this.m_lineLength += this.WriteToStream(ch);
      this.m_noLineBreaks = false;
      this.m_onNewLine = ch == '\n' || ch == '\r';
      this.SetLastCharState(ch);
    }

    private void OutputSpaceOrLineBreak()
    {
      if (this.m_noLineBreaks)
      {
        this.m_outputStream.Write(' ');
        ++this.m_lineLength;
        this.m_lastCharacter = ' ';
      }
      else
        this.OutputPossibleLineBreak(' ');
    }

    private void InsertSpaceIfNeeded(char ch)
    {
      if (ch == ' ')
        return;
      if (this.m_addSpaceIfTrue != null)
      {
        if (this.m_addSpaceIfTrue(ch))
          this.OutputSpaceOrLineBreak();
        this.m_addSpaceIfTrue = (Func<char, bool>) null;
      }
      else if ((ch == '+' || ch == '-') && (int) this.m_lastCharacter == (int) ch)
      {
        if (!this.m_lastCountOdd)
          return;
        this.OutputSpaceOrLineBreak();
      }
      else
      {
        if (this.m_lastCharacter != '@' && !JSScanner.IsValidIdentifierPart(this.m_lastCharacter) || !JSScanner.IsValidIdentifierPart(ch))
          return;
        this.OutputSpaceOrLineBreak();
      }
    }

    private void InsertSpaceIfNeeded(string text)
    {
      char ch = text[0];
      if (this.m_addSpaceIfTrue != null)
      {
        if (this.m_addSpaceIfTrue(ch))
          this.OutputSpaceOrLineBreak();
        this.m_addSpaceIfTrue = (Func<char, bool>) null;
      }
      else if ((ch == '+' || ch == '-') && (int) this.m_lastCharacter == (int) ch)
      {
        if (!this.m_lastCountOdd)
          return;
        this.OutputSpaceOrLineBreak();
      }
      else
      {
        if (this.m_lastCharacter != '@' && !JSScanner.IsValidIdentifierPart(this.m_lastCharacter) || text[0] != '\\' && !JSScanner.StartsWithValidIdentifierPart(text))
          return;
        this.OutputSpaceOrLineBreak();
      }
    }

    private void SetLastCharState(char ch)
    {
      this.m_lastCountOdd = (ch == '+' || ch == '-') && ((int) ch != (int) this.m_lastCharacter || !this.m_lastCountOdd);
      this.m_lastCharacter = ch;
    }

    private void SetLastCharState(string text)
    {
      if (string.IsNullOrEmpty(text))
        return;
      char ch = text[text.Length - 1];
      switch (ch)
      {
        case '+':
        case '-':
          int index = text.Length - 1;
          do
            ;
          while (--index >= 0 && (int) text[index] == (int) ch);
          this.m_lastCountOdd = index >= 0 || (int) this.m_lastCharacter != (int) ch ? (text.Length - 1 - index) % 2 == 1 : text.Length % 2 == 1 ^ this.m_lastCountOdd;
          break;
        default:
          this.m_lastCountOdd = false;
          break;
      }
      this.m_lastCharacter = ch;
    }

    private void Indent() => ++this.m_indentLevel;

    private void Unindent() => --this.m_indentLevel;

    private void OutputPossibleLineBreak(char ch)
    {
      if (ch == ' ')
      {
        this.BreakLine(false);
        if (this.m_onNewLine)
          return;
        this.m_lineLength += this.WriteToStream(ch);
        this.m_lastCharacter = ch;
      }
      else
      {
        this.InsertSpaceIfNeeded(ch);
        this.m_segmentStartLine = this.m_lineCount;
        this.m_segmentStartColumn = this.m_lineLength;
        this.m_lineLength += this.WriteToStream(ch);
        this.m_onNewLine = false;
        this.m_lastCharacter = ch;
        this.BreakLine(false);
      }
    }

    private bool ReplaceableSemicolon()
    {
      bool flag = false;
      if (this.m_lineLength < this.m_settings.LineBreakThreshold)
      {
        this.m_segmentStartLine = this.m_lineCount;
        this.m_segmentStartColumn = this.m_lineLength;
        this.m_outputStream.Write(';');
        ++this.m_lineLength;
        this.m_onNewLine = false;
        this.m_lastCharacter = ';';
        flag = true;
      }
      this.BreakLine(false);
      return flag;
    }

    private void BreakLine(bool forceBreak)
    {
      if (this.m_onNewLine || !forceBreak && this.m_lineLength < this.m_settings.LineBreakThreshold)
        return;
      if (this.m_settings.OutputMode == OutputMode.MultipleLines)
      {
        this.NewLine();
      }
      else
      {
        this.m_outputStream.Write('\n');
        ++this.m_lineCount;
        this.m_lineLength = 0;
        this.m_onNewLine = true;
        this.m_lastCharacter = ' ';
      }
    }

    private void NewLine()
    {
      if (this.m_settings.OutputMode != OutputMode.MultipleLines || this.m_onNewLine)
        return;
      this.m_outputStream.WriteLine();
      ++this.m_lineCount;
      if (this.m_indentLevel > 0)
      {
        int num = this.m_indentLevel * this.m_settings.IndentSize;
        this.m_lineLength = num;
        while (num-- > 0)
          this.m_outputStream.Write(' ');
      }
      else
        this.m_lineLength = 0;
      this.m_lastCharacter = ' ';
      this.m_onNewLine = true;
    }

    private int WriteToStream(string text)
    {
      if (this.m_settings.AlwaysEscapeNonAscii)
      {
        StringBuilder stringBuilder = (StringBuilder) null;
        int startIndex = 0;
        for (int index = 0; index < text.Length; ++index)
        {
          if (text[index] > '\u007F')
          {
            if (stringBuilder == null)
              stringBuilder = new StringBuilder();
            if (index > startIndex)
              stringBuilder.Append(text, startIndex, index - startIndex);
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\\u{0:x4}".FormatInvariant((object) (int) text[index]));
            startIndex = index + 1;
          }
        }
        if (stringBuilder != null)
        {
          if (startIndex < text.Length)
            stringBuilder.Append(text, startIndex, text.Length - startIndex);
          text = stringBuilder.ToString();
        }
      }
      this.m_outputStream.Write(text);
      return text.Length;
    }

    private int WriteToStream(char ch)
    {
      if (this.m_settings.AlwaysEscapeNonAscii && ch > '\u007F')
      {
        this.m_outputStream.Write("\\u{0:x4}", (object) (int) ch);
        return 6;
      }
      this.m_outputStream.Write(ch);
      return 1;
    }

    public static string OperatorString(JSToken token)
    {
      switch (token)
      {
        case JSToken.ArrowFunction:
          return "=>";
        case JSToken.RestSpread:
          return "...";
        case JSToken.FirstOperator:
          return "delete";
        case JSToken.Increment:
          return "++";
        case JSToken.Decrement:
          return "--";
        case JSToken.Void:
          return "void";
        case JSToken.TypeOf:
          return "typeof";
        case JSToken.LogicalNot:
          return "!";
        case JSToken.BitwiseNot:
          return "~";
        case JSToken.FirstBinaryOperator:
          return "+";
        case JSToken.Minus:
          return "-";
        case JSToken.Multiply:
          return "*";
        case JSToken.Divide:
          return "/";
        case JSToken.Modulo:
          return "%";
        case JSToken.BitwiseAnd:
          return "&";
        case JSToken.BitwiseOr:
          return "|";
        case JSToken.BitwiseXor:
          return "^";
        case JSToken.LeftShift:
          return "<<";
        case JSToken.RightShift:
          return ">>";
        case JSToken.UnsignedRightShift:
          return ">>>";
        case JSToken.Equal:
          return "==";
        case JSToken.NotEqual:
          return "!=";
        case JSToken.StrictEqual:
          return "===";
        case JSToken.StrictNotEqual:
          return "!==";
        case JSToken.LessThan:
          return "<";
        case JSToken.LessThanEqual:
          return "<=";
        case JSToken.GreaterThan:
          return ">";
        case JSToken.GreaterThanEqual:
          return ">=";
        case JSToken.LogicalAnd:
          return "&&";
        case JSToken.LogicalOr:
          return "||";
        case JSToken.InstanceOf:
          return "instanceof";
        case JSToken.In:
          return "in";
        case JSToken.Comma:
          return ",";
        case JSToken.Assign:
          return "=";
        case JSToken.PlusAssign:
          return "+=";
        case JSToken.MinusAssign:
          return "-=";
        case JSToken.MultiplyAssign:
          return "*=";
        case JSToken.DivideAssign:
          return "/=";
        case JSToken.ModuloAssign:
          return "%=";
        case JSToken.BitwiseAndAssign:
          return "&=";
        case JSToken.BitwiseOrAssign:
          return "|=";
        case JSToken.BitwiseXorAssign:
          return "^=";
        case JSToken.LeftShiftAssign:
          return "<<=";
        case JSToken.RightShiftAssign:
          return ">>=";
        case JSToken.UnsignedRightShiftAssign:
          return ">>>=";
        case JSToken.Const:
          return "const";
        case JSToken.Let:
          return "let";
        case JSToken.Yield:
          return "yield";
        case JSToken.Get:
          return "get";
        case JSToken.Set:
          return "set";
        default:
          return string.Empty;
      }
    }

    private void AcceptNodeWithParens(AstNode node, bool needsParens)
    {
      if (needsParens)
      {
        this.OutputPossibleLineBreak('(');
        this.m_startOfStatement = false;
        this.m_noIn = false;
      }
      node.Accept((IVisitor) this);
      if (needsParens)
        this.Output(')');
      this.m_startOfStatement = false;
    }

    private void OutputFunctionArgsAndBody(FunctionObject node)
    {
      if (node == null)
        return;
      if (node.ParameterDeclarations != null)
      {
        this.Indent();
        bool flag = node.FunctionType != FunctionType.ArrowFunction || node.ParameterDeclarations.Count != 1 || (node.ParameterDeclarations[0] as ParameterDeclaration).IfNotNull<ParameterDeclaration, bool>((Func<ParameterDeclaration, bool>) (d => d.HasRest), true);
        if (flag)
        {
          this.m_startOfStatement = false;
          this.OutputPossibleLineBreak('(');
          this.MarkSegment((AstNode) node, (string) null, node.ParameterDeclarations.Context);
        }
        AstNode astNode = (AstNode) null;
        for (int index = 0; index < node.ParameterDeclarations.Count; ++index)
        {
          if (index > 0)
          {
            this.OutputPossibleLineBreak(',');
            this.MarkSegment((AstNode) node, (string) null, astNode.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (p => p.TerminatingContext)) ?? node.ParameterDeclarations.Context);
            if (this.m_settings.OutputMode == OutputMode.MultipleLines)
              this.OutputPossibleLineBreak(' ');
          }
          astNode = node.ParameterDeclarations[index];
          astNode?.Accept((IVisitor) this);
        }
        this.Unindent();
        if (flag)
        {
          this.OutputPossibleLineBreak(')');
          this.MarkSegment((AstNode) node, (string) null, node.ParameterDeclarations.Context);
        }
      }
      else if (node.FunctionType == FunctionType.ArrowFunction)
      {
        this.OutputPossibleLineBreak('(');
        this.OutputPossibleLineBreak(')');
        this.m_startOfStatement = false;
      }
      if (node.FunctionType == FunctionType.ArrowFunction)
      {
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        this.Output(OutputVisitor.OperatorString(JSToken.ArrowFunction));
        if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
      }
      if (node.Body == null || node.Body.Count == 0)
      {
        this.Output("{}");
        this.MarkSegment((AstNode) node, (string) null, node.Body.IfNotNull<Block, Context>((Func<Block, Context>) (b => b.Context)));
        this.BreakLine(false);
      }
      else if (node.FunctionType == FunctionType.ArrowFunction && node.Body.Count == 1 && node.Body.IsConcise)
      {
        node.Body[0].Accept((IVisitor) this);
      }
      else
      {
        if (node.FunctionType == FunctionType.ArrowFunction)
          this.Indent();
        if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && node.Body.BraceOnNewLine)
          this.NewLine();
        else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
          this.OutputPossibleLineBreak(' ');
        node.Body.Accept((IVisitor) this);
        if (node.FunctionType != FunctionType.ArrowFunction)
          return;
        this.Unindent();
      }
    }

    private void OutputBlock(Block block)
    {
      if (block != null && block.ForceBraces)
        this.OutputBlockWithBraces(block);
      else if (block == null || block.Count == 0)
      {
        this.OutputPossibleLineBreak(';');
        this.MarkSegment((AstNode) block, (string) null, block.IfNotNull<Block, Context>((Func<Block, Context>) (b => b.Context)));
      }
      else if (block.Count == 1)
      {
        this.Indent();
        this.NewLine();
        if (block[0] is ImportantComment)
        {
          block[0].Accept((IVisitor) this);
          this.OutputPossibleLineBreak(';');
          this.MarkSegment((AstNode) block, (string) null, block.Context);
        }
        else
        {
          this.m_startOfStatement = true;
          block[0].Accept((IVisitor) this);
        }
        this.Unindent();
      }
      else
        this.OutputBlockWithBraces(block);
    }

    private void OutputBlockWithBraces(Block block)
    {
      if (this.m_settings.BlocksStartOnSameLine == BlockStart.NewLine || this.m_settings.BlocksStartOnSameLine == BlockStart.UseSource && block.BraceOnNewLine)
        this.NewLine();
      else if (this.m_settings.OutputMode == OutputMode.MultipleLines)
        this.OutputPossibleLineBreak(' ');
      block.Accept((IVisitor) this);
    }

    private string InlineSafeString(string text)
    {
      if (this.m_settings.InlineSafeStrings)
      {
        if (text.IndexOf("</", StringComparison.OrdinalIgnoreCase) >= 0)
          text = text.Replace("</", "<\\/");
        if (text.IndexOf("]]>", StringComparison.Ordinal) >= 0)
          text = text.Replace("]]>", "]\\]>");
      }
      return text;
    }

    public static string NormalizeNumber(double numericValue, Context originalContext)
    {
      if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
      {
        if (originalContext != null && !string.IsNullOrEmpty(originalContext.Code) && !originalContext.Document.IsGenerated)
          return originalContext.Code;
        string str = double.IsNaN(numericValue) ? "NaN" : "Infinity";
        return !double.IsNegativeInfinity(numericValue) ? str : "-Infinity";
      }
      if (numericValue == 0.0)
        return 1.0 / numericValue >= 0.0 ? "0" : "-0";
      string normal = OutputVisitor.GetSmallestRep(numericValue.ToStringInvariant("R"));
      if (Math.Floor(numericValue) == numericValue)
      {
        string str = OutputVisitor.NormalOrHexIfSmaller(numericValue, normal);
        if (str.Length < normal.Length)
          normal = str;
      }
      return normal;
    }

    private static string GetSmallestRep(string number)
    {
      Match match = CommonData.DecimalFormat.Match(number);
      if (match.Success)
      {
        string str = match.Result("${man}");
        if (string.IsNullOrEmpty(match.Result("${exp}")))
        {
          if (string.IsNullOrEmpty(str))
          {
            if (string.IsNullOrEmpty(match.Result("${sig}")))
            {
              number = match.Result("${neg}") + "0";
            }
            else
            {
              int length = match.Result("${zer}").Length;
              if (length > 2)
                number = match.Result("${neg}") + match.Result("${sig}") + (object) 'e' + length.ToStringInvariant();
            }
          }
          else
            number = match.Result("${neg}") + match.Result("${mag}") + (object) '.' + str;
        }
        else if (string.IsNullOrEmpty(str))
        {
          number = match.Result("${neg}") + match.Result("${mag}") + "e" + match.Result("${eng}") + match.Result("${pow}");
        }
        else
        {
          int number1;
          if ((match.Result("${eng}") + match.Result("${pow}")).TryParseIntInvariant(NumberStyles.Integer, out number1))
            number = match.Result("${neg}") + match.Result("${mag}") + str + (object) 'e' + (number1 - str.Length).ToStringInvariant();
          else
            number = match.Result("${neg}") + match.Result("${mag}") + (object) '.' + str + (object) 'e' + match.Result("${eng}") + match.Result("${pow}");
        }
      }
      return number;
    }

    private static string NormalOrHexIfSmaller(double doubleValue, string normal)
    {
      int num1 = normal.Length - 2;
      int num2 = Math.Sign(doubleValue);
      if (num2 < 0)
      {
        doubleValue = -doubleValue;
        --num1;
      }
      char[] chArray = new char[normal.Length - 1];
      int length = chArray.Length;
      for (; num1 > 0 && doubleValue > 0.0; --num1)
      {
        int num3 = (int) (doubleValue % 16.0);
        chArray[--length] = (char) ((num3 < 10 ? 48 : 87) + num3);
        doubleValue = Math.Floor(doubleValue / 16.0);
      }
      if (num1 > 0)
      {
        int num4;
        chArray[num4 = length - 1] = 'x';
        int startIndex;
        chArray[startIndex = num4 - 1] = '0';
        if (num2 < 0)
          chArray[--startIndex] = '-';
        normal = new string(chArray, startIndex, chArray.Length - startIndex);
      }
      return normal;
    }

    public static string EscapeString(string text)
    {
      char ch = OutputVisitor.QuoteFactor(text) < 0 ? '\'' : '"';
      int startIndex = 0;
      StringBuilder stringBuilder = (StringBuilder) null;
      string str = string.Empty;
      if (!string.IsNullOrEmpty(text))
      {
        for (int index = 0; index < text.Length; ++index)
        {
          char number1 = text[index];
          switch (number1)
          {
            case '\b':
              number1 = 'b';
              goto case '\\';
            case '\t':
              number1 = 't';
              goto case '\\';
            case '\n':
              number1 = 'n';
              goto case '\\';
            case '\f':
              number1 = 'f';
              goto case '\\';
            case '\r':
              number1 = 'r';
              goto case '\\';
            case '"':
            case '\'':
              if ((int) number1 != (int) ch)
                break;
              goto case '\\';
            case '\\':
              if (stringBuilder == null)
                stringBuilder = new StringBuilder();
              if (startIndex < index)
                stringBuilder.Append(text.Substring(startIndex, index - startIndex));
              startIndex = index + 1;
              stringBuilder.Append('\\');
              stringBuilder.Append(number1);
              break;
            case '\u2028':
            case '\u2029':
              if (stringBuilder == null)
                stringBuilder = new StringBuilder();
              if (startIndex < index)
                stringBuilder.Append(text.Substring(startIndex, index - startIndex));
              startIndex = index + 1;
              stringBuilder.Append("\\u");
              stringBuilder.Append(((int) number1).ToStringInvariant("x4"));
              break;
            default:
              if (number1 < ' ')
              {
                if (stringBuilder == null)
                  stringBuilder = new StringBuilder();
                if (startIndex < index)
                  stringBuilder.Append(text.Substring(startIndex, index - startIndex));
                startIndex = index + 1;
                int number2 = (int) number1;
                stringBuilder.Append("\\x");
                stringBuilder.Append(number2.ToStringInvariant("x2"));
                break;
              }
              break;
          }
        }
        if (stringBuilder != null)
        {
          if (startIndex < text.Length)
            stringBuilder.Append(text.Substring(startIndex));
          str = stringBuilder.ToString();
        }
        else
          str = text;
      }
      return ch.ToString() + str + (object) ch;
    }

    private static int QuoteFactor(string text)
    {
      int num = 0;
      if (!text.IsNullOrWhiteSpace())
      {
        for (int index = 0; index < text.Length; ++index)
        {
          if (text[index] == '\'')
            ++num;
          else if (text[index] == '"')
            --num;
        }
      }
      return num;
    }

    private object StartSymbol(AstNode node) => this.m_settings.SymbolsMap != null ? this.m_settings.SymbolsMap.StartSymbol(node, this.m_lineCount, this.m_lineLength) : (object) null;

    private void MarkSegment(AstNode node, string name, Context context)
    {
      if (this.m_settings.SymbolsMap == null || node == null)
        return;
      this.m_settings.SymbolsMap.MarkSegment(node, this.m_segmentStartLine, this.m_segmentStartColumn, name, context);
    }

    private void EndSymbol(object symbol)
    {
      if (this.m_settings.SymbolsMap == null || symbol == null)
        return;
      string parentContext = (string) null;
      if (this.m_functionStack.Count > 0)
        parentContext = this.m_functionStack.Peek();
      this.m_settings.SymbolsMap.EndSymbol(symbol, this.m_lineCount, this.m_lineLength, parentContext);
    }

    private void SetContextOutputPosition(Context context)
    {
      if (context == null)
        return;
      context.OutputLine = this.m_segmentStartLine + 1;
      context.OutputColumn = this.m_segmentStartColumn;
    }

    private static void SetContextOutputPosition(Context context, Context fromContext)
    {
      if (context == null || fromContext == null)
        return;
      context.OutputLine = fromContext.OutputLine;
      context.OutputColumn = fromContext.OutputColumn;
    }
  }
}
