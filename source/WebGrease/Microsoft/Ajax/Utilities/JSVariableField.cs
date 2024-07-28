// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSVariableField
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public class JSVariableField
  {
    private ActivationObject m_owningScope;
    private HashSet<INameReference> m_referenceTable;
    private HashSet<INameDeclaration> m_declarationTable;
    private bool m_canCrunch;
    private bool m_isDeclared;
    private bool m_isGenerated;
    private string m_crunchedName;

    public Context OriginalContext { get; set; }

    public string Name { get; private set; }

    public FieldType FieldType { get; set; }

    public FieldAttributes Attributes { get; set; }

    public object FieldValue { get; set; }

    public bool IsFunction { get; internal set; }

    public bool IsAmbiguous { get; set; }

    public bool IsPlaceholder { get; set; }

    public bool HasNoReferences { get; set; }

    public bool InitializationOnly { get; set; }

    public int Position { get; set; }

    public bool WasRemoved { get; set; }

    public bool IsExported { get; set; }

    public JSVariableField OuterField { get; set; }

    public ActivationObject OwningScope
    {
      get => this.OuterField != null ? this.OuterField.OwningScope : this.m_owningScope;
      set => this.m_owningScope = value;
    }

    public JSVariableField GhostedField { get; set; }

    public int RefCount => this.m_referenceTable.Count;

    public ICollection<INameReference> References => (ICollection<INameReference>) this.m_referenceTable;

    public INameReference OnlyReference
    {
      get
      {
        INameReference[] array = new INameReference[1];
        if (this.m_referenceTable.Count == 1)
          this.m_referenceTable.CopyTo(array, 0);
        return array[0];
      }
    }

    public ICollection<INameDeclaration> Declarations => (ICollection<INameDeclaration>) this.m_declarationTable;

    public INameDeclaration OnlyDeclaration
    {
      get
      {
        INameDeclaration[] array = new INameDeclaration[1];
        if (this.m_declarationTable.Count == 1)
          this.m_declarationTable.CopyTo(array, 0);
        return array[0];
      }
    }

    public bool IsLiteral => (this.Attributes & FieldAttributes.Literal) != FieldAttributes.PrivateScope;

    public bool CanCrunch
    {
      get => this.m_canCrunch;
      set
      {
        this.m_canCrunch = value;
        if (this.OuterField == null || value)
          return;
        this.OuterField.CanCrunch = false;
      }
    }

    public bool IsDeclared
    {
      get => this.m_isDeclared;
      set
      {
        this.m_isDeclared = value;
        if (this.OuterField == null)
          return;
        this.OuterField.IsDeclared = value;
      }
    }

    public bool IsGenerated
    {
      get => this.OuterField == null ? this.m_isGenerated : this.OuterField.IsGenerated;
      set
      {
        this.m_isGenerated = value;
        if (this.OuterField == null)
          return;
        this.OuterField.IsGenerated = value;
      }
    }

    public bool IsOuterReference
    {
      get
      {
        if (this.OuterField != null)
        {
          for (JSVariableField outerField = this.OuterField; outerField != null; outerField = outerField.OuterField)
          {
            if (!outerField.IsPlaceholder)
              return true;
          }
        }
        return false;
      }
    }

    public string CrunchedName
    {
      get => this.OuterField == null ? this.m_crunchedName : this.OuterField.CrunchedName;
      set
      {
        if (!this.m_canCrunch)
          return;
        if (this.OuterField != null)
          this.OuterField.CrunchedName = value;
        else
          this.m_crunchedName = value;
      }
    }

    public bool IsReferenced
    {
      get
      {
        if (this.FieldValue is FunctionObject fieldValue)
          return fieldValue.IsReferenced;
        return this.FieldValue is ClassNode || this.RefCount > 0;
      }
    }

    public bool IsReferencedInnerScope
    {
      get
      {
        foreach (INameReference reference in (IEnumerable<INameReference>) this.References)
        {
          if (reference.VariableField.OuterField != null)
            return true;
        }
        return false;
      }
    }

    public JSVariableField(
      FieldType fieldType,
      string name,
      FieldAttributes fieldAttributes,
      object value)
    {
      this.m_referenceTable = new HashSet<INameReference>();
      this.m_declarationTable = new HashSet<INameDeclaration>();
      this.Name = name;
      this.Attributes = fieldAttributes;
      this.FieldValue = value;
      this.SetFieldsBasedOnType(fieldType);
    }

    internal JSVariableField(FieldType fieldType, JSVariableField outerField)
    {
      if (outerField == null)
        throw new ArgumentNullException(nameof (outerField));
      this.m_referenceTable = new HashSet<INameReference>();
      this.m_declarationTable = new HashSet<INameDeclaration>();
      this.OuterField = outerField;
      this.Name = outerField.Name;
      this.Attributes = outerField.Attributes;
      this.FieldValue = outerField.FieldValue;
      this.IsGenerated = outerField.IsGenerated;
      this.SetFieldsBasedOnType(fieldType);
    }

    private void SetFieldsBasedOnType(FieldType fieldType)
    {
      this.FieldType = fieldType;
      switch (this.FieldType)
      {
        case FieldType.Local:
          this.CanCrunch = true;
          break;
        case FieldType.Predefined:
          this.IsDeclared = false;
          this.CanCrunch = false;
          break;
        case FieldType.Global:
        case FieldType.WithField:
        case FieldType.UndefinedGlobal:
        case FieldType.Super:
          this.CanCrunch = false;
          break;
        case FieldType.Arguments:
          this.IsDeclared = false;
          this.CanCrunch = false;
          break;
        case FieldType.Argument:
        case FieldType.CatchError:
          this.IsDeclared = true;
          this.CanCrunch = true;
          break;
        case FieldType.GhostCatch:
          this.CanCrunch = true;
          this.IsPlaceholder = true;
          break;
        case FieldType.GhostFunction:
          this.CanCrunch = this.OuterField == null || this.OuterField.CanCrunch;
          this.IsFunction = true;
          this.IsPlaceholder = true;
          break;
        default:
          throw new ArgumentException("Invalid field type", nameof (fieldType));
      }
    }

    public void AddReference(INameReference reference)
    {
      if (reference == null)
        return;
      this.m_referenceTable.Add(reference);
      if (this.OuterField == null)
        return;
      this.OuterField.AddReference(reference);
    }

    public void AddReferences(IEnumerable<INameReference> references)
    {
      if (references == null)
        return;
      foreach (INameReference reference in references)
        this.AddReference(reference);
    }

    public void Detach() => this.OuterField = (JSVariableField) null;

    public override string ToString()
    {
      string crunchedName = this.CrunchedName;
      return !string.IsNullOrEmpty(crunchedName) ? crunchedName : this.Name;
    }

    public override int GetHashCode() => this.Name.GetHashCode();

    public bool IsSameField(JSVariableField otherField)
    {
      if (this == otherField)
        return true;
      if (otherField == null)
        return false;
      JSVariableField jsVariableField1 = this.OuterField != null ? this.OuterField : this;
      while (jsVariableField1.OuterField != null)
        jsVariableField1 = jsVariableField1.OuterField;
      JSVariableField jsVariableField2 = otherField.OuterField != null ? otherField.OuterField : otherField;
      while (jsVariableField2.OuterField != null)
        jsVariableField2 = jsVariableField2.OuterField;
      return jsVariableField1 == jsVariableField2;
    }
  }
}
