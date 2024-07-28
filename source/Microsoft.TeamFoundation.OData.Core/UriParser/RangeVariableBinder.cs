// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.RangeVariableBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class RangeVariableBinder
  {
    internal static SingleValueNode BindRangeVariableToken(
      RangeVariableToken rangeVariableToken,
      BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<RangeVariableToken>(rangeVariableToken, nameof (rangeVariableToken));
      return NodeFactory.CreateRangeVariableReferenceNode(state.RangeVariables.SingleOrDefault<RangeVariable>((Func<RangeVariable, bool>) (p => p.Name == rangeVariableToken.Name)) ?? throw new ODataException(Strings.MetadataBinder_ParameterNotInScope((object) rangeVariableToken.Name)));
    }
  }
}
