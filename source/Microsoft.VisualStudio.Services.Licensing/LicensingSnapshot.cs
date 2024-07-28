// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingSnapshot
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicensingSnapshot
  {
    public LicensingSnapshot()
    {
    }

    public LicensingSnapshot(
      Guid scopeId,
      List<UserLicense> licenses,
      List<UserLicense> previousLicenses,
      List<UserExtensionLicense> extLicenses,
      int? totalUserLicenseCount = null,
      int? totalPreviousUserLicenseCount = null,
      int? totalExtensionLicenseCount = null)
    {
      this.UserLicenses = licenses;
      this.PreviousUserLicenses = previousLicenses;
      this.UserExtensionLicenses = extLicenses;
      this.ScopeId = scopeId;
      this.TotalUserLicenseCount = totalUserLicenseCount;
      this.TotalPreviousUserLicenseCount = totalPreviousUserLicenseCount;
      this.TotalExtensionLicenseCount = totalExtensionLicenseCount;
    }

    public Guid ScopeId { get; set; }

    public List<UserLicense> UserLicenses { get; set; }

    public List<UserLicense> PreviousUserLicenses { get; set; }

    public List<UserExtensionLicense> UserExtensionLicenses { get; set; }

    public int? TotalUserLicenseCount { get; set; }

    public int? TotalPreviousUserLicenseCount { get; set; }

    public int? TotalExtensionLicenseCount { get; set; }
  }
}
