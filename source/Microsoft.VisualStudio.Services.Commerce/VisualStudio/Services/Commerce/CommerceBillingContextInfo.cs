// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceBillingContextInfo
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceBillingContextInfo : IEquatable<CommerceBillingContextInfo>
  {
    public Guid? TenantId { get; set; }

    public Guid? ObjectId { get; set; }

    public string AdministratorPuid { get; set; }

    public string AdministratorEmail { get; set; }

    public CommerceBillingSystemType BillingSystemType { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as CommerceBillingContextInfo);

    public override int GetHashCode()
    {
      Guid? nullable = this.TenantId;
      int hashCode1 = nullable.GetHashCode();
      nullable = this.ObjectId;
      int hashCode2 = nullable.GetHashCode();
      return hashCode1 ^ hashCode2 ^ (string.IsNullOrEmpty(this.AdministratorPuid) ? 0 : this.AdministratorPuid.GetHashCode()) ^ (string.IsNullOrEmpty(this.AdministratorEmail) ? 0 : this.AdministratorEmail.GetHashCode());
    }

    public bool Equals(CommerceBillingContextInfo other)
    {
      if (other == null)
        return false;
      Guid? tenantId = this.TenantId;
      Guid? nullable = other.TenantId;
      if ((tenantId.HasValue == nullable.HasValue ? (tenantId.HasValue ? (tenantId.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        nullable = this.ObjectId;
        Guid? objectId = other.ObjectId;
        if ((nullable.HasValue == objectId.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == objectId.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.AdministratorEmail == other.AdministratorEmail)
          return this.AdministratorPuid == other.AdministratorPuid;
      }
      return false;
    }
  }
}
