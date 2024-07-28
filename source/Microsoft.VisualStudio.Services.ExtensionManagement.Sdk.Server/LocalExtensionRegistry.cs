// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.LocalExtensionRegistry
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal static class LocalExtensionRegistry
  {
    public const string Root = "/Configuration/LocalContributions/";
    public const string RootFormat = "/Configuration/LocalContributions/{0}/{1}/";
    public const string AssetRegistryFormatRoot = "/Configuration/LocalContributions/{0}/{1}/Assets/{2}/{3}/";
    public const string AssetRegistryFormatField = "/Configuration/LocalContributions/{0}/{1}/Assets/{2}/{3}/{4}";
    public const string Version = "Version";
    public const string Installed = "Installed";
    public const string Fallback = "Fallback";
    public const string SupportedHosts = "SupportedHosts";
    public const string Assets = "Assets";
    public const string FileId = "FileId";
    public const string ContentType = "ContentType";
    public const string Addressable = "Addressable";
    public const string DefaultLanguage = "_";
    public const int PublisherIndex = 2;
    public const int ExtensionIndex = 3;
    public const int LocaleIndex = 5;
    public const int AssetStartIndex = 6;
    public const int MinPathDepth = 4;
    public const int MinPathDepthAsset = 7;

    public static bool IsDefaultLanguage(string language) => language.Equals("_");
  }
}
