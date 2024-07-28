// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ActivationObject
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public abstract class ActivationObject
  {
    private bool m_useStrict;
    private bool m_isKnownAtCompileTime;

    internal bool Existing { get; set; }

    public AstNode Owner { get; set; }

    public bool HasSuperBinding { get; set; }

    public bool UseStrict
    {
      get => this.m_useStrict;
      set
      {
        if (!value)
          return;
        this.m_useStrict = value;
        foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
          childScope.UseStrict = value;
      }
    }

    public bool IsKnownAtCompileTime
    {
      get => this.m_isKnownAtCompileTime;
      set
      {
        this.m_isKnownAtCompileTime = value;
        if (value || this.Settings.EvalTreatment != EvalTreatment.MakeAllSafe)
          return;
        if (!(this.Owner is FunctionObject owner))
        {
          this.Parent.IfNotNull<ActivationObject, bool>((Func<ActivationObject, bool>) (p => p.IsKnownAtCompileTime = false));
        }
        else
        {
          if (!owner.IsReferenced)
            return;
          this.Parent.IsKnownAtCompileTime = false;
        }
      }
    }

    public ActivationObject Parent { get; private set; }

    public bool IsInWithScope { get; set; }

    public IDictionary<string, JSVariableField> NameTable { get; private set; }

    public IList<ActivationObject> ChildScopes { get; private set; }

    public ICollection<Lookup> ScopeLookups { get; private set; }

    public ICollection<INameDeclaration> VarDeclaredNames { get; private set; }

    public ICollection<INameDeclaration> LexicallyDeclaredNames { get; private set; }

    public ICollection<BindingIdentifier> GhostedCatchParameters { get; private set; }

    public ICollection<FunctionObject> GhostedFunctions { get; private set; }

    public string ScopeName { get; set; }

    public ScopeType ScopeType { get; protected set; }

    protected CodeSettings Settings { get; private set; }

    protected ActivationObject(ActivationObject parent, CodeSettings codeSettings)
    {
      this.m_isKnownAtCompileTime = true;
      this.m_useStrict = false;
      this.Settings = codeSettings;
      this.Parent = parent;
      this.NameTable = (IDictionary<string, JSVariableField>) new Dictionary<string, JSVariableField>();
      this.ChildScopes = (IList<ActivationObject>) new List<ActivationObject>();
      if (parent != null)
      {
        parent.ChildScopes.Add(this);
        this.UseStrict = parent.UseStrict;
      }
      this.ScopeLookups = (ICollection<Lookup>) new HashSet<Lookup>();
      this.VarDeclaredNames = (ICollection<INameDeclaration>) new HashSet<INameDeclaration>();
      this.LexicallyDeclaredNames = (ICollection<INameDeclaration>) new HashSet<INameDeclaration>();
      this.GhostedCatchParameters = (ICollection<BindingIdentifier>) new HashSet<BindingIdentifier>();
      this.GhostedFunctions = (ICollection<FunctionObject>) new HashSet<FunctionObject>();
    }

    public static bool DeleteFromBindingPattern(AstNode binding, bool normalizePattern)
    {
      bool flag = false;
      if (binding != null)
      {
        if (binding.Parent is AstNodeList parent5 && parent5.Parent is ArrayLiteral)
          flag = parent5.ReplaceChild(binding, (AstNode) new ConstantWrapper((object) Missing.Value, PrimitiveType.Other, binding.Context.Clone()));
        else if (binding.Parent is ObjectLiteralProperty parent4)
        {
          parent5 = parent4.Parent as AstNodeList;
          flag = parent4.Parent.ReplaceChild((AstNode) parent4, (AstNode) null);
        }
        else if (binding.Parent is VariableDeclaration parent1 && parent1.Parent is Declaration parent2 && (!(parent2.Parent is ForIn parent3) || parent3.Variable != parent2) && (parent1.Initializer == null || parent1.Initializer.IsConstant))
        {
          flag = parent1.Parent.ReplaceChild((AstNode) parent1, (AstNode) null);
          if (parent2.Count == 0)
            parent2.Parent.ReplaceChild((AstNode) parent2, (AstNode) null);
        }
        if (flag)
        {
          if (binding is BindingIdentifier bindingIdentifier)
          {
            bindingIdentifier.VariableField.Declarations.Remove((INameDeclaration) bindingIdentifier);
            if (!bindingIdentifier.VariableField.IsReferenced && bindingIdentifier.VariableField.Declarations.Count == 0)
              bindingIdentifier.VariableField.WasRemoved = true;
          }
          if (normalizePattern && parent5 != null)
          {
            if (parent5.Parent is ArrayLiteral)
            {
              for (int index = parent5.Count - 1; index >= 0 && parent5[index] is ConstantWrapper constantWrapper && constantWrapper.Value == Missing.Value; --index)
                parent5.RemoveAt(index);
            }
            if (parent5.Count == 0)
              ActivationObject.DeleteFromBindingPattern(parent5.Parent, normalizePattern);
          }
        }
      }
      return flag;
    }

    public static void RemoveBinding(AstNode binding)
    {
      foreach (BindingIdentifier binding1 in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(binding))
      {
        BindingIdentifier boundName = binding1;
        boundName.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.Declarations.Remove((INameDeclaration) boundName)));
      }
      ActivationObject.DeleteFromBindingPattern(binding, true);
    }

    public abstract void DeclareScope();

    protected void DefineLexicalDeclarations()
    {
      foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) this.LexicallyDeclaredNames)
      {
        AstNode fieldValue = (AstNode) (lexicallyDeclaredName.Parent as FunctionObject) ?? (AstNode) (lexicallyDeclaredName.Parent as ClassNode);
        this.DefineField(lexicallyDeclaredName, fieldValue);
      }
    }

    protected void DefineVarDeclarations()
    {
      foreach (INameDeclaration varDeclaredName in (IEnumerable<INameDeclaration>) this.VarDeclaredNames)
        this.DefineField(varDeclaredName, (AstNode) null);
    }

    private void DefineField(INameDeclaration nameDecl, AstNode fieldValue)
    {
      JSVariableField variableField = this[nameDecl.Name];
      if (nameDecl.IsParameter)
      {
        if (variableField == null)
        {
          variableField = new JSVariableField(FieldType.CatchError, nameDecl.Name, FieldAttributes.PrivateScope, (object) null)
          {
            OriginalContext = nameDecl.Context,
            IsDeclared = true
          };
          this.AddField(variableField);
        }
        else
          variableField.OriginalContext.HandleError(JSError.DuplicateCatch, true);
      }
      else
      {
        if (variableField == null)
        {
          variableField = this.CreateField(nameDecl.Name, (object) null, FieldAttributes.PrivateScope);
          variableField.OriginalContext = nameDecl.Context;
          variableField.IsDeclared = true;
          variableField.IsFunction = nameDecl is FunctionObject;
          variableField.FieldValue = (object) fieldValue;
          AstNode astNode = nameDecl.Parent.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (p => p.Parent));
          JSVariableField jsVariableField = variableField;
          int num;
          switch (astNode)
          {
            case ConstStatement _:
              num = 1;
              break;
            case LexicalDeclaration lexicalDeclaration:
              num = lexicalDeclaration.StatementToken == JSToken.Const ? 1 : 0;
              break;
            default:
              num = 0;
              break;
          }
          jsVariableField.InitializationOnly = num != 0;
          this.AddField(variableField);
        }
        else
        {
          if (nameDecl.Parent.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (p => p.Parent)) is LexicalDeclaration)
            nameDecl.Context.HandleError(JSError.DuplicateLexicalDeclaration, true);
          if (nameDecl.Initializer != null && nameDecl is INameReference reference)
            variableField.AddReference(reference);
          if (fieldValue != null)
            variableField.FieldValue = (object) fieldValue;
        }
        AstNode astNode1 = (AstNode) nameDecl;
        while ((astNode1 = astNode1.Parent) != null)
        {
          switch (astNode1)
          {
            case Block _:
              goto label_21;
            case ExportNode _:
              variableField.IsExported = true;
              goto label_21;
            case ImportNode _:
              variableField.InitializationOnly = true;
              goto label_21;
            default:
              continue;
          }
        }
      }
label_21:
      nameDecl.VariableField = variableField;
      variableField.Declarations.Add(nameDecl);
      if (!this.IsInWithScope && !nameDecl.RenameNotAllowed)
        return;
      variableField.CanCrunch = false;
    }

    internal virtual void AnalyzeScope()
    {
      this.AnalyzeNonGlobalScope();
      this.ManualRenameFields();
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
        childScope.AnalyzeScope();
    }

    private void AnalyzeNonGlobalScope()
    {
      foreach (JSVariableField variableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
      {
        if (variableField.OuterField == null)
        {
          if (!variableField.IsReferenced && !variableField.IsGenerated && variableField.FieldType != FieldType.CatchError && variableField.FieldType != FieldType.GhostCatch && !variableField.IsExported && variableField.OriginalContext != null)
            this.UnreferencedVariableField(variableField);
          else if (variableField.FieldType == FieldType.Local && variableField.RefCount == 1 && this.IsKnownAtCompileTime && this.Settings.RemoveUnneededCode && this.Settings.IsModificationAllowed(TreeModifications.RemoveUnusedVariables))
            ActivationObject.SingleReferenceVariableField(variableField);
        }
      }
    }

    private void UnreferencedVariableField(JSVariableField variableField)
    {
      if (variableField.FieldValue is FunctionObject fieldValue)
      {
        this.UnreferencedFunction(variableField, fieldValue);
      }
      else
      {
        if (variableField.FieldType == FieldType.Argument || variableField.WasRemoved)
          return;
        this.UnreferencedVariable(variableField);
      }
    }

    private void UnreferencedFunction(JSVariableField variableField, FunctionObject functionObject)
    {
      if (functionObject.Binding == null || variableField.FieldType == FieldType.GhostFunction)
        return;
      if (JSScanner.IsValidIdentifier(functionObject.Binding.Name))
      {
        (functionObject.Binding.Context ?? variableField.OriginalContext).HandleError(JSError.FunctionNotReferenced);
        if (!this.IsKnownAtCompileTime || !this.Settings.MinifyCode || !this.Settings.RemoveUnneededCode || this is BlockScope)
          return;
        functionObject.Parent.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (p => p.ReplaceChild((AstNode) functionObject, (AstNode) null)));
      }
      else
        variableField.CanCrunch = false;
    }

    private void UnreferencedVariable(JSVariableField variableField)
    {
      bool flag = true;
      if (variableField.Declarations.Count == 1 && this.IsKnownAtCompileTime)
      {
        INameDeclaration onlyDeclaration = variableField.OnlyDeclaration;
        VariableDeclaration variableDeclaration = onlyDeclaration.IfNotNull<INameDeclaration, VariableDeclaration>((Func<INameDeclaration, VariableDeclaration>) (decl => decl.Parent as VariableDeclaration));
        if (variableDeclaration != null)
        {
          if (variableDeclaration.Parent is Declaration parent2 && (variableDeclaration.Initializer == null || variableDeclaration.Initializer.IsConstant))
          {
            if (parent2.Parent is ForIn parent1 && parent2 == parent1.Variable)
              flag = false;
            else if (this.Settings.RemoveUnneededCode && this.Settings.IsModificationAllowed(TreeModifications.RemoveUnusedVariables))
            {
              variableField.Declarations.Remove(onlyDeclaration);
              if (variableField.GhostedField == null)
                variableField.WasRemoved = true;
              parent2.Remove(variableDeclaration);
              if (parent2.Count == 0)
                parent2.Parent.ReplaceChild((AstNode) parent2, (AstNode) null);
            }
          }
          else if (variableDeclaration.Parent is ForIn)
            flag = false;
        }
        else if (onlyDeclaration is BindingIdentifier binding)
          ActivationObject.DeleteFromBindingPattern((AstNode) binding, true);
      }
      if (!flag || !variableField.HasNoReferences)
        return;
      variableField.OriginalContext.HandleError(JSError.VariableDefinedNotReferenced);
    }

    private static void SingleReferenceVariableField(JSVariableField variableField)
    {
      if (variableField.Declarations.Count != 1)
        return;
      INameDeclaration onlyDeclaration = variableField.OnlyDeclaration;
      VariableDeclaration varDecl = onlyDeclaration.IfNotNull<INameDeclaration, VariableDeclaration>((Func<INameDeclaration, VariableDeclaration>) (d => d.Parent as VariableDeclaration));
      if (varDecl == null || varDecl.Initializer == null || !varDecl.Initializer.IsConstant)
        return;
      INameReference onlyReference = variableField.OnlyReference;
      if (onlyReference == null || onlyReference.IsAssignment || onlyReference.VariableField == null || onlyReference.VariableField.OuterField != null || !onlyReference.VariableField.CanCrunch || onlyReference.VariableField.IsExported || varDecl.Index >= onlyReference.Index || ActivationObject.IsIterativeReference(varDecl.Initializer, onlyReference))
        return;
      Declaration declaration = varDecl.Parent as Declaration;
      if (declaration == null)
        return;
      variableField.References.Remove(onlyReference);
      AstNode refNode = onlyReference as AstNode;
      refNode.Parent.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (p => p.ReplaceChild(refNode, varDecl.Initializer)));
      variableField.Declarations.Remove(onlyDeclaration);
      variableField.WasRemoved = true;
      declaration.Remove(varDecl);
      if (declaration.Count != 0)
        return;
      declaration.Parent.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (p => p.ReplaceChild((AstNode) declaration, (AstNode) null)));
    }

    private static bool IsIterativeReference(AstNode initializer, INameReference reference)
    {
      RegExpLiteral regExpLiteral = initializer as RegExpLiteral;
      if (initializer is ArrayLiteral || initializer is ObjectLiteral || regExpLiteral != null && regExpLiteral.PatternSwitches != null && regExpLiteral.PatternSwitches.IndexOf("g", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        Block parentBlock = ActivationObject.GetParentBlock(initializer);
        AstNode astNode = reference as AstNode;
        for (AstNode parent = astNode.Parent; parent != null && parent != parentBlock && !(parent is FunctionObject); parent = parent.Parent)
        {
          if (parent is WhileNode || parent is DoWhile || parent is ForNode forNode && astNode != forNode.Initializer || parent is ForIn forIn && astNode == forIn.Body)
            return true;
          astNode = parent;
        }
      }
      return false;
    }

    private static Block GetParentBlock(AstNode node)
    {
      for (; node != null; node = node.Parent)
      {
        if (node is Block parentBlock)
          return parentBlock;
      }
      return (Block) null;
    }

    protected void ManualRenameFields()
    {
      if (!this.Settings.IsModificationAllowed(TreeModifications.LocalRenaming))
        return;
      if (this.Settings.HasRenamePairs)
      {
        foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
        {
          if (jsVariableField.OuterField == null && jsVariableField.FieldType != FieldType.Arguments && jsVariableField.FieldType != FieldType.Predefined)
          {
            string newName = this.Settings.GetNewName(jsVariableField.Name);
            if (!string.IsNullOrEmpty(newName))
            {
              jsVariableField.CanCrunch = true;
              jsVariableField.CrunchedName = newName;
              jsVariableField.CanCrunch = false;
            }
          }
        }
      }
      if (this.Settings.LocalRenaming == LocalRenaming.KeepAll)
        return;
      foreach (string noAutoRename in this.Settings.NoAutoRenameCollection)
      {
        JSVariableField jsVariableField;
        if (this.NameTable.TryGetValue(noAutoRename, out jsVariableField) && jsVariableField.OuterField == null && jsVariableField.CanCrunch)
          jsVariableField.CanCrunch = false;
      }
    }

    internal void ValidateGeneratedNames()
    {
      foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
      {
        if (jsVariableField.IsGenerated && jsVariableField.CrunchedName == null)
        {
          HashSet<string> stringSet = new HashSet<string>();
          this.GenerateAvoidList(stringSet, jsVariableField.Name);
          CrunchEnumerator crunchEnumerator = new CrunchEnumerator((IEnumerable<string>) stringSet);
          jsVariableField.CrunchedName = crunchEnumerator.NextName();
        }
      }
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
      {
        if (!childScope.Existing)
          childScope.ValidateGeneratedNames();
      }
    }

    private bool GenerateAvoidList(HashSet<string> table, string name)
    {
      bool avoidList = false;
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
      {
        if (childScope.GenerateAvoidList(table, name))
          avoidList = true;
      }
      if (!avoidList)
        avoidList = this.NameTable.ContainsKey(name);
      if (avoidList)
      {
        foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
          table.Add(jsVariableField.ToString());
      }
      return avoidList;
    }

    internal virtual void AutoRenameFields()
    {
      if (this.m_isKnownAtCompileTime)
      {
        IEnumerable<JSVariableField> uncrunchedLocals = this.GetUncrunchedLocals();
        if (uncrunchedLocals != null)
        {
          HashSet<string> avoidNames = new HashSet<string>();
          foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
          {
            if (!jsVariableField.CanCrunch || jsVariableField.CrunchedName != null || jsVariableField.OuterField != null && !jsVariableField.IsGenerated && jsVariableField.OwningScope != null && !jsVariableField.OwningScope.IsKnownAtCompileTime)
              avoidNames.Add(jsVariableField.ToString());
          }
          CrunchEnumerator crunchEnumerator = new CrunchEnumerator((IEnumerable<string>) avoidNames);
          foreach (JSVariableField jsVariableField in uncrunchedLocals)
          {
            if (jsVariableField.CanCrunch && (jsVariableField.RefCount > 0 || jsVariableField.IsDeclared || jsVariableField.IsPlaceholder || !this.Settings.RemoveFunctionExpressionNames || !this.Settings.IsModificationAllowed(TreeModifications.RemoveFunctionExpressionNames)))
              jsVariableField.CrunchedName = crunchEnumerator.NextName();
          }
        }
      }
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
        childScope.AutoRenameFields();
    }

    internal IEnumerable<JSVariableField> GetUncrunchedLocals()
    {
      List<JSVariableField> uncrunchedLocals = new List<JSVariableField>(this.NameTable.Count);
      foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
      {
        if (jsVariableField != null && jsVariableField.OuterField == null && jsVariableField.CrunchedName == null && jsVariableField.CanCrunch && !jsVariableField.WasRemoved && (this.Settings.LocalRenaming == LocalRenaming.CrunchAll || !jsVariableField.Name.StartsWith("L_", StringComparison.Ordinal)) && (!this.Settings.PreserveFunctionNames || !jsVariableField.IsFunction))
          uncrunchedLocals.Add(jsVariableField);
      }
      if (uncrunchedLocals.Count == 0)
        return (IEnumerable<JSVariableField>) null;
      uncrunchedLocals.Sort(ReferenceComparer.Instance);
      return (IEnumerable<JSVariableField>) uncrunchedLocals;
    }

    public virtual JSVariableField this[string name]
    {
      get
      {
        JSVariableField jsVariableField;
        if (!this.NameTable.TryGetValue(name, out jsVariableField))
          jsVariableField = (JSVariableField) null;
        return jsVariableField;
      }
    }

    public JSVariableField CanReference(string name)
    {
      JSVariableField jsVariableField = this[name];
      if (jsVariableField == null)
      {
        for (ActivationObject parent = this.Parent; parent != null && jsVariableField == null; parent = parent.Parent)
          jsVariableField = parent[name];
      }
      return jsVariableField;
    }

    public JSVariableField FindReference(string name)
    {
      JSVariableField reference = this[name];
      if (reference == null && name != null)
      {
        if (string.CompareOrdinal(name, "super") == 0 && this.HasSuperBinding)
        {
          reference = new JSVariableField(FieldType.Super, name, FieldAttributes.PrivateScope, (object) null);
          this.NameTable.Add(name, reference);
        }
        else if (this.Parent != null)
        {
          reference = this.CreateInnerField(this.Parent.FindReference(name));
          reference.IsPlaceholder = true;
        }
        else
          reference = this.AddField(new JSVariableField(FieldType.UndefinedGlobal, name, FieldAttributes.PrivateScope, (object) null));
      }
      return reference;
    }

    public virtual JSVariableField DeclareField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      JSVariableField field;
      if (!this.NameTable.TryGetValue(name, out field))
      {
        field = this.CreateField(name, value, attributes);
        this.AddField(field);
      }
      return field;
    }

    public virtual JSVariableField CreateField(JSVariableField outerField) => outerField.IfNotNull<JSVariableField, JSVariableField>((Func<JSVariableField, JSVariableField>) (o => new JSVariableField(o.FieldType, o)));

    public abstract JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes);

    public virtual JSVariableField CreateInnerField(JSVariableField outerField)
    {
      JSVariableField variableField = (JSVariableField) null;
      if (outerField != null)
      {
        variableField = this.CreateField(outerField);
        this.AddField(variableField);
      }
      return variableField;
    }

    internal JSVariableField AddField(JSVariableField variableField)
    {
      this.NameTable[variableField.Name] = variableField;
      variableField.OwningScope = variableField.OuterField == null ? this : variableField.OuterField.OwningScope;
      return variableField;
    }

    public INameDeclaration VarDeclaredName(string name)
    {
      foreach (INameDeclaration varDeclaredName in (IEnumerable<INameDeclaration>) this.VarDeclaredNames)
      {
        if (string.CompareOrdinal(varDeclaredName.Name, name) == 0)
          return varDeclaredName;
      }
      return (INameDeclaration) null;
    }

    public INameDeclaration LexicallyDeclaredName(string name)
    {
      foreach (INameDeclaration lexicallyDeclaredName in (IEnumerable<INameDeclaration>) this.LexicallyDeclaredNames)
      {
        if (string.CompareOrdinal(lexicallyDeclaredName.Name, name) == 0)
          return lexicallyDeclaredName;
      }
      return (INameDeclaration) null;
    }

    public void AddGlobal(string name)
    {
      ActivationObject activationObject = this;
      while (activationObject.Parent != null)
        activationObject = activationObject.Parent;
      if (activationObject[name] != null)
        return;
      activationObject.AddField(activationObject.CreateField(name, (object) null, FieldAttributes.PrivateScope));
    }
  }
}
