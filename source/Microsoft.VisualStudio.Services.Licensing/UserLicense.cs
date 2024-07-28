// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.UserLicense
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class UserLicense : ICloneable, IEquatable<UserLicense>
  {
    [JsonProperty(PropertyName = "accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; }

    [JsonProperty(PropertyName = "source")]
    public LicensingSource Source { get; set; }

    [JsonProperty(PropertyName = "origin")]
    public LicensingOrigin Origin { get; set; }

    [JsonProperty(PropertyName = "license")]
    public int License { get; set; }

    [JsonProperty(PropertyName = "licenseName")]
    public string LicenseName => this.GetLicenseName();

    [JsonProperty(PropertyName = "licenseAndStatusName")]
    public string LicenseAndStatusName => this.GetLicenseAndStatusName();

    [JsonProperty(PropertyName = "assignmentSource")]
    public AssignmentSource AssignmentSource { get; set; }

    [JsonProperty(PropertyName = "status")]
    public AccountUserStatus Status { get; set; }

    [JsonProperty(PropertyName = "assignmentDate")]
    public DateTimeOffset AssignmentDate { get; set; }

    [JsonProperty(PropertyName = "dateCreated")]
    public DateTimeOffset DateCreated { get; set; }

    [JsonProperty(PropertyName = "lastUpdated")]
    public DateTimeOffset LastUpdated { get; set; }

    [JsonProperty(PropertyName = "lastAccessed")]
    public DateTimeOffset LastAccessed { get; set; }

    public UserLicense()
    {
    }

    public UserLicense(
      Guid scopeId,
      Guid userId,
      AssignmentSource assignmentSource,
      int license,
      LicensingOrigin origin,
      LicensingSource source,
      AccountUserStatus status)
    {
      this.AccountId = scopeId;
      this.UserId = userId;
      this.AssignmentDate = DateTimeOffset.UtcNow;
      this.AssignmentSource = assignmentSource != AssignmentSource.None ? assignmentSource : AssignmentSource.Unknown;
      this.DateCreated = DateTimeOffset.UtcNow;
      this.LastAccessed = DateTimeOffset.MinValue;
      this.LastUpdated = DateTimeOffset.UtcNow;
      this.License = license;
      this.Origin = origin;
      this.Source = source;
      this.Status = status;
    }

    public override bool Equals(object obj)
    {
      UserLicense userLicense = obj as UserLicense;
      return obj != null && userLicense != null && this.Equals(userLicense);
    }

    public bool Equals(UserLicense userLicense) => userLicense != null && this.UserId == userLicense.UserId && this.License == userLicense.License && this.Source == userLicense.Source && this.Origin == userLicense.Origin && this.Status == userLicense.Status && this.AssignmentSource == userLicense.AssignmentSource && this.AssignmentDate == userLicense.AssignmentDate && this.LastUpdated == userLicense.LastUpdated && this.DateCreated == userLicense.DateCreated && this.LastAccessed == userLicense.LastAccessed;

    public override int GetHashCode()
    {
      Guid userId = this.UserId;
      int num1 = 23 * (23 * (23 * (23 * (23 * this.UserId.GetHashCode() + this.Source.GetHashCode()) + this.Origin.GetHashCode()) + this.Status.GetHashCode()) + this.AssignmentSource.GetHashCode());
      DateTimeOffset assignmentDate = this.AssignmentDate;
      int hashCode1 = this.AssignmentDate.GetHashCode();
      int num2 = 23 * (num1 + hashCode1);
      DateTimeOffset lastUpdated = this.LastUpdated;
      int hashCode2 = this.LastUpdated.GetHashCode();
      int num3 = 23 * (num2 + hashCode2);
      DateTimeOffset dateCreated = this.DateCreated;
      int hashCode3 = this.DateCreated.GetHashCode();
      int num4 = 23 * (num3 + hashCode3);
      DateTimeOffset lastAccessed = this.LastAccessed;
      int hashCode4 = this.LastAccessed.GetHashCode();
      return num4 + hashCode4;
    }

    public object Clone() => (object) new UserLicense()
    {
      AccountId = this.AccountId,
      UserId = this.UserId,
      Source = this.Source,
      Origin = this.Origin,
      License = this.License,
      AssignmentSource = this.AssignmentSource,
      Status = this.Status,
      AssignmentDate = this.AssignmentDate,
      DateCreated = this.DateCreated,
      LastAccessed = this.LastAccessed,
      LastUpdated = this.LastUpdated
    };

    public UserLicense Transfer(Guid userId) => new UserLicense()
    {
      AccountId = this.AccountId,
      UserId = userId,
      Source = this.Source,
      Origin = this.Origin,
      License = this.License,
      AssignmentSource = this.AssignmentSource,
      Status = this.Status,
      AssignmentDate = this.AssignmentDate,
      DateCreated = this.DateCreated,
      LastAccessed = this.LastAccessed,
      LastUpdated = this.LastUpdated
    };

    public UserLicense ToPreviousUserLicense() => new UserLicense()
    {
      AccountId = this.AccountId,
      UserId = this.UserId,
      Source = this.Source,
      License = this.License,
      Status = this.Status,
      LastUpdated = this.LastUpdated
    };

    private string GetLicenseName() => Microsoft.VisualStudio.Services.Licensing.License.GetLicense(this.Source, this.License).ToString();

    private string GetLicenseAndStatusName() => this.GetLicenseName() + "-" + this.Status.ToString();

    public static implicit operator AccountEntitlement(UserLicense userLicense)
    {
      if (userLicense == null)
        return (AccountEntitlement) null;
      return new AccountEntitlement()
      {
        AccountId = userLicense.AccountId,
        UserId = userLicense.UserId,
        License = Microsoft.VisualStudio.Services.Licensing.License.GetLicense(userLicense.Source, userLicense.License),
        Origin = userLicense.Origin,
        AssignmentSource = userLicense.AssignmentSource,
        AssignmentDate = userLicense.AssignmentDate,
        UserStatus = userLicense.Status,
        LastAccessedDate = userLicense.LastAccessed,
        DateCreated = userLicense.DateCreated,
        LastUpdated = userLicense.LastUpdated
      };
    }

    public static implicit operator AccountUser(UserLicense userLicense)
    {
      if (userLicense == null)
        return (AccountUser) null;
      return new AccountUser(userLicense.AccountId, userLicense.UserId)
      {
        CreationDate = userLicense.DateCreated,
        LastUpdated = userLicense.LastUpdated,
        UserStatus = userLicense.Status
      };
    }
  }
}
