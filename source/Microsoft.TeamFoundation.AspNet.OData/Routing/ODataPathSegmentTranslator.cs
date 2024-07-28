// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPathSegmentTranslator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataPathSegmentTranslator : PathSegmentTranslator<ODataPathSegment>
  {
    private readonly IDictionary<string, SingleValueNode> _parameterAliasNodes;
    private readonly IEdmModel _model;

    public static IEnumerable<ODataPathSegment> Translate(
      IEdmModel model,
      Microsoft.OData.UriParser.ODataPath path,
      IDictionary<string, SingleValueNode> parameterAliasNodes)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (path == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (path));
      ODataPathSegmentTranslator translator = new ODataPathSegmentTranslator(model, parameterAliasNodes);
      return path.WalkWith<ODataPathSegment>((PathSegmentTranslator<ODataPathSegment>) translator);
    }

    public ODataPathSegmentTranslator(
      IEdmModel model,
      IDictionary<string, SingleValueNode> parameterAliasNodes)
    {
      this._model = model != null ? model : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      this._parameterAliasNodes = parameterAliasNodes ?? (IDictionary<string, SingleValueNode>) new Dictionary<string, SingleValueNode>();
    }

    public override ODataPathSegment Translate(TypeSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(EntitySetSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(SingletonSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(PropertySegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(DynamicPathSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(CountSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(NavigationPropertySegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(NavigationPropertyLinkSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(ValueSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(BatchSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(MetadataSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(PathTemplateSegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(KeySegment segment) => (ODataPathSegment) segment;

    public override ODataPathSegment Translate(OperationImportSegment segment)
    {
      if (segment.OperationImports.Single<IEdmOperationImport>() is IEdmActionImport)
        return (ODataPathSegment) segment;
      OperationImportSegment operationImportSegment = segment;
      if (segment.Parameters.Any<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Value is ParameterAliasNode || p.Value is Microsoft.OData.UriParser.ConvertNode)))
      {
        IEnumerable<OperationSegmentParameter> parameters = segment.Parameters.Select<OperationSegmentParameter, OperationSegmentParameter>((Func<OperationSegmentParameter, OperationSegmentParameter>) (e => new OperationSegmentParameter(e.Name, this.TranslateNode(e.Value))));
        operationImportSegment = new OperationImportSegment(segment.OperationImports, segment.EntitySet, parameters);
      }
      return (ODataPathSegment) operationImportSegment;
    }

    public override ODataPathSegment Translate(OperationSegment segment)
    {
      if (segment.Operations.Single<IEdmOperation>() is IEdmFunction)
      {
        OperationSegment operationSegment = segment;
        if (segment.Parameters.Any<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Value is ParameterAliasNode || p.Value is Microsoft.OData.UriParser.ConvertNode)))
        {
          IEnumerable<OperationSegmentParameter> parameters = segment.Parameters.Select<OperationSegmentParameter, OperationSegmentParameter>((Func<OperationSegmentParameter, OperationSegmentParameter>) (e => new OperationSegmentParameter(e.Name, this.TranslateNode(e.Value))));
          operationSegment = new OperationSegment(segment.Operations, parameters, segment.EntitySet);
        }
        segment = operationSegment;
      }
      ReturnedEntitySetAnnotation annotationValue = this._model.GetAnnotationValue<ReturnedEntitySetAnnotation>((IEdmElement) segment.Operations.Single<IEdmOperation>());
      IEdmEntitySet entitySet = (IEdmEntitySet) null;
      if (annotationValue != null)
        entitySet = this._model.EntityContainer.FindEntitySet(annotationValue.EntitySetName);
      if (entitySet != null)
        segment = new OperationSegment(segment.Operations, segment.Parameters, (IEdmEntitySetBase) entitySet);
      return (ODataPathSegment) segment;
    }

    private object TranslateNode(object node)
    {
      switch (node)
      {
        case null:
          throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (node));
        case ConstantNode constantNode:
          return (object) constantNode;
        case Microsoft.OData.UriParser.ConvertNode convertNode:
          return this.ConvertNode(this.TranslateNode((object) convertNode.Source), convertNode.TypeReference);
        case ParameterAliasNode parameterAliasNode:
          SingleValueNode node1;
          return this._parameterAliasNodes.TryGetValue(parameterAliasNode.Alias, out node1) && node1 != null ? this.TranslateNode((object) node1) : (object) null;
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.CannotRecognizeNodeType, (object) typeof (ODataPathSegmentTranslator), (object) node.GetType().FullName);
      }
    }

    private object ConvertNode(object node, IEdmTypeReference typeReference)
    {
      if (node == null)
        return (object) null;
      if (!(node is ConstantNode constantNode) || constantNode.Value is UriTemplateExpression || constantNode.Value is ODataEnumValue)
        return node;
      string literalText = constantNode.LiteralText;
      return (object) new ConstantNode(ODataUriUtils.ConvertFromUriLiteral(literalText, ODataVersion.V4, this._model, typeReference), literalText, typeReference);
    }

    internal static SingleValueNode TranslateParameterAlias(
      SingleValueNode node,
      IDictionary<string, SingleValueNode> parameterAliasNodes)
    {
      if (node == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (node));
      if (parameterAliasNodes == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (parameterAliasNodes));
      if (!(node is ParameterAliasNode parameterAliasNode))
        return node;
      SingleValueNode node1;
      if (!parameterAliasNodes.TryGetValue(parameterAliasNode.Alias, out node1) || node1 == null)
        return (SingleValueNode) null;
      if (node1 is ParameterAliasNode)
        node1 = ODataPathSegmentTranslator.TranslateParameterAlias(node1, parameterAliasNodes);
      return node1;
    }
  }
}
