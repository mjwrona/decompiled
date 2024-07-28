// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ResourceIdentityTypeEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class ResourceIdentityTypeEnumExtension
  {
    internal static string ToSerializedValue(this ResourceIdentityType? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this ResourceIdentityType value)
    {
      switch (value)
      {
        case ResourceIdentityType.SystemAssigned:
          return "SystemAssigned";
        case ResourceIdentityType.UserAssigned:
          return "UserAssigned";
        case ResourceIdentityType.SystemAssignedUserAssigned:
          return "SystemAssigned, UserAssigned";
        case ResourceIdentityType.None:
          return "None";
        default:
          return (string) null;
      }
    }

    internal static ResourceIdentityType? ParseResourceIdentityType(this string value)
    {
      switch (value)
      {
        case "SystemAssigned":
          return new ResourceIdentityType?(ResourceIdentityType.SystemAssigned);
        case "UserAssigned":
          return new ResourceIdentityType?(ResourceIdentityType.UserAssigned);
        case "SystemAssigned, UserAssigned":
          return new ResourceIdentityType?(ResourceIdentityType.SystemAssignedUserAssigned);
        case "None":
          return new ResourceIdentityType?(ResourceIdentityType.None);
        default:
          return new ResourceIdentityType?();
      }
    }
  }
}
