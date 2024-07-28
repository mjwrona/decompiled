// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioExpressRight
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioExpressRight : ClientRightBase
  {
    private readonly string m_name;
    private Dictionary<string, object> m_attributes;
    private static readonly Version s_currentVersion = new Version(1, 0, 0, 0);
    public const string VSWinExpressRightName = "VSWinExpress";
    public const string VWDExpressRightName = "VWDExpress";
    public const string VPDExpressRightName = "VPDExpress";
    public const string WDExpressRightName = "WDExpress";

    public static VisualStudioExpressRight Create(
      string name,
      Version productVersion,
      VisualStudioEdition edition,
      DateTimeOffset expirationDate,
      string licenseSourceName,
      string licenseUrl,
      string licenseDescriptionId,
      string licenseFallbackDescription = null,
      Dictionary<string, object> attributes = null)
    {
      if (attributes == null)
        attributes = new Dictionary<string, object>()
        {
          {
            "IsTrialLicense",
            (object) false
          }
        };
      else if (!attributes.ContainsKey("IsTrialLicense"))
        attributes.Add("IsTrialLicense", (object) false);
      if (!attributes.ContainsKey("SubscriptionLevel"))
        attributes.Add("SubscriptionLevel", (object) "Community");
      VisualStudioExpressRight studioExpressRight = new VisualStudioExpressRight(name);
      studioExpressRight.ClientVersion = productVersion;
      studioExpressRight.AuthorizedVSEdition = edition.ToString();
      studioExpressRight.ExpirationDate = expirationDate;
      studioExpressRight.LicenseDescriptionId = licenseDescriptionId;
      studioExpressRight.LicenseFallbackDescription = licenseFallbackDescription;
      studioExpressRight.LicenseSourceName = licenseSourceName;
      studioExpressRight.LicenseUrl = licenseUrl;
      studioExpressRight.m_attributes = attributes;
      return studioExpressRight;
    }

    private VisualStudioExpressRight(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.m_name = name;
    }

    public override string Name => this.m_name;

    public override Dictionary<string, object> Attributes => this.m_attributes;

    public override Version Version => VisualStudioExpressRight.s_currentVersion;

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override int CompareTo(object obj) => this.CompareTo(obj as VisualStudioExpressRight);

    public int CompareTo(VisualStudioExpressRight right)
    {
      int num = DateTimeOffset.Compare(this.ExpirationDate, right.ExpirationDate);
      return num != 0 ? num : 0;
    }
  }
}
