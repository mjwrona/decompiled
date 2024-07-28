// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlTerm
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlTerm : CsdlNamedElement
  {
    private readonly CsdlTypeReference type;
    private readonly string appliesTo;
    private readonly string defaultValue;

    public CsdlTerm(
      string name,
      CsdlTypeReference type,
      string appliesTo,
      string defaultValue,
      CsdlLocation location)
      : base(name, location)
    {
      this.type = type;
      this.appliesTo = appliesTo;
      this.defaultValue = defaultValue;
    }

    public CsdlTypeReference Type => this.type;

    public string AppliesTo => this.appliesTo;

    public string DefaultValue => this.defaultValue;
  }
}
