// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.DeviceJoinInfomation
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  public sealed class DeviceJoinInfomation
  {
    internal DeviceJoinInfomation(ref NativeMethods.DSREG_JOIN_INFO joinInfo)
    {
      this.JoinType = joinInfo.joinType;
      this.DeviceId = joinInfo.DeviceId;
      this.IdpDomain = joinInfo.IdpDomain;
      this.TenantId = joinInfo.TenantId;
      this.JoinUserEmail = joinInfo.JoinUserEmail;
      this.TenantDisplayName = joinInfo.TenantDisplayName;
      this.MdmEnrollmentUrl = joinInfo.MdmEnrollmentUrl;
      this.MdmTermsOfUseUrl = joinInfo.MdmTermsOfUseUrl;
      this.MdmComplianceUrl = joinInfo.MdmComplianceUrl;
      this.UserSettingSyncUrl = string.IsNullOrEmpty(joinInfo.UserSettingSyncUrl) ? joinInfo.UserSettingSyncUrl : Encoding.ASCII.GetString(Convert.FromBase64String(joinInfo.UserSettingSyncUrl));
      if (!(joinInfo.pUserInfo != IntPtr.Zero))
        return;
      NativeMethods.DSREG_USER_INFO structure = Marshal.PtrToStructure<NativeMethods.DSREG_USER_INFO>(joinInfo.pUserInfo);
      this.UserEmail = structure.UserEmail;
      this.UserKeyId = structure.UserKeyId;
      this.UserKeyName = structure.UserKeyName;
    }

    public DeviceJoinType JoinType { get; }

    public string DeviceId { get; }

    public string IdpDomain { get; }

    public string TenantId { get; }

    public string JoinUserEmail { get; }

    public string TenantDisplayName { get; }

    public string MdmEnrollmentUrl { get; }

    public string MdmTermsOfUseUrl { get; }

    public string MdmComplianceUrl { get; }

    public string UserSettingSyncUrl { get; }

    public string UserEmail { get; }

    public string UserKeyId { get; }

    public string UserKeyName { get; }

    public override string ToString() => string.Format("JoinType: {0}\r\nDeviceId: {1}\r\nIdpDomain: {2}\r\nTenantId: {3}\r\nJoinUserEmail: {4}\r\nTenantDisplayName: {5}\r\nMdmEnrollmentUrl: {6}\r\nMdmTermsOfUseUrl: {7}\r\nMdmComplianceUrl: {8}\r\nUserSettingSyncUrl: {9}\r\nUserEmail: {10}\r\nUserKeyId: {11}\r\nUserKeyName: {12}", (object) this.JoinType, (object) this.DeviceId, (object) this.IdpDomain, (object) this.TenantId, (object) this.JoinUserEmail, (object) this.TenantDisplayName, (object) this.MdmEnrollmentUrl, (object) this.MdmTermsOfUseUrl, (object) this.MdmComplianceUrl, (object) this.UserSettingSyncUrl, (object) this.UserEmail, (object) this.UserKeyId, (object) this.UserKeyId);
  }
}
