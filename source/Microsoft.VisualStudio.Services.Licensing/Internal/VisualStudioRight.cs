// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioRight
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioRight : ClientRightBase, IVisualStudioRightAttributes
  {
    private readonly string m_name;
    private Dictionary<string, object> m_attributes;
    private static readonly Version s_currentVersion = new Version(1, 0, 0, 0);
    private static readonly int[] s_visualStudioEditionOrder = VisualStudioRight.GenerateVisualStudioEditionOrder();
    public const string TestProfessionalRightName = "TestProfessional";
    public const string VisualStudioRightName = "VisualStudio";
    public const string VisualStudioMacRightName = "VSonMac";

    public static VisualStudioRight Create(
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
      VisualStudioRight visualStudioRight = new VisualStudioRight(name);
      visualStudioRight.ClientVersion = productVersion;
      visualStudioRight.Edition = edition;
      visualStudioRight.ExpirationDate = expirationDate;
      visualStudioRight.LicenseDescriptionId = licenseDescriptionId;
      visualStudioRight.LicenseFallbackDescription = licenseFallbackDescription;
      visualStudioRight.LicenseSourceName = licenseSourceName;
      visualStudioRight.LicenseUrl = licenseUrl;
      visualStudioRight.m_attributes = attributes;
      return visualStudioRight;
    }

    private VisualStudioRight(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.m_name = name;
    }

    public override Dictionary<string, object> Attributes => this.m_attributes;

    public override string AuthorizedVSEdition => this.Edition.ToString();

    public override string Name => this.m_name;

    public override Version Version => VisualStudioRight.s_currentVersion;

    [IgnoreDataMember]
    [JsonIgnore]
    public VisualStudioEdition Edition { get; set; }

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override int CompareTo(object obj) => this.CompareTo(obj as VisualStudioRight);

    public int CompareTo(VisualStudioRight right)
    {
      int num1 = VisualStudioRight.CompareVisualStudioEdition(this.Edition, right.Edition);
      if (num1 != 0)
        return num1;
      int num2 = DateTimeOffset.Compare(this.ExpirationDate, right.ExpirationDate);
      if (num2 != 0)
        return num2;
      int num3 = string.Compare(this.LicenseSourceName, right.LicenseSourceName, StringComparison.OrdinalIgnoreCase);
      return num3 != 0 ? num3 * -1 : 0;
    }

    public static int CompareVisualStudioEdition(
      VisualStudioEdition left,
      VisualStudioEdition right)
    {
      left = VisualStudioRight.MakeSureVisualStudioEditionIsValid(left);
      right = VisualStudioRight.MakeSureVisualStudioEditionIsValid(right);
      return VisualStudioRight.s_visualStudioEditionOrder[(int) left].CompareTo(VisualStudioRight.s_visualStudioEditionOrder[(int) right]);
    }

    private static VisualStudioEdition MakeSureVisualStudioEditionIsValid(
      VisualStudioEdition edition)
    {
      return !Enum.IsDefined(typeof (VisualStudioEdition), (object) edition) ? VisualStudioEdition.Unspecified : edition;
    }

    private static int[] GenerateVisualStudioEditionOrder()
    {
      int[] studioEditionOrder = new int[7]
      {
        10,
        20,
        0,
        0,
        0,
        30,
        0
      };
      studioEditionOrder[2] = 40;
      studioEditionOrder[3] = 50;
      studioEditionOrder[4] = 60;
      studioEditionOrder[6] = 70;
      return studioEditionOrder;
    }

    public static string MapProductFamilyToRightName(VisualStudioFamily family) => family == VisualStudioFamily.VisualStudioMac ? "VSonMac" : "VisualStudio";
  }
}
