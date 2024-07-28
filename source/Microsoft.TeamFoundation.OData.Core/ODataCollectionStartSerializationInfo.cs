// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataCollectionStartSerializationInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public sealed class ODataCollectionStartSerializationInfo
  {
    private string collectionTypeName;

    public string CollectionTypeName
    {
      get => this.collectionTypeName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, nameof (CollectionTypeName));
        ValidationUtils.ValidateCollectionTypeName(value);
        this.collectionTypeName = value;
      }
    }

    internal static ODataCollectionStartSerializationInfo Validate(
      ODataCollectionStartSerializationInfo serializationInfo)
    {
      if (serializationInfo != null)
        ExceptionUtils.CheckArgumentNotNull<string>(serializationInfo.CollectionTypeName, "serializationInfo.CollectionTypeName");
      return serializationInfo;
    }
  }
}
