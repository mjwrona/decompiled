// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ClientToolReleaseConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class ClientToolReleaseConstants
  {
    public const string ArtifactToolName = "ArtifactTool";

    public static class Registry
    {
      public const string Root = "/Configuration/ClientToolReleases";
      public static readonly string SettingsRoot = "/Configuration/ClientToolReleases/Settings";
      public static readonly string SettingsWildcard = ClientToolReleaseConstants.Registry.SettingsRoot + "/*";
      public static readonly string StorageAccountKey = ClientToolReleaseConstants.Registry.SettingsRoot + "/StorageAccount";
      public static readonly string StorageContainerKey = ClientToolReleaseConstants.Registry.SettingsRoot + "/StorageContainer";
      public const string CurrentKeyName = "Current";
      public const string PreviousKeyName = "Previous";
    }
  }
}
