// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlFunctionImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlFunctionImport : CsdlOperationImport
  {
    public CsdlFunctionImport(
      string name,
      string schemaOperationQualifiedTypeName,
      string entitySet,
      bool includeInServiceDocument,
      CsdlLocation location)
      : base(name, schemaOperationQualifiedTypeName, entitySet, (IEnumerable<CsdlOperationParameter>) new CsdlOperationParameter[0], (CsdlOperationReturn) null, location)
    {
      this.IncludeInServiceDocument = includeInServiceDocument;
    }

    public bool IncludeInServiceDocument { get; private set; }
  }
}
