// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseFilter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class LicenseFilter : IEquatable<LicenseFilter>
  {
    public string Name { get; set; }

    public AccountUserStatus? Status { get; set; }

    public LicenseFilter()
    {
    }

    public LicenseFilter(string name, AccountUserStatus? status)
    {
      this.Name = name;
      this.Status = status;
    }

    public override bool Equals(object obj)
    {
      LicenseFilter other = obj as LicenseFilter;
      return obj != null && other != null && this.Equals(other);
    }

    public bool Equals(LicenseFilter other)
    {
      if (other == null || !string.Equals(this.Name, other.Name))
        return false;
      AccountUserStatus? status1 = this.Status;
      AccountUserStatus? status2 = other.Status;
      return status1.GetValueOrDefault() == status2.GetValueOrDefault() & status1.HasValue == status2.HasValue;
    }

    public override int GetHashCode() => 23 * (this.Name == null ? 0 : this.Name.GetHashCode()) + (!this.Status.HasValue ? 0 : this.Status.GetHashCode());

    public string ToQueryString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      if (this.Status.HasValue)
        stringBuilder1.Append("(");
      stringBuilder1.AppendFormat("{0} eq '{1}'", (object) "licenseId", (object) ParsingUtilities.EscapeSingleQuote(this.Name));
      AccountUserStatus? status = this.Status;
      if (status.HasValue)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        status = this.Status;
        string str = status.Value.ToString();
        stringBuilder2.AppendFormat(" and {0} eq '{1}'", (object) "licenseStatus", (object) str).Append(")");
      }
      return stringBuilder1.ToString();
    }
  }
}
