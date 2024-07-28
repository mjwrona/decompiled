// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.TokenAuditEventName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class TokenAuditEventName
  {
    public static class Pat
    {
      public const string Create = "Token.PatCreateEvent";
      public const string Update = "Token.PatUpdateEvent";
      public const string Revoke = "Token.PatRevokeEvent";
      public const string Expire = "Token.PatExpiredEvent";
      public const string SystemRevoke = "Token.PatSystemRevokeEvent";
      public const string PatPublicDiscovery = "Token.PatPublicDiscoveryEvent";
    }

    public static class Ssh
    {
      public const string Create = "Token.SshCreateEvent";
      public const string Update = "Token.SshUpdateEvent";
      public const string Revoke = "Token.SshRevokeEvent";
      public const string Expire = "Token.SshExpireEvent";
    }
  }
}
