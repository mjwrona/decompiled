// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ExtensionAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ExtensionAuditConstants
  {
    private static readonly string ExtensionArea = "Extension.";
    public static readonly string Disabled = ExtensionAuditConstants.ExtensionArea + nameof (Disabled);
    public static readonly string Enabled = ExtensionAuditConstants.ExtensionArea + nameof (Enabled);
    public static readonly string Installed = ExtensionAuditConstants.ExtensionArea + nameof (Installed);
    public static readonly string Uninstalled = ExtensionAuditConstants.ExtensionArea + nameof (Uninstalled);
    public static readonly string VersionUpdated = ExtensionAuditConstants.ExtensionArea + nameof (VersionUpdated);
    public static readonly string PublisherName = nameof (PublisherName);
    public static readonly string ExtensionName = nameof (ExtensionName);
    public static readonly string Version = nameof (Version);
    public static readonly string FromVersion = nameof (FromVersion);
  }
}
