// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.QueryNodeKind
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "QueryNodeKind is not a flag.")]
  public enum QueryNodeKind
  {
    None,
    Constant,
    Convert,
    NonResourceRangeVariableReference,
    BinaryOperator,
    UnaryOperator,
    SingleValuePropertyAccess,
    CollectionPropertyAccess,
    SingleValueFunctionCall,
    Any,
    CollectionNavigationNode,
    SingleNavigationNode,
    SingleValueOpenPropertyAccess,
    SingleResourceCast,
    All,
    CollectionResourceCast,
    ResourceRangeVariableReference,
    SingleResourceFunctionCall,
    CollectionFunctionCall,
    CollectionResourceFunctionCall,
    NamedFunctionParameter,
    ParameterAlias,
    EntitySet,
    KeyLookup,
    SearchTerm,
    CollectionOpenPropertyAccess,
    CollectionComplexNode,
    SingleComplexNode,
    Count,
    SingleValueCast,
    CollectionPropertyNode,
    AggregatedCollectionPropertyNode,
    In,
    CollectionConstant,
  }
}
