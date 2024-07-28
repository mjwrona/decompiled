// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.AnnotationSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.UriParser
{
  public sealed class AnnotationSegment : ODataPathSegment
  {
    private readonly IEdmTerm term;

    public AnnotationSegment(IEdmTerm term)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTerm>(term, nameof (term));
      this.term = term;
      this.Identifier = term.Name;
      this.TargetEdmType = term.Type.Definition;
      this.SingleResult = !term.Type.IsCollection();
    }

    public IEdmTerm Term => this.term;

    public override IEdmType EdmType => this.term.Type.Definition;

    public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentTranslator<T>>(translator, nameof (translator));
      return translator.Translate(this);
    }

    public override void HandleWith(PathSegmentHandler handler)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentHandler>(handler, nameof (handler));
      handler.Handle(this);
    }

    internal override bool Equals(ODataPathSegment other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(other, nameof (other));
      return other is AnnotationSegment annotationSegment && annotationSegment.term == this.term;
    }
  }
}
