// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ModuleScope
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public class ModuleScope : ActivationObject
  {
    private Dictionary<string, JSVariableField> m_knownExports;

    public bool HasDefaultExport { get; set; }

    public bool IsNotComplete { get; set; }

    public ModuleScope(ModuleDeclaration module, ActivationObject parent, CodeSettings settings)
      : base(parent, settings)
    {
      this.Owner = (AstNode) module;
      this.UseStrict = true;
      this.ScopeType = ScopeType.Module;
      this.m_knownExports = new Dictionary<string, JSVariableField>();
    }

    public override void DeclareScope()
    {
      this.DefineLexicalDeclarations();
      this.DefineVarDeclarations();
      foreach (JSVariableField jsVariableField in (IEnumerable<JSVariableField>) this.NameTable.Values)
      {
        if (jsVariableField.IsExported)
          this.m_knownExports.Add(jsVariableField.Name, jsVariableField);
      }
    }

    internal override void AnalyzeScope() => base.AnalyzeScope();

    public override JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      return new JSVariableField(FieldType.Local, name, attributes, value);
    }
  }
}
