// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.CustomerIntelligenceActions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public static class CustomerIntelligenceActions
  {
    public static readonly string Install = "InstallExtension";
    public static readonly string Update = "UpdateExtension";
    public static readonly string Uninstall = "UninstallExtension";
    public static readonly string InstallError = nameof (InstallError);

    public static class ExtensionProperties
    {
      public static readonly string IsMarketplaceExtension = nameof (IsMarketplaceExtension);
      public static readonly string IsPaidExtension = "IsPaid";
    }
  }
}
