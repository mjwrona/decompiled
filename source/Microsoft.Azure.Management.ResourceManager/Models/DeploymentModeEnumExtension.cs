// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentModeEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class DeploymentModeEnumExtension
  {
    internal static string ToSerializedValue(this DeploymentMode? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this DeploymentMode value)
    {
      switch (value)
      {
        case DeploymentMode.Incremental:
          return "Incremental";
        case DeploymentMode.Complete:
          return "Complete";
        default:
          return (string) null;
      }
    }

    internal static DeploymentMode? ParseDeploymentMode(this string value)
    {
      switch (value)
      {
        case "Incremental":
          return new DeploymentMode?(DeploymentMode.Incremental);
        case "Complete":
          return new DeploymentMode?(DeploymentMode.Complete);
        default:
          return new DeploymentMode?();
      }
    }
  }
}
