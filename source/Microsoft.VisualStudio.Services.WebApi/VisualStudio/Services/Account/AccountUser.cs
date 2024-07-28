// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountUser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Account
{
  [DataContract]
  public sealed class AccountUser
  {
    private AccountUser()
    {
    }

    public AccountUser(Guid accountId, Guid userId)
      : this()
    {
      this.AccountId = accountId;
      this.UserId = userId;
    }

    [DataMember]
    public Guid AccountId { get; set; }

    [DataMember]
    public Guid UserId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTimeOffset CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AccountUserStatus UserStatus { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AccountLicenseSource LicenseSource { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTimeOffset LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AccountServiceRights ServiceRights { get; set; }

    public AccountUser Clone() => new AccountUser(this.AccountId, this.UserId)
    {
      AccountId = this.AccountId,
      UserId = this.UserId,
      CreationDate = this.CreationDate,
      LastUpdated = this.LastUpdated,
      LicenseSource = this.LicenseSource,
      UserStatus = this.UserStatus,
      ServiceRights = this.ServiceRights
    };

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AccountUser: AccountId {0}, UserId {1} (UserStatus: {2}; LicenseSource: {3}; ServiceRights: {4})", (object) this.AccountId, (object) this.UserId, (object) this.UserStatus, (object) this.LicenseSource, (object) this.ServiceRights);
  }
}
