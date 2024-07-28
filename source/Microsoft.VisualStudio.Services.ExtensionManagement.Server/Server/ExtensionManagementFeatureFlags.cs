// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionManagementFeatureFlags
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [GenerateAllConstants(null)]
  public static class ExtensionManagementFeatureFlags
  {
    public static readonly string CheckHostAuthorization = "VisualStudio.Services.ExtensionManagement.CheckHostAuthorization";
    public static readonly string AsyncVersionChecks = "VisualStudio.Services.ExtensionManagement.AsyncVersionChecks";
    public static readonly string UseJobForAsyncStateProcessing = "VisualStudio.Services.ExtensionManagement.UseJobForAsyncStateProcessing";
    public static readonly string UseJobForInstallEvents = "VisualStudio.Services.ExtensionManagement.UseJobForInstallEvents";
    public static readonly string RefreshBuiltInExtensionIfNeededOnAllRequests = "VisualStudio.Services.ExtensionManagement.RefreshBuiltInExtensionIfNeededOnAllRequests";
    public static readonly string DisableVSSReadConsistencyLevelExtensionUpdateFiltering = "VisualStudio.Services.ExtensionManagement.DisableVSSReadConsistencyLevelExtensionUpdateFiltering";
    public static readonly string EnableDocumentsResponseStreaming = "VisualStudio.Services.ExtensionManagement.EnableDocumentsResponseStreaming";
  }
}
