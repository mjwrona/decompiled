// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlEnumType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlEnumType : CsdlNamedElement
  {
    private readonly string underlyingTypeName;
    private readonly bool isFlags;
    private readonly List<CsdlEnumMember> members;

    public CsdlEnumType(
      string name,
      string underlyingTypeName,
      bool isFlags,
      IEnumerable<CsdlEnumMember> members,
      CsdlLocation location)
      : base(name, location)
    {
      this.underlyingTypeName = underlyingTypeName;
      this.isFlags = isFlags;
      this.members = new List<CsdlEnumMember>(members);
    }

    public string UnderlyingTypeName => this.underlyingTypeName;

    public bool IsFlags => this.isFlags;

    public IEnumerable<CsdlEnumMember> Members => (IEnumerable<CsdlEnumMember>) this.members;
  }
}
