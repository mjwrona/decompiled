// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ChangeTypeEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class ChangeTypeEnumExtension
  {
    internal static string ToSerializedValue(this ChangeType? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this ChangeType value)
    {
      switch (value)
      {
        case ChangeType.Create:
          return "Create";
        case ChangeType.Delete:
          return "Delete";
        case ChangeType.Ignore:
          return "Ignore";
        case ChangeType.Deploy:
          return "Deploy";
        case ChangeType.NoChange:
          return "NoChange";
        case ChangeType.Modify:
          return "Modify";
        default:
          return (string) null;
      }
    }

    internal static ChangeType? ParseChangeType(this string value)
    {
      switch (value)
      {
        case "Create":
          return new ChangeType?(ChangeType.Create);
        case "Delete":
          return new ChangeType?(ChangeType.Delete);
        case "Ignore":
          return new ChangeType?(ChangeType.Ignore);
        case "Deploy":
          return new ChangeType?(ChangeType.Deploy);
        case "NoChange":
          return new ChangeType?(ChangeType.NoChange);
        case "Modify":
          return new ChangeType?(ChangeType.Modify);
        default:
          return new ChangeType?();
      }
    }
  }
}
