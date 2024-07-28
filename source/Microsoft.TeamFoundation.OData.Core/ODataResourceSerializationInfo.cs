// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceSerializationInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  public sealed class ODataResourceSerializationInfo
  {
    private string navigationSourceName;
    private string navigationSourceEntityTypeName;
    private string expectedTypeName;

    public string NavigationSourceName
    {
      get => this.navigationSourceName;
      set => this.navigationSourceName = value;
    }

    public string NavigationSourceEntityTypeName
    {
      get => this.navigationSourceEntityTypeName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, nameof (NavigationSourceEntityTypeName));
        this.navigationSourceEntityTypeName = value;
      }
    }

    public EdmNavigationSourceKind NavigationSourceKind { get; set; }

    public bool IsFromCollection { get; set; }

    public string ExpectedTypeName
    {
      get => this.expectedTypeName ?? this.NavigationSourceEntityTypeName;
      set
      {
        ExceptionUtils.CheckArgumentStringNotEmpty(value, nameof (ExpectedTypeName));
        this.expectedTypeName = value;
      }
    }

    internal static ODataResourceSerializationInfo Validate(
      ODataResourceSerializationInfo serializationInfo)
    {
      if (serializationInfo != null && serializationInfo.NavigationSourceKind != EdmNavigationSourceKind.None)
      {
        if (serializationInfo.NavigationSourceKind != EdmNavigationSourceKind.UnknownEntitySet)
          ExceptionUtils.CheckArgumentNotNull<string>(serializationInfo.NavigationSourceName, "serializationInfo.NavigationSourceName");
        ExceptionUtils.CheckArgumentNotNull<string>(serializationInfo.NavigationSourceEntityTypeName, "serializationInfo.NavigationSourceEntityTypeName");
      }
      return serializationInfo;
    }
  }
}
