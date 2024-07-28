// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataObjectModelExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  public static class ODataObjectModelExtensions
  {
    public static void SetSerializationInfo(
      this ODataResourceSet resourceSet,
      ODataResourceSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResourceSet>(resourceSet, nameof (resourceSet));
      resourceSet.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataResourceBase resource,
      ODataResourceSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResourceBase>(resource, nameof (resource));
      resource.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataResource resource,
      ODataResourceSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataResource>(resource, nameof (resource));
      resource.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataNestedResourceInfo nestedResourceInfo,
      ODataNestedResourceInfoSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataNestedResourceInfo>(nestedResourceInfo, "resource");
      nestedResourceInfo.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataProperty property,
      ODataPropertySerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataProperty>(property, nameof (property));
      property.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataCollectionStart collectionStart,
      ODataCollectionStartSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataCollectionStart>(collectionStart, nameof (collectionStart));
      collectionStart.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataDeltaResourceSet deltaResourceSet,
      ODataDeltaResourceSetSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaResourceSet>(deltaResourceSet, nameof (deltaResourceSet));
      ODataResourceSerializationInfo serializationInfo1 = new ODataResourceSerializationInfo()
      {
        NavigationSourceName = serializationInfo.EntitySetName,
        NavigationSourceEntityTypeName = serializationInfo.EntityTypeName,
        NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
        ExpectedTypeName = serializationInfo.ExpectedTypeName
      };
      deltaResourceSet.SerializationInfo = serializationInfo1;
    }

    public static void SetSerializationInfo(
      this ODataDeltaResourceSet deltaResourceSet,
      ODataResourceSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaResourceSet>(deltaResourceSet, nameof (deltaResourceSet));
      deltaResourceSet.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataDeltaDeletedEntry deltaDeletedEntry,
      ODataResourceSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaDeletedEntry>(deltaDeletedEntry, nameof (deltaDeletedEntry));
      ODataDeltaSerializationInfo serializationInfo1 = new ODataDeltaSerializationInfo()
      {
        NavigationSourceName = serializationInfo.NavigationSourceName
      };
      deltaDeletedEntry.SerializationInfo = serializationInfo1;
    }

    public static void SetSerializationInfo(
      this ODataDeltaDeletedEntry deltaDeletedEntry,
      ODataDeltaSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaDeletedEntry>(deltaDeletedEntry, nameof (deltaDeletedEntry));
      deltaDeletedEntry.SerializationInfo = serializationInfo;
    }

    public static void SetSerializationInfo(
      this ODataDeltaLinkBase deltalink,
      ODataDeltaSerializationInfo serializationInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataDeltaLinkBase>(deltalink, nameof (deltalink));
      deltalink.SerializationInfo = serializationInfo;
    }
  }
}
