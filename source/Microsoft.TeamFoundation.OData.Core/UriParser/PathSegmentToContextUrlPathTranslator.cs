// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentToContextUrlPathTranslator
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
  internal sealed class PathSegmentToContextUrlPathTranslator : PathSegmentTranslator<string>
  {
    internal static readonly PathSegmentToContextUrlPathTranslator DefaultInstance = new PathSegmentToContextUrlPathTranslator(false);
    internal static readonly PathSegmentToContextUrlPathTranslator KeyAsSegmentInstance = new PathSegmentToContextUrlPathTranslator(true);
    private KeySerializer KeySerializer;

    private PathSegmentToContextUrlPathTranslator(bool keyAsSegment) => this.KeySerializer = KeySerializer.Create(keyAsSegment);

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

    public override string Translate(OperationSegment segment) => "/" + segment.Operations.OperationGroupFullName();

    public override string Translate(OperationImportSegment segment) => string.Empty;

    public override string Translate(DynamicPathSegment segment) => "/" + segment.Identifier;

    public override string Translate(CountSegment segment) => string.Empty;

    public override string Translate(FilterSegment segment) => string.Empty;

    public override string Translate(ReferenceSegment segment) => string.Empty;

    public override string Translate(EachSegment segment) => string.Empty;

    public override string Translate(NavigationPropertyLinkSegment segment) => string.Empty;

    public override string Translate(ValueSegment segment) => string.Empty;

    public override string Translate(BatchSegment segment) => string.Empty;

    public override string Translate(BatchReferenceSegment segment) => string.Empty;

    public override string Translate(MetadataSegment segment) => string.Empty;

    public override string Translate(PathTemplateSegment segment) => "/" + segment.Identifier;
  }
}
