// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceSetBase
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  public abstract class ODataResourceSetBase : ODataItem
  {
    private Uri nextPageLink;
    private Uri deltaLink;
    private ODataResourceSerializationInfo serializationInfo;
    private string typeName;

    public string TypeName
    {
      get
      {
        if (this.typeName == null && this.SerializationInfo != null && this.SerializationInfo.ExpectedTypeName != null)
          this.typeName = EdmLibraryExtensions.GetCollectionTypeName(this.SerializationInfo.ExpectedTypeName);
        return this.typeName;
      }
      set => this.typeName = value;
    }

    public long? Count { get; set; }

    public Uri Id { get; set; }

    public Uri NextPageLink
    {
      get => this.nextPageLink;
      set => this.nextPageLink = !(this.DeltaLink != (Uri) null) || !(value != (Uri) null) ? value : throw new ODataException(Strings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
    }

    public Uri DeltaLink
    {
      get => this.deltaLink;
      set => this.deltaLink = !(this.NextPageLink != (Uri) null) || !(value != (Uri) null) ? value : throw new ODataException(Strings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
    }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
    public ICollection<ODataInstanceAnnotation> InstanceAnnotations
    {
      get => this.GetInstanceAnnotations();
      set => this.SetInstanceAnnotations(value);
    }

    internal ODataResourceSerializationInfo SerializationInfo
    {
      get => this.serializationInfo;
      set => this.serializationInfo = ODataResourceSerializationInfo.Validate(value);
    }
  }
}
