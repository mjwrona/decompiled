// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionManagementConstants
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public static class ExtensionManagementConstants
  {
    internal static readonly string ExtensionManagementSettingsRoot = "/Service/ExtensionManagement/Settings";
    internal static readonly string ForceVersionCheckSpan = "/Service/ExtensionManagement/Settings/ForceVersionCheckSpan";
    internal static readonly string PerformVersionCheckDelay = "/Service/ExtensionManagement/Settings/PerformVersionCheckDelay";
    internal static readonly string ProcessExtensionStateJobDelay = "/Service/ExtensionManagement/Settings/ProcessExtensionStateJobDelay";
    internal static readonly string MaxVersionCheckDelay = "/Service/ExtensionManagement/Settings/MaxVersionCheckDelay";
    internal static readonly string ExtensionMessageBusDelay = "/Service/ExtensionManagement/Settings/MessageBusTaskDelayInSeconds";
    internal static readonly string DirectUpdateExtensionMessageBusDelay = "/Service/ExtensionManagement/Settings/DirectUpdateExtensionMessageBusDelay";
    internal static readonly string ExtensionStateDeletionDelay = "/Service/ExtensionManagement/Settings/StateDeletionTaskDelayInSeconds";
    internal static readonly string BypassGalleryDeletionCheck = "/Service/ExtensionManagement/Settings/BypassGalleryDeletionCheck";
    internal static readonly string PublishEventRetryJobDelay = "/Service/ExtensionManagement/Settings/PublishEventRetryJobDelay";
    internal static readonly string ExtensionDataMaxDocumentSize = "/Service/ExtensionData/Settings/ExtensionDataMaxDocumetnSize";
    internal static readonly string ExtensionsTelemetryMaxBatchSize = "/Service/ExtensionManagement/Settings/ExtensionsTelemetryMaxBatchSize";
  }
}
