// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.PropertyChangeTypeEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class PropertyChangeTypeEnumExtension
  {
    internal static string ToSerializedValue(this PropertyChangeType? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this PropertyChangeType value)
    {
      switch (value)
      {
        case PropertyChangeType.Create:
          return "Create";
        case PropertyChangeType.Delete:
          return "Delete";
        case PropertyChangeType.Modify:
          return "Modify";
        case PropertyChangeType.Array:
          return "Array";
        default:
          return (string) null;
      }
    }

    internal static PropertyChangeType? ParsePropertyChangeType(this string value)
    {
      switch (value)
      {
        case "Create":
          return new PropertyChangeType?(PropertyChangeType.Create);
        case "Delete":
          return new PropertyChangeType?(PropertyChangeType.Delete);
        case "Modify":
          return new PropertyChangeType?(PropertyChangeType.Modify);
        case "Array":
          return new PropertyChangeType?(PropertyChangeType.Array);
        default:
          return new PropertyChangeType?();
      }
    }
  }
}
