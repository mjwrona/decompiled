// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPathSegmentHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataPathSegmentHandler : PathSegmentHandler
  {
    private readonly IList<string> _pathTemplate;
    private readonly IList<string> _pathUriLiteral;
    private IEdmNavigationSource _navigationSource;

    public ODataPathSegmentHandler()
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate = (IList<string>) new List<string>()
      {
        "~"
      };
      this._pathUriLiteral = (IList<string>) new List<string>();
    }

    public IEdmNavigationSource NavigationSource => this._navigationSource;

    public string PathTemplate => string.Join("/", (IEnumerable<string>) this._pathTemplate);

    public string PathLiteral => string.Join("/", (IEnumerable<string>) this._pathUriLiteral);

    public override void Handle(EntitySetSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) segment.EntitySet;
      this._pathTemplate.Add("entityset");
      this._pathUriLiteral.Add(segment.EntitySet.Name);
    }

    public override void Handle(KeySegment segment)
    {
      this._navigationSource = segment.NavigationSource;
      if (this._pathTemplate.Last<string>() == "$ref")
        this._pathTemplate.Insert(this._pathTemplate.Count - 1, "key");
      else
        this._pathTemplate.Add("key");
      string str = ODataPathSegmentHandler.ConvertKeysToString(segment.Keys, segment.EdmType);
      if (!this._pathUriLiteral.Any<string>())
        this._pathUriLiteral.Add("(" + str + ")");
      else if (this._pathUriLiteral.Last<string>() == "$ref")
        this._pathUriLiteral[this._pathUriLiteral.Count - 2] = this._pathUriLiteral[this._pathUriLiteral.Count - 2] + "(" + str + ")";
      else
        this._pathUriLiteral[this._pathUriLiteral.Count - 1] = this._pathUriLiteral[this._pathUriLiteral.Count - 1] + "(" + str + ")";
    }

    public override void Handle(NavigationPropertyLinkSegment segment)
    {
      this._navigationSource = segment.NavigationSource;
      this._pathTemplate.Add("navigation");
      this._pathTemplate.Add("$ref");
      this._pathUriLiteral.Add(segment.NavigationProperty.Name);
      this._pathUriLiteral.Add("$ref");
    }

    public override void Handle(NavigationPropertySegment segment)
    {
      this._navigationSource = segment.NavigationSource;
      this._pathTemplate.Add("navigation");
      this._pathUriLiteral.Add(segment.NavigationProperty.Name);
    }

    public override void Handle(DynamicPathSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("dynamicproperty");
      this._pathUriLiteral.Add(segment.Identifier);
    }

    public override void Handle(OperationImportSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) segment.EntitySet;
      if (segment.OperationImports.Single<IEdmOperationImport>() is IEdmActionImport edmActionImport)
      {
        this._pathTemplate.Add("unboundaction");
        this._pathUriLiteral.Add(edmActionImport.Name);
      }
      else
      {
        this._pathTemplate.Add("unboundfunction");
        Dictionary<string, string> dictionary = segment.Parameters.ToDictionary<OperationSegmentParameter, string, string>((Func<OperationSegmentParameter, string>) (parameterValue => parameterValue.Name), (Func<OperationSegmentParameter, string>) (parameterValue => ODataPathSegmentHandler.TranslateNode(parameterValue.Value)));
        this._pathUriLiteral.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", new object[2]
        {
          (object) segment.OperationImports.Single<IEdmOperationImport>().Name,
          (object) string.Join(",", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (v => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
          {
            (object) v.Key,
            (object) v.Value
          }))))
        }));
      }
    }

    public override void Handle(OperationSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) segment.EntitySet;
      if (segment.Operations.Single<IEdmOperation>() is IEdmAction element1)
      {
        this._pathTemplate.Add("action");
        this._pathUriLiteral.Add(element1.FullName());
      }
      else
      {
        this._pathTemplate.Add("function");
        Dictionary<string, string> dictionary = segment.Parameters.ToDictionary<OperationSegmentParameter, string, string>((Func<OperationSegmentParameter, string>) (parameterValue => parameterValue.Name), (Func<OperationSegmentParameter, string>) (parameterValue => ODataPathSegmentHandler.TranslateNode(parameterValue.Value)));
        IEdmFunction element = (IEdmFunction) segment.Operations.Single<IEdmOperation>();
        IEnumerable<string> values = dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (v => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
        {
          (object) v.Key,
          (object) v.Value
        })));
        this._pathUriLiteral.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", new object[2]
        {
          (object) element.FullName(),
          (object) string.Join(",", values)
        }));
      }
    }

    public override void Handle(PathTemplateSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("property");
      this._pathUriLiteral.Add(segment.LiteralText);
    }

    public override void Handle(PropertySegment segment)
    {
      this._pathTemplate.Add("property");
      this._pathUriLiteral.Add(segment.Property.Name);
    }

    public override void Handle(SingletonSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) segment.Singleton;
      this._pathTemplate.Add("singleton");
      this._pathUriLiteral.Add(segment.Singleton.Name);
    }

    public override void Handle(TypeSegment segment)
    {
      this._navigationSource = segment.NavigationSource;
      this._pathTemplate.Add("cast");
      IEdmType type = segment.EdmType;
      if (segment.EdmType.TypeKind == EdmTypeKind.Collection)
        type = ((IEdmCollectionType) segment.EdmType).ElementType.Definition;
      this._pathUriLiteral.Add(type.FullTypeName());
    }

    public override void Handle(ValueSegment segment)
    {
      this._pathTemplate.Add("$value");
      this._pathUriLiteral.Add("$value");
    }

    public override void Handle(CountSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("$count");
      this._pathUriLiteral.Add("$count");
    }

    public override void Handle(BatchSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("$batch");
      this._pathUriLiteral.Add("$batch");
    }

    public override void Handle(MetadataSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("$metadata");
      this._pathUriLiteral.Add("$metadata");
    }

    public override void Handle(ODataPathSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add(segment.ToString());
      this._pathUriLiteral.Add(segment.ToString());
    }

    public virtual void Handle(UnresolvedPathSegment segment)
    {
      this._navigationSource = (IEdmNavigationSource) null;
      this._pathTemplate.Add("unresolved");
      this._pathUriLiteral.Add(segment.SegmentValue);
    }

    private static string ConvertKeysToString(
      IEnumerable<KeyValuePair<string, object>> keys,
      IEdmType edmType)
    {
      IEdmEntityType type = edmType as IEdmEntityType;
      if (!(keys is IList<KeyValuePair<string, object>> keyValuePairList))
        keyValuePairList = (IList<KeyValuePair<string, object>>) keys.ToList<KeyValuePair<string, object>>();
      IList<KeyValuePair<string, object>> source = keyValuePairList;
      if (source.Count < 1)
        return string.Empty;
      if (source.Count < 2)
      {
        KeyValuePair<string, object> keyValue = source.First<KeyValuePair<string, object>>();
        if (type.Key().Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (k => k.Name == keyValue.Key)))
          return string.Join(",", source.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (keyValuePair => ODataPathSegmentHandler.TranslateKeySegmentValue(keyValuePair.Value))).ToArray<string>());
      }
      return string.Join(",", source.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (keyValuePair => keyValuePair.Key + "=" + ODataPathSegmentHandler.TranslateKeySegmentValue(keyValuePair.Value))).ToArray<string>());
    }

    private static string TranslateKeySegmentValue(object value)
    {
      switch (value)
      {
        case null:
          throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
        case UriTemplateExpression templateExpression:
          return templateExpression.LiteralText;
        case ConstantNode constantNode:
          if (constantNode.Value is ODataEnumValue odataEnumValue)
            return ODataUriUtils.ConvertToUriLiteral((object) odataEnumValue, ODataVersion.V4);
          break;
      }
      return ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4);
    }

    private static string TranslateNode(object node)
    {
      switch (node)
      {
        case ConstantNode constantNode:
          if (constantNode.Value is UriTemplateExpression templateExpression)
            return templateExpression.LiteralText;
          return constantNode.Value is ODataEnumValue odataEnumValue ? ODataUriUtils.ConvertToUriLiteral((object) odataEnumValue, ODataVersion.V4) : constantNode.LiteralText;
        case ConvertNode convertNode:
          return ODataPathSegmentHandler.TranslateNode((object) convertNode.Source);
        case ParameterAliasNode parameterAliasNode:
          return parameterAliasNode.Alias;
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.CannotRecognizeNodeType, (object) typeof (ODataPathSegmentHandler), (object) node.GetType().FullName);
      }
    }
  }
}
