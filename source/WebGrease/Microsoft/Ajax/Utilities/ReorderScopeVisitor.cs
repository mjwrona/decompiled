// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ReorderScopeVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  internal class ReorderScopeVisitor : TreeVisitor
  {
    private List<FunctionObject> m_functionDeclarations;
    private List<FunctionObject> m_functionExpressions;
    private List<ModuleDeclaration> m_moduleDeclarations;
    private List<DirectivePrologue> m_moduleDirectives;
    private List<Var> m_varStatements;
    private bool m_moveVarStatements;
    private bool m_moveFunctionDecls;
    private bool m_combineAdjacentVars;
    private int m_conditionalCommentLevel;
    private GlobalScope m_globalScope;

    private ReorderScopeVisitor(JSParser parser)
    {
      CodeSettings settings = parser.Settings;
      this.m_moveVarStatements = settings.ReorderScopeDeclarations && settings.IsModificationAllowed(TreeModifications.CombineVarStatementsToTopOfScope);
      this.m_moveFunctionDecls = settings.ReorderScopeDeclarations && settings.IsModificationAllowed(TreeModifications.MoveFunctionToTopOfScope);
      this.m_combineAdjacentVars = settings.IsModificationAllowed(TreeModifications.CombineVarStatements);
      this.m_globalScope = parser.GlobalScope;
    }

    public static void Apply(Block block, JSParser parser)
    {
      if (parser == null)
        throw new ArgumentNullException(nameof (parser));
      if (block == null)
        return;
      ReorderScopeVisitor reorderScopeVisitor = new ReorderScopeVisitor(parser);
      block.Accept((IVisitor) reorderScopeVisitor);
      int num = 0;
      if (reorderScopeVisitor.m_moduleDirectives != null)
      {
        foreach (DirectivePrologue moduleDirective in reorderScopeVisitor.m_moduleDirectives)
          num = ReorderScopeVisitor.RelocateDirectivePrologue(block, num, moduleDirective);
      }
      while (num < block.Count && (block[num] is DirectivePrologue || block[num] is ImportantComment))
        ++num;
      if (reorderScopeVisitor.m_functionDeclarations != null)
      {
        foreach (FunctionObject functionDeclaration in reorderScopeVisitor.m_functionDeclarations)
          num = ReorderScopeVisitor.RelocateFunction(block, num, (AstNode) functionDeclaration);
      }
      if (reorderScopeVisitor.m_varStatements != null && reorderScopeVisitor.m_varStatements.Count > 1)
      {
        foreach (Var varStatement in reorderScopeVisitor.m_varStatements)
          num = ReorderScopeVisitor.RelocateVar(block, num, varStatement);
      }
      if (reorderScopeVisitor.m_functionDeclarations != null)
      {
        foreach (FunctionObject functionDeclaration in reorderScopeVisitor.m_functionDeclarations)
          ReorderScopeVisitor.Apply(functionDeclaration.Body, parser);
      }
      if (reorderScopeVisitor.m_functionExpressions != null)
      {
        foreach (FunctionObject functionExpression in reorderScopeVisitor.m_functionExpressions)
          ReorderScopeVisitor.Apply(functionExpression.Body, parser);
      }
      if (reorderScopeVisitor.m_moduleDeclarations == null)
        return;
      foreach (ModuleDeclaration moduleDeclaration in reorderScopeVisitor.m_moduleDeclarations)
        ReorderScopeVisitor.Apply(moduleDeclaration.Body, parser);
    }

    private static int RelocateDirectivePrologue(
      Block block,
      int insertAt,
      DirectivePrologue directivePrologue)
    {
      while (insertAt < block.Count && block[insertAt] is ImportantComment)
        ++insertAt;
      if (block[insertAt] != directivePrologue)
      {
        directivePrologue.Parent.ReplaceChild((AstNode) directivePrologue, (AstNode) null);
        block.Insert(insertAt, (AstNode) directivePrologue);
      }
      return ++insertAt;
    }

    private static int RelocateFunction(Block block, int insertAt, AstNode funcDecl)
    {
      if (funcDecl.Parent is ExportNode)
        funcDecl = funcDecl.Parent;
      if (block[insertAt] != funcDecl)
      {
        if (funcDecl.Parent == block)
        {
          funcDecl.Parent.ReplaceChild(funcDecl, (AstNode) null);
          block.Insert(insertAt++, funcDecl);
        }
      }
      else
        ++insertAt;
      return insertAt;
    }

    private static int RelocateVar(Block block, int insertAt, Var varStatement)
    {
      if (varStatement.Parent is ForIn parent2)
        insertAt = ReorderScopeVisitor.RelocateForInVar(block, insertAt, varStatement, parent2);
      else if (block[insertAt] != varStatement)
      {
        if (block[insertAt] is Var element1 && block[insertAt + 1] == varStatement)
        {
          element1.Append((AstNode) varStatement);
          block.RemoveAt(insertAt + 1);
        }
        else if (element1 != null && varStatement.Parent is ForNode parent1 && parent1.Initializer == varStatement && parent1 == block[insertAt + 1])
        {
          varStatement.InsertAt(0, (AstNode) element1);
          block.RemoveAt(insertAt);
        }
        else
        {
          int num = 0;
          for (int index = 0; index < varStatement.Count; ++index)
          {
            if (varStatement[index].Initializer != null)
              ++num;
          }
          if (num <= 2)
          {
            List<AstNode> astNodeList1 = new List<AstNode>();
            for (int index1 = 0; index1 < varStatement.Count; ++index1)
            {
              VariableDeclaration variableDeclaration1 = varStatement[index1];
              if (variableDeclaration1.Initializer != null)
              {
                AstNode initializer = variableDeclaration1.Initializer;
                variableDeclaration1.Initializer = (AstNode) null;
                AstNode astNode = BindingTransform.FromBinding(variableDeclaration1.Binding);
                if (variableDeclaration1.IsCCSpecialCase)
                {
                  List<AstNode> astNodeList2 = astNodeList1;
                  VariableDeclaration variableDeclaration2 = new VariableDeclaration(variableDeclaration1.Context);
                  variableDeclaration2.Binding = astNode;
                  variableDeclaration2.AssignContext = variableDeclaration1.AssignContext;
                  variableDeclaration2.Initializer = initializer;
                  variableDeclaration2.IsCCSpecialCase = true;
                  variableDeclaration2.UseCCOn = variableDeclaration1.UseCCOn;
                  variableDeclaration2.TerminatingContext = variableDeclaration1.TerminatingContext;
                  VariableDeclaration variableDeclaration3 = variableDeclaration2;
                  astNodeList2.Add((AstNode) variableDeclaration3);
                }
                else
                  astNodeList1.Add((AstNode) new BinaryOperator(variableDeclaration1.Context)
                  {
                    Operand1 = astNode,
                    Operand2 = initializer,
                    OperatorToken = JSToken.Assign,
                    OperatorContext = variableDeclaration1.AssignContext
                  });
              }
              if (!(variableDeclaration1.Binding is BindingIdentifier))
              {
                bool flag = true;
                foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(variableDeclaration1.Binding))
                {
                  if (flag)
                  {
                    Var var = varStatement;
                    int index2 = index1;
                    VariableDeclaration variableDeclaration4 = new VariableDeclaration(binding.Context);
                    variableDeclaration4.Binding = (AstNode) new BindingIdentifier(binding.Context)
                    {
                      Name = binding.Name,
                      VariableField = binding.VariableField
                    };
                    VariableDeclaration variableDeclaration5 = variableDeclaration4;
                    var[index2] = variableDeclaration5;
                    flag = false;
                  }
                  else
                  {
                    Var var = varStatement;
                    int index3;
                    index1 = index3 = index1 + 1;
                    VariableDeclaration variableDeclaration6 = new VariableDeclaration(binding.Context);
                    variableDeclaration6.Binding = (AstNode) new BindingIdentifier(binding.Context)
                    {
                      Name = binding.Name,
                      VariableField = binding.VariableField
                    };
                    VariableDeclaration element = variableDeclaration6;
                    var.InsertAt(index3, (AstNode) element);
                  }
                }
              }
            }
            if (astNodeList1.Count > 0)
            {
              AstNode astNode = astNodeList1[0];
              for (int index = 1; index < astNodeList1.Count; ++index)
                astNode = CommaOperator.CombineWithComma(astNode.Context.FlattenToStart(), astNode, astNodeList1[index]);
              varStatement.Parent.ReplaceChild((AstNode) varStatement, astNode);
            }
            else
              varStatement.Parent.ReplaceChild((AstNode) varStatement, (AstNode) null);
            if (element1 != null)
              element1.Append((AstNode) varStatement);
            else
              block.Insert(insertAt, (AstNode) varStatement);
          }
        }
      }
      return insertAt;
    }

    private static int RelocateForInVar(Block block, int insertAt, Var varStatement, ForIn forIn)
    {
      VariableDeclaration variableDeclaration1;
      if (varStatement.Count == 1 && (variableDeclaration1 = varStatement[0]).Initializer == null)
      {
        IList<BindingIdentifier> bindingIdentifierList = BindingsVisitor.Bindings(variableDeclaration1.Binding);
        forIn.Variable = BindingTransform.FromBinding(variableDeclaration1.Binding);
        if (!(variableDeclaration1.Binding is BindingIdentifier))
        {
          bool flag = true;
          foreach (BindingIdentifier bindingIdentifier in (IEnumerable<BindingIdentifier>) bindingIdentifierList)
          {
            if (flag)
            {
              Var var = varStatement;
              VariableDeclaration variableDeclaration2 = new VariableDeclaration(bindingIdentifier.Context);
              variableDeclaration2.Binding = (AstNode) new BindingIdentifier(bindingIdentifier.Context)
              {
                Name = bindingIdentifier.Name,
                VariableField = bindingIdentifier.VariableField
              };
              VariableDeclaration variableDeclaration3 = variableDeclaration2;
              var[0] = variableDeclaration3;
              flag = false;
            }
            else
            {
              Var var = varStatement;
              VariableDeclaration variableDeclaration4 = new VariableDeclaration(bindingIdentifier.Context);
              variableDeclaration4.Binding = (AstNode) new BindingIdentifier(bindingIdentifier.Context)
              {
                Name = bindingIdentifier.Name,
                VariableField = bindingIdentifier.VariableField
              };
              VariableDeclaration element = variableDeclaration4;
              var.Append((AstNode) element);
            }
          }
        }
        if (block[insertAt] is Var var1)
          var1.Append((AstNode) varStatement);
        else
          block.Insert(insertAt, (AstNode) varStatement);
      }
      return insertAt;
    }

    private static void UnnestBlocks(Block node)
    {
      for (int index = node.Count - 1; index >= 0; --index)
      {
        if (node[index] is Block node1)
        {
          ReorderScopeVisitor.UnnestBlocks(node1);
          if (!node1.HasOwnScope)
          {
            node.RemoveAt(index);
            node.InsertRange(index, node1.Children);
          }
        }
        else if (node[index] is EmptyStatement)
          node.RemoveAt(index);
        else if (index > 0 && node[index - 1] is ConditionalCompilationComment compilationComment1 && node[index] is ConditionalCompilationComment compilationComment2)
        {
          compilationComment1.Statements.Append((AstNode) compilationComment2.Statements);
          node.RemoveAt(index);
        }
      }
    }

    public override void Visit(Block node)
    {
      if (node == null)
        return;
      ReorderScopeVisitor.UnnestBlocks(node);
      node.ForceBraces = node.Parent is TryNode;
      if (this.m_combineAdjacentVars)
      {
        for (int index = node.Count - 1; index > 0; --index)
        {
          if (node[index - 1] is Declaration declaration)
          {
            if (declaration.StatementToken == ReorderScopeVisitor.DeclarationType(node[index]))
            {
              declaration.Append(node[index]);
              node.RemoveAt(index);
            }
          }
          else if (node[index - 1] is ExportNode exportNode1 && exportNode1.Count == 1 && exportNode1.ModuleName.IsNullOrWhiteSpace())
          {
            JSToken jsToken = ReorderScopeVisitor.DeclarationType(exportNode1[0]);
            if (jsToken != JSToken.None && node[index] is ExportNode exportNode && exportNode.Count == 1 && exportNode.ModuleName.IsNullOrWhiteSpace() && jsToken == ReorderScopeVisitor.DeclarationType(exportNode[0]))
            {
              ((Declaration) exportNode1[0]).Append(exportNode[0]);
              node.RemoveAt(index);
            }
          }
        }
      }
      if (node.IsModule)
      {
        int index1 = node.Count - 1;
        while (index1 >= 0 && node[index1] is ImportantComment)
          --index1;
        if (index1 > 0)
        {
          ExportNode exportNode = ReorderScopeVisitor.IfTargetExport(node[index1]);
          int num = node.Count - (exportNode == null ? 1 : 2);
          List<ExportNode> exportNodeList = new List<ExportNode>();
          for (int index2 = num; index2 >= 0; --index2)
          {
            if (node[index2] is ExportNode node1 && node1.ModuleName.IsNullOrWhiteSpace())
            {
              if (ReorderScopeVisitor.IfTargetExport((AstNode) node1) != null)
              {
                if (node1 != exportNode)
                {
                  if (exportNode != null)
                  {
                    exportNode.Insert(0, (AstNode) node1);
                    node.RemoveAt(index2);
                  }
                  else
                  {
                    node.RemoveAt(index2);
                    node.Append((AstNode) node1);
                    exportNode = node1;
                  }
                }
              }
              else if (node1.Count == 1)
                exportNodeList.Add(node1);
            }
          }
          int count = exportNodeList.Count;
        }
      }
      base.Visit(node);
    }

    private static JSToken DeclarationType(AstNode node) => node is Declaration declaration ? declaration.StatementToken : JSToken.None;

    private static ExportNode IfTargetExport(AstNode node) => !(node is ExportNode exportNode) || !exportNode.ModuleName.IsNullOrWhiteSpace() || exportNode.Count <= 0 || !(exportNode[0] is ImportExportSpecifier) ? (ExportNode) null : exportNode;

    public override void Visit(ConditionalCompilationComment node)
    {
      if (node == null || node.Statements == null || node.Statements.Count <= 0)
        return;
      ++this.m_conditionalCommentLevel;
      base.Visit(node);
      --this.m_conditionalCommentLevel;
    }

    public override void Visit(ConditionalCompilationIf node)
    {
      if (node == null)
        return;
      ++this.m_conditionalCommentLevel;
      base.Visit(node);
    }

    public override void Visit(ConditionalCompilationEnd node)
    {
      if (node == null)
        return;
      --this.m_conditionalCommentLevel;
    }

    public override void Visit(ConstantWrapper node)
    {
      if (node == null || !(node.Parent is Block) || !this.IsMinificationHint(node))
        return;
      node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
    }

    public override void Visit(DirectivePrologue node)
    {
      if (node == null)
        return;
      if (this.IsMinificationHint((ConstantWrapper) node))
      {
        node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
      }
      else
      {
        if (this.m_moduleDirectives == null)
          this.m_moduleDirectives = new List<DirectivePrologue>();
        this.m_moduleDirectives.Add(node);
      }
    }

    public override void Visit(FunctionObject node)
    {
      if (node == null)
        return;
      if (this.m_moveVarStatements || this.m_moveFunctionDecls)
      {
        if (node.FunctionType == FunctionType.Declaration && this.m_conditionalCommentLevel == 0)
        {
          if (this.m_functionDeclarations == null)
            this.m_functionDeclarations = new List<FunctionObject>();
          this.m_functionDeclarations.Add(node);
        }
        else
        {
          if (this.m_functionExpressions == null)
            this.m_functionExpressions = new List<FunctionObject>();
          this.m_functionExpressions.Add(node);
        }
      }
      else
        base.Visit(node);
    }

    public override void Visit(Var node)
    {
      if (node == null)
        return;
      if (this.m_moveVarStatements && this.m_conditionalCommentLevel == 0)
      {
        if (this.m_varStatements == null)
          this.m_varStatements = new List<Var>();
        this.m_varStatements.Add(node);
      }
      base.Visit(node);
    }

    public override void Visit(GroupingOperator node)
    {
      if (node == null)
        return;
      if (node.Parent != null)
      {
        bool flag = false;
        if (node.Operand == null)
          flag = true;
        else if (node.Parent is Block)
        {
          if (!(node.Operand is FunctionObject) && !(node.Operand is ObjectLiteral))
            flag = true;
        }
        else if (node.Parent is AstNodeList)
        {
          if (!(node.Operand is BinaryOperator operand) || operand.OperatorToken != JSToken.Comma)
            flag = true;
        }
        else if (node.Parent.IsExpression)
        {
          OperatorPrecedence operatorPrecedence = node.Parent.Precedence;
          if (node.Parent is Conditional parent)
            operatorPrecedence = parent.Condition == node ? OperatorPrecedence.LogicalOr : OperatorPrecedence.Assignment;
          if (operatorPrecedence <= node.Operand.Precedence)
            flag = true;
        }
        else
          flag = true;
        if (flag)
          node.Parent.ReplaceChild((AstNode) node, node.Operand);
      }
      if (node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
    }

    public override void Visit(ModuleDeclaration node)
    {
      if (node == null || node.Body == null)
        return;
      if (this.m_moduleDeclarations == null)
        this.m_moduleDeclarations = new List<ModuleDeclaration>();
      this.m_moduleDeclarations.Add(node);
    }

    private bool IsMinificationHint(ConstantWrapper node)
    {
      bool flag = false;
      if (node.PrimitiveType == PrimitiveType.String)
      {
        string str1 = node.ToString();
        char[] separator = new char[1]{ ',' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          int length = str2.IndexOf(':');
          if (length >= 0 && string.Compare(str2.Substring(length + 1).Trim(), "nomunge", StringComparison.OrdinalIgnoreCase) == 0)
          {
            flag = true;
            string strA = str2.Substring(0, length).Trim();
            if (string.IsNullOrEmpty(strA) || string.CompareOrdinal(strA, "*") == 0)
              strA = (string) null;
            foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) (node.EnclosingScope ?? (ActivationObject) this.m_globalScope).NameTable.Values)
            {
              if (jsVariableField.OuterField == null && (strA == null || string.CompareOrdinal(strA, jsVariableField.Name) == 0))
                jsVariableField.CanCrunch = false;
            }
          }
        }
      }
      return flag;
    }
  }
}
