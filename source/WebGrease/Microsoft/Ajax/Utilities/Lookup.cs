// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Lookup
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public sealed class Lookup : Expression, INameReference, IRenameable
  {
    public JSVariableField VariableField { get; set; }

    public bool IsGenerated { get; set; }

    public ReferenceType RefType { get; set; }

    public string Name { get; set; }

    public bool IsAssignment
    {
      get
      {
        bool isAssignment;
        if (this.Parent is BinaryOperator parent)
        {
          isAssignment = parent.IsAssign && parent.Operand1 == this;
        }
        else
        {
          isAssignment = this.Parent is UnaryOperator parent1 && (parent1.OperatorToken == JSToken.Increment || parent1.OperatorToken == JSToken.Decrement);
          if (!isAssignment)
            isAssignment = this.Parent is ForIn parent2 && this == parent2.Variable;
        }
        return isAssignment;
      }
    }

    public AstNode AssignmentValue
    {
      get
      {
        AstNode assignmentValue = (AstNode) null;
        if (this.Parent is BinaryOperator parent)
          assignmentValue = parent.OperatorToken != JSToken.Assign || parent.Operand1 != this ? (AstNode) null : parent.Operand2;
        return assignmentValue;
      }
    }

    public string OriginalName => this.Name;

    public bool WasRenamed => this.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (f => !f.CrunchedName.IsNullOrWhiteSpace()));

    public Lookup(Context context)
      : base(context)
    {
      this.RefType = ReferenceType.Variable;
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool IsEquivalentTo(AstNode otherNode)
    {
      if (!(otherNode is Lookup lookup))
        return false;
      return this.VariableField != null ? this.VariableField.IsSameField(lookup.VariableField) : string.CompareOrdinal(this.Name, lookup.Name) == 0;
    }

    internal override string GetFunctionGuess(AstNode target) => this.Name;

    public override string ToString() => this.Name;

    public ActivationObject VariableScope
    {
      get
      {
        ActivationObject variableScope = this.EnclosingScope;
        while (variableScope is BlockScope)
          variableScope = variableScope.Parent;
        return variableScope;
      }
    }
  }
}
