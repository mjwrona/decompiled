// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.BlockScope
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Reflection;

namespace Microsoft.Ajax.Utilities
{
  public class BlockScope : ActivationObject
  {
    public BlockScope(ActivationObject parent, CodeSettings settings, ScopeType scopeType)
      : base(parent, settings)
    {
      this.ScopeType = scopeType;
    }

    public override void DeclareScope() => this.DefineLexicalDeclarations();

    public override JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      return new JSVariableField(FieldType.Local, name, attributes, value);
    }
  }
}
