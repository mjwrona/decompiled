// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.DottedIdentifierBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  internal sealed class DottedIdentifierBinder : BinderBase
  {
    internal DottedIdentifierBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
      : base(bindMethod, state)
    {
    }

    internal QueryNode BindDottedIdentifier(DottedIdentifierToken dottedIdentifierToken)
    {
      ExceptionUtils.CheckArgumentNotNull<DottedIdentifierToken>(dottedIdentifierToken, "castToken");
      ExceptionUtils.CheckArgumentNotNull<BindingState>(this.state, "state");
      QueryNode segment = (QueryNode) null;
      IEdmType edmType = (IEdmType) null;
      if (this.state.ImplicitRangeVariable != null)
      {
        if (dottedIdentifierToken.NextToken == null)
        {
          segment = (QueryNode) NodeFactory.CreateRangeVariableReferenceNode(this.state.ImplicitRangeVariable);
          edmType = this.state.ImplicitRangeVariable.TypeReference.Definition;
        }
        else
        {
          segment = this.bindMethod(dottedIdentifierToken.NextToken);
          edmType = segment.GetEdmType();
        }
      }
      SingleResourceNode singleResourceNode = segment as SingleResourceNode;
      IEdmSchemaType typeFromModel = UriEdmHelpers.FindTypeFromModel(this.state.Model, dottedIdentifierToken.Identifier, this.Resolver);
      if (!(typeFromModel is IEdmStructuredType structuredType))
      {
        SingleValueNode singleValueNode = segment as SingleValueNode;
        QueryNode boundFunction;
        if (new FunctionCallBinder(this.bindMethod, this.state).TryBindDottedIdentifierAsFunctionCall(dottedIdentifierToken, singleValueNode, out boundFunction))
          return boundFunction;
        if (!string.IsNullOrEmpty(dottedIdentifierToken.Identifier) && dottedIdentifierToken.Identifier[dottedIdentifierToken.Identifier.Length - 1] == '\'')
        {
          QueryNode boundEnum;
          if (EnumBinder.TryBindDottedIdentifierAsEnum(dottedIdentifierToken, (SingleValueNode) singleResourceNode, this.state, this.Resolver, out boundEnum))
            return boundEnum;
          throw new ODataException(Microsoft.OData.Strings.Binder_IsNotValidEnumConstant((object) dottedIdentifierToken.Identifier));
        }
        switch (UriEdmHelpers.FindTypeFromModel(this.state.Model, dottedIdentifierToken.Identifier, this.Resolver).ToTypeReference())
        {
          case IEdmPrimitiveTypeReference _:
          case IEdmEnumTypeReference _:
            return typeFromModel is IEdmPrimitiveType primitiveType && dottedIdentifierToken.NextToken != null ? (QueryNode) new SingleValueCastNode(singleValueNode, primitiveType) : (QueryNode) new ConstantNode((object) dottedIdentifierToken.Identifier, dottedIdentifierToken.Identifier);
          default:
            throw new ODataException(Microsoft.OData.Strings.CastBinder_ChildTypeIsNotEntity((object) dottedIdentifierToken.Identifier));
        }
      }
      else
      {
        UriEdmHelpers.CheckRelatedTo(edmType, (IEdmType) typeFromModel);
        this.state.ParsedSegments.Add((ODataPathSegment) new TypeSegment((IEdmType) typeFromModel, edmType, (IEdmNavigationSource) null));
        return segment is CollectionResourceNode source ? (QueryNode) new CollectionResourceCastNode(source, structuredType) : (QueryNode) new SingleResourceCastNode(singleResourceNode, structuredType);
      }
    }
  }
}
