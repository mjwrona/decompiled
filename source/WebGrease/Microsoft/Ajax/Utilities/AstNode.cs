// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.AstNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public abstract class AstNode
  {
    private static readonly IEnumerable<AstNode> s_emptyChildrenCollection = (IEnumerable<AstNode>) new AstNode[0];
    private ActivationObject m_enclosingScope;

    public AstNode Parent { get; set; }

    public Context Context { get; set; }

    public virtual Context TerminatingContext { get; set; }

    public virtual bool IsExpression => false;

    public virtual bool IsConstant => false;

    public bool IsDebugOnly { get; set; }

    public long Index { get; set; }

    public virtual OperatorPrecedence Precedence => OperatorPrecedence.None;

    public virtual bool IsDeclaration => false;

    public bool IsWindowLookup
    {
      get
      {
        if (!(this is Lookup lookup) || string.CompareOrdinal(lookup.Name, "window") != 0)
          return false;
        return lookup.VariableField == null || lookup.VariableField.FieldType == FieldType.Predefined;
      }
    }

    public virtual AstNode LeftHandSide => this;

    public virtual ActivationObject EnclosingScope
    {
      get
      {
        ActivationObject enclosingScope = this.m_enclosingScope;
        if (enclosingScope != null)
          return enclosingScope;
        return this.Parent != null ? this.Parent.EnclosingScope : (ActivationObject) null;
      }
      set => this.m_enclosingScope = value;
    }

    public bool HasOwnScope => this.m_enclosingScope != null;

    public virtual IEnumerable<AstNode> Children => AstNode.s_emptyChildrenCollection;

    public virtual bool ContainsInOperator
    {
      get
      {
        foreach (AstNode child in this.Children)
        {
          if (child.ContainsInOperator)
            return true;
        }
        return false;
      }
    }

    protected AstNode(Context context) => this.Context = context != null ? context : throw new ArgumentNullException(nameof (context));

    internal virtual string GetFunctionGuess(AstNode target) => string.Empty;

    internal virtual bool EncloseBlock(EncloseBlockType type) => false;

    public virtual PrimitiveType FindPrimitiveType() => PrimitiveType.Other;

    public virtual bool ReplaceChild(AstNode oldNode, AstNode newNode) => false;

    public virtual bool IsEquivalentTo(AstNode otherNode) => false;

    public abstract void Accept(IVisitor visitor);

    public void UpdateWith(Context context)
    {
      if (context == null)
        return;
      if (this.Context == null)
        this.Context = context;
      else
        this.Context.UpdateWith(context);
    }

    public static Block ForceToBlock(AstNode node)
    {
      switch (node)
      {
        case Block block:
        case null:
          return block;
        default:
          block = new Block(node.Context.Clone());
          block.Append(node);
          goto case null;
      }
    }

    internal static IEnumerable<AstNode> EnumerateNonNullNodes<T>(IList<T> nodes) where T : AstNode
    {
      for (int ndx = 0; ndx < ((ICollection<T>) nodes).Count; ++ndx)
      {
        if ((object) nodes[ndx] != null)
          yield return (AstNode) nodes[ndx];
      }
    }

    internal static IEnumerable<AstNode> EnumerateNonNullNodes(
      AstNode n1,
      AstNode n2 = null,
      AstNode n3 = null,
      AstNode n4 = null)
    {
      return AstNode.EnumerateNonNullNodes<AstNode>((IList<AstNode>) new AstNode[4]
      {
        n1,
        n2,
        n3,
        n4
      });
    }
  }
}
