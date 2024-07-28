// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.Account
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Account
{
  [DataContract]
  public sealed class Account
  {
    private Account()
    {
    }

    public Account(Guid accountId)
      : this()
    {
      this.AccountId = accountId;
      this.Properties = new PropertiesCollection();
    }

    [DataMember]
    public Guid AccountId { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid NamespaceId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri AccountUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AccountName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OrganizationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AccountType AccountType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid AccountOwner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AccountStatus AccountStatus { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusReason { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid LastUpdatedBy { get; internal set; }

    [IgnoreDataMember]
    public byte[] Revision { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool HasMoved => this.NewCollectionId != Guid.Empty;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid NewCollectionId => this.NamespaceId;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdatedDate { get; internal set; }

    public object GetProperty(string name) => this.Properties != null ? this.Properties[name] : (object) null;

    public bool TryGetProperty(string name, out object value)
    {
      value = (object) null;
      return this.Properties != null && this.Properties.TryGetValue(name, out value);
    }

    public Microsoft.VisualStudio.Services.Account.Account Clone()
    {
      Microsoft.VisualStudio.Services.Account.Account account = new Microsoft.VisualStudio.Services.Account.Account(this.AccountId);
      account.NamespaceId = this.NamespaceId;
      account.AccountUri = this.AccountUri;
      account.AccountName = this.AccountName;
      account.OrganizationName = this.OrganizationName;
      account.AccountType = this.AccountType;
      account.AccountOwner = this.AccountOwner;
      account.CreatedBy = this.CreatedBy;
      account.CreatedDate = this.CreatedDate;
      account.AccountStatus = this.AccountStatus;
      account.StatusReason = this.StatusReason;
      account.LastUpdatedBy = this.LastUpdatedBy;
      account.LastUpdatedDate = this.LastUpdatedDate;
      account.Revision = this.Revision;
      if (this.Properties != null)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) this.Properties)
          account.Properties[property.Key] = property.Value;
      }
      return account;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Account {0} (Organization: {1}; Type: {2}; Owner: {3}; AccountStatus: {4}; CreatedBy: {5})", (object) this.AccountId, (object) this.OrganizationName, (object) this.AccountType, (object) this.AccountOwner, (object) this.AccountStatus, (object) this.CreatedBy);
  }
}
