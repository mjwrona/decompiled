// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.WithScope
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public sealed class WithScope : BlockScope
  {
    public WithScope(ActivationObject parent, CodeSettings settings)
      : base(parent, settings, ScopeType.With)
    {
      this.IsInWithScope = true;
    }

    public override JSVariableField CreateInnerField(JSVariableField outerField) => outerField.IfNotNull<JSVariableField, JSVariableField>((Func<JSVariableField, JSVariableField>) (o =>
    {
      JSVariableField innerField = this.AddField(this.CreateField(outerField));
      outerField.CanCrunch = false;
      if (outerField.FieldType == FieldType.UndefinedGlobal)
      {
        do
        {
          outerField.Attributes |= FieldAttributes.RTSpecialName;
        }
        while ((outerField = outerField.OuterField) != null);
      }
      return innerField;
    }));

    public override void DeclareScope()
    {
      this.DefineLexicalDeclarations();
      foreach (INameDeclaration varDeclaredName in (IEnumerable<INameDeclaration>) this.VarDeclaredNames)
      {
        if (varDeclaredName.VariableField != null)
          varDeclaredName.VariableField.CanCrunch = false;
      }
    }

    public override JSVariableField CreateField(JSVariableField outerField) => new JSVariableField(FieldType.WithField, outerField);

    public override JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      return new JSVariableField(FieldType.WithField, name, attributes, (object) null);
    }
  }
}
