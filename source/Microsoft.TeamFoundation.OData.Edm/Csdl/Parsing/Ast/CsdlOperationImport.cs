// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlOperationImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal abstract class CsdlOperationImport : CsdlFunctionBase
  {
    private readonly string entitySet;

    protected CsdlOperationImport(
      string name,
      string schemaOperationQualifiedTypeName,
      string entitySet,
      IEnumerable<CsdlOperationParameter> parameters,
      CsdlOperationReturn returnType,
      CsdlLocation location)
      : base(name, parameters, returnType, location)
    {
      this.entitySet = entitySet;
      this.SchemaOperationQualifiedTypeName = schemaOperationQualifiedTypeName;
    }

    public string EntitySet => this.entitySet;

    public string SchemaOperationQualifiedTypeName { get; private set; }
  }
}
