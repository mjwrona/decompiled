// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.TokenAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class TokenAuditConstants
  {
    public const string RevokeEvent = "RevokeEvent";
    public const string PatCreateEvent = "PatCreateEvent";
    public const string PatUpdateEvent = "PatUpdateEvent";
    public const string PatRevokeEvent = "PatRevokeEvent";
    public const string PatExpiredEvent = "PatExpiredEvent";
    public const string PatExpiringEvent = "PatExpiringEvent";
    public const string PatAccessSuccessEvent = "PatAccessSuccessEvent";
    public const string PatAccessExpiredEvent = "PatAccessExpiredEvent";
    public const string PatAccessInvalidEvent = "PatAccessInvalidEvent";
    public const string PatSystemRevokeEvent = "PatSystemRevokeEvent";
    public const string PatPublicDiscoveryEvent = "PatPublicDiscoveryEvent";
    public const string SshCreateEvent = "SshCreateEvent";
    public const string SshUpdateEvent = "SshUpdateEvent";
    public const string SshRevokeEvent = "SshRevokeEvent";
    public const string SshRemoveEvent = "SshRemoveEvent";
    public const string SshExpiredEvent = "SshExpiredEvent";
    public const string SshAccessSuccessEvent = "SshAccessSuccessEvent";
    public const string SshAccessExpiredEvent = "SshAccessExpiredEvent";
    public const string SshAccessInvalidEvent = "SshAccessInvalidEvent";
    public const string HostAuthorizedEvent = "HostAuthorizedEvent";
    public const string HostAuthorizationErrorEvent = "HostAuthorizationErrorEvent";
    public const string HostRevokedEvent = "HostRevokedEvent";
    public const string AppAccessSuccessEvent = "AppAccessSuccessEvent";
    public const string AppAccessErrorEvent = "AppAccessErrorEvent";
    public const string RegistrationUpdatedEvent = "RegistrationUpdatedEvent";
    public const string Area = "Token";
    public const string Separator = ".";
    public const string AuditLogRevokeEvent = "Token.RevokeEvent";
    public const string AuditLogPatAccessExpiredEvent = "Token.PatAccessExpiredEvent";
    public const string AuditLogPatAccessInvalidEvent = "Token.PatAccessInvalidEvent";
    public const string AuditLogSshRemoveEvent = "Token.SshRemoveEvent";
    public const string AuditLogSshAccessExpiredEvent = "Token.SshAccessExpiredEvent";
    public const string AuditLogSshAccessInvalidEvent = "Token.SshAccessInvalidEvent";
  }
}
