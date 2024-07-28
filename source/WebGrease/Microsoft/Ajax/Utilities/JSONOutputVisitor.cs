// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSONOutputVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  public class JSONOutputVisitor : IVisitor
  {
    private TextWriter m_writer;
    private CodeSettings m_settings;

    public bool IsValid { get; private set; }

    private JSONOutputVisitor(TextWriter writer, CodeSettings settings)
    {
      this.m_writer = writer;
      this.m_settings = settings;
      this.IsValid = true;
    }

    public static bool Apply(TextWriter writer, AstNode node, CodeSettings settings)
    {
      if (node == null)
        return false;
      JSONOutputVisitor jsonOutputVisitor = new JSONOutputVisitor(writer, settings);
      node.Accept((IVisitor) jsonOutputVisitor);
      return jsonOutputVisitor.IsValid;
    }

    public void Visit(ArrayLiteral node)
    {
      if (node == null)
        return;
      bool flag1 = false;
      if (this.m_settings.OutputMode == OutputMode.MultipleLines && (node.Elements.Count > 5 || JSONOutputVisitor.NotJustPrimitives(node.Elements)))
        flag1 = true;
      this.m_writer.Write('[');
      if (node.Elements != null)
      {
        if (flag1)
        {
          this.m_settings.Indent();
          try
          {
            bool flag2 = true;
            foreach (AstNode element in node.Elements)
            {
              if (flag2)
                flag2 = false;
              else
                this.m_writer.Write(',');
              this.NewLine();
              element.Accept((IVisitor) this);
            }
          }
          finally
          {
            this.m_settings.Unindent();
          }
          this.NewLine();
        }
        else
          node.Elements.Accept((IVisitor) this);
      }
      this.m_writer.Write(']');
    }

    public void Visit(AstNodeList node)
    {
      if (node == null)
        return;
      for (int index = 0; index < node.Count; ++index)
      {
        if (index > 0)
        {
          this.m_writer.Write(',');
          if (this.m_settings.OutputMode == OutputMode.MultipleLines)
            this.m_writer.Write(' ');
        }
        if (node[index] != null)
          node[index].Accept((IVisitor) this);
      }
    }

    public void Visit(Block node)
    {
      if (node == null || node.Count <= 0)
        return;
      node[0].Accept((IVisitor) this);
    }

    public void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      switch (node.PrimitiveType)
      {
        case PrimitiveType.Null:
          this.m_writer.Write("null");
          break;
        case PrimitiveType.Boolean:
          this.m_writer.Write((bool) node.Value ? "true" : "false");
          break;
        case PrimitiveType.Number:
          this.OutputNumber((double) node.Value, node.Context);
          break;
        case PrimitiveType.String:
        case PrimitiveType.Other:
          this.OutputString(node.Value.ToString());
          break;
      }
    }

    public void Visit(CustomNode node)
    {
      if (node == null)
        return;
      this.OutputString(node.ToCode());
    }

    public void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      if (node.OperatorToken == JSToken.Minus)
      {
        this.m_writer.Write('-');
        if (node.Operand == null)
          return;
        node.Operand.Accept((IVisitor) this);
      }
      else
        this.IsValid = false;
    }

    public void Visit(ObjectLiteral node)
    {
      if (node == null)
        return;
      this.m_writer.Write('{');
      if (node.Properties != null)
      {
        bool flag1 = false;
        if (this.m_settings.OutputMode == OutputMode.MultipleLines && (node.Properties.Count > 5 || JSONOutputVisitor.NotJustPrimitives(node.Properties)))
          flag1 = true;
        if (flag1)
        {
          this.m_settings.Indent();
          try
          {
            bool flag2 = true;
            foreach (AstNode property in node.Properties)
            {
              if (flag2)
                flag2 = false;
              else
                this.m_writer.Write(',');
              this.NewLine();
              property.Accept((IVisitor) this);
            }
          }
          finally
          {
            this.m_settings.Unindent();
          }
          this.NewLine();
        }
        else
          node.Properties.Accept((IVisitor) this);
      }
      this.m_writer.Write('}');
    }

    public void Visit(ObjectLiteralField node)
    {
      if (node == null)
        return;
      if (node.PrimitiveType == PrimitiveType.String)
      {
        this.OutputString(node.Value.ToString());
      }
      else
      {
        this.m_writer.Write('"');
        this.Visit((ConstantWrapper) node);
        this.m_writer.Write('"');
      }
    }

    public void Visit(ObjectLiteralProperty node)
    {
      if (node == null)
        return;
      if (node.Name != null)
        node.Name.Accept((IVisitor) this);
      this.m_writer.Write(':');
      if (node.Value == null)
        return;
      node.Value.Accept((IVisitor) this);
    }

    public void Visit(AspNetBlockNode node) => this.IsValid = false;

    public void Visit(BinaryOperator node) => this.IsValid = false;

    public void Visit(BindingIdentifier node) => this.IsValid = false;

    public void Visit(Break node) => this.IsValid = false;

    public void Visit(ClassNode node) => this.IsValid = false;

    public void Visit(ComprehensionNode node) => this.IsValid = false;

    public void Visit(ComprehensionForClause node) => this.IsValid = false;

    public void Visit(ComprehensionIfClause node) => this.IsValid = false;

    public void Visit(CallNode node) => this.IsValid = false;

    public void Visit(ConditionalCompilationComment node) => this.IsValid = false;

    public void Visit(ConditionalCompilationElse node) => this.IsValid = false;

    public void Visit(ConditionalCompilationElseIf node) => this.IsValid = false;

    public void Visit(ConditionalCompilationEnd node) => this.IsValid = false;

    public void Visit(ConditionalCompilationIf node) => this.IsValid = false;

    public void Visit(ConditionalCompilationOn node) => this.IsValid = false;

    public void Visit(ConditionalCompilationSet node) => this.IsValid = false;

    public void Visit(Conditional node) => this.IsValid = false;

    public void Visit(ConstantWrapperPP node) => this.IsValid = false;

    public void Visit(ConstStatement node) => this.IsValid = false;

    public void Visit(ContinueNode node) => this.IsValid = false;

    public void Visit(DebuggerNode node) => this.IsValid = false;

    public void Visit(DirectivePrologue node) => this.IsValid = false;

    public void Visit(DoWhile node) => this.IsValid = false;

    public void Visit(EmptyStatement node) => this.IsValid = false;

    public void Visit(ExportNode node) => this.IsValid = false;

    public void Visit(ForIn node) => this.IsValid = false;

    public void Visit(ForNode node) => this.IsValid = false;

    public void Visit(FunctionObject node) => this.IsValid = false;

    public void Visit(GetterSetter node) => this.IsValid = false;

    public void Visit(GroupingOperator node)
    {
      this.IsValid = false;
      if (node == null || node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public void Visit(IfNode node) => this.IsValid = false;

    public void Visit(ImportantComment node) => this.IsValid = false;

    public void Visit(ImportExportSpecifier node) => this.IsValid = false;

    public void Visit(ImportNode node) => this.IsValid = false;

    public void Visit(InitializerNode node) => this.IsValid = false;

    public void Visit(LabeledStatement node) => this.IsValid = false;

    public void Visit(LexicalDeclaration node) => this.IsValid = false;

    public void Visit(Lookup node) => this.IsValid = false;

    public void Visit(Member node) => this.IsValid = false;

    public void Visit(ModuleDeclaration node) => this.IsValid = false;

    public void Visit(ParameterDeclaration node) => this.IsValid = false;

    public void Visit(RegExpLiteral node) => this.IsValid = false;

    public void Visit(ReturnNode node) => this.IsValid = false;

    public void Visit(Switch node) => this.IsValid = false;

    public void Visit(SwitchCase node) => this.IsValid = false;

    public void Visit(TemplateLiteral node) => this.IsValid = false;

    public void Visit(TemplateLiteralExpression node) => this.IsValid = false;

    public void Visit(ThisLiteral node) => this.IsValid = false;

    public void Visit(ThrowNode node) => this.IsValid = false;

    public void Visit(TryNode node) => this.IsValid = false;

    public void Visit(Var node) => this.IsValid = false;

    public void Visit(VariableDeclaration node) => this.IsValid = false;

    public void Visit(WhileNode node) => this.IsValid = false;

    public void Visit(WithNode node) => this.IsValid = false;

    private void OutputString(string text)
    {
      this.m_writer.Write('"');
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        switch (ch)
        {
          case '\b':
            this.m_writer.Write("\\b");
            break;
          case '\t':
            this.m_writer.Write("\\t");
            break;
          case '\n':
            this.m_writer.Write("\\n");
            break;
          case '\f':
            this.m_writer.Write("\\f");
            break;
          case '\r':
            this.m_writer.Write("\\r");
            break;
          case '"':
            this.m_writer.Write("\\\"");
            break;
          default:
            if (ch < ' ')
            {
              this.m_writer.Write("\\u{0:x4}", (object) (int) ch);
              break;
            }
            this.m_writer.Write(ch);
            break;
        }
      }
      this.m_writer.Write('"');
    }

    public void OutputNumber(double numericValue, Context originalContext)
    {
      if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
      {
        if (originalContext != null && !string.IsNullOrEmpty(originalContext.Code) && !originalContext.Document.IsGenerated)
        {
          this.m_writer.Write(originalContext.Code);
        }
        else
        {
          string str = double.IsNaN(numericValue) ? "NaN" : "Infinity";
          this.m_writer.Write(double.IsNegativeInfinity(numericValue) ? "-Infinity" : str);
        }
      }
      else if (numericValue == 0.0)
        this.m_writer.Write(1.0 / numericValue < 0.0 ? "-0" : "0");
      else
        this.m_writer.Write(JSONOutputVisitor.GetSmallestRep(numericValue.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture)));
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
                number = match.Result("${neg}") + match.Result("${sig}") + (object) 'e' + length.ToString((IFormatProvider) CultureInfo.InvariantCulture);
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
          int result;
          if (int.TryParse(match.Result("${eng}") + match.Result("${pow}"), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            number = match.Result("${neg}") + match.Result("${mag}") + str + (object) 'e' + (result - str.Length).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          else
            number = match.Result("${neg}") + match.Result("${mag}") + (object) '.' + str + (object) 'e' + match.Result("${eng}") + match.Result("${pow}");
        }
      }
      return number;
    }

    private static bool NotJustPrimitives(AstNodeList nodeList)
    {
      foreach (AstNode node in nodeList)
      {
        switch (node)
        {
          case ConstantWrapper _:
          case UnaryOperator _:
            continue;
          default:
            return true;
        }
      }
      return false;
    }

    private void NewLine()
    {
      this.m_writer.WriteLine();
      this.m_writer.Write(this.m_settings.TabSpaces);
    }
  }
}
