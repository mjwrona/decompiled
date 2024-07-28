// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.FunctionScope
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public sealed class FunctionScope : ActivationObject
  {
    private HashSet<ActivationObject> m_refScopes;

    internal FunctionScope(
      ActivationObject parent,
      bool isExpression,
      CodeSettings settings,
      FunctionObject funcObj)
      : base(parent, settings)
    {
      this.ScopeType = ScopeType.Function;
      this.m_refScopes = new HashSet<ActivationObject>();
      if (isExpression)
        this.AddReference(this.Parent);
      this.Owner = (AstNode) funcObj;
    }

    public override void DeclareScope()
    {
      if (this.Owner.EnclosingScope == this)
      {
        this.DefineParameters();
        this.DefineLexicalDeclarations();
        this.DefineArgumentsObject();
        this.DefineVarDeclarations();
      }
      else
        this.DefineFunctionExpressionName();
    }

    private void DefineFunctionExpressionName()
    {
      FunctionObject owner = (FunctionObject) this.Owner;
      JSVariableField field = this.CreateField(owner.Binding.Name, (object) owner, FieldAttributes.PrivateScope);
      field.IsFunction = true;
      field.OriginalContext = owner.Binding.Context;
      owner.Binding.VariableField = field;
      this.AddField(field);
    }

    private void DefineParameters()
    {
      FunctionObject owner = (FunctionObject) this.Owner;
      if (owner.ParameterDeclarations == null)
        return;
      foreach (ParameterDeclaration parameterDeclaration in owner.ParameterDeclarations)
      {
        foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(parameterDeclaration.Binding))
        {
          JSVariableField variableField = this[binding.Name];
          if (variableField == null)
          {
            variableField = new JSVariableField(FieldType.Argument, binding.Name, FieldAttributes.PrivateScope, (object) null)
            {
              Position = parameterDeclaration.Position,
              OriginalContext = parameterDeclaration.Context,
              CanCrunch = !binding.RenameNotAllowed
            };
            this.AddField(variableField);
          }
          binding.VariableField = variableField;
          variableField.Declarations.Add((INameDeclaration) binding);
        }
      }
    }

    private void DefineArgumentsObject()
    {
      if (this["arguments"] != null)
        return;
      this.AddField(new JSVariableField(FieldType.Arguments, "arguments", FieldAttributes.PrivateScope, (object) null));
    }

    public override JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      return new JSVariableField(FieldType.Local, name, attributes, value);
    }

    internal void AddReference(ActivationObject scope)
    {
      while (scope != null && scope is BlockScope)
        scope = scope.Parent;
      if (scope == null)
        return;
      this.m_refScopes.Add(scope);
    }
  }
}
