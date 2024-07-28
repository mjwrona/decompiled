// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlFunctionBase
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal abstract class CsdlFunctionBase : CsdlNamedElement
  {
    private readonly List<CsdlOperationParameter> parameters;
    private readonly CsdlOperationReturn operationReturn;

    protected CsdlFunctionBase(
      string name,
      IEnumerable<CsdlOperationParameter> parameters,
      CsdlOperationReturn operationReturn,
      CsdlLocation location)
      : base(name, location)
    {
      this.parameters = new List<CsdlOperationParameter>(parameters);
      this.operationReturn = operationReturn;
    }

    public IEnumerable<CsdlOperationParameter> Parameters => (IEnumerable<CsdlOperationParameter>) this.parameters;

    public CsdlOperationReturn Return => this.operationReturn;
  }
}
