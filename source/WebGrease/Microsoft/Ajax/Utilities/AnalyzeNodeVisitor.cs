// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.AnalyzeNodeVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  internal class AnalyzeNodeVisitor : TreeVisitor
  {
    private JSParser m_parser;
    private bool m_encounteredCCOn;
    private MatchPropertiesVisitor m_matchVisitor;
    private Stack<ActivationObject> m_scopeStack;
    private JSError m_strictNameError = JSError.StrictModeVariableName;
    private HashSet<string> m_noRename;
    private bool m_stripDebug;
    private bool m_lookForDebugNamespaces;
    private bool m_possibleDebugNamespace;
    private int m_possibleDebugNamespaceIndex;
    private List<string[]> m_possibleDebugMatches;
    private string[][] m_debugNamespaceParts;

    public AnalyzeNodeVisitor(JSParser parser)
    {
      this.m_parser = parser;
      this.m_scopeStack = new Stack<ActivationObject>();
      this.m_scopeStack.Push((ActivationObject) parser.GlobalScope);
      this.m_stripDebug = this.m_parser.Settings.StripDebugStatements && this.m_parser.Settings.IsModificationAllowed(TreeModifications.StripDebugStatements);
      this.m_lookForDebugNamespaces = this.m_stripDebug && this.m_parser.DebugLookups.Count > 0;
      if (this.m_lookForDebugNamespaces)
      {
        this.m_possibleDebugMatches = new List<string[]>();
        this.m_debugNamespaceParts = new string[this.m_parser.DebugLookups.Count][];
        int num = 0;
        foreach (string debugLookup in (IEnumerable<string>) this.m_parser.DebugLookups)
          this.m_debugNamespaceParts[num++] = debugLookup.Split('.');
      }
      if (this.m_parser.Settings.LocalRenaming == LocalRenaming.KeepAll)
        return;
      this.m_noRename = new HashSet<string>(this.m_parser.Settings.NoAutoRenameCollection);
    }

    public override void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      if (node.Operand1 != null)
        node.Operand1.Accept((IVisitor) this);
      if (node.Operand2 != null)
        node.Operand2.Accept((IVisitor) this);
      if ((node.Operand1 == null || node.Operand1.IsDebugOnly) && (node.Operand2 == null || node.Operand2.IsDebugOnly))
      {
        node.IsDebugOnly = true;
      }
      else
      {
        if (node.Operand1 != null && node.Operand1.IsDebugOnly)
          node.Operand1 = AnalyzeNodeVisitor.ClearDebugExpression(node.Operand1);
        if (node.Operand2 != null && node.Operand2.IsDebugOnly)
          node.Operand2 = AnalyzeNodeVisitor.ClearDebugExpression(node.Operand2);
        if (node.OperatorToken == JSToken.Minus && this.m_parser.Settings.IsModificationAllowed(TreeModifications.SimplifyStringToNumericConversion))
        {
          if (!(node.Operand1 is Lookup operand1) || !(node.Operand2 is ConstantWrapper operand2) || !operand2.IsIntegerLiteral || operand2.ToNumber() != 0.0)
            return;
          UnaryOperator newNode = new UnaryOperator(node.Context)
          {
            Operand = (AstNode) operand1,
            OperatorToken = JSToken.FirstBinaryOperator
          };
          node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
        }
        else if ((node.OperatorToken == JSToken.StrictEqual || node.OperatorToken == JSToken.StrictNotEqual) && this.m_parser.Settings.IsModificationAllowed(TreeModifications.ReduceStrictOperatorIfTypesAreSame))
        {
          PrimitiveType primitiveType1 = node.Operand1.FindPrimitiveType();
          if (primitiveType1 == PrimitiveType.Other)
            return;
          PrimitiveType primitiveType2 = node.Operand2.FindPrimitiveType();
          if (primitiveType1 == primitiveType2)
          {
            node.OperatorToken = node.OperatorToken == JSToken.StrictEqual ? JSToken.Equal : JSToken.NotEqual;
          }
          else
          {
            if (primitiveType2 == PrimitiveType.Other)
              return;
            node.Context.HandleError(JSError.StrictComparisonIsAlwaysTrueOrFalse);
            node.Parent.ReplaceChild((AstNode) node, (AstNode) new ConstantWrapper((object) (node.OperatorToken == JSToken.StrictNotEqual), PrimitiveType.Boolean, node.Context));
            DetachReferences.Apply((AstNode) node);
          }
        }
        else if (node.IsAssign)
        {
          if (!(node.Operand1 is Lookup operand1))
            return;
          if (operand1.VariableField != null && operand1.VariableField.InitializationOnly)
          {
            operand1.Context.HandleError(JSError.AssignmentToConstant, true);
          }
          else
          {
            if (!this.m_scopeStack.Peek().UseStrict)
              return;
            if (operand1.VariableField == null || operand1.VariableField.FieldType == FieldType.UndefinedGlobal)
            {
              node.Operand1.Context.HandleError(JSError.StrictModeUndefinedVariable, true);
            }
            else
            {
              if (operand1.VariableField.FieldType != FieldType.Arguments && (operand1.VariableField.FieldType != FieldType.Predefined || string.CompareOrdinal(operand1.Name, "eval") != 0))
                return;
              node.Operand1.Context.HandleError(JSError.StrictModeInvalidAssign, true);
            }
          }
        }
        else
        {
          if (!(node.Parent is Block) && (!(node.Parent is CommaOperator) || !(node.Parent.Parent is Block)) || node.OperatorToken != JSToken.LogicalOr && node.OperatorToken != JSToken.LogicalAnd)
            return;
          LogicalNot logicalNot = new LogicalNot(node.Operand1, this.m_parser.Settings);
          if (logicalNot.Measure() >= 0)
            return;
          logicalNot.Apply();
          node.OperatorToken = node.OperatorToken == JSToken.LogicalAnd ? JSToken.LogicalOr : JSToken.LogicalAnd;
        }
      }
    }

    public override void Visit(BindingIdentifier node)
    {
      if (node == null)
        return;
      AnalyzeNodeVisitor.ValidateIdentifier(this.m_scopeStack.Peek().UseStrict, node.Name, node.Context, this.m_strictNameError);
    }

    private void CombineExpressions(Block node)
    {
      for (int ndx = node.Count - 1; ndx > 0; --ndx)
      {
        if (ndx < node.Count)
        {
          if (node[ndx - 1].IsExpression)
            this.CombineWithPreviousExpression(node, ndx);
          else if (node[ndx - 1] is Var previousVar)
            AnalyzeNodeVisitor.CombineWithPreviousVar(node, ndx, previousVar);
        }
      }
    }

    private void CombineWithPreviousExpression(Block node, int ndx)
    {
      if (node[ndx].IsExpression)
        AnalyzeNodeVisitor.CombineTwoExpressions(node, ndx);
      else if (node[ndx] is ReturnNode returnNode)
        AnalyzeNodeVisitor.CombineReturnWithExpression(node, ndx, returnNode);
      else if (node[ndx] is ForNode forNode)
        this.CombineForNodeWithExpression(node, ndx, forNode);
      else if (node[ndx] is IfNode ifNode)
      {
        ifNode.Condition = CommaOperator.CombineWithComma(node[ndx - 1].Context.FlattenToStart(), node[ndx - 1], ifNode.Condition);
        node.RemoveAt(ndx - 1);
      }
      else
      {
        if (!(node[ndx] is WhileNode whileNode) || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.ChangeWhileToFor))
          return;
        AstNode astNode = node[ndx - 1];
        Block block = node;
        int index = ndx;
        ForNode forNode1 = new ForNode(astNode.Context.FlattenToStart());
        forNode1.Initializer = astNode;
        forNode1.Condition = whileNode.Condition;
        forNode1.Body = whileNode.Body;
        ForNode forNode2 = forNode1;
        block[index] = (AstNode) forNode2;
        node.RemoveAt(ndx - 1);
      }
    }

    private static void CombineTwoExpressions(Block node, int ndx)
    {
      BinaryOperator operand1_1 = node[ndx - 1] as BinaryOperator;
      BinaryOperator operand2 = node[ndx] as BinaryOperator;
      if (operand1_1 != null && operand2 != null && operand1_1.IsAssign && operand2.IsAssign && operand2.OperatorToken != JSToken.Assign && operand2.Operand1 is Lookup operand1_2 && operand1_1.Operand1.IsEquivalentTo(operand2.Operand1))
      {
        if (operand1_1.OperatorToken == JSToken.Assign)
        {
          BinaryOperator binaryOperator = new BinaryOperator(operand1_1.Operand2.Context.Clone().CombineWith(operand2.Operand2.Context))
          {
            Operand1 = operand1_1.Operand2,
            Operand2 = operand2.Operand2,
            OperatorToken = JSScanner.StripAssignment(operand2.OperatorToken),
            OperatorContext = operand2.OperatorContext
          };
          operand1_1.Operand2 = (AstNode) binaryOperator;
          if (operand1_2.VariableField != null)
            operand1_2.VariableField.References.Remove((INameReference) operand1_2);
          node[ndx] = (AstNode) null;
        }
        else
        {
          AstNode astNode = CommaOperator.CombineWithComma(operand1_1.Context.Clone().CombineWith(operand2.Context), (AstNode) operand1_1, (AstNode) operand2);
          node[ndx - 1] = astNode;
          node[ndx] = (AstNode) null;
        }
      }
      else
      {
        AstNode astNode = CommaOperator.CombineWithComma(node[ndx - 1].Context.Clone().CombineWith(node[ndx].Context), node[ndx - 1], node[ndx]);
        node[ndx] = astNode;
        node[ndx - 1] = (AstNode) null;
      }
    }

    private static void CombineReturnWithExpression(Block node, int ndx, ReturnNode returnNode)
    {
      if (returnNode.Operand == null || !returnNode.Operand.IsExpression)
        return;
      if (node[ndx - 1] is BinaryOperator binaryOperator && binaryOperator.IsAssign && binaryOperator.Operand1 is Lookup operand1)
      {
        if (returnNode.Operand.IsEquivalentTo((AstNode) operand1))
        {
          if (binaryOperator.OperatorToken == JSToken.Assign)
          {
            if (operand1.VariableField == null || operand1.VariableField.OuterField != null || operand1.VariableField.IsReferencedInnerScope)
            {
              DetachReferences.Apply(returnNode.Operand);
              returnNode.Operand = (AstNode) binaryOperator;
              node[ndx - 1] = (AstNode) null;
            }
            else
            {
              JSVariableField variableField = operand1.VariableField;
              DetachReferences.Apply((AstNode) operand1, returnNode.Operand);
              returnNode.Operand = binaryOperator.Operand2;
              node[ndx - 1] = (AstNode) null;
              if (variableField.RefCount != 0)
                return;
              INameDeclaration onlyDeclaration = variableField.OnlyDeclaration;
              if (onlyDeclaration == null || onlyDeclaration.Initializer != null && !onlyDeclaration.Initializer.IsConstant || !(onlyDeclaration.Parent is VariableDeclaration parent1))
                return;
              Declaration parent2 = parent1.Parent as Declaration;
              parent2.Remove(parent1);
              variableField.WasRemoved = true;
              if (parent2.Count != 0)
                return;
              parent2.Parent.ReplaceChild((AstNode) parent2, (AstNode) null);
            }
          }
          else
          {
            if (operand1.VariableField != null)
              DetachReferences.Apply(returnNode.Operand);
            node.RemoveAt(ndx - 1);
            returnNode.Operand = (AstNode) binaryOperator;
            if (operand1.VariableField == null || operand1.VariableField.OuterField != null || operand1.VariableField.IsReferencedInnerScope)
              return;
            binaryOperator.OperatorToken = JSScanner.StripAssignment(binaryOperator.OperatorToken);
          }
        }
        else
        {
          AstNode astNode = CommaOperator.CombineWithComma(node[ndx - 1].Context.FlattenToStart(), node[ndx - 1], returnNode.Operand);
          returnNode.Operand = astNode;
          node[ndx - 1] = (AstNode) null;
        }
      }
      else
      {
        AstNode astNode = CommaOperator.CombineWithComma(node[ndx - 1].Context.FlattenToStart(), node[ndx - 1], returnNode.Operand);
        returnNode.Operand = astNode;
        node[ndx - 1] = (AstNode) null;
      }
    }

    private void CombineForNodeWithExpression(Block node, int ndx, ForNode forNode)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.MoveInExpressionsIntoForStatement) && node[ndx - 1].ContainsInOperator)
        return;
      if (forNode.Initializer == null)
      {
        forNode.Initializer = node[ndx - 1];
        node[ndx - 1] = (AstNode) null;
      }
      else
      {
        if (!forNode.Initializer.IsExpression)
          return;
        AstNode astNode = CommaOperator.CombineWithComma(node[ndx - 1].Context.FlattenToStart(), node[ndx - 1], forNode.Initializer);
        forNode.Initializer = astNode;
        node[ndx - 1] = (AstNode) null;
      }
    }

    private static void CombineWithPreviousVar(Block node, int ndx, Var previousVar)
    {
      if (previousVar.Count == 0)
        return;
      BinaryOperator binaryOperator = node[ndx] as BinaryOperator;
      VariableDeclaration variableDeclaration = previousVar[previousVar.Count - 1];
      Lookup lookup;
      if (binaryOperator == null || !binaryOperator.IsAssign || (lookup = binaryOperator.Operand1 as Lookup) == null || lookup.VariableField == null || AnalyzeNodeVisitor.ContainsReference(binaryOperator.Operand2, lookup.VariableField) || !(variableDeclaration.Binding is BindingIdentifier binding) || binding.VariableField != lookup.VariableField)
        return;
      if (variableDeclaration.Initializer != null)
      {
        if (binaryOperator.OperatorToken == JSToken.Assign)
        {
          if (!variableDeclaration.Initializer.IsConstant)
            return;
          variableDeclaration.Initializer = binaryOperator.Operand2;
          lookup.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.References.Remove((INameReference) lookup)));
          node[ndx] = (AstNode) null;
        }
        else
        {
          lookup.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.References.Remove((INameReference) lookup)));
          binaryOperator.OperatorToken = JSScanner.StripAssignment(binaryOperator.OperatorToken);
          binaryOperator.Operand1 = variableDeclaration.Initializer;
          binaryOperator.UpdateWith(binaryOperator.Operand1.Context);
          variableDeclaration.Initializer = (AstNode) binaryOperator;
          node[ndx] = (AstNode) null;
        }
      }
      else
      {
        if (binaryOperator.OperatorToken != JSToken.Assign)
          return;
        lookup.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.References.Remove((INameReference) lookup)));
        variableDeclaration.Initializer = binaryOperator.Operand2;
        node[ndx] = (AstNode) null;
      }
    }

    private static bool ContainsReference(AstNode node, JSVariableField targetField)
    {
      if (node is Lookup lookup)
        return lookup.VariableField != null ? lookup.VariableField == targetField : string.CompareOrdinal(lookup.Name, targetField.Name) == 0;
      foreach (AstNode child in node.Children)
      {
        if (AnalyzeNodeVisitor.ContainsReference(child, targetField))
          return true;
      }
      return false;
    }

    private static AstNode FindLastStatement(Block node)
    {
      int index = node.Count - 1;
      while (index >= 0 && (node[index] is FunctionObject || node[index] is ImportantComment))
        --index;
      return index < 0 ? (AstNode) null : node[index];
    }

    public override void Visit(Block node)
    {
      if (node == null)
        return;
      ActivationObject activationObject = (ActivationObject) null;
      if (node.HasOwnScope)
      {
        activationObject = (ActivationObject) (node.EnclosingScope as BlockScope);
        if (node.Parent is FunctionObject parent)
          activationObject = parent.EnclosingScope;
      }
      if (activationObject != null)
      {
        foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) activationObject.LexicallyDeclaredNames)
        {
          INameDeclaration nameDeclaration = activationObject.VarDeclaredName(lexicallyDeclaredName.Name);
          if (nameDeclaration != null)
          {
            nameDeclaration.Context.HandleError(JSError.DuplicateLexicalDeclaration, lexicallyDeclaredName is LexicalDeclaration);
            lexicallyDeclaredName.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
            nameDeclaration.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
          }
        }
      }
      bool flag1 = node.Parent is FunctionObject;
      if (node.HasOwnScope)
        this.m_scopeStack.Push(node.EnclosingScope);
      JSError strictNameError = this.m_strictNameError;
      try
      {
        this.m_strictNameError = JSError.StrictModeVariableName;
        for (int index = node.Count - 1; index >= 0; --index)
        {
          node[index].Accept((IVisitor) this);
          if (this.m_stripDebug && node.Count > index && node[index].IsDebugOnly)
            node.RemoveAt(index);
        }
      }
      finally
      {
        this.m_strictNameError = strictNameError;
        if (node.HasOwnScope)
          this.m_scopeStack.Pop();
      }
      if (this.m_parser.Settings.RemoveUnneededCode)
      {
        for (int index1 = 0; index1 < node.Count; ++index1)
        {
          if (node[index1] is IfNode ifNode && ifNode.TrueBlock != null && ifNode.TrueBlock.Count > 0 && ifNode.FalseBlock != null)
          {
            if (ifNode.TrueBlock[ifNode.TrueBlock.Count - 1] is ReturnNode)
            {
              node.InsertRange(index1 + 1, ifNode.FalseBlock.Children);
              ifNode.FalseBlock = (Block) null;
            }
          }
          else if (node[index1] is ReturnNode || node[index1] is Break || node[index1] is ContinueNode || node[index1] is ThrowNode)
          {
            for (int index2 = node.Count - 1; index2 > index1; --index2)
            {
              if (node[index2].IsDeclaration)
              {
                if (node[index2] is Declaration declaration && declaration.StatementToken != JSToken.Const)
                {
                  for (int index3 = 0; index3 < declaration.Count; ++index3)
                  {
                    if (declaration[index3].Initializer != null)
                    {
                      DetachReferences.Apply(declaration[index3].Initializer);
                      declaration[index3].Initializer = (AstNode) null;
                    }
                  }
                }
              }
              else
              {
                DetachReferences.Apply(node[index2]);
                node.RemoveAt(index2);
              }
            }
          }
        }
      }
      if (flag1 && node.Count > 0 && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionReturnToCondition) && AnalyzeNodeVisitor.FindLastStatement(node) is IfNode lastStatement1 && lastStatement1.FalseBlock == null && lastStatement1.TrueBlock.Count == 1 && lastStatement1.TrueBlock[0] is ReturnNode returnNode1)
      {
        if (returnNode1.Operand == null)
        {
          if (lastStatement1.Condition.IsConstant)
            node.ReplaceChild((AstNode) lastStatement1, (AstNode) null);
          else
            node.ReplaceChild((AstNode) lastStatement1, lastStatement1.Condition);
        }
        else if (returnNode1.Operand.IsExpression)
        {
          Conditional node1 = new Conditional(lastStatement1.Condition.Context.FlattenToStart())
          {
            Condition = lastStatement1.Condition,
            TrueExpression = returnNode1.Operand,
            FalseExpression = (AstNode) AnalyzeNodeVisitor.CreateVoidNode(returnNode1.Context.FlattenToStart())
          };
          node.ReplaceChild((AstNode) lastStatement1, (AstNode) new ReturnNode(lastStatement1.Context)
          {
            Operand = (AstNode) node1
          });
          this.Optimize(node1);
        }
      }
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.CombineAdjacentExpressionStatements))
        this.CombineExpressions(node);
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.MoveVarIntoFor))
      {
        for (int index4 = node.Count - 1; index4 > 0; --index4)
        {
          if (node[index4 - 1] is Var var1 && node[index4] is ForNode forNode)
          {
            if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.MoveInExpressionsIntoForStatement) || !var1.ContainsInOperator)
            {
              if (forNode.Initializer != null)
              {
                if (forNode.Initializer is Var initializer2)
                {
                  initializer2.InsertAt(0, (AstNode) var1);
                  node.RemoveAt(index4 - 1);
                }
                else if (forNode.Initializer is BinaryOperator initializer1 && AnalyzeNodeVisitor.AreAssignmentsInVar(initializer1, var1))
                {
                  AnalyzeNodeVisitor.ConvertAssignmentsToVarDecls(initializer1, (Declaration) var1, this.m_parser);
                  forNode.Initializer = (AstNode) var1;
                  node.RemoveAt(index4 - 1);
                }
              }
              else
              {
                node.RemoveAt(index4 - 1);
                forNode.Initializer = (AstNode) var1;
              }
            }
          }
          else if (var1 != null && node[index4] is WhileNode whileNode && this.m_parser.Settings.IsModificationAllowed(TreeModifications.ChangeWhileToFor))
          {
            Block block = node;
            int index5 = index4;
            ForNode forNode1 = new ForNode(whileNode.Context.FlattenToStart());
            forNode1.Initializer = (AstNode) var1;
            forNode1.Condition = whileNode.Condition;
            forNode1.Body = whileNode.Body;
            ForNode forNode2 = forNode1;
            block[index5] = (AstNode) forNode2;
            node.RemoveAt(index4 - 1);
          }
          else if (var1 != null && node[index4] is ForIn forIn && !(forIn.Variable is Declaration))
          {
            VariableDeclaration variableDeclaration1 = var1[var1.Count - 1];
            if (variableDeclaration1.IsEquivalentTo(forIn.Variable) && (variableDeclaration1.Initializer == null || variableDeclaration1.Initializer.IsConstant))
            {
              AstNode binding1 = BindingTransform.ToBinding(forIn.Variable);
              if (binding1 != null)
              {
                VariableDeclaration variableDeclaration2 = new VariableDeclaration(forIn.Variable.Context.Clone());
                variableDeclaration2.Binding = binding1;
                VariableDeclaration element = variableDeclaration2;
                Var var = new Var(forIn.Variable.Context.Clone());
                var.Append((AstNode) element);
                forIn.Variable = (AstNode) var;
                IList<BindingIdentifier> bindingIdentifierList = BindingsVisitor.Bindings(variableDeclaration1.Binding);
                foreach (BindingIdentifier binding2 in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(binding1))
                {
                  foreach (BindingIdentifier binding3 in (IEnumerable<BindingIdentifier>) bindingIdentifierList)
                  {
                    if (binding3.IsEquivalentTo((AstNode) binding2))
                    {
                      ActivationObject.RemoveBinding((AstNode) binding3);
                      break;
                    }
                  }
                }
              }
            }
          }
        }
      }
      if (AnalyzeNodeVisitor.FindLastStatement(node) is ReturnNode lastStatement2)
      {
        bool flag2 = false;
        int index = AnalyzeNodeVisitor.PreviousStatementIndex(node, (AstNode) lastStatement2);
        if (lastStatement2.Operand is Lookup operand1 && index >= 0 && node[index] is Declaration declaration)
        {
          VariableDeclaration variableDeclaration = declaration[declaration.Count - 1];
          if (variableDeclaration.Initializer != null && variableDeclaration.IsEquivalentTo((AstNode) operand1) && variableDeclaration.Binding is BindingIdentifier binding && binding.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.RefCount == 1)))
          {
            binding.VariableField.References.Remove((INameReference) operand1);
            binding.VariableField.Declarations.Remove((INameDeclaration) binding);
            if (declaration.Count == 1)
            {
              lastStatement2.Operand = variableDeclaration.Initializer;
              node.RemoveAt(index);
            }
            else
            {
              lastStatement2.Operand = variableDeclaration.Initializer;
              declaration[declaration.Count - 1] = (VariableDeclaration) null;
            }
          }
        }
        for (; index >= 0 && lastStatement2 != null && node[index] is IfNode ifNode && ifNode.TrueBlock != null && ifNode.TrueBlock.Count == 1 && ifNode.FalseBlock == null; lastStatement2 = node[index--] as ReturnNode)
        {
          bool flag3 = false;
          if (ifNode.TrueBlock[0] is ReturnNode returnNode2)
          {
            if (lastStatement2.Operand == null)
            {
              if (returnNode2.Operand == null)
              {
                if (!flag1)
                {
                  if (ifNode.Condition.IsConstant)
                  {
                    node.RemoveAt(index);
                    flag3 = true;
                  }
                  else
                    node[index] = ifNode.Condition;
                }
                else if (ifNode.Condition.IsConstant)
                {
                  node.ReplaceChild((AstNode) lastStatement2, (AstNode) null);
                  node.RemoveAt(index);
                  flag3 = true;
                }
                else if (node.ReplaceChild((AstNode) lastStatement2, ifNode.Condition))
                {
                  node.RemoveAt(index);
                  flag3 = true;
                }
              }
              else
              {
                Conditional node2 = new Conditional(ifNode.Condition.Context.FlattenToStart())
                {
                  Condition = ifNode.Condition,
                  TrueExpression = returnNode2.Operand,
                  FalseExpression = (AstNode) AnalyzeNodeVisitor.CreateVoidNode(returnNode2.Context.FlattenToStart())
                };
                if (node.ReplaceChild((AstNode) lastStatement2, (AstNode) new ReturnNode(returnNode2.Context.FlattenToStart())
                {
                  Operand = (AstNode) node2
                }))
                {
                  node.RemoveAt(index);
                  this.Optimize(node2);
                  flag3 = true;
                }
              }
            }
            else if (returnNode2.Operand == null)
            {
              Conditional node3 = new Conditional(ifNode.Condition.Context.FlattenToStart())
              {
                Condition = ifNode.Condition,
                TrueExpression = (AstNode) AnalyzeNodeVisitor.CreateVoidNode(lastStatement2.Context.FlattenToStart()),
                FalseExpression = lastStatement2.Operand
              };
              if (node.ReplaceChild((AstNode) lastStatement2, (AstNode) new ReturnNode(lastStatement2.Context.FlattenToStart())
              {
                Operand = (AstNode) node3
              }))
              {
                node.RemoveAt(index);
                this.Optimize(node3);
                flag3 = true;
              }
            }
            else if (returnNode2.Operand.IsEquivalentTo(lastStatement2.Operand))
            {
              if (ifNode.Condition.IsConstant)
              {
                DetachReferences.Apply(returnNode2.Operand);
                node.RemoveAt(index);
                flag3 = true;
              }
              else
              {
                DetachReferences.Apply(returnNode2.Operand);
                lastStatement2.Operand = CommaOperator.CombineWithComma(ifNode.Condition.Context.FlattenToStart(), ifNode.Condition, lastStatement2.Operand);
                node.RemoveAt(index);
                flag3 = true;
              }
            }
            else
            {
              Conditional node4 = new Conditional(ifNode.Condition.Context.FlattenToStart())
              {
                Condition = ifNode.Condition,
                TrueExpression = returnNode2.Operand,
                FalseExpression = lastStatement2.Operand
              };
              lastStatement2.Operand = (AstNode) node4;
              node.RemoveAt(index);
              this.Optimize(node4);
              flag3 = true;
            }
          }
          if (flag3)
            flag2 = true;
          else
            break;
        }
        if (flag2 && this.m_parser.Settings.IsModificationAllowed(TreeModifications.CombineAdjacentExpressionStatements))
          this.CombineExpressions(node);
        if (lastStatement2 != null && lastStatement2.Operand is Conditional operand2)
        {
          if (operand2.FalseExpression is UnaryOperator falseExpression && falseExpression.OperatorToken == JSToken.Void && falseExpression.Operand is ConstantWrapper)
          {
            if (operand2.TrueExpression is UnaryOperator trueExpression1 && trueExpression1.OperatorToken == JSToken.Void)
            {
              if (flag1)
              {
                node.ReplaceChild((AstNode) lastStatement2, operand2.Condition);
              }
              else
              {
                node.ReplaceChild((AstNode) lastStatement2, operand2.Condition);
                node.Append((AstNode) new ReturnNode(lastStatement2.Context.Clone()));
              }
            }
            else if (flag1)
            {
              IfNode newNode = new IfNode(lastStatement2.Context)
              {
                Condition = operand2.Condition,
                TrueBlock = AstNode.ForceToBlock((AstNode) new ReturnNode(lastStatement2.Context.Clone())
                {
                  Operand = operand2.TrueExpression
                })
              };
              node.ReplaceChild((AstNode) lastStatement2, (AstNode) newNode);
            }
          }
          else if (flag1 && operand2.TrueExpression is UnaryOperator trueExpression2 && trueExpression2.OperatorToken == JSToken.Void && trueExpression2.Operand is ConstantWrapper)
          {
            new LogicalNot(operand2.Condition, this.m_parser.Settings).Apply();
            IfNode newNode = new IfNode(lastStatement2.Context)
            {
              Condition = operand2.Condition,
              TrueBlock = AstNode.ForceToBlock((AstNode) new ReturnNode(lastStatement2.Context.Clone())
              {
                Operand = operand2.FalseExpression
              })
            };
            node.ReplaceChild((AstNode) lastStatement2, (AstNode) newNode);
          }
        }
      }
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.CombineEquivalentIfReturns))
      {
        for (int index = node.Count - 1; index > 0; --index)
        {
          AstNode matchExpression1 = (AstNode) null;
          AstNode condition1;
          if (AnalyzeNodeVisitor.IsIfReturnExpr(node[index], out condition1, ref matchExpression1) != null)
          {
            AstNode matchExpression2 = matchExpression1;
            AstNode condition2;
            IfNode ifNode1 = AnalyzeNodeVisitor.IsIfReturnExpr(node[index - 1], out condition2, ref matchExpression2);
            if (ifNode1 != null)
            {
              IfNode ifNode2 = ifNode1;
              BinaryOperator binaryOperator1 = new BinaryOperator(condition2.Context.FlattenToStart());
              binaryOperator1.Operand1 = condition2;
              binaryOperator1.Operand2 = condition1;
              binaryOperator1.OperatorToken = JSToken.LogicalOr;
              binaryOperator1.TerminatingContext = ifNode1.TerminatingContext ?? node.TerminatingContext;
              BinaryOperator binaryOperator2 = binaryOperator1;
              ifNode2.Condition = (AstNode) binaryOperator2;
              DetachReferences.Apply(matchExpression1);
              node.RemoveAt(index);
            }
          }
        }
      }
      if (flag1 && this.m_parser.Settings.IsModificationAllowed(TreeModifications.InvertIfReturn))
      {
        for (int index6 = node.Count - 1; index6 >= 0; --index6)
        {
          if (node[index6] is IfNode ifNode3 && ifNode3.FalseBlock == null && ifNode3.TrueBlock != null && ifNode3.TrueBlock.Count == 1 && ifNode3.TrueBlock[0] is ReturnNode returnNode3 && returnNode3.Operand == null)
          {
            LogicalNot.Apply(ifNode3.Condition, this.m_parser.Settings);
            ifNode3.TrueBlock.Clear();
            int index7 = index6 + 1;
            if (node.Count == index7 + 1)
            {
              if (node[index7] is IfNode ifNode && (ifNode.FalseBlock == null || ifNode.FalseBlock.Count == 0))
              {
                node.RemoveAt(index7);
                ifNode3.Condition = (AstNode) new BinaryOperator(ifNode3.Condition.Context.FlattenToStart())
                {
                  Operand1 = ifNode3.Condition,
                  Operand2 = ifNode.Condition,
                  OperatorToken = JSToken.LogicalAnd
                };
                ifNode3.TrueBlock = ifNode.TrueBlock;
              }
              else if (node[index7].IsExpression && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionCallToConditionAndCall))
              {
                AstNode expression = node[index7];
                node.RemoveAt(index7);
                this.IfConditionExpressionToExpression(ifNode3, expression);
              }
            }
            while (node.Count > index7)
            {
              AstNode astNode = node[index7];
              node.RemoveAt(index7);
              ifNode3.TrueBlock.Append(astNode);
            }
          }
        }
      }
      else
      {
        if (!(node.Parent is ForNode) && !(node.Parent is ForIn) && !(node.Parent is WhileNode) && !(node.Parent is DoWhile) || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.InvertIfContinue))
          return;
        for (int index8 = node.Count - 1; index8 >= 0; --index8)
        {
          if (node[index8] is IfNode ifNode4 && ifNode4.FalseBlock == null && ifNode4.TrueBlock != null && ifNode4.TrueBlock.Count == 1 && ifNode4.TrueBlock[0] is ContinueNode continueNode && (string.IsNullOrEmpty(continueNode.Label) || AnalyzeNodeVisitor.LabelMatchesParent(continueNode.Label, node.Parent)))
          {
            if (index8 < node.Count - 1)
            {
              LogicalNot.Apply(ifNode4.Condition, this.m_parser.Settings);
              ifNode4.TrueBlock.Clear();
              int index9 = index8 + 1;
              if (node.Count == index9 + 1)
              {
                if (node[index9] is IfNode ifNode && (ifNode.FalseBlock == null || ifNode.FalseBlock.Count == 0))
                {
                  ifNode4.Condition = (AstNode) new BinaryOperator(ifNode4.Condition.Context.FlattenToStart())
                  {
                    Operand1 = ifNode4.Condition,
                    Operand2 = ifNode.Condition,
                    OperatorToken = JSToken.LogicalAnd
                  };
                  ifNode4.TrueBlock = ifNode.TrueBlock;
                  node.RemoveAt(index9);
                }
                else if (node[index9].IsExpression && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionCallToConditionAndCall))
                {
                  AstNode expression = node[index9];
                  node.RemoveAt(index9);
                  this.IfConditionExpressionToExpression(ifNode4, expression);
                }
              }
              while (node.Count > index9)
              {
                AstNode astNode = node[index9];
                node.RemoveAt(index9);
                ifNode4.TrueBlock.Append(astNode);
              }
            }
            else if (ifNode4.Condition.IsConstant)
              node.RemoveAt(index8);
            else
              node[index8] = ifNode4.Condition;
          }
        }
      }
    }

    private static bool LabelMatchesParent(string label, AstNode parentNode)
    {
      bool flag = false;
      for (; parentNode.Parent is LabeledStatement parent; parentNode = (AstNode) parent)
      {
        if (string.CompareOrdinal(label, parent.Label) == 0)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    private static IfNode IsIfReturnExpr(
      AstNode node,
      out AstNode condition,
      ref AstNode matchExpression)
    {
      condition = (AstNode) null;
      if (node is IfNode ifNode && ifNode.FalseBlock == null && ifNode.TrueBlock != null && ifNode.TrueBlock.Count == 1 && ifNode.TrueBlock[0] is ReturnNode returnNode && (matchExpression == null || matchExpression.IsEquivalentTo(returnNode.Operand)))
      {
        matchExpression = returnNode.Operand;
        condition = ifNode.Condition;
      }
      return condition == null || matchExpression == null ? (IfNode) null : ifNode;
    }

    private static int PreviousStatementIndex(Block node, AstNode child)
    {
      int index = node.IndexOf(child) - 1;
      while (index >= 0 && (node[index] is FunctionObject || node[index] is ImportantComment))
        --index;
      return index;
    }

    public override void Visit(Break node)
    {
      if (node == null)
        return;
      if (!node.Label.IsNullOrWhiteSpace() && node.LabelInfo == null && this.m_parser.Settings.RemoveUnneededCode && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryLabels))
        node.Label = (string) null;
      if (AnalyzeNodeVisitor.IsInsideLoop((AstNode) node, true))
        return;
      node.Context.HandleError(JSError.BadBreak, true);
    }

    public override void Visit(CallNode node)
    {
      if (node == null)
        return;
      Member function1 = node.Function as Member;
      if (node.IsConstructor)
      {
        if (!(node.Function is FunctionObject functionObject))
        {
          GroupingOperator function2 = node.Function as GroupingOperator;
          Func<GroupingOperator, FunctionObject> action = (Func<GroupingOperator, FunctionObject>) (g => g.Operand as FunctionObject);
          if ((functionObject = function2.IfNotNull<GroupingOperator, FunctionObject>(action)) == null)
          {
            if (this.m_parser.Settings.CollapseToLiteral && node.Function is Lookup function3)
            {
              if (function3.Name == "Object" && this.m_parser.Settings.IsModificationAllowed(TreeModifications.NewObjectToObjectLiteral))
              {
                if (node.Arguments == null || node.Arguments.Count == 0)
                {
                  ObjectLiteral newNode = new ObjectLiteral(node.Context);
                  if (node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode))
                    return;
                  goto label_17;
                }
                else
                {
                  if (node.Arguments.Count == 1 && node.Arguments[0] is ObjectLiteral newNode)
                  {
                    node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
                    newNode.Accept((IVisitor) this);
                    return;
                  }
                  goto label_17;
                }
              }
              else if (function3.Name == "Array" && this.m_parser.Settings.IsModificationAllowed(TreeModifications.NewArrayToArrayLiteral))
              {
                ConstantWrapper constantWrapper = node.Arguments == null || node.Arguments.Count != 1 ? (ConstantWrapper) null : node.Arguments[0] as ConstantWrapper;
                if (node.Arguments == null || node.Arguments.Count != 1 || constantWrapper != null && !constantWrapper.IsNumericLiteral)
                {
                  ArrayLiteral newNode = new ArrayLiteral(node.Context)
                  {
                    Elements = node.Arguments
                  };
                  if (node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode))
                  {
                    newNode.Accept((IVisitor) this);
                    return;
                  }
                  goto label_17;
                }
                else
                  goto label_17;
              }
              else
                goto label_17;
            }
            else
              goto label_17;
          }
        }
        if (functionObject.FunctionType == FunctionType.ArrowFunction)
          node.Function.Context.HandleError(JSError.ArrowCannotBeConstructor, true);
      }
label_17:
      IList<ResourceStrings> resourceStrings1 = this.m_parser.Settings.ResourceStrings;
      if (node.InBrackets && resourceStrings1.Count > 0)
      {
        if (this.m_matchVisitor == null)
          this.m_matchVisitor = new MatchPropertiesVisitor();
        for (int index = resourceStrings1.Count - 1; index >= 0; --index)
        {
          ResourceStrings resourceStrings2 = resourceStrings1[index];
          if (resourceStrings2 != null && this.m_matchVisitor.Match(node.Function, resourceStrings2.Name))
          {
            if (node.Arguments.Count == 1)
            {
              if (node.Arguments[0] is ConstantWrapper constantWrapper)
              {
                string name = constantWrapper.Value.ToString();
                ConstantWrapper newNode = new ConstantWrapper((object) resourceStrings2[name], PrimitiveType.String, node.Context);
                node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
                newNode.Accept((IVisitor) this);
                return;
              }
              node.Context.HandleError(JSError.ResourceReferenceMustBeConstant, true);
            }
            else
              node.Context.HandleError(JSError.ResourceReferenceMustBeConstant, true);
          }
        }
      }
      if (node.InBrackets && node.Arguments != null)
      {
        string constantArgument = node.Arguments.SingleConstantArgument;
        if (constantArgument != null)
        {
          string newName;
          if (this.m_parser.Settings.HasRenamePairs && this.m_parser.Settings.ManualRenamesProperties && this.m_parser.Settings.IsModificationAllowed(TreeModifications.PropertyRenaming) && !string.IsNullOrEmpty(newName = this.m_parser.Settings.GetNewName(constantArgument)))
          {
            if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.BracketMemberToDotMember) && JSScanner.IsSafeIdentifier(newName) && !JSScanner.IsKeyword(newName, (node.EnclosingScope ?? (ActivationObject) this.m_parser.GlobalScope).UseStrict))
            {
              Member newNode = new Member(node.Context)
              {
                Root = node.Function,
                Name = constantArgument,
                NameContext = node.Arguments[0].Context
              };
              node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
              newNode.Accept((IVisitor) this);
              return;
            }
            node.Arguments[0] = (AstNode) new ConstantWrapper((object) newName, PrimitiveType.String, node.Arguments[0].Context);
          }
          else if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.BracketMemberToDotMember) && JSScanner.IsSafeIdentifier(constantArgument) && !JSScanner.IsKeyword(constantArgument, (node.EnclosingScope ?? (ActivationObject) this.m_parser.GlobalScope).UseStrict))
          {
            Member newNode = new Member(node.Context)
            {
              Root = node.Function,
              Name = constantArgument,
              NameContext = node.Arguments[0].Context
            };
            node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
            newNode.Accept((IVisitor) this);
            return;
          }
        }
      }
      base.Visit(node);
      if (node.Function != null && node.Function.IsDebugOnly)
      {
        node.IsDebugOnly = true;
        if (!node.IsConstructor)
          return;
        AstNode parent = node.Parent;
        CallNode oldNode = node;
        ObjectLiteral objectLiteral = new ObjectLiteral(node.Context);
        objectLiteral.IsDebugOnly = true;
        ObjectLiteral newNode = objectLiteral;
        parent.ReplaceChild((AstNode) oldNode, (AstNode) newNode);
      }
      else
      {
        Member function4 = node.Function as Member;
        Lookup function5 = node.Function as Lookup;
        bool flag = false;
        if (function5 != null && string.CompareOrdinal(function5.Name, "eval") == 0 && function5.VariableField.FieldType == FieldType.Predefined)
          flag = true;
        else if (function4 != null && string.CompareOrdinal(function4.Name, "eval") == 0)
        {
          if (function4.Root.IsWindowLookup)
            flag = true;
        }
        else if (node.Function is CallNode function6 && function6.InBrackets && function6.Function.IsWindowLookup && function6.Arguments.IsSingleConstantArgument("eval"))
          flag = true;
        if (!flag || this.m_parser.Settings.EvalTreatment == EvalTreatment.Ignore)
          return;
        this.m_scopeStack.Peek().IsKnownAtCompileTime = false;
      }
    }

    public override void Visit(ClassNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.ClassType == ClassType.Expression && node.Binding != null && node.Binding is BindingIdentifier binding && binding.VariableField != null && binding.VariableField.RefCount == 0 && this.m_parser.Settings.RemoveFunctionExpressionNames && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveFunctionExpressionNames))
        node.Binding = (AstNode) null;
      if (node.Elements == null)
        return;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (AstNode element in node.Elements)
      {
        string name;
        if (element is FunctionObject functionObject && functionObject.Binding != null && !(name = functionObject.Binding.Name).IsNullOrWhiteSpace())
        {
          Context context = functionObject.Binding.Context ?? functionObject.Context;
          if (!stringSet.Add(AnalyzeNodeVisitor.ClassElementKeyName(functionObject.FunctionType, name)))
            context.HandleError(JSError.DuplicateClassElementName, true);
          if (functionObject.FunctionType == FunctionType.Getter || functionObject.FunctionType == FunctionType.Setter)
          {
            if (stringSet.Contains(AnalyzeNodeVisitor.ClassElementKeyName(FunctionType.Method, name)))
              context.HandleError(JSError.DuplicateClassElementName, true);
          }
          else if (stringSet.Contains(AnalyzeNodeVisitor.ClassElementKeyName(FunctionType.Getter, name)) || stringSet.Contains(AnalyzeNodeVisitor.ClassElementKeyName(FunctionType.Setter, name)))
            context.HandleError(JSError.DuplicateClassElementName, true);
          if ((functionObject.FunctionType != FunctionType.Method || functionObject.IsGenerator) && string.CompareOrdinal(name, "constructor") == 0)
            context.HandleError(JSError.SpecialConstructor, true);
          else if (functionObject.IsStatic && string.CompareOrdinal(name, "prototype") == 0)
            context.HandleError(JSError.StaticPrototype, true);
        }
      }
    }

    private static string ClassElementKeyName(FunctionType funcType, string name)
    {
      switch (funcType)
      {
        case FunctionType.Getter:
          return "get_" + name;
        case FunctionType.Setter:
          return "set_" + name;
        default:
          return "method_" + name;
      }
    }

    public override void Visit(ComprehensionNode node)
    {
      if (node == null)
        return;
      node.BlockScope.IfNotNull<BlockScope>((Action<BlockScope>) (s => this.m_scopeStack.Push((ActivationObject) s)));
      try
      {
        if (node.Clauses != null)
          node.Clauses.Accept((IVisitor) this);
        if (node.Expression == null)
          return;
        node.Expression.Accept((IVisitor) this);
      }
      finally
      {
        node.BlockScope.IfNotNull<BlockScope, ActivationObject>((Func<BlockScope, ActivationObject>) (s => this.m_scopeStack.Pop()));
      }
    }

    private void Optimize(Conditional node)
    {
      if (node.Condition != null && node.Condition.IsDebugOnly)
      {
        if (node.FalseExpression == null || node.FalseExpression.IsDebugOnly)
        {
          AstNode parent = node.Parent;
          Conditional oldNode = node;
          ConstantWrapper constantWrapper = new ConstantWrapper((object) null, PrimitiveType.Null, node.Context);
          constantWrapper.IsDebugOnly = true;
          ConstantWrapper newNode = constantWrapper;
          parent.ReplaceChild((AstNode) oldNode, (AstNode) newNode);
        }
        else
          node.Parent.ReplaceChild((AstNode) node, node.FalseExpression);
      }
      else
      {
        if (node.Condition is UnaryOperator condition && condition.OperatorToken == JSToken.LogicalNot && !condition.OperatorInConditionalCompilationComment && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfNotTrueFalseToIfFalseTrue))
        {
          node.Condition = condition.Operand;
          node.SwapBranches();
        }
        if (!(node.TrueExpression is BinaryOperator trueExpression) || !trueExpression.IsAssign || !(node.FalseExpression is BinaryOperator falseExpression) || falseExpression.OperatorToken != trueExpression.OperatorToken || !trueExpression.Operand1.IsEquivalentTo(falseExpression.Operand1))
          return;
        DetachReferences.Apply(falseExpression.Operand1);
        BinaryOperator binaryOperator = new BinaryOperator(node.Context);
        binaryOperator.Operand1 = trueExpression.Operand1;
        binaryOperator.Operand2 = (AstNode) new Conditional(node.Context)
        {
          Condition = node.Condition,
          QuestionContext = node.QuestionContext,
          TrueExpression = trueExpression.Operand2,
          ColonContext = node.ColonContext,
          FalseExpression = falseExpression.Operand2
        };
        binaryOperator.OperatorContext = trueExpression.OperatorContext;
        binaryOperator.OperatorToken = trueExpression.OperatorToken;
        binaryOperator.TerminatingContext = node.TerminatingContext;
        BinaryOperator newNode = binaryOperator;
        node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
      }
    }

    public override void Visit(Conditional node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.Optimize(node);
    }

    public override void Visit(ConditionalCompilationOn node) => this.m_encounteredCCOn = true;

    private static bool StringSourceIsNotInlineSafe(string source)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(source))
        flag = source.IndexOf("</", StringComparison.Ordinal) >= 0 || source.IndexOf("]]>", StringComparison.Ordinal) >= 0;
      return flag;
    }

    public override void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      if (node.PrimitiveType == PrimitiveType.String && this.m_parser.Settings.ErrorIfNotInlineSafe && node.Context != null && AnalyzeNodeVisitor.StringSourceIsNotInlineSafe(node.Context.Code))
        node.Context.HandleError(JSError.StringNotInlineSafe, true);
      AstNode astNode = (AstNode) null;
      for (AstNode parent = node.Parent; parent != null; parent = parent.Parent)
      {
        if (parent is CallNode callNode && astNode == callNode.Arguments && callNode.Function is Lookup function && function.Name == "RegExp")
        {
          node.IsParameterToRegExp = true;
          break;
        }
        astNode = parent;
      }
    }

    public override void Visit(ConstStatement node)
    {
      if (node == null)
        return;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings((AstNode) node))
      {
        if (!stringSet.Add(binding.Name))
          binding.Context.HandleError(JSError.DuplicateConstantDeclaration, true);
      }
      base.Visit(node);
    }

    public override void Visit(ContinueNode node)
    {
      if (node == null)
        return;
      if (!node.Label.IsNullOrWhiteSpace() && node.LabelInfo == null && this.m_parser.Settings.RemoveUnneededCode && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryLabels))
        node.Label = (string) null;
      if (AnalyzeNodeVisitor.IsInsideLoop((AstNode) node, false))
        return;
      node.Context.HandleError(JSError.BadContinue, true);
    }

    public override void Visit(DebuggerNode node)
    {
      if (node == null)
        return;
      node.IsDebugOnly = true;
    }

    public override void Visit(DoWhile node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Body != null && node.Body.Count == 0)
        node.Body = (Block) null;
      if (node.Condition == null || !node.Condition.IsDebugOnly)
        return;
      if (node.Body == null)
        node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
      else
        node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
    }

    public override void Visit(ExportNode node)
    {
      if (node == null)
        return;
      if (!node.IsDefault)
      {
        if (!node.ModuleName.IsNullOrWhiteSpace())
        {
          foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings((AstNode) node))
          {
            if (binding.VariableField != null)
              binding.VariableField.CanCrunch = false;
          }
        }
        else if (node.Count == 1 && (node[0] is Declaration || node[0] is FunctionObject || node[0] is ClassNode))
        {
          foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(node[0]))
          {
            if (binding.VariableField != null)
              binding.VariableField.CanCrunch = false;
          }
        }
      }
      base.Visit(node);
    }

    public override void Visit(ForNode node)
    {
      if (node == null)
        return;
      if (node.BlockScope != null)
      {
        foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) node.BlockScope.LexicallyDeclaredNames)
        {
          if (node.Body != null && node.Body.HasOwnScope)
          {
            INameDeclaration nameDeclaration = node.Body.EnclosingScope.LexicallyDeclaredName(lexicallyDeclaredName.Name);
            if (nameDeclaration != null)
            {
              nameDeclaration.Context.HandleError(JSError.DuplicateLexicalDeclaration, true);
              if (nameDeclaration.VariableField != null)
              {
                nameDeclaration.VariableField.OuterField = lexicallyDeclaredName.VariableField;
                if (lexicallyDeclaredName.VariableField != null && !nameDeclaration.VariableField.CanCrunch)
                  lexicallyDeclaredName.VariableField.CanCrunch = false;
              }
            }
          }
          INameDeclaration nameDeclaration1 = node.BlockScope.VarDeclaredName(lexicallyDeclaredName.Name);
          if (nameDeclaration1 != null)
          {
            nameDeclaration1.Context.HandleError(JSError.DuplicateLexicalDeclaration, lexicallyDeclaredName is LexicalDeclaration);
            nameDeclaration1.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
            lexicallyDeclaredName.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
          }
        }
      }
      base.Visit(node);
      if (node.Body != null && node.Body.Count == 0 && !node.Body.HasOwnScope)
        node.Body = (Block) null;
      if (node.Initializer != null && node.Initializer.IsDebugOnly)
        node.Initializer = (AstNode) null;
      if (node.Incrementer != null && node.Incrementer.IsDebugOnly)
        node.Incrementer = (AstNode) null;
      if (node.Condition == null || !node.Condition.IsDebugOnly)
        return;
      if (node.Initializer == null && node.Incrementer == null && node.Body == null)
        node.IsDebugOnly = true;
      else
        node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
    }

    public override void Visit(ForIn node)
    {
      if (node == null)
        return;
      if (node.BlockScope != null)
      {
        foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) node.BlockScope.LexicallyDeclaredNames)
        {
          if (node.Body != null && node.Body.HasOwnScope)
          {
            INameDeclaration nameDeclaration = node.Body.EnclosingScope.LexicallyDeclaredName(lexicallyDeclaredName.Name);
            if (nameDeclaration != null)
            {
              nameDeclaration.Context.HandleError(JSError.DuplicateLexicalDeclaration, true);
              if (nameDeclaration.VariableField != null)
              {
                nameDeclaration.VariableField.OuterField = lexicallyDeclaredName.VariableField;
                if (lexicallyDeclaredName.VariableField != null && !nameDeclaration.VariableField.CanCrunch)
                  lexicallyDeclaredName.VariableField.CanCrunch = false;
              }
            }
          }
          INameDeclaration nameDeclaration1 = node.BlockScope.VarDeclaredName(lexicallyDeclaredName.Name);
          if (nameDeclaration1 != null)
          {
            nameDeclaration1.Context.HandleError(JSError.DuplicateLexicalDeclaration, lexicallyDeclaredName is LexicalDeclaration);
            nameDeclaration1.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
            lexicallyDeclaredName.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
          }
        }
      }
      base.Visit(node);
      if (node.Body != null && node.Body.Count == 0 && !node.Body.HasOwnScope)
        node.Body = (Block) null;
      if (node.Collection == null || !node.Collection.IsDebugOnly)
        return;
      if (node.Body == null)
      {
        node.IsDebugOnly = true;
      }
      else
      {
        ForIn forIn = node;
        ObjectLiteral objectLiteral1 = new ObjectLiteral(node.Collection.Context);
        objectLiteral1.IsDebugOnly = true;
        ObjectLiteral objectLiteral2 = objectLiteral1;
        forIn.Collection = (AstNode) objectLiteral2;
      }
    }

    public override void Visit(FunctionObject node)
    {
      if (node == null)
        return;
      if (node.Binding == null || node.Binding.Name.IsNullOrWhiteSpace() || node.IsExpression && node.Binding.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.RefCount == 0)) && this.m_parser.Settings.RemoveFunctionExpressionNames && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveFunctionExpressionNames))
        node.NameGuess = AnalyzeNodeVisitor.GuessAtName((AstNode) node);
      bool useStrict = this.m_scopeStack.Peek().UseStrict;
      if (useStrict && node.Binding != null && (string.CompareOrdinal(node.Binding.Name, "eval") == 0 || string.CompareOrdinal(node.Binding.Name, "arguments") == 0))
      {
        if (node.Binding.Context != null)
          node.Binding.Context.HandleError(JSError.StrictModeFunctionName, true);
        else if (node.Context != null)
          node.Context.HandleError(JSError.StrictModeFunctionName, true);
      }
      if (node.FunctionType == FunctionType.Setter && (node.ParameterDeclarations == null || node.ParameterDeclarations.Count != 1))
        (node.ParameterDeclarations.IfNotNull<AstNodeList, Context>((Func<AstNodeList, Context>) (p => p.Context)) ?? node.Context).HandleError(JSError.SetterMustHaveOneParameter, true);
      else if (node.ParameterDeclarations.IfNotNull<AstNodeList, bool>((Func<AstNodeList, bool>) (p => p.Count > 1)))
      {
        int lastParameterIndex = node.ParameterDeclarations.Count - 1;
        node.ParameterDeclarations.ForEach<ParameterDeclaration>((Action<ParameterDeclaration>) (paramDecl =>
        {
          if (paramDecl.Position == lastParameterIndex || !paramDecl.HasRest)
            return;
          paramDecl.Context.HandleError(JSError.RestParameterNotLast, true);
        }));
      }
      if (node.ParameterDeclarations != null && node.ParameterDeclarations.Count > 0)
      {
        JSError strictNameError = this.m_strictNameError;
        this.m_strictNameError = JSError.StrictModeArgumentName;
        node.ParameterDeclarations.Accept((IVisitor) this);
        this.m_strictNameError = strictNameError;
        HashSet<string> stringSet = new HashSet<string>();
        foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings((AstNode) node.ParameterDeclarations))
        {
          if (!stringSet.Add(binding.Name))
          {
            if (useStrict)
              binding.Context.HandleError(JSError.StrictModeDuplicateArgument, true);
            else
              binding.Context.HandleError(JSError.DuplicateName);
          }
        }
      }
      if (node.Body != null)
      {
        this.m_scopeStack.Push(node.EnclosingScope);
        try
        {
          node.Body.Accept((IVisitor) this);
        }
        finally
        {
          this.m_scopeStack.Pop();
        }
      }
      if (node.ParameterDeclarations != null && node.ParameterDeclarations.Count > 0)
      {
        bool removeIfUnreferenced = this.m_parser.Settings.RemoveUnneededCode && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnusedParameters);
        bool foundLastReference = false;
        for (int index = node.ParameterDeclarations.Count - 1; index >= 0; --index)
        {
          if (node.ParameterDeclarations[index] is ParameterDeclaration parameterDeclaration)
          {
            if (AnalyzeNodeVisitor.CheckParametersAreReferenced(parameterDeclaration.Binding, removeIfUnreferenced, foundLastReference))
            {
              if (!foundLastReference)
              {
                parameterDeclaration.Context.HandleError(JSError.ArgumentNotReferenced);
                if (removeIfUnreferenced)
                  node.ParameterDeclarations.RemoveAt(index);
              }
            }
            else
              foundLastReference = true;
          }
        }
      }
      if (node.FunctionType != FunctionType.ArrowFunction || !node.Body.IfNotNull<Block, bool>((Func<Block, bool>) (b => b.Count == 1)) || !(node.Body[0] is ReturnNode oldNode))
        return;
      node.Body.ReplaceChild((AstNode) oldNode, oldNode.Operand);
      node.Body.IsConcise = true;
    }

    private static bool CheckParametersAreReferenced(
      AstNode binding,
      bool removeIfUnreferenced,
      bool foundLastReference)
    {
      bool flag;
      if (binding is BindingIdentifier bindingIdentifier)
      {
        flag = false;
        if (bindingIdentifier.VariableField != null)
        {
          flag = !bindingIdentifier.VariableField.IsReferenced;
          if (flag && removeIfUnreferenced && !foundLastReference)
          {
            bindingIdentifier.VariableField.Declarations.Remove((INameDeclaration) bindingIdentifier);
            bindingIdentifier.VariableField.WasRemoved = true;
          }
        }
      }
      else
      {
        flag = true;
        foreach (BindingIdentifier binding1 in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(binding))
        {
          if (binding1.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => !v.IsReferenced)))
          {
            binding1.Context.HandleError(JSError.ArgumentNotReferenced);
            if (removeIfUnreferenced)
              ActivationObject.DeleteFromBindingPattern((AstNode) binding1, false);
          }
          else
            flag = false;
        }
        AnalyzeNodeVisitor.TrimTrailingElisionsFromArrayBindings(binding);
      }
      return flag;
    }

    private static void TrimTrailingElisionsFromArrayBindings(AstNode binding)
    {
      switch (binding)
      {
        case ArrayLiteral arrayLiteral:
          bool flag = true;
          for (int index = arrayLiteral.Elements.Count - 1; index >= 0; --index)
          {
            if (arrayLiteral.Elements[index] is ConstantWrapper)
            {
              if (flag)
                arrayLiteral.Elements.RemoveAt(index);
            }
            else
            {
              flag = false;
              AnalyzeNodeVisitor.TrimTrailingElisionsFromArrayBindings(arrayLiteral.Elements[index]);
            }
          }
          break;
        case ObjectLiteral objectLiteral:
          objectLiteral.Properties.ForEach<ObjectLiteralProperty>((Action<ObjectLiteralProperty>) (property => AnalyzeNodeVisitor.TrimTrailingElisionsFromArrayBindings(property.Value)));
          break;
      }
    }

    public override void Visit(IfNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.TrueBlock != null && node.TrueBlock.Count == 0)
        node.TrueBlock = (Block) null;
      if (node.FalseBlock != null && node.FalseBlock.Count == 0)
        node.FalseBlock = (Block) null;
      if (node.Condition != null && node.Condition.IsDebugOnly)
      {
        node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
        node.TrueBlock = (Block) null;
      }
      if (node.TrueBlock != null && node.FalseBlock != null)
      {
        if (node.TrueBlock.IsExpression && node.FalseBlock.IsExpression && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfExpressionsToExpression))
        {
          LogicalNot logicalNot = new LogicalNot(node.Condition, this.m_parser.Settings);
          Conditional conditional;
          if (logicalNot.Measure() < 0)
          {
            logicalNot.Apply();
            conditional = new Conditional(node.Context)
            {
              Condition = node.Condition,
              TrueExpression = node.FalseBlock[0],
              FalseExpression = node.TrueBlock[0]
            };
          }
          else
            conditional = new Conditional(node.Context)
            {
              Condition = node.Condition,
              TrueExpression = node.TrueBlock[0],
              FalseExpression = node.FalseBlock[0]
            };
          node.Parent.ReplaceChild((AstNode) node, (AstNode) conditional);
          this.Optimize(conditional);
        }
        else
        {
          LogicalNot logicalNot = new LogicalNot(node.Condition, this.m_parser.Settings);
          if (logicalNot.Measure() < 0)
          {
            logicalNot.Apply();
            node.SwapBranches();
          }
          if (node.TrueBlock.Count == 1 && node.FalseBlock.Count == 1 && node.TrueBlock[0] is ReturnNode returnNode1 && returnNode1.Operand != null && node.FalseBlock[0] is ReturnNode returnNode2 && returnNode2.Operand != null)
          {
            Conditional node1 = new Conditional(node.Condition.Context.FlattenToStart())
            {
              Condition = node.Condition,
              TrueExpression = returnNode1.Operand,
              FalseExpression = returnNode2.Operand
            };
            ReturnNode newNode = new ReturnNode(node.Context)
            {
              Operand = (AstNode) node1
            };
            node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
            this.Optimize(node1);
          }
        }
      }
      else if (node.FalseBlock != null)
      {
        if (node.FalseBlock.IsExpression && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionCallToConditionAndCall))
        {
          JSToken jsToken = JSToken.LogicalOr;
          LogicalNot logicalNot = new LogicalNot(node.Condition, this.m_parser.Settings);
          if (logicalNot.Measure() < 0)
          {
            logicalNot.Apply();
            jsToken = JSToken.LogicalAnd;
          }
          BinaryOperator newNode = new BinaryOperator(node.Context)
          {
            Operand1 = node.Condition,
            Operand2 = node.FalseBlock[0],
            OperatorToken = jsToken
          };
          node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
        }
        else if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionFalseToIfNotConditionTrue))
        {
          new LogicalNot(node.Condition, this.m_parser.Settings).Apply();
          node.SwapBranches();
        }
      }
      else if (node.TrueBlock != null)
      {
        if (node.TrueBlock.IsExpression && this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfConditionCallToConditionAndCall))
          this.IfConditionExpressionToExpression(node, node.TrueBlock[0]);
      }
      else if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.IfEmptyToExpression))
      {
        if (node.Condition == null || node.Condition.IsConstant || node.Condition.IsDebugOnly)
          node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
        else
          node.Parent.ReplaceChild((AstNode) node, node.Condition);
      }
      if (node.FalseBlock != null || node.TrueBlock == null || node.TrueBlock.Count != 1 || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.CombineNestedIfs) || !(node.TrueBlock[0] is IfNode ifNode) || ifNode.FalseBlock != null)
        return;
      node.Condition = (AstNode) new BinaryOperator(node.Condition.Context.FlattenToStart())
      {
        Operand1 = node.Condition,
        Operand2 = ifNode.Condition,
        OperatorToken = JSToken.LogicalAnd
      };
      node.TrueBlock = ifNode.TrueBlock;
    }

    private void IfConditionExpressionToExpression(IfNode ifNode, AstNode expression)
    {
      JSToken jsToken = JSToken.LogicalAnd;
      LogicalNot logicalNot = new LogicalNot(ifNode.Condition, this.m_parser.Settings);
      if (logicalNot.Measure() < 0)
      {
        logicalNot.Apply();
        jsToken = JSToken.LogicalOr;
      }
      BinaryOperator newNode = new BinaryOperator(ifNode.Context)
      {
        Operand1 = ifNode.Condition,
        Operand2 = expression,
        OperatorToken = jsToken
      };
      ifNode.Parent.ReplaceChild((AstNode) ifNode, (AstNode) newNode);
    }

    public override void Visit(ImportNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Count != 1 || node[0] is ImportExportSpecifier)
        return;
      foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(node[0]))
      {
        if (binding.VariableField != null)
          binding.VariableField.CanCrunch = false;
      }
    }

    public override void Visit(LabeledStatement node)
    {
      if (node == null)
        return;
      if (node.Statement != null)
        node.Statement.Accept((IVisitor) this);
      if (node.LabelInfo == null || node.LabelInfo.HasIssues)
        return;
      if (node.LabelInfo.RefCount == 0)
        node.LabelContext.HandleError(JSError.UnusedLabel);
      if (node.LabelInfo.RefCount == 0 && this.m_parser.Settings.RemoveUnneededCode && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryLabels))
      {
        if (node.Statement == null)
          node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
        else
          node.Parent.ReplaceChild((AstNode) node, node.Statement);
      }
      else
      {
        if (this.m_parser.Settings.LocalRenaming == LocalRenaming.KeepAll || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.LocalRenaming))
          return;
        node.LabelInfo.MinLabel = CrunchEnumerator.CrunchedLabel(node.LabelInfo.NestLevel);
      }
    }

    public override void Visit(Lookup node)
    {
      this.m_possibleDebugNamespace = false;
      if (node == null)
        return;
      if (node.Parent is CallNode)
        node.RefType = ((CallNode) node.Parent).IsConstructor ? ReferenceType.Constructor : ReferenceType.Function;
      ActivationObject activationObject = this.m_scopeStack.Peek();
      if (JSScanner.IsKeyword(node.Name, activationObject.UseStrict) && node.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.FieldType != FieldType.Super)))
        node.Context.HandleError(JSError.KeywordUsedAsIdentifier, true);
      bool parentIsMember = node.Parent is Member;
      if (node.VariableField != null && node.VariableField.FieldType == FieldType.Predefined)
      {
        if (string.CompareOrdinal(node.Name, "NaN") == 0)
          node.Parent.ReplaceChild((AstNode) node, (AstNode) new ConstantWrapper((object) double.NaN, PrimitiveType.Number, node.Context));
        else if (string.CompareOrdinal(node.Name, "Infinity") == 0)
          node.Parent.ReplaceChild((AstNode) node, (AstNode) new ConstantWrapper((object) double.PositiveInfinity, PrimitiveType.Number, node.Context));
        else if (this.m_lookForDebugNamespaces && parentIsMember && string.CompareOrdinal(node.Name, "window") == 0)
        {
          this.m_possibleDebugNamespace = true;
          this.m_possibleDebugNamespaceIndex = 0;
          this.m_possibleDebugMatches.Clear();
        }
      }
      if (!this.m_lookForDebugNamespaces || this.m_possibleDebugNamespace || !this.InitialDebugNameSpaceMatches(node.Name, parentIsMember))
        return;
      node.IsDebugOnly = true;
      AstNode parent = node.Parent;
      Lookup oldNode = node;
      ObjectLiteral objectLiteral = new ObjectLiteral(node.Context);
      objectLiteral.IsDebugOnly = true;
      ObjectLiteral newNode = objectLiteral;
      parent.ReplaceChild((AstNode) oldNode, (AstNode) newNode);
    }

    public override void Visit(Member node)
    {
      if (node == null)
        return;
      IList<ResourceStrings> resourceStrings1 = this.m_parser.Settings.ResourceStrings;
      if (resourceStrings1.Count > 0)
      {
        if (this.m_matchVisitor == null)
          this.m_matchVisitor = new MatchPropertiesVisitor();
        for (int index = resourceStrings1.Count - 1; index >= 0; --index)
        {
          ResourceStrings resourceStrings2 = resourceStrings1[index];
          if (this.m_matchVisitor.Match(node.Root, resourceStrings2.Name))
          {
            ConstantWrapper newNode = new ConstantWrapper((object) (resourceStrings2[node.Name] ?? string.Empty), PrimitiveType.String, node.Context);
            node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
            newNode.Accept((IVisitor) this);
            return;
          }
        }
      }
      if (this.m_parser.Settings.HasRenamePairs && this.m_parser.Settings.ManualRenamesProperties && this.m_parser.Settings.IsModificationAllowed(TreeModifications.PropertyRenaming))
      {
        string newName = this.m_parser.Settings.GetNewName(node.Name);
        if (!string.IsNullOrEmpty(newName))
          node.Name = newName;
      }
      if (JSScanner.IsKeyword(node.Name, this.m_scopeStack.Peek().UseStrict))
        node.NameContext.HandleError(JSError.KeywordUsedAsIdentifier);
      if (node.Root != null)
        node.Root.Accept((IVisitor) this);
      if (!this.m_stripDebug)
        return;
      bool parentIsMember = node.Parent is Member;
      if (node.Root.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (r => r.IsDebugOnly)))
      {
        node.IsDebugOnly = true;
      }
      else
      {
        if (!this.m_possibleDebugNamespace)
          return;
        if (this.m_possibleDebugMatches.Count == 0)
        {
          if (this.InitialDebugNameSpaceMatches(node.Name, parentIsMember))
            node.IsDebugOnly = true;
        }
        else
        {
          for (int index = this.m_possibleDebugMatches.Count - 1; index >= 0; --index)
          {
            string[] possibleDebugMatch = this.m_possibleDebugMatches[index];
            if (string.CompareOrdinal(node.Name, possibleDebugMatch[this.m_possibleDebugNamespaceIndex]) == 0)
            {
              if (possibleDebugMatch.Length == this.m_possibleDebugNamespaceIndex + 1)
              {
                node.IsDebugOnly = true;
                this.m_possibleDebugMatches.Clear();
                break;
              }
            }
            else
              this.m_possibleDebugMatches.RemoveAt(index);
          }
          if (this.m_possibleDebugMatches.Count > 0 && parentIsMember)
          {
            ++this.m_possibleDebugNamespaceIndex;
          }
          else
          {
            this.m_possibleDebugMatches.Clear();
            this.m_possibleDebugNamespace = false;
          }
        }
        if (!node.IsDebugOnly)
          return;
        AstNode parent = node.Parent;
        Member oldNode = node;
        ObjectLiteral objectLiteral = new ObjectLiteral(node.Context);
        objectLiteral.IsDebugOnly = true;
        ObjectLiteral newNode = objectLiteral;
        parent.ReplaceChild((AstNode) oldNode, (AstNode) newNode);
      }
    }

    private bool InitialDebugNameSpaceMatches(string name, bool parentIsMember)
    {
      foreach (string[] debugNamespacePart in this.m_debugNamespaceParts)
      {
        if (string.CompareOrdinal(name, debugNamespacePart[0]) == 0)
        {
          if (debugNamespacePart.Length == 1)
          {
            this.m_possibleDebugMatches.Clear();
            this.m_possibleDebugNamespace = false;
            return true;
          }
          if (parentIsMember)
            this.m_possibleDebugMatches.Add(debugNamespacePart);
        }
      }
      if (this.m_possibleDebugMatches.Count > 0)
      {
        this.m_possibleDebugNamespace = true;
        this.m_possibleDebugNamespaceIndex = 1;
      }
      return false;
    }

    public override void Visit(ObjectLiteral node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (this.m_parser.Settings.LocalRenaming != LocalRenaming.KeepAll)
        node.Properties.ForEach<ObjectLiteralProperty>((Action<ObjectLiteralProperty>) (property =>
        {
          if (property.Name != null)
            return;
          string name = property.Value.ToString();
          if (!JSScanner.IsValidIdentifier(name) || this.m_noRename.Contains(name) || !AnalyzeNodeVisitor.FieldCanBeRenamed(property.Value))
            return;
          property.Name = new ObjectLiteralField((object) name, PrimitiveType.String, property.Value.Context)
          {
            IsIdentifier = true
          };
        }));
      if (!this.m_scopeStack.Peek().UseStrict)
        return;
      Dictionary<string, string> nameMap = new Dictionary<string, string>();
      node.Properties.ForEach<ObjectLiteralProperty>((Action<ObjectLiteralProperty>) (property =>
      {
        string propertyType = AnalyzeNodeVisitor.GetPropertyType(property.Value as FunctionObject);
        string key = ((AstNode) property.Name ?? property.Value).ToString() + propertyType;
        if (propertyType == "data")
        {
          string str;
          if (nameMap.TryGetValue(key, out str) || nameMap.TryGetValue(((AstNode) property.Name ?? property.Value).ToString() + "get", out str) || nameMap.TryGetValue(((AstNode) property.Name ?? property.Value).ToString() + "set", out str))
          {
            ((AstNode) property.Name ?? property.Value).Context.HandleError(JSError.StrictModeDuplicateProperty, true);
            if (!(str != propertyType))
              return;
            nameMap.Add(key, propertyType);
          }
          else
            nameMap.Add(key, propertyType);
        }
        else
        {
          string str;
          if (nameMap.TryGetValue(key, out str) || nameMap.TryGetValue(((AstNode) property.Name ?? property.Value).ToString() + "data", out str))
          {
            ((AstNode) property.Name ?? property.Value).Context.HandleError(JSError.StrictModeDuplicateProperty, true);
            if (!(str != propertyType))
              return;
            nameMap.Add(key, propertyType);
          }
          else
            nameMap.Add(key, propertyType);
        }
      }));
    }

    private static bool FieldCanBeRenamed(AstNode node)
    {
      bool flag = false;
      if (node != null)
      {
        flag = (node as INameDeclaration).IfNotNull<INameDeclaration, bool>((Func<INameDeclaration, bool>) (n => !n.RenameNotAllowed && n.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch))));
        if (!flag)
          flag = (node as INameReference).IfNotNull<INameReference, bool>((Func<INameReference, bool>) (n => n.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch))));
      }
      return flag;
    }

    public override void Visit(ObjectLiteralField node)
    {
      if (node == null || node.PrimitiveType != PrimitiveType.String || !this.m_parser.Settings.HasRenamePairs || !this.m_parser.Settings.ManualRenamesProperties || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.PropertyRenaming))
        return;
      string newName = this.m_parser.Settings.GetNewName(node.Value.ToString());
      if (string.IsNullOrEmpty(newName))
        return;
      node.Value = (object) newName;
    }

    public override void Visit(ObjectLiteralProperty node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Value == null || !node.Value.IsDebugOnly)
        return;
      node.Value = (AstNode) new ConstantWrapper((object) null, PrimitiveType.Null, node.Value.Context);
    }

    private static string GetPropertyType(FunctionObject funcObj)
    {
      switch (funcObj.IfNotNull<FunctionObject, FunctionType>((Func<FunctionObject, FunctionType>) (f => f.FunctionType)))
      {
        case FunctionType.Getter:
          return "get";
        case FunctionType.Setter:
          return "set";
        case FunctionType.Method:
          return "method";
        default:
          return "data";
      }
    }

    public override void Visit(RegExpLiteral node)
    {
      if (node == null)
        return;
      try
      {
        if (new Regex(node.Pattern, RegexOptions.ECMAScript) != null)
          return;
        node.Context.HandleError(JSError.RegExpSyntax, true);
      }
      catch (ArgumentException ex)
      {
        node.Context.HandleError(JSError.RegExpSyntax, true);
      }
    }

    public override void Visit(ReturnNode node)
    {
      if (node == null)
        return;
      ActivationObject activationObject = this.m_scopeStack.Peek();
      while (true)
      {
        switch (activationObject)
        {
          case null:
          case FunctionScope _:
            goto label_4;
          default:
            activationObject = activationObject.Parent;
            continue;
        }
      }
label_4:
      if (activationObject == null)
        node.Context.HandleError(JSError.BadReturn);
      if (node.Operand == null)
        return;
      node.Operand.Accept((IVisitor) this);
      if (node.Operand == null || node.Operand.IsDebugOnly)
      {
        node.Operand = (AstNode) null;
      }
      else
      {
        if (!(node.Operand.LeftHandSide is Lookup leftHandSide) || leftHandSide.VariableField == null || leftHandSide.VariableField.OuterField != null || !(leftHandSide.Parent is BinaryOperator parent) || !parent.IsAssign || leftHandSide.VariableField.IsReferencedInnerScope)
          return;
        if (parent.OperatorToken != JSToken.Assign)
        {
          parent.OperatorToken = JSScanner.StripAssignment(parent.OperatorToken);
        }
        else
        {
          if (parent.Parent != node)
            return;
          leftHandSide.VariableField.References.Remove((INameReference) leftHandSide);
          node.Operand = parent.Operand2;
        }
      }
    }

    public override void Visit(Switch node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Expression != null && node.Expression.IsDebugOnly)
        node.Expression = (AstNode) new ConstantWrapper((object) null, PrimitiveType.Null, node.Expression.Context);
      if (node.BlockScope != null)
      {
        foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) node.BlockScope.LexicallyDeclaredNames)
        {
          INameDeclaration nameDeclaration = node.BlockScope.VarDeclaredName(lexicallyDeclaredName.Name);
          if (nameDeclaration != null)
          {
            nameDeclaration.Context.HandleError(JSError.DuplicateLexicalDeclaration, lexicallyDeclaredName is LexicalDeclaration);
            nameDeclaration.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
            lexicallyDeclaredName.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.CanCrunch = false));
          }
        }
      }
      if (!this.m_parser.Settings.RemoveUnneededCode)
        return;
      string str = string.Empty;
      if (node.Parent is LabeledStatement parent)
        str = parent.Label;
      int position = -1;
      bool flag1 = false;
      for (int index = 0; index < node.Cases.Count; ++index)
      {
        if (node.Cases[index] is SwitchCase switchCase)
        {
          if (switchCase.IsDefault)
          {
            position = index;
            flag1 = true;
          }
          if (flag1 && switchCase.Statements.Count > 0)
          {
            if (switchCase.Statements.Count == 1)
            {
              if (!(switchCase.Statements[0] is Break statement) || statement.Label != null && statement.Label != str)
              {
                flag1 = false;
                break;
              }
              break;
            }
            flag1 = false;
            break;
          }
        }
      }
      if (flag1 && position >= 0 && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveEmptyDefaultCase))
      {
        node.Cases.RemoveAt(position);
        position = -1;
      }
      if (position == -1 && this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveEmptyCaseWhenNoDefault))
      {
        bool flag2 = true;
        Break @break = (Break) null;
        for (int index = node.Cases.Count - 1; index >= 0; --index)
        {
          if (node.Cases[index] is SwitchCase switchCase)
          {
            if (switchCase.Statements.Count == 0 && flag2)
            {
              DetachReferences.Apply(switchCase.CaseValue);
              node.Cases.RemoveAt(index);
            }
            else
            {
              Break statement = switchCase.Statements.Count == 1 ? switchCase.Statements[0] as Break : (Break) null;
              if (statement != null)
              {
                if (statement.Label == null || statement.Label == str)
                {
                  @break = statement;
                  DetachReferences.Apply(switchCase.CaseValue);
                  node.Cases.RemoveAt(index);
                  flag2 = true;
                }
                else
                {
                  flag2 = false;
                  @break = (Break) null;
                }
              }
              else
              {
                if (flag2 && switchCase.Statements.Count > 0 && @break != null)
                {
                  switch (switchCase.Statements[switchCase.Statements.Count - 1])
                  {
                    case Break _:
                    case ContinueNode _:
                    case ReturnNode _:
                    case ThrowNode _:
                      break;
                    default:
                      switchCase.Statements.Append((AstNode) @break);
                      break;
                  }
                }
                @break = (Break) null;
                flag2 = false;
              }
            }
          }
        }
      }
      if (node.Cases.Count <= 0 || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveBreakFromLastCaseBlock) || !(node.Cases[node.Cases.Count - 1] is SwitchCase switchCase1))
        return;
      Block statements = switchCase1.Statements;
      Break break1 = statements.Count > 0 ? statements[statements.Count - 1] as Break : (Break) null;
      if (break1 == null || break1.Label != null && !(break1.Label == str))
        return;
      statements.RemoveLast();
    }

    public override void Visit(TryNode node)
    {
      if (node == null)
        return;
      node.TryBlock.IfNotNull<Block>((Action<Block>) (b => b.Accept((IVisitor) this)));
      if (node.TryBlock != null && node.TryBlock.Count == 0)
        node.TryBlock = (Block) null;
      this.DoCatchBlock(node);
      node.FinallyBlock.IfNotNull<Block>((Action<Block>) (b => b.Accept((IVisitor) this)));
      if (node.FinallyBlock == null || node.FinallyBlock.Count != 0 || node.CatchBlock == null || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveEmptyFinally))
        return;
      node.FinallyBlock = (Block) null;
    }

    private void DoCatchBlock(TryNode node)
    {
      node.CatchBlock.IfNotNull<Block>((Action<Block>) (b => b.Accept((IVisitor) this)));
      if (node.CatchParameter == null)
        return;
      this.m_strictNameError = JSError.StrictModeCatchName;
      node.CatchParameter.Accept((IVisitor) this);
      this.m_strictNameError = JSError.StrictModeVariableName;
      IList<BindingIdentifier> bindingIdentifierList = BindingsVisitor.Bindings((AstNode) node.CatchParameter);
      foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) node.CatchBlock.EnclosingScope.LexicallyDeclaredNames)
      {
        foreach (BindingIdentifier bindingIdentifier in (IEnumerable<BindingIdentifier>) bindingIdentifierList)
        {
          if (lexicallyDeclaredName != bindingIdentifier && string.CompareOrdinal(lexicallyDeclaredName.Name, bindingIdentifier.Name) == 0)
          {
            lexicallyDeclaredName.Context.HandleError(JSError.DuplicateLexicalDeclaration, lexicallyDeclaredName is LexicalDeclaration);
            if (lexicallyDeclaredName.VariableField != null)
            {
              lexicallyDeclaredName.VariableField.OuterField = bindingIdentifier.VariableField;
              if (bindingIdentifier.VariableField != null && !lexicallyDeclaredName.VariableField.CanCrunch)
                bindingIdentifier.VariableField.CanCrunch = false;
            }
          }
        }
      }
      foreach (INameDeclaration varDeclaredName in (IEnumerable<INameDeclaration>) node.CatchBlock.EnclosingScope.VarDeclaredNames)
      {
        foreach (BindingIdentifier bindingIdentifier in (IEnumerable<BindingIdentifier>) bindingIdentifierList)
        {
          if (string.CompareOrdinal(varDeclaredName.Name, bindingIdentifier.Name) == 0)
            varDeclaredName.Context.HandleError(JSError.DuplicateLexicalDeclaration);
        }
      }
    }

    public override void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Operand != null && node.Operand.IsDebugOnly)
      {
        node.IsDebugOnly = true;
        switch (node.OperatorToken)
        {
          case JSToken.FirstOperator:
            node.Operand = (AstNode) new ConstantWrapper((object) true, PrimitiveType.Boolean, node.Context);
            break;
          case JSToken.Increment:
          case JSToken.Decrement:
            AstNode parent1 = node.Parent;
            UnaryOperator oldNode1 = node;
            ConstantWrapper constantWrapper1 = new ConstantWrapper((object) 0, PrimitiveType.Number, node.Context);
            constantWrapper1.IsDebugOnly = true;
            ConstantWrapper newNode1 = constantWrapper1;
            parent1.ReplaceChild((AstNode) oldNode1, (AstNode) newNode1);
            break;
          case JSToken.Void:
            node.Operand = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Operand.Context);
            break;
          case JSToken.TypeOf:
            AstNode parent2 = node.Parent;
            UnaryOperator oldNode2 = node;
            ConstantWrapper constantWrapper2 = new ConstantWrapper((object) "object", PrimitiveType.String, node.Context);
            constantWrapper2.IsDebugOnly = true;
            ConstantWrapper newNode2 = constantWrapper2;
            parent2.ReplaceChild((AstNode) oldNode2, (AstNode) newNode2);
            break;
          case JSToken.LogicalNot:
            AstNode parent3 = node.Parent;
            UnaryOperator oldNode3 = node;
            ConstantWrapper constantWrapper3 = new ConstantWrapper((object) true, PrimitiveType.Boolean, node.Context);
            constantWrapper3.IsDebugOnly = true;
            ConstantWrapper newNode3 = constantWrapper3;
            parent3.ReplaceChild((AstNode) oldNode3, (AstNode) newNode3);
            break;
          case JSToken.BitwiseNot:
            AstNode parent4 = node.Parent;
            UnaryOperator oldNode4 = node;
            ConstantWrapper constantWrapper4 = new ConstantWrapper((object) -1, PrimitiveType.Number, node.Context);
            constantWrapper4.IsDebugOnly = true;
            ConstantWrapper newNode4 = constantWrapper4;
            parent4.ReplaceChild((AstNode) oldNode4, (AstNode) newNode4);
            break;
          case JSToken.FirstBinaryOperator:
            AstNode parent5 = node.Parent;
            UnaryOperator oldNode5 = node;
            ConstantWrapper constantWrapper5 = new ConstantWrapper((object) 0, PrimitiveType.Number, node.Context);
            constantWrapper5.IsDebugOnly = true;
            ConstantWrapper newNode5 = constantWrapper5;
            parent5.ReplaceChild((AstNode) oldNode5, (AstNode) newNode5);
            break;
          case JSToken.Minus:
            AstNode parent6 = node.Parent;
            UnaryOperator oldNode6 = node;
            ConstantWrapper constantWrapper6 = new ConstantWrapper((object) 0, PrimitiveType.Number, node.Context);
            constantWrapper6.IsDebugOnly = true;
            ConstantWrapper newNode6 = constantWrapper6;
            parent6.ReplaceChild((AstNode) oldNode6, (AstNode) newNode6);
            break;
          default:
            node.Operand = AnalyzeNodeVisitor.ClearDebugExpression(node.Operand);
            break;
        }
      }
      else if (node.OperatorToken == JSToken.FirstOperator)
      {
        if (!this.m_scopeStack.Peek().UseStrict || !(node.Operand is Lookup))
          return;
        node.Context.HandleError(JSError.StrictModeInvalidDelete, true);
      }
      else if (node.OperatorToken == JSToken.Increment || node.OperatorToken == JSToken.Decrement)
      {
        if (!(node.Operand is Lookup operand))
          return;
        if (operand.VariableField != null && operand.VariableField.InitializationOnly)
          operand.Context.HandleError(JSError.AssignmentToConstant, true);
        if (!this.m_scopeStack.Peek().UseStrict || operand.VariableField != null && operand.VariableField.FieldType != FieldType.UndefinedGlobal && operand.VariableField.FieldType != FieldType.Arguments && (operand.VariableField.FieldType != FieldType.Predefined || string.CompareOrdinal(operand.Name, "eval") != 0))
          return;
        node.Operand.Context.HandleError(JSError.StrictModeInvalidPreOrPost, true);
      }
      else if (node.OperatorToken == JSToken.TypeOf)
      {
        if (!this.m_parser.Settings.RemoveUnneededCode || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveWindowDotFromTypeOf) || !(node.Operand is Member operand) || !(operand.Root is Lookup root) || root.VariableField == null || root.VariableField.FieldType != FieldType.Predefined || !(root.Name == "window"))
          return;
        string name = operand.Name;
        ActivationObject enclosingScope = operand.EnclosingScope;
        JSVariableField jsVariableField = enclosingScope.CanReference(name);
        if (jsVariableField != null && jsVariableField.FieldType != FieldType.Predefined && jsVariableField.FieldType != FieldType.Global && jsVariableField.FieldType != FieldType.UndefinedGlobal)
          return;
        DetachReferences.Apply((AstNode) root);
        root.Name = name;
        root.VariableField = enclosingScope.FindReference(name);
        node.Operand = (AstNode) root;
        root.VariableField.AddReference((INameReference) root);
      }
      else
      {
        if (!(node.Operand is ConstantWrapper operand) || !operand.IsNumericLiteral)
          return;
        double number = operand.ToNumber();
        if (node.OperatorToken == JSToken.Minus && this.m_parser.Settings.IsModificationAllowed(TreeModifications.ApplyUnaryMinusToNumericLiteral))
        {
          operand.Value = (object) -number;
          if (!node.Parent.ReplaceChild((AstNode) node, (AstNode) operand))
            return;
          operand.Context = node.Context.Clone();
        }
        else
        {
          if (node.OperatorToken != JSToken.FirstBinaryOperator || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnaryPlusOnNumericLiteral) || !node.Parent.ReplaceChild((AstNode) node, (AstNode) operand))
            return;
          operand.Context = node.Context.Clone();
        }
      }
    }

    public override void Visit(Var node)
    {
      if (node == null)
        return;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveDuplicateVar))
      {
        int index = 0;
        while (index < node.Count)
        {
          if (node[index].Binding is BindingIdentifier binding)
          {
            string name = binding.Name;
            if (node[index].Initializer != null)
              AnalyzeNodeVisitor.DeleteNoInits(node, ++index, name);
            else if (AnalyzeNodeVisitor.VarDeclExists(node, index + 1, name))
            {
              binding.VariableField.Declarations.Remove((INameDeclaration) binding);
              node.RemoveAt(index);
            }
            else
              ++index;
          }
          else
            ++index;
        }
      }
      base.Visit(node);
    }

    public override void Visit(VariableDeclaration node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Initializer != null && node.Initializer.IsDebugOnly)
        node.Initializer = AnalyzeNodeVisitor.ClearDebugExpression(node.Initializer);
      if (node.Initializer == null && !(node.Binding is BindingIdentifier) && node.Parent.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (p => !(p.Parent is ForIn))))
        node.Binding.Context.HandleError(JSError.BindingPatternRequiresInitializer, true);
      if (!node.IsCCSpecialCase || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.RemoveUnnecessaryCCOnStatements))
        return;
      node.UseCCOn = !this.m_encounteredCCOn;
      this.m_encounteredCCOn = true;
    }

    public override void Visit(WhileNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.Body != null && node.Body.Count == 0)
        node.Body = (Block) null;
      if (node.Condition == null || !node.Condition.IsDebugOnly)
        return;
      if (node.Body == null)
        node.IsDebugOnly = true;
      else
        node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
    }

    public override void Visit(WithNode node)
    {
      if (node == null)
        return;
      if (this.m_scopeStack.Peek().UseStrict)
        node.Context.HandleError(JSError.StrictModeNoWith, true);
      else
        node.Context.HandleError(JSError.WithNotRecommended);
      ActivationObject activationObject = node.Body.IfNotNull<Block, ActivationObject>((Func<Block, ActivationObject>) (b => b.EnclosingScope));
      base.Visit(node);
      if (node.Body != null && node.Body.Count == 0)
        node.Body = (Block) null;
      if (node.Body == null && activationObject != null)
        activationObject.IsKnownAtCompileTime = true;
      if (node.WithObject == null || !node.WithObject.IsDebugOnly)
        return;
      if (node.Body == null)
      {
        node.IsDebugOnly = true;
      }
      else
      {
        WithNode withNode = node;
        ObjectLiteral objectLiteral1 = new ObjectLiteral(node.WithObject.Context);
        objectLiteral1.IsDebugOnly = true;
        ObjectLiteral objectLiteral2 = objectLiteral1;
        withNode.WithObject = (AstNode) objectLiteral2;
      }
    }

    private static AstNode ClearDebugExpression(AstNode node)
    {
      switch (node)
      {
        case null:
        case ObjectLiteral _:
        case ConstantWrapper _:
          return node;
        default:
          ObjectLiteral objectLiteral = new ObjectLiteral(node.Context);
          objectLiteral.IsDebugOnly = true;
          return (AstNode) objectLiteral;
      }
    }

    private static string GuessAtName(AstNode node)
    {
      string str = string.Empty;
      AstNode parent = node.Parent;
      if (parent != null)
      {
        if (parent is AstNodeList)
          parent = parent.Parent;
        if (parent is CallNode callNode && callNode.IsConstructor)
          parent = parent.Parent;
        str = parent.GetFunctionGuess(node);
      }
      return str;
    }

    private static bool AreAssignmentsInVar(BinaryOperator binaryOp, Var varStatement)
    {
      bool flag = false;
      if (binaryOp != null)
      {
        if (binaryOp.OperatorToken == JSToken.Assign)
        {
          if (binaryOp.Operand1 is Lookup operand1)
            flag = varStatement.Contains(operand1.Name);
        }
        else if (binaryOp.OperatorToken == JSToken.Comma)
          flag = AnalyzeNodeVisitor.AreAssignmentsInVar(binaryOp.Operand1 as BinaryOperator, varStatement) && AnalyzeNodeVisitor.AreAssignmentsInVar(binaryOp.Operand2 as BinaryOperator, varStatement);
      }
      return flag;
    }

    private static void ConvertAssignmentsToVarDecls(
      BinaryOperator binaryOp,
      Declaration declaration,
      JSParser parser)
    {
      if (binaryOp == null)
        return;
      if (binaryOp.OperatorToken == JSToken.Assign)
      {
        if (!(binaryOp.Operand1 is Lookup operand1))
          return;
        BindingIdentifier bindingIdentifier1 = new BindingIdentifier(operand1.Context);
        bindingIdentifier1.Name = operand1.Name;
        bindingIdentifier1.TerminatingContext = operand1.TerminatingContext;
        bindingIdentifier1.VariableField = operand1.VariableField;
        BindingIdentifier bindingIdentifier2 = bindingIdentifier1;
        VariableDeclaration variableDeclaration = new VariableDeclaration(binaryOp.Context.Clone());
        variableDeclaration.Binding = (AstNode) bindingIdentifier2;
        variableDeclaration.AssignContext = binaryOp.OperatorContext;
        variableDeclaration.Initializer = binaryOp.Operand2;
        VariableDeclaration element = variableDeclaration;
        operand1.VariableField.Declarations.Add((INameDeclaration) bindingIdentifier2);
        declaration.Append((AstNode) element);
      }
      else
      {
        if (binaryOp.OperatorToken != JSToken.Comma)
          return;
        AnalyzeNodeVisitor.ConvertAssignmentsToVarDecls(binaryOp.Operand1 as BinaryOperator, declaration, parser);
        AnalyzeNodeVisitor.ConvertAssignmentsToVarDecls(binaryOp.Operand2 as BinaryOperator, declaration, parser);
      }
    }

    private static bool VarDeclExists(Var node, int ndx, string name)
    {
      for (; ndx < node.Count; ++ndx)
      {
        foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings((AstNode) node[ndx]))
        {
          if (string.CompareOrdinal(name, binding.Name) == 0)
            return true;
        }
      }
      return false;
    }

    private static void DeleteNoInits(Var node, int min, string name)
    {
      for (int index = node.Count - 1; index >= min; --index)
      {
        VariableDeclaration variableDeclaration = node[index];
        if (variableDeclaration.Binding is BindingIdentifier binding && string.CompareOrdinal(name, binding.Name) == 0 && variableDeclaration.Initializer == null)
        {
          node.RemoveAt(index);
          binding.VariableField.Declarations.Remove((INameDeclaration) binding);
        }
      }
    }

    private static UnaryOperator CreateVoidNode(Context context) => new UnaryOperator(context.FlattenToStart())
    {
      Operand = (AstNode) new ConstantWrapper((object) 0.0, PrimitiveType.Number, context),
      OperatorToken = JSToken.Void
    };

    private static void ValidateIdentifier(
      bool isStrict,
      string identifier,
      Context context,
      JSError error)
    {
      if (JSScanner.IsKeyword(identifier, isStrict))
      {
        context.HandleError(JSError.KeywordUsedAsIdentifier, true);
      }
      else
      {
        if (!isStrict || string.CompareOrdinal(identifier, "eval") != 0 && string.CompareOrdinal(identifier, "arguments") != 0)
          return;
        context.HandleError(error, true);
      }
    }

    private static bool IsInsideLoop(AstNode node, bool orSwitch)
    {
      bool flag = false;
      while (true)
      {
        switch (node)
        {
          case null:
          case FunctionObject _:
            goto label_5;
          case WhileNode _:
          case DoWhile _:
          case ForIn _:
          case ForNode _:
            goto label_2;
          default:
            if (!orSwitch || !(node is SwitchCase))
            {
              node = node.Parent;
              continue;
            }
            goto label_2;
        }
      }
label_2:
      return true;
label_5:
      return flag;
    }
  }
}
