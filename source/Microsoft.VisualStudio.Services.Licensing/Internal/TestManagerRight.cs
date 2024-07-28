// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.TestManagerRight
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
  public class TestManagerRight : ClientRightBase, IVisualStudioRightAttributes
  {
    private readonly string m_name;
    private Dictionary<string, object> m_attributes;
    private static readonly Version s_currentVersion = new Version(1, 0, 0, 0);
    private static readonly int[] s_visualStudioEditionOrder = TestManagerRight.GenerateVisualStudioEditionOrder();
    public const string TestProfessionalRightName = "TestProfessional";
    public const string TestManagerRightName = "TestManager";

    public static TestManagerRight Create(
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
      TestManagerRight testManagerRight = new TestManagerRight(name);
      testManagerRight.ClientVersion = productVersion;
      testManagerRight.Edition = edition;
      testManagerRight.ExpirationDate = expirationDate;
      testManagerRight.LicenseDescriptionId = licenseDescriptionId;
      testManagerRight.LicenseFallbackDescription = licenseFallbackDescription;
      testManagerRight.LicenseSourceName = licenseSourceName;
      testManagerRight.LicenseUrl = licenseUrl;
      testManagerRight.m_attributes = attributes;
      return testManagerRight;
    }

    private TestManagerRight(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.m_name = name;
    }

    public override Dictionary<string, object> Attributes => this.m_attributes;

    public override string AuthorizedVSEdition => this.Edition.ToString();

    public override string Name => this.m_name;

    public override Version Version => TestManagerRight.s_currentVersion;

    [IgnoreDataMember]
    [JsonIgnore]
    public VisualStudioEdition Edition { get; set; }

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override int CompareTo(object obj) => this.CompareTo(obj as TestManagerRight);

    public int CompareTo(TestManagerRight right)
    {
      int num1 = TestManagerRight.CompareVisualStudioEdition(this.Edition, right.Edition);
      if (num1 != 0)
        return num1;
      int num2 = DateTimeOffset.Compare(this.ExpirationDate, right.ExpirationDate);
      return num2 != 0 ? num2 : 0;
    }

    public static int CompareVisualStudioEdition(
      VisualStudioEdition left,
      VisualStudioEdition right)
    {
      if (left > VisualStudioEdition.Community)
        left = VisualStudioEdition.Unspecified;
      if (right > VisualStudioEdition.Community)
        right = VisualStudioEdition.Unspecified;
      return TestManagerRight.s_visualStudioEditionOrder[(int) left].CompareTo(TestManagerRight.s_visualStudioEditionOrder[(int) right]);
    }

    private static int[] GenerateVisualStudioEditionOrder()
    {
      int[] studioEditionOrder = new int[6]
      {
        10,
        20,
        0,
        0,
        0,
        30
      };
      studioEditionOrder[2] = 40;
      studioEditionOrder[3] = 50;
      studioEditionOrder[4] = 60;
      return studioEditionOrder;
    }
  }
}
