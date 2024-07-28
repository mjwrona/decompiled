// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ResolutionVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public class ResolutionVisitor : IVisitor
  {
    private long m_orderIndex;
    private bool m_isUnreachable;
    private int m_withDepth;
    private Stack<ActivationObject> m_lexicalStack;
    private Stack<ActivationObject> m_variableStack;
    private CodeSettings m_settings;
    private ScriptVersion m_scriptVersion;

    private ActivationObject CurrentLexicalScope => this.m_lexicalStack.Peek();

    private ActivationObject CurrentVariableScope => this.m_variableStack.Peek();

    private long NextOrderIndex => !this.m_isUnreachable ? ++this.m_orderIndex : 0L;

    private ResolutionVisitor(ActivationObject rootScope, JSParser parser)
    {
      this.m_lexicalStack = new Stack<ActivationObject>();
      this.m_lexicalStack.Push(rootScope);
      this.m_variableStack = new Stack<ActivationObject>();
      this.m_variableStack.Push(rootScope);
      this.m_settings = parser.Settings;
      this.m_scriptVersion = parser.ParsedVersion;
    }

    public static void Apply(Block block, ActivationObject scope, JSParser parser)
    {
      if (block == null || scope == null || parser == null)
        return;
      ResolutionVisitor resolutionVisitor = new ResolutionVisitor(scope, parser);
      block.Accept((IVisitor) resolutionVisitor);
      ResolutionVisitor.CreateFields(scope);
      ResolutionVisitor.ResolveLookups(scope, parser.Settings);
      ResolutionVisitor.AddGhostedFields(scope);
    }

    private static void CollapseBlockScope(ActivationObject blockScope)
    {
      blockScope.ScopeLookups.CopyItemsTo<Lookup>(blockScope.Parent.ScopeLookups);
      blockScope.VarDeclaredNames.CopyItemsTo<INameDeclaration>(blockScope.Parent.VarDeclaredNames);
      blockScope.ChildScopes.CopyItemsTo<ActivationObject>((ICollection<ActivationObject>) blockScope.Parent.ChildScopes);
      blockScope.GhostedCatchParameters.CopyItemsTo<BindingIdentifier>(blockScope.Parent.GhostedCatchParameters);
      blockScope.GhostedFunctions.CopyItemsTo<FunctionObject>(blockScope.Parent.GhostedFunctions);
      blockScope.Parent.ChildScopes.Remove(blockScope);
    }

    private static void CreateFields(ActivationObject scope)
    {
      scope.DeclareScope();
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) scope.ChildScopes)
        ResolutionVisitor.CreateFields(childScope);
    }

    private static void ResolveLookups(ActivationObject scope, CodeSettings settings)
    {
      foreach (Lookup scopeLookup in (IEnumerable<Lookup>) scope.ScopeLookups)
        ResolutionVisitor.ResolveLookup(scope, scopeLookup, settings);
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) scope.ChildScopes)
        ResolutionVisitor.ResolveLookups(childScope, settings);
      foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) scope.NameTable.Values)
      {
        if (jsVariableField.RefCount == 0)
          jsVariableField.HasNoReferences = true;
      }
    }

    private static void MakeExpectedGlobal(JSVariableField varField)
    {
      do
      {
        varField.FieldType = FieldType.Global;
        varField = varField.OuterField;
      }
      while (varField != null);
    }

    private static void ResolveLookup(ActivationObject scope, Lookup lookup, CodeSettings settings)
    {
      lookup.VariableField = scope.FindReference(lookup.Name);
      if (lookup.VariableField.FieldType == FieldType.UndefinedGlobal)
        ResolutionVisitor.ResolveUndefinedGlobal(lookup);
      else if (lookup.VariableField.FieldType == FieldType.Predefined)
        ResolutionVisitor.ResolvePredefinedGlobal(lookup, scope, settings);
      lookup.VariableField.AddReference((INameReference) lookup);
      lookup.VariableField.IsPlaceholder = false;
      if (!(lookup.Parent is ImportExportSpecifier) && !(lookup.Parent is ExportNode))
        return;
      lookup.VariableField.IsExported = true;
    }

    private static void ResolvePredefinedGlobal(
      Lookup lookup,
      ActivationObject scope,
      CodeSettings settings)
    {
      if (lookup.Name.Length == 6 && string.CompareOrdinal(lookup.Name, "window") == 0)
      {
        if (lookup.Parent is Member parent1)
        {
          scope.AddGlobal(parent1.Name);
        }
        else
        {
          if (!(lookup.Parent is CallNode parent) || !parent.InBrackets || parent.Arguments.Count != 1 || !(parent.Arguments[0] is ConstantWrapper) || parent.Arguments[0].FindPrimitiveType() != PrimitiveType.String)
            return;
          string name = parent.Arguments[0].ToString();
          if (!JSScanner.IsValidIdentifier(name))
            return;
          scope.AddGlobal(name);
        }
      }
      else
      {
        if (settings.EvalTreatment == EvalTreatment.Ignore || lookup.Name.Length != 4 || string.CompareOrdinal(lookup.Name, "eval") != 0 || !(lookup.Parent is CallNode parent2) || parent2.Function != lookup)
          return;
        scope.IsKnownAtCompileTime = false;
      }
    }

    private static void ResolveUndefinedGlobal(Lookup lookup)
    {
      if (lookup.IsGenerated)
        return;
      if (lookup.Parent is UnaryOperator parent1 && parent1.OperatorToken == JSToken.TypeOf)
        ResolutionVisitor.MakeExpectedGlobal(lookup.VariableField);
      else if (lookup.Parent is TemplateLiteral)
      {
        string[] strArray = new string[1]{ "safehtml" };
        bool flag = false;
        foreach (string strB in strArray)
        {
          if (lookup.Name.Length == strB.Length && string.CompareOrdinal(lookup.Name, strB) == 0)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          return;
        lookup.Context.ReportUndefined(lookup);
        lookup.Context.HandleError(JSError.UndeclaredFunction);
      }
      else
      {
        lookup.Context.ReportUndefined(lookup);
        bool flag = lookup.Parent is CallNode parent && parent.Function == lookup;
        lookup.Context.HandleError(flag ? JSError.UndeclaredFunction : JSError.UndeclaredVariable);
      }
    }

    private static void AddGhostedFields(ActivationObject scope)
    {
      foreach (BindingIdentifier ghostedCatchParameter in (IEnumerable<BindingIdentifier>) scope.GhostedCatchParameters)
        ResolutionVisitor.ResolveGhostedCatchParameter(scope, ghostedCatchParameter);
      foreach (FunctionObject ghostedFunction in (IEnumerable<FunctionObject>) scope.GhostedFunctions)
        ResolutionVisitor.ResolveGhostedFunctions(scope, ghostedFunction);
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) scope.ChildScopes)
        ResolutionVisitor.AddGhostedFields(childScope);
    }

    private static void ResolveGhostedCatchParameter(
      ActivationObject scope,
      BindingIdentifier catchBinding)
    {
      if (catchBinding == null)
        return;
      JSVariableField variableField = scope[catchBinding.Name];
      if (variableField == null)
      {
        variableField = new JSVariableField(FieldType.GhostCatch, catchBinding.Name, FieldAttributes.PrivateScope, (object) null)
        {
          OriginalContext = catchBinding.Context
        };
        scope.AddField(variableField);
      }
      else if (variableField.FieldType != FieldType.GhostCatch)
      {
        variableField.IsAmbiguous = true;
        if (variableField.OuterField != null)
          catchBinding.Context.HandleError(JSError.AmbiguousCatchVar);
      }
      catchBinding.VariableField.OuterField = variableField;
      variableField.GhostedField = catchBinding.VariableField;
      if (catchBinding.VariableField.RefCount <= 0)
        return;
      variableField.AddReferences((IEnumerable<INameReference>) catchBinding.VariableField.References);
    }

    private static void ResolveGhostedFunctions(ActivationObject scope, FunctionObject funcObject)
    {
      BindingIdentifier binding = funcObject.Binding;
      JSVariableField variableField1 = binding.VariableField;
      JSVariableField variableField2 = scope[binding.Name];
      if (variableField2 == null)
      {
        variableField2 = new JSVariableField(FieldType.GhostFunction, binding.Name, FieldAttributes.PrivateScope, (object) funcObject)
        {
          OriginalContext = variableField1.OriginalContext,
          CanCrunch = binding.VariableField != null && binding.VariableField.CanCrunch
        };
        scope.AddField(variableField2);
      }
      else if (variableField2.FieldType == FieldType.GhostFunction)
      {
        variableField2.IsAmbiguous = true;
      }
      else
      {
        variableField2.IsFunction = true;
        if (variableField2.OuterField != null)
        {
          variableField2.IsAmbiguous = true;
          binding.Context.HandleError(JSError.AmbiguousNamedFunctionExpression);
        }
        else if (variableField2.IsReferenced && ResolutionVisitor.IsBindingIdentifierWithName(funcObject.Parent as VariableDeclaration, binding.Name) && (!(funcObject.Parent is BinaryOperator parent) || parent.OperatorToken != JSToken.Assign || parent.Operand2 != funcObject || !(parent.Operand1 is Lookup operand1) || string.CompareOrdinal(operand1.Name, binding.Name) != 0))
          variableField2.IsAmbiguous = true;
      }
      variableField1.OuterField = variableField2;
      variableField2.GhostedField = variableField1;
      if (variableField1.RefCount <= 0)
        return;
      variableField2.AddReferences((IEnumerable<INameReference>) variableField1.References);
    }

    private static bool IsBindingIdentifierWithName(VariableDeclaration varDecl, string name)
    {
      BindingIdentifier binding = varDecl == null ? (BindingIdentifier) null : varDecl.Binding as BindingIdentifier;
      return binding != null && binding.Name.Length == name.Length && string.CompareOrdinal(binding.Name, name) == 0;
    }

    private static void AddDeclaredNames(AstNode node, ICollection<INameDeclaration> collection)
    {
      if (node is INameDeclaration nameDeclaration)
      {
        collection.Add(nameDeclaration);
      }
      else
      {
        foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(node))
          collection.Add((INameDeclaration) binding);
      }
    }

    private static ModuleScope GetModuleScope(AstNode node)
    {
      ActivationObject enclosingScope = node.EnclosingScope;
      switch (enclosingScope)
      {
        case ModuleScope moduleScope2:
        case null:
label_4:
          return moduleScope2;
        default:
          node.Context.HandleError(JSError.ExportNotAtModuleLevel);
          ActivationObject parent = enclosingScope.Parent;
          while (true)
          {
            switch (parent)
            {
              case null:
              case ModuleScope moduleScope2:
                goto label_4;
              default:
                parent = parent.Parent;
                continue;
            }
          }
      }
    }

    public void Visit(ArrayLiteral node)
    {
      if (node == null)
        return;
      if (node.Elements != null)
        node.Elements.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(AspNetBlockNode node)
    {
    }

    public void Visit(AstNodeList node)
    {
      if (node == null)
        return;
      for (int index = 0; index < node.Count; ++index)
        node[index]?.Accept((IVisitor) this);
    }

    public void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      if (node.Operand1 != null)
        node.Operand1.Accept((IVisitor) this);
      if (node.Operand2 != null)
        node.Operand2.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(BindingIdentifier node)
    {
      if (node == null)
        return;
      ResolutionVisitor.AddDeclaredNames((AstNode) node, this.CurrentLexicalScope.LexicallyDeclaredNames);
    }

    public void Visit(Block node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (!node.HasOwnScope && node.Parent != null && !(node.Parent is SwitchCase) && !(node.Parent is FunctionObject) && !(node.Parent is ModuleDeclaration) && !(node.Parent is ClassNode) && !(node.Parent is ConditionalCompilationComment))
      {
        Block block = node;
        BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Lexical);
        blockScope1.Owner = (AstNode) node;
        blockScope1.IsInWithScope = this.m_withDepth > 0;
        BlockScope blockScope2 = blockScope1;
        block.EnclosingScope = (ActivationObject) blockScope2;
      }
      ActivationObject enclosingScope = node.HasOwnScope ? node.EnclosingScope : (ActivationObject) null;
      if (enclosingScope != null)
        this.m_lexicalStack.Push(enclosingScope);
      try
      {
        for (int index = 0; index < node.Count; ++index)
          node[index]?.Accept((IVisitor) this);
      }
      finally
      {
        this.m_isUnreachable = false;
        if (enclosingScope != null)
          this.m_lexicalStack.Pop();
      }
      if (enclosingScope == null || node.EnclosingScope.ScopeType != ScopeType.Lexical || node.EnclosingScope.LexicallyDeclaredNames.Count != 0)
        return;
      ResolutionVisitor.CollapseBlockScope(enclosingScope);
      node.EnclosingScope = (ActivationObject) null;
    }

    public void Visit(Break node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      this.m_isUnreachable = true;
    }

    public void Visit(CallNode node)
    {
      if (node == null)
        return;
      if (node.Function != null)
        node.Function.Accept((IVisitor) this);
      if (node.Arguments != null)
        node.Arguments.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ClassNode node)
    {
      if (node == null)
        return;
      if (node.Heritage != null)
        node.Heritage.Accept((IVisitor) this);
      BindingIdentifier binding = node.Binding as BindingIdentifier;
      ClassNode classNode = node;
      BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Class);
      blockScope1.Owner = (AstNode) node;
      blockScope1.IsInWithScope = this.m_withDepth > 0;
      blockScope1.UseStrict = true;
      blockScope1.ScopeName = binding == null ? (string) null : binding.Name.IfNullOrWhiteSpace((string) null);
      BlockScope blockScope2 = blockScope1;
      classNode.Scope = blockScope2;
      if (node.Binding != null)
      {
        if (node.ClassType == ClassType.Declaration)
          ResolutionVisitor.AddDeclaredNames(node.Binding, this.CurrentLexicalScope.LexicallyDeclaredNames);
        else
          ResolutionVisitor.AddDeclaredNames(node.Binding, node.Scope.LexicallyDeclaredNames);
      }
      if (node.Elements == null)
        return;
      this.m_lexicalStack.Push((ActivationObject) node.Scope);
      try
      {
        node.Elements.Accept((IVisitor) this);
      }
      finally
      {
        this.m_lexicalStack.Pop();
      }
    }

    public void Visit(ComprehensionNode node)
    {
      if (node == null)
        return;
      ComprehensionNode comprehensionNode = node;
      BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Lexical);
      blockScope1.Owner = (AstNode) node;
      blockScope1.IsInWithScope = this.m_withDepth > 0;
      BlockScope blockScope2 = blockScope1;
      comprehensionNode.BlockScope = blockScope2;
      this.m_lexicalStack.Push((ActivationObject) node.BlockScope);
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
        this.m_lexicalStack.Pop();
      }
    }

    public void Visit(ComprehensionForClause node)
    {
      if (node == null)
        return;
      ResolutionVisitor.AddDeclaredNames(node.Binding, this.CurrentLexicalScope.LexicallyDeclaredNames);
      if (node.Expression == null)
        return;
      node.Expression.Accept((IVisitor) this);
    }

    public void Visit(ComprehensionIfClause node)
    {
      if (node == null || node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public void Visit(ConditionalCompilationComment node)
    {
      if (node == null || node.Statements == null)
        return;
      node.Statements.Accept((IVisitor) this);
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
      if (node == null)
        return;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      long orderIndex1 = this.m_orderIndex;
      if (node.TrueExpression != null)
        node.TrueExpression.Accept((IVisitor) this);
      long orderIndex2 = this.m_orderIndex;
      this.m_orderIndex = orderIndex1;
      if (node.FalseExpression != null)
        node.FalseExpression.Accept((IVisitor) this);
      this.m_orderIndex = Math.Max(orderIndex2, this.m_orderIndex);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ConstantWrapperPP node)
    {
    }

    public void Visit(ConstStatement node)
    {
      if (node == null)
        return;
      node.Index = -1L;
      for (int index = 0; index < node.Count; ++index)
        node[index]?.Accept((IVisitor) this);
    }

    public void Visit(ContinueNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      this.m_isUnreachable = true;
    }

    public void Visit(CustomNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
    }

    public void Visit(DebuggerNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
    }

    public void Visit(DirectivePrologue node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (!node.UseStrict)
        return;
      this.CurrentVariableScope.UseStrict = true;
    }

    public void Visit(DoWhile node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Body != null)
        node.Body.Accept((IVisitor) this);
      if (node.Condition == null)
        return;
      node.Condition.Accept((IVisitor) this);
    }

    public void Visit(EmptyStatement node)
    {
    }

    public void Visit(ExportNode node)
    {
      if (node == null)
        return;
      ModuleScope moduleScope = ResolutionVisitor.GetModuleScope((AstNode) node);
      if (node.IsDefault)
      {
        if (moduleScope != null)
        {
          if (moduleScope.HasDefaultExport)
            (node.DefaultContext ?? node.Context).HandleError(JSError.MultipleDefaultExports, true);
          else
            moduleScope.HasDefaultExport = true;
        }
        foreach (AstNode child in node.Children)
          child.Accept((IVisitor) this);
      }
      else if (!node.ModuleName.IsNullOrWhiteSpace())
      {
        if (node.Count != 0 || moduleScope == null)
          return;
        moduleScope.IsNotComplete = true;
      }
      else
      {
        foreach (AstNode child in node.Children)
          child.Accept((IVisitor) this);
      }
    }

    public void Visit(ForIn node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Collection != null)
        node.Collection.Accept((IVisitor) this);
      if (node.Variable != null)
      {
        if (node.Variable is LexicalDeclaration)
        {
          ForIn forIn = node;
          BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Lexical);
          blockScope1.Owner = (AstNode) node;
          blockScope1.IsInWithScope = this.m_withDepth > 0;
          BlockScope blockScope2 = blockScope1;
          forIn.BlockScope = blockScope2;
          this.m_lexicalStack.Push((ActivationObject) node.BlockScope);
        }
      }
      try
      {
        if (node.Variable != null)
          node.Variable.Accept((IVisitor) this);
        if (node.Body == null)
          return;
        node.Body.Accept((IVisitor) this);
      }
      finally
      {
        if (node.BlockScope != null)
          this.m_lexicalStack.Pop();
      }
    }

    public void Visit(ForNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Initializer != null)
      {
        if (node.Initializer is LexicalDeclaration)
        {
          ForNode forNode = node;
          BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Lexical);
          blockScope1.Owner = (AstNode) node;
          blockScope1.IsInWithScope = this.m_withDepth > 0;
          BlockScope blockScope2 = blockScope1;
          forNode.BlockScope = blockScope2;
          this.m_lexicalStack.Push((ActivationObject) node.BlockScope);
        }
      }
      try
      {
        if (node.Initializer != null)
          node.Initializer.Accept((IVisitor) this);
        if (node.Condition != null)
          node.Condition.Accept((IVisitor) this);
        if (node.Body != null)
          node.Body.Accept((IVisitor) this);
        if (node.Incrementer == null)
          return;
        node.Incrementer.Accept((IVisitor) this);
      }
      finally
      {
        if (node.BlockScope != null)
          this.m_lexicalStack.Pop();
      }
    }

    public void Visit(FunctionObject node)
    {
      if (node == null)
        return;
      node.Index = -1L;
      ActivationObject parent = this.CurrentLexicalScope;
      if (node.FunctionType == FunctionType.Expression && node.Binding != null && !node.Binding.Name.IsNullOrWhiteSpace())
      {
        FunctionScope functionScope = new FunctionScope(parent, true, this.m_settings, node);
        functionScope.IsInWithScope = this.m_withDepth > 0;
        functionScope.ScopeName = node.Binding.Name;
        parent = (ActivationObject) functionScope;
        this.CurrentVariableScope.GhostedFunctions.Add(node);
      }
      bool flag = node.Parent != null && node.Parent.Parent is ClassNode;
      FunctionObject functionObject = node;
      FunctionScope functionScope1 = new FunctionScope(parent, node.FunctionType != FunctionType.Declaration, this.m_settings, node);
      functionScope1.IsInWithScope = this.m_withDepth > 0;
      functionScope1.HasSuperBinding = flag;
      functionScope1.ScopeName = node.Binding == null ? (string) null : node.Binding.Name.IfNullOrWhiteSpace((string) null);
      FunctionScope functionScope2 = functionScope1;
      functionObject.EnclosingScope = (ActivationObject) functionScope2;
      this.m_lexicalStack.Push(node.EnclosingScope);
      this.m_variableStack.Push(node.EnclosingScope);
      long orderIndex = this.m_orderIndex;
      try
      {
        if (node.Body != null)
        {
          this.m_orderIndex = 0L;
          node.Body.Accept((IVisitor) this);
        }
        if (node.ParameterDeclarations != null)
          node.ParameterDeclarations.Accept((IVisitor) this);
      }
      finally
      {
        this.m_lexicalStack.Pop();
        this.m_variableStack.Pop();
        this.m_orderIndex = orderIndex;
      }
      if (node.FunctionType != FunctionType.Declaration || node.Binding == null || node.Binding.Name.IsNullOrWhiteSpace())
        return;
      ActivationObject currentLexicalScope = this.CurrentLexicalScope;
      currentLexicalScope.LexicallyDeclaredNames.Add((INameDeclaration) node.Binding);
      if (currentLexicalScope == this.CurrentVariableScope || this.m_scriptVersion == ScriptVersion.EcmaScript6 || this.m_settings.ScriptVersion == ScriptVersion.EcmaScript6)
        return;
      node.Context.HandleError(JSError.MisplacedFunctionDeclaration);
      this.CurrentVariableScope.GhostedFunctions.Add(node);
    }

    public void Visit(GetterSetter node)
    {
    }

    public void Visit(GroupingOperator node)
    {
      if (node == null)
        return;
      if (node.Operand != null)
        node.Operand.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(IfNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      long orderIndex1 = this.m_orderIndex;
      if (node.TrueBlock != null)
        node.TrueBlock.Accept((IVisitor) this);
      long orderIndex2 = this.m_orderIndex;
      this.m_orderIndex = orderIndex1;
      if (node.FalseBlock != null)
        node.FalseBlock.Accept((IVisitor) this);
      this.m_orderIndex = Math.Max(orderIndex2, this.m_orderIndex);
    }

    public void Visit(ImportantComment node)
    {
    }

    public void Visit(ImportExportSpecifier node)
    {
      if (node == null || node.LocalIdentifier == null)
        return;
      node.LocalIdentifier.Accept((IVisitor) this);
    }

    public void Visit(ImportNode node)
    {
      if (node == null)
        return;
      if (node.ModuleName.IsNullOrWhiteSpace())
        node.Context.HandleError(JSError.ImportNoModuleName, true);
      foreach (AstNode child in node.Children)
        child.Accept((IVisitor) this);
    }

    public void Visit(InitializerNode node)
    {
      if (node == null)
        return;
      if (node.Binding != null)
        node.Binding.Accept((IVisitor) this);
      if (node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public void Visit(LabeledStatement node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Statement == null)
        return;
      node.Statement.Accept((IVisitor) this);
    }

    public void Visit(LexicalDeclaration node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      for (int index = 0; index < node.Count; ++index)
        node[index]?.Accept((IVisitor) this);
    }

    public void Visit(Lookup node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      this.CurrentLexicalScope.ScopeLookups.Add(node);
    }

    public void Visit(Member node)
    {
      if (node == null)
        return;
      if (node.Root != null)
        node.Root.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ModuleDeclaration node)
    {
      if (node == null)
        return;
      if (node.Binding == null)
      {
        if (node.Body == null)
          return;
        ModuleScope moduleScope1 = new ModuleScope(node, this.CurrentLexicalScope, this.m_settings);
        moduleScope1.IsInWithScope = this.m_withDepth > 0;
        moduleScope1.ScopeName = node.ModuleName.IfNullOrWhiteSpace((string) null);
        ModuleScope moduleScope2 = moduleScope1;
        node.EnclosingScope = (ActivationObject) moduleScope2;
        this.m_variableStack.Push(node.EnclosingScope);
        this.m_lexicalStack.Push(node.EnclosingScope);
        try
        {
          node.Body.Accept((IVisitor) this);
        }
        finally
        {
          this.m_variableStack.Pop();
          this.m_lexicalStack.Pop();
        }
      }
      else
        node.Binding.Accept((IVisitor) this);
    }

    public void Visit(ObjectLiteral node)
    {
      if (node == null)
        return;
      node.Properties.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ObjectLiteralField node)
    {
    }

    public void Visit(ObjectLiteralProperty node)
    {
      if (node == null)
        return;
      if (node.Value != null)
        node.Value.Accept((IVisitor) this);
      if (node.Name == null && !(node.Value is Lookup))
        node.Context.HandleError(JSError.ImplicitPropertyNameMustBeIdentifier, true);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ParameterDeclaration node)
    {
      if (node == null || node.Initializer == null)
        return;
      node.Initializer.Accept((IVisitor) this);
    }

    public void Visit(RegExpLiteral node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ReturnNode node)
    {
      if (node == null)
        return;
      if (node.Operand != null)
        node.Operand.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
      this.m_isUnreachable = true;
    }

    public void Visit(Switch node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Expression != null)
        node.Expression.Accept((IVisitor) this);
      Switch @switch = node;
      BlockScope blockScope1 = new BlockScope(this.CurrentLexicalScope, this.m_settings, ScopeType.Block);
      blockScope1.Owner = (AstNode) node;
      blockScope1.IsInWithScope = this.m_withDepth > 0;
      BlockScope blockScope2 = blockScope1;
      @switch.BlockScope = (ActivationObject) blockScope2;
      this.m_lexicalStack.Push(node.BlockScope);
      try
      {
        if (node.Cases != null)
          node.Cases.Accept((IVisitor) this);
      }
      finally
      {
        this.m_lexicalStack.Pop();
      }
      if (node.BlockScope.LexicallyDeclaredNames.Count != 0)
        return;
      ResolutionVisitor.CollapseBlockScope(node.BlockScope);
      node.BlockScope = (ActivationObject) null;
    }

    public void Visit(SwitchCase node)
    {
      if (node == null)
        return;
      if (node.CaseValue != null)
        node.CaseValue.Accept((IVisitor) this);
      if (node.Statements == null)
        return;
      node.Statements.Accept((IVisitor) this);
    }

    public void Visit(TemplateLiteral node)
    {
      if (node == null)
        return;
      if (node.Function != null)
        node.Function.Accept((IVisitor) this);
      if (node.Expressions == null)
        return;
      node.Expressions.Accept((IVisitor) this);
    }

    public void Visit(TemplateLiteralExpression node)
    {
      if (node == null || node.Expression == null)
        return;
      node.Expression.Accept((IVisitor) this);
    }

    public void Visit(ThisLiteral node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
    }

    public void Visit(ThrowNode node)
    {
      if (node == null)
        return;
      if (node.Operand != null)
        node.Operand.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
      this.m_isUnreachable = true;
    }

    public void Visit(TryNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.TryBlock != null)
        node.TryBlock.Accept((IVisitor) this);
      if (node.CatchParameter != null && node.CatchParameter.Binding is BindingIdentifier binding)
        this.CurrentVariableScope.GhostedCatchParameters.Add(binding);
      if (node.CatchBlock != null)
      {
        Block catchBlock = node.CatchBlock;
        CatchScope catchScope1 = new CatchScope(this.CurrentLexicalScope, this.m_settings);
        catchScope1.Owner = (AstNode) node.CatchBlock;
        catchScope1.CatchParameter = node.CatchParameter;
        catchScope1.IsInWithScope = this.m_withDepth > 0;
        CatchScope catchScope2 = catchScope1;
        catchBlock.EnclosingScope = (ActivationObject) catchScope2;
        ResolutionVisitor.AddDeclaredNames((AstNode) node.CatchParameter, node.CatchBlock.EnclosingScope.LexicallyDeclaredNames);
        node.CatchBlock.Accept((IVisitor) this);
      }
      if (node.FinallyBlock == null)
        return;
      node.FinallyBlock.Accept((IVisitor) this);
    }

    public void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      if (node.Operand != null)
        node.Operand.Accept((IVisitor) this);
      node.Index = this.NextOrderIndex;
    }

    public void Visit(Var node)
    {
      if (node == null)
        return;
      node.Index = -1L;
      for (int index = 0; index < node.Count; ++index)
        node[index]?.Accept((IVisitor) this);
    }

    public void Visit(VariableDeclaration node)
    {
      if (node == null)
        return;
      if (node.Parent is LexicalDeclaration)
      {
        ResolutionVisitor.AddDeclaredNames(node.Binding, this.CurrentLexicalScope.LexicallyDeclaredNames);
      }
      else
      {
        ResolutionVisitor.AddDeclaredNames(node.Binding, this.CurrentLexicalScope.VarDeclaredNames);
        ResolutionVisitor.AddDeclaredNames(node.Binding, this.CurrentVariableScope.VarDeclaredNames);
      }
      if (node.Initializer != null)
      {
        node.Initializer.Accept((IVisitor) this);
        node.Index = this.NextOrderIndex;
      }
      else
        node.Index = -1L;
    }

    public void Visit(WhileNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.Condition != null)
        node.Condition.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      node.Body.Accept((IVisitor) this);
    }

    public void Visit(WithNode node)
    {
      if (node == null)
        return;
      node.Index = this.NextOrderIndex;
      if (node.WithObject != null)
        node.WithObject.Accept((IVisitor) this);
      if (node.Body == null)
        return;
      Block body = node.Body;
      WithScope withScope1 = new WithScope(this.CurrentLexicalScope, this.m_settings);
      withScope1.Owner = (AstNode) node;
      WithScope withScope2 = withScope1;
      body.EnclosingScope = (ActivationObject) withScope2;
      try
      {
        ++this.m_withDepth;
        node.Body.Accept((IVisitor) this);
      }
      finally
      {
        --this.m_withDepth;
      }
    }
  }
}
