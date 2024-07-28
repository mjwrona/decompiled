// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.SecurityIdentifierInfo
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SecurityIdentifierInfo
  {
    private byte[] m_binarySid;
    private byte[] m_domainBinarySid;
    private readonly SecurityIdentifier m_securityId;

    public SecurityIdentifierInfo(SecurityIdentifier securityId) => this.m_securityId = securityId ?? throw new ArgumentNullException(nameof (securityId));

    public SecurityIdentifier SecurityId => this.m_securityId;

    public byte[] GetBinaryForm()
    {
      if (this.m_binarySid == null)
      {
        byte[] binaryForm = new byte[this.m_securityId.BinaryLength];
        this.m_securityId.GetBinaryForm(binaryForm, 0);
        this.m_binarySid = binaryForm;
      }
      return this.m_binarySid;
    }

    public byte[] GetAccountDomainBinaryForm()
    {
      if (this.m_domainBinarySid == null && this.m_securityId.AccountDomainSid != (SecurityIdentifier) null)
      {
        byte[] binaryForm = new byte[this.m_securityId.AccountDomainSid.BinaryLength];
        this.m_securityId.AccountDomainSid.GetBinaryForm(binaryForm, 0);
        this.m_domainBinarySid = binaryForm;
      }
      return this.m_domainBinarySid;
    }
  }
}
