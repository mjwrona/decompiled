// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.FunctionObject
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class FunctionObject : AstNode
  {
    private BindingIdentifier m_binding;
    private AstNodeList m_parameters;
    private Block m_body;

    public bool IsStatic { get; set; }

    public Context StaticContext { get; set; }

    public BindingIdentifier Binding
    {
      get => this.m_binding;
      set
      {
        this.m_binding.IfNotNull<BindingIdentifier, AstNode>((Func<BindingIdentifier, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_binding = value;
        this.m_binding.IfNotNull<BindingIdentifier, AstNode>((Func<BindingIdentifier, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string NameGuess { get; set; }

    public AstNodeList ParameterDeclarations
    {
      get => this.m_parameters;
      set
      {
        this.m_parameters.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_parameters = value;
        this.m_parameters.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Block Body
    {
      get => this.m_body;
      set
      {
        this.m_body.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_body = value;
        this.m_body.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public override bool IsDeclaration => this.FunctionType == FunctionType.Declaration;

    public FunctionType FunctionType { get; set; }

    public override bool IsExpression => this.FunctionType != FunctionType.Declaration && this.FunctionType != FunctionType.Method;

    public bool IsGenerator { get; set; }

    public bool IsSourceElement { get; set; }

    public override OperatorPrecedence Precedence => this.FunctionType != FunctionType.ArrowFunction ? OperatorPrecedence.Primary : OperatorPrecedence.Assignment;

    public FunctionObject(Context functionContext)
      : base(functionContext)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public bool IsReferenced => this.SafeIsReferenced(new HashSet<FunctionObject>());

    private bool SafeIsReferenced(HashSet<FunctionObject> visited)
    {
      if (!visited.Contains(this))
      {
        visited.Add(this);
        if (this.FunctionType != FunctionType.Declaration || this.Binding.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v.FieldType == FieldType.Global || v.IsExported)))
          return true;
        foreach (INameReference reference in (IEnumerable<INameReference>) this.Binding.VariableField.References)
        {
          ActivationObject variableScope = reference.VariableScope;
          if (variableScope == null || variableScope is GlobalScope || variableScope.Owner is FunctionObject owner && owner.SafeIsReferenced(visited))
            return true;
        }
      }
      return false;
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.Binding, (AstNode) this.ParameterDeclarations, (AstNode) this.Body);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Binding == oldNode)
      {
        this.Binding = newNode as BindingIdentifier;
        return true;
      }
      if (this.Body == oldNode)
      {
        this.Body = AstNode.ForceToBlock(newNode);
        return true;
      }
      return this.ParameterDeclarations == oldNode && (newNode as AstNodeList).IfNotNull<AstNodeList, bool>((Func<AstNodeList, bool>) (list =>
      {
        this.ParameterDeclarations = list;
        return true;
      }));
    }
  }
}
