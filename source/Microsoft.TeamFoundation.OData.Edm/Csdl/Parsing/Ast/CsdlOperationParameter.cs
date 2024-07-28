// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlOperationParameter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlOperationParameter : CsdlNamedElement
  {
    private readonly CsdlTypeReference type;
    private readonly bool isOptional;
    private readonly string defaultValue;

    public CsdlOperationParameter(string name, CsdlTypeReference type, CsdlLocation location)
      : base(name, location)
    {
      this.type = type;
    }

    public CsdlOperationParameter(
      string name,
      CsdlTypeReference type,
      CsdlLocation location,
      bool isOptional,
      string defaultValue)
      : this(name, type, location)
    {
      this.isOptional = isOptional;
      this.defaultValue = defaultValue;
    }

    public CsdlTypeReference Type => this.type;

    public bool IsOptional => this.isOptional;

    public string DefaultValue => this.defaultValue;
  }
}
