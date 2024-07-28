// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BatchSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  public sealed class BatchSegment : ODataPathSegment
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "BatchSegment is immutable")]
    public static readonly BatchSegment Instance = new BatchSegment();

    private BatchSegment()
    {
      this.Identifier = "$batch";
      this.TargetKind = RequestTargetKind.Batch;
    }

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

    internal override bool Equals(ODataPathSegment other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(other, nameof (other));
      return other is BatchSegment;
    }
  }
}
