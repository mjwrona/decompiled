// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataStreamPropertyInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System;

namespace Microsoft.OData
{
  public sealed class ODataStreamPropertyInfo : ODataPropertyInfo, IODataStreamReferenceInfo
  {
    private string edmPropertyName;
    private ODataResourceMetadataBuilder metadataBuilder;
    private Uri editLink;
    private Uri computedEditLink;
    private Uri readLink;
    private Uri computedReadLink;
    private EdmPrimitiveTypeKind primitiveTypeKind;

    public Uri EditLink
    {
      get
      {
        if (this.HasNonComputedEditLink)
          return this.editLink;
        Uri computedEditLink = this.computedEditLink;
        if ((object) computedEditLink != null)
          return computedEditLink;
        return this.metadataBuilder != null ? (this.computedEditLink = this.metadataBuilder.GetStreamEditLink(this.edmPropertyName)) : (Uri) null;
      }
      set
      {
        this.editLink = value;
        this.HasNonComputedEditLink = true;
      }
    }

    public Uri ReadLink
    {
      get
      {
        if (this.HasNonComputedReadLink)
          return this.readLink;
        Uri computedReadLink = this.computedReadLink;
        if ((object) computedReadLink != null)
          return computedReadLink;
        return this.metadataBuilder != null ? (this.computedReadLink = this.metadataBuilder.GetStreamReadLink(this.edmPropertyName)) : (Uri) null;
      }
      set
      {
        this.readLink = value;
        this.HasNonComputedReadLink = true;
      }
    }

    public string ContentType { get; set; }

    public string ETag { get; set; }

    public override EdmPrimitiveTypeKind PrimitiveTypeKind
    {
      get => this.primitiveTypeKind;
      set => this.primitiveTypeKind = value == EdmPrimitiveTypeKind.Binary || value == EdmPrimitiveTypeKind.String || value == EdmPrimitiveTypeKind.None ? value : throw new ODataException(Strings.StreamItemInvalidPrimitiveKind((object) value));
    }

    internal bool HasNonComputedEditLink { get; private set; }

    internal bool HasNonComputedReadLink { get; private set; }

    internal void SetMetadataBuilder(ODataResourceMetadataBuilder builder, string propertyName)
    {
      this.metadataBuilder = builder;
      this.edmPropertyName = propertyName;
      this.computedEditLink = (Uri) null;
      this.computedReadLink = (Uri) null;
    }

    internal ODataResourceMetadataBuilder GetMetadataBuilder() => this.metadataBuilder;
  }
}
