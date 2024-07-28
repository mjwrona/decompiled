// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.VssTokenStorageFactory
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  public static class VssTokenStorageFactory
  {
    private const string c_tokenStorageKeyPostFix = "TokenStorage";
    private const string c_registryRoot = "Software\\Microsoft\\VSCommon\\14.0\\ClientServices";
    private const string RegistryRootPath = "Software\\Microsoft\\VSCommon\\14.0\\ClientServices\\TokenStorage";

    public static VssTokenStorage GetTokenStorageNamespace(string storageNamespace)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(storageNamespace, nameof (storageNamespace));
      return (VssTokenStorage) new RegistryTokenStorage("Software\\Microsoft\\VSCommon\\14.0\\ClientServices\\TokenStorage" + "\\" + storageNamespace);
    }

    internal static void DeleteTokenStorage(string storageNamespace) => RegistryTokenStorage.DeleteTokenStorage("Software\\Microsoft\\VSCommon\\14.0\\ClientServices\\TokenStorage", storageNamespace);
  }
}
