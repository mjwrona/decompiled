// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlApplyExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlApplyExpression : CsdlExpressionBase
  {
    private readonly string function;
    private readonly List<CsdlExpressionBase> arguments;

    public CsdlApplyExpression(
      string function,
      IEnumerable<CsdlExpressionBase> arguments,
      CsdlLocation location)
      : base(location)
    {
      this.function = function;
      this.arguments = new List<CsdlExpressionBase>(arguments);
    }

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.FunctionApplication;

    public string Function => this.function;

    public IEnumerable<CsdlExpressionBase> Arguments => (IEnumerable<CsdlExpressionBase>) this.arguments;
  }
}
