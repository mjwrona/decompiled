// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AuthenticationAuditEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [DataContract]
  public class AuthenticationAuditEvent
  {
    public const string SignInEventName = "User.SignInEvent";
    public const string PatAccessEventName = "User.PatAccessEvent";
    public const string SshAccessEventName = "User.SshAccessEvent";

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public string Reason { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public string Action { get; set; }

    public static AuthenticationAuditEvent Create(
      IVssRequestContext requestContext,
      string status,
      string reason,
      string action)
    {
      requestContext.GetUserIdentity();
      return new AuthenticationAuditEvent()
      {
        Status = status,
        Reason = reason,
        Uri = requestContext.RequestUri().ToString(),
        Action = action
      };
    }
  }
}
