// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentToStringTranslator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  internal sealed class PathSegmentToStringTranslator : PathSegmentTranslator<string>
  {
    internal static readonly PathSegmentToStringTranslator Instance = new PathSegmentToStringTranslator();

    private PathSegmentToStringTranslator()
    {
    }

    public override string Translate(TypeSegment segment) => segment.EdmType.FullTypeName();

    public override string Translate(NavigationPropertySegment segment) => segment.NavigationProperty.Name;

    public override string Translate(PropertySegment segment) => segment.Property.Name;

    public override string Translate(AnnotationSegment segment) => segment.Term.FullName();

    public override string Translate(OperationSegment segment) => segment.Operations.OperationGroupFullName();

    public override string Translate(OperationImportSegment segment) => segment.OperationImports.OperationImportGroupFullName();

    public override string Translate(DynamicPathSegment segment) => segment.Identifier;

    public override string Translate(FilterSegment segment) => segment.LiteralText;

    public override string Translate(ReferenceSegment segment) => segment.Identifier;

    public override string Translate(EachSegment segment) => segment.Identifier;

    public override string Translate(PathTemplateSegment segment) => segment.LiteralText;
  }
}
