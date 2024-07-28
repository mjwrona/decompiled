// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataDeltaResourceSetSerializationInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public sealed class ODataDeltaResourceSetSerializationInfo
  {
    private string entitySetName;
    private string entityTypeName;
    private string expectedEntityTypeName;

    public string EntitySetName
    {
      get => this.entitySetName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, nameof (EntitySetName));
        this.entitySetName = value;
      }
    }

    public string EntityTypeName
    {
      get => this.entityTypeName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, nameof (EntityTypeName));
        this.entityTypeName = value;
      }
    }

    public string ExpectedTypeName
    {
      get => this.expectedEntityTypeName ?? this.EntityTypeName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotEmpty(value, nameof (ExpectedTypeName));
        this.expectedEntityTypeName = value;
      }
    }

    internal static ODataDeltaResourceSetSerializationInfo Validate(
      ODataDeltaResourceSetSerializationInfo serializationInfo)
    {
      if (serializationInfo != null)
      {
        ExceptionUtils.CheckArgumentNotNull<string>(serializationInfo.EntitySetName, "serializationInfo.EntitySetName");
        ExceptionUtils.CheckArgumentNotNull<string>(serializationInfo.EntityTypeName, "serializationInfo.EntityTypeName");
      }
      return serializationInfo;
    }
  }
}
