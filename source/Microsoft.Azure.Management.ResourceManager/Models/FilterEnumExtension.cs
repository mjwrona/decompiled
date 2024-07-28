// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.FilterEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class FilterEnumExtension
  {
    internal static string ToSerializedValue(this Filter? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this Filter value) => value == Filter.AtScope ? "atScope()" : (string) null;

    internal static Filter? ParseFilter(this string value)
    {
      switch (value)
      {
        case "atScope()":
          return new Filter?(Filter.AtScope);
        default:
          return new Filter?();
      }
    }
  }
}
