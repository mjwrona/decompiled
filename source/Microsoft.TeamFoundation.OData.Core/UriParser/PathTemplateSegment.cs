// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathTemplateSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class PathTemplateSegment : ODataPathSegment
  {
    public PathTemplateSegment(string literalText)
    {
      this.LiteralText = literalText;
      this.Identifier = literalText;
      this.SingleResult = true;
      this.TargetKind = RequestTargetKind.Nothing;
    }

    public string LiteralText { get; private set; }

    public override IEdmType EdmType => (IEdmType) null;

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
  }
}
