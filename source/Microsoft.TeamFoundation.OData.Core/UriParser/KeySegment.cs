// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.KeySegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class KeySegment : ODataPathSegment
  {
    private readonly ReadOnlyCollection<KeyValuePair<string, object>> keys;
    private readonly IEdmEntityType edmType;
    private readonly IEdmNavigationSource navigationSource;

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
    public KeySegment(
      IEnumerable<KeyValuePair<string, object>> keys,
      IEdmEntityType edmType,
      IEdmNavigationSource navigationSource)
    {
      this.keys = new ReadOnlyCollection<KeyValuePair<string, object>>((IList<KeyValuePair<string, object>>) keys.ToList<KeyValuePair<string, object>>());
      this.edmType = edmType;
      this.navigationSource = navigationSource;
      this.SingleResult = true;
      if (navigationSource == null)
        return;
      ExceptionUtil.ThrowIfTypesUnrelated((IEdmType) edmType, (IEdmType) navigationSource.EntityType(), "KeySegments");
    }

    public KeySegment(
      ODataPathSegment previous,
      IEnumerable<KeyValuePair<string, object>> keys,
      IEdmEntityType edmType,
      IEdmNavigationSource navigationSource)
      : this(keys, edmType, navigationSource)
    {
      if (previous == null)
        return;
      this.CopyValuesFrom(previous);
      this.SingleResult = true;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
    public IEnumerable<KeyValuePair<string, object>> Keys => this.keys.AsEnumerable<KeyValuePair<string, object>>();

    public override IEdmType EdmType => (IEdmType) this.edmType;

    public IEdmNavigationSource NavigationSource => this.navigationSource;

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
      return other is KeySegment keySegment && keySegment.Keys.SequenceEqual<KeyValuePair<string, object>>(this.Keys) && keySegment.EdmType == this.edmType && keySegment.NavigationSource == this.navigationSource;
    }
  }
}
