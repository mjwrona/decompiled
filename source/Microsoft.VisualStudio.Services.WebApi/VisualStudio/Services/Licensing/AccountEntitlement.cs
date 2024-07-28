// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountEntitlement
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DataContract]
  public class AccountEntitlement : IEquatable<AccountEntitlement>
  {
    [DataMember]
    private License license { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid AccountId { get; internal set; }

    [DataMember]
    public Guid UserId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef User { get; internal set; }

    public License License
    {
      get
      {
        License license = this.license;
        return (object) license != null ? license : License.None;
      }
      set => this.license = value;
    }

    [DataMember]
    public AssignmentSource AssignmentSource { get; set; }

    [DataMember]
    public LicensingOrigin Origin { get; set; }

    [DataMember(Name = "status")]
    public AccountUserStatus UserStatus { get; set; }

    [DataMember]
    public DateTimeOffset AssignmentDate { get; set; }

    [DataMember]
    public DateTimeOffset LastAccessedDate { get; set; }

    [DataMember]
    public DateTimeOffset DateCreated { get; set; }

    [DataMember]
    public DateTimeOffset LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AccountRights Rights { get; set; }

    public static bool operator ==(AccountEntitlement left, AccountEntitlement right) => object.Equals((object) left, (object) right);

    public static bool operator !=(AccountEntitlement left, AccountEntitlement right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj) => this.Equals(obj as AccountEntitlement);

    public bool Equals(AccountEntitlement other)
    {
      if (other != (AccountEntitlement) null && this.UserId.Equals(other.UserId) && this.License.Equals(other.License) && this.AssignmentSource == other.AssignmentSource && this.Origin == other.Origin && this.UserStatus == other.UserStatus)
      {
        DateTimeOffset dateTimeOffset = this.AssignmentDate;
        if (dateTimeOffset.Equals(other.AssignmentDate))
        {
          dateTimeOffset = this.LastAccessedDate;
          if (dateTimeOffset.Equals(other.LastAccessedDate))
          {
            dateTimeOffset = this.DateCreated;
            return dateTimeOffset.Equals(other.DateCreated);
          }
        }
      }
      return false;
    }

    public override int GetHashCode() => ((((((((((-508375918 * -1521134295 + EqualityComparer<License>.Default.GetHashCode(this.license)) * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(this.UserId)) * -1521134295 + EqualityComparer<IdentityRef>.Default.GetHashCode(this.User)) * -1521134295 + EqualityComparer<License>.Default.GetHashCode(this.License)) * -1521134295 + this.AssignmentSource.GetHashCode()) * -1521134295 + this.Origin.GetHashCode()) * -1521134295 + this.UserStatus.GetHashCode()) * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(this.AssignmentDate)) * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(this.LastAccessedDate)) * -1521134295 + EqualityComparer<AccountRights>.Default.GetHashCode(this.Rights)) * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(this.DateCreated);
  }
}
