// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlLoginInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class SqlLoginInfo : IEquatable<SqlLoginInfo>
  {
    public SqlLoginInfo(
      string loginName,
      byte[] sid,
      bool hasAccess,
      bool isEnabled,
      bool isSysAdmin,
      bool isNTUser,
      bool isNTGroup,
      DateTime createDate,
      DateTime modifyDate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      ArgumentUtility.CheckForNull<byte[]>(sid, nameof (sid));
      this.LoginName = loginName;
      this.Sid = sid;
      this.HasAccess = hasAccess;
      this.IsEnabled = isEnabled;
      this.IsSysAdmin = isSysAdmin;
      this.IsNTUser = isNTUser;
      this.IsNTGroup = this.IsNTGroup;
      this.CreateDate = createDate;
      this.ModifyDate = modifyDate;
    }

    public string LoginName { get; private set; }

    public byte[] Sid { get; private set; }

    public bool HasAccess { get; private set; }

    public bool IsEnabled { get; private set; }

    public bool IsSysAdmin { get; private set; }

    public bool IsNTUser { get; private set; }

    public bool IsNTGroup { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public bool IsNTName => this.IsNTUser || this.IsNTGroup;

    public SecurityIdentifier SecurityIdentifier => this.Sid.Length > 1 && this.Sid[0] == (byte) 1 && this.IsNTName ? new SecurityIdentifier(this.Sid, 0) : (SecurityIdentifier) null;

    public bool IsNetworkService
    {
      get
      {
        SecurityIdentifier securityIdentifier = this.SecurityIdentifier;
        return securityIdentifier != (SecurityIdentifier) null && securityIdentifier.IsWellKnown(WellKnownSidType.NetworkServiceSid);
      }
    }

    public bool IsMachineLogin => this.LoginName.EndsWith("$", StringComparison.OrdinalIgnoreCase);

    public string GetMachineName()
    {
      if (!this.IsMachineLogin)
        throw new InvalidOperationException();
      return this.LoginName.TrimEnd('$').Split('\\')[1];
    }

    public bool Equals(SqlLoginInfo other) => other != null && string.Equals(this.LoginName, other.LoginName, StringComparison.OrdinalIgnoreCase) && ArrayUtility.Equals(this.Sid, other.Sid) && this.HasAccess == other.HasAccess && this.IsEnabled == other.IsEnabled && this.IsSysAdmin == other.IsSysAdmin && this.IsNTUser == other.IsNTUser && this.IsNTGroup == other.IsNTGroup;

    public override bool Equals(object obj) => this.Equals(obj as SqlLoginInfo);

    public override int GetHashCode() => this.LoginName.GetHashCode() ^ ArrayUtility.GetHashCode(this.Sid);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Name: {0}, Sid: {1}, HasAccess: {2}, IsEnabled: {3}, IsSysAdmin: {4}", (object) this.LoginName, (object) this.SecurityIdentifier, (object) this.HasAccess, (object) this.IsEnabled, (object) this.IsSysAdmin);
  }
}
