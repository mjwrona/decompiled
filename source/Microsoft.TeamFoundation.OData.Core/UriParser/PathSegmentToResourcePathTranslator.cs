// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentToResourcePathTranslator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal sealed class PathSegmentToResourcePathTranslator : PathSegmentTranslator<string>
  {
    private KeySerializer KeySerializer;

    public PathSegmentToResourcePathTranslator(ODataUrlKeyDelimiter odataUrlKeyDelimiter) => this.KeySerializer = KeySerializer.Create(odataUrlKeyDelimiter.EnableKeyAsSegment);

    public override string Translate(TypeSegment segment)
    {
      IEdmType type = segment.EdmType;
      if (type is IEdmCollectionType edmCollectionType)
        type = edmCollectionType.ElementType.Definition;
      return "/" + type.FullTypeName();
    }

    public override string Translate(NavigationPropertySegment segment) => "/" + segment.NavigationProperty.Name;

    public override string Translate(EntitySetSegment segment) => "/" + segment.EntitySet.Name;

    public override string Translate(SingletonSegment segment) => "/" + segment.Singleton.Name;

    public override string Translate(KeySegment segment)
    {
      List<KeyValuePair<string, object>> list = segment.Keys.ToList<KeyValuePair<string, object>>();
      StringBuilder builder = new StringBuilder();
      this.KeySerializer.AppendKeyExpression<KeyValuePair<string, object>>(builder, (ICollection<KeyValuePair<string, object>>) new Collection<KeyValuePair<string, object>>((IList<KeyValuePair<string, object>>) list), (Func<KeyValuePair<string, object>, string>) (p => p.Key), (Func<KeyValuePair<string, object>, object>) (p => p.Value));
      return builder.ToString();
    }

    public override string Translate(PropertySegment segment) => "/" + segment.Property.Name;

    public override string Translate(AnnotationSegment segment) => "/" + segment.Term.FullName();

    public override string Translate(OperationSegment segment)
    {
      string str1 = (string) null;
      NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
      foreach (OperationSegmentParameter parameter in segment.Parameters)
      {
        string str2 = nodeToStringBuilder.TranslateNode((QueryNode) parameter.Value);
        str1 = str1 + (string.IsNullOrEmpty(str1) ? (string) null : ",") + parameter.Name + "=" + str2;
      }
      if (string.IsNullOrEmpty(str1))
        return "/" + segment.Operations.OperationGroupFullName();
      return "/" + segment.Operations.OperationGroupFullName() + "(" + str1 + ")";
    }

    public override string Translate(OperationImportSegment segment)
    {
      NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
      string str1 = (string) null;
      foreach (OperationSegmentParameter parameter in segment.Parameters)
      {
        string str2 = nodeToStringBuilder.TranslateNode((QueryNode) parameter.Value);
        str1 = str1 + (string.IsNullOrEmpty(str1) ? (string) null : ",") + parameter.Name + "=" + str2;
      }
      if (string.IsNullOrEmpty(str1))
        return "/" + segment.Identifier;
      return "/" + segment.Identifier + "(" + str1 + ")";
    }

    public override string Translate(DynamicPathSegment segment) => "/" + segment.Identifier;

    public override string Translate(CountSegment segment) => "/" + segment.Identifier;

    public override string Translate(FilterSegment segment) => "/" + segment.LiteralText;

    public override string Translate(ReferenceSegment segment) => "/" + segment.Identifier;

    public override string Translate(EachSegment segment) => "/" + segment.Identifier;

    public override string Translate(NavigationPropertyLinkSegment segment) => "/" + segment.NavigationProperty.Name + "/" + "$ref";

    public override string Translate(ValueSegment segment) => "/" + segment.Identifier;

    public override string Translate(BatchSegment segment) => "/" + segment.Identifier;

    public override string Translate(BatchReferenceSegment segment) => "/" + segment.ContentId;

    public override string Translate(MetadataSegment segment) => "/" + segment.Identifier;

    public override string Translate(PathTemplateSegment segment) => "/" + segment.Identifier;
  }
}
