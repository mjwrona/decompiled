// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlNavigationProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlNavigationProperty : CsdlNamedElement
  {
    private readonly string type;
    private readonly bool? nullable;
    private readonly IEdmPathExpression partnerPath;
    private readonly bool containsTarget;
    private readonly CsdlOnDelete onDelete;
    private readonly IEnumerable<CsdlReferentialConstraint> referentialConstraints;

    public CsdlNavigationProperty(
      string name,
      string type,
      bool? nullable,
      string partner,
      bool containsTarget,
      CsdlOnDelete onDelete,
      IEnumerable<CsdlReferentialConstraint> referentialConstraints,
      CsdlLocation location)
      : base(name, location)
    {
      this.type = type;
      this.nullable = nullable;
      this.partnerPath = partner == null ? (IEdmPathExpression) null : (IEdmPathExpression) new EdmPathExpression(partner);
      this.containsTarget = containsTarget;
      this.onDelete = onDelete;
      this.referentialConstraints = referentialConstraints;
    }

    public string Type => this.type;

    public bool? Nullable => this.nullable;

    public IEdmPathExpression PartnerPath => this.partnerPath;

    public bool ContainsTarget => this.containsTarget;

    public CsdlOnDelete OnDelete => this.onDelete;

    public IEnumerable<CsdlReferentialConstraint> ReferentialConstraints => this.referentialConstraints;
  }
}
