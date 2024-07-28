// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTypeDefinitionReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsTypeDefinitionReference : 
    CsdlSemanticsNamedTypeReference,
    IEdmTypeDefinitionReference,
    IEdmTypeReference,
    IEdmElement
  {
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, bool> ComputeIsUnboundedFunc = (Func<CsdlSemanticsTypeDefinitionReference, bool>) (me => me.ComputeIsUnbounded());
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeMaxLengthFunc = (Func<CsdlSemanticsTypeDefinitionReference, int?>) (me => me.ComputeMaxLength());
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, bool?> ComputeIsUnicodeFunc = (Func<CsdlSemanticsTypeDefinitionReference, bool?>) (me => me.ComputeIsUnicode());
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputePrecisionFunc = (Func<CsdlSemanticsTypeDefinitionReference, int?>) (me => me.ComputePrecision());
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeScaleFunc = (Func<CsdlSemanticsTypeDefinitionReference, int?>) (me => me.ComputeScale());
    private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeSridFunc = (Func<CsdlSemanticsTypeDefinitionReference, int?>) (me => me.ComputeSrid());
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, bool> isUnboundedCache = new Cache<CsdlSemanticsTypeDefinitionReference, bool>();
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> maxLengthCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, bool?> isUnicodeCache = new Cache<CsdlSemanticsTypeDefinitionReference, bool?>();
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> precisionCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> scaleCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
    private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> sridCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();

    public CsdlSemanticsTypeDefinitionReference(
      CsdlSemanticsSchema schema,
      CsdlNamedTypeReference reference)
      : base(schema, reference)
    {
    }

    public bool IsUnbounded => this.isUnboundedCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputeIsUnboundedFunc, (Func<CsdlSemanticsTypeDefinitionReference, bool>) null);

    public int? MaxLength => this.maxLengthCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputeMaxLengthFunc, (Func<CsdlSemanticsTypeDefinitionReference, int?>) null);

    public bool? IsUnicode => this.isUnicodeCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputeIsUnicodeFunc, (Func<CsdlSemanticsTypeDefinitionReference, bool?>) null);

    public int? Precision => this.precisionCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputePrecisionFunc, (Func<CsdlSemanticsTypeDefinitionReference, int?>) null);

    public int? Scale => this.scaleCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputeScaleFunc, (Func<CsdlSemanticsTypeDefinitionReference, int?>) null);

    public int? SpatialReferenceIdentifier => this.sridCache.GetValue(this, CsdlSemanticsTypeDefinitionReference.ComputeSridFunc, (Func<CsdlSemanticsTypeDefinitionReference, int?>) null);

    private CsdlNamedTypeReference Reference => (CsdlNamedTypeReference) this.Element;

    private bool ComputeIsUnbounded() => this.UnderlyingType().CanSpecifyMaxLength() && this.Reference.IsUnbounded;

    private int? ComputeMaxLength() => !this.UnderlyingType().CanSpecifyMaxLength() ? new int?() : this.Reference.MaxLength;

    private bool? ComputeIsUnicode() => !this.UnderlyingType().IsString() ? new bool?() : this.Reference.IsUnicode;

    private int? ComputePrecision()
    {
      if (this.UnderlyingType().IsDecimal())
        return this.Reference.Precision;
      return this.UnderlyingType().IsTemporal() ? new int?(this.Reference.Precision ?? 0) : new int?();
    }

    private int? ComputeScale() => !this.UnderlyingType().IsDecimal() ? new int?() : this.Reference.Scale;

    private int? ComputeSrid()
    {
      if (this.UnderlyingType().IsGeography())
        return this.DefaultSridIfUnspecified(4326);
      return this.UnderlyingType().IsGeometry() ? this.DefaultSridIfUnspecified(0) : new int?();
    }

    private int? DefaultSridIfUnspecified(int defaultSrid)
    {
      int? referenceIdentifier = this.Reference.SpatialReferenceIdentifier;
      int minValue = int.MinValue;
      return !(referenceIdentifier.GetValueOrDefault() == minValue & referenceIdentifier.HasValue) ? this.Reference.SpatialReferenceIdentifier : new int?(defaultSrid);
    }
  }
}
