// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioEmulatorAndroidRight
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
  public class VisualStudioEmulatorAndroidRight : ClientRightBase, IVisualStudioRightAttributes
  {
    private readonly string m_name;
    private Dictionary<string, object> m_attributes;
    private static readonly Version s_currentVersion = new Version(1, 0, 0, 0);
    private static readonly int[] s_visualStudioEditionOrder = VisualStudioEmulatorAndroidRight.GenerateVisualStudioEditionOrder();
    public const string EmulatorAndroidRightName = "VisualStudioEmulatorAndroid";

    public static VisualStudioEmulatorAndroidRight Create(
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
      VisualStudioEmulatorAndroidRight emulatorAndroidRight = new VisualStudioEmulatorAndroidRight(name);
      emulatorAndroidRight.ClientVersion = productVersion;
      emulatorAndroidRight.Edition = edition;
      emulatorAndroidRight.ExpirationDate = expirationDate;
      emulatorAndroidRight.LicenseDescriptionId = licenseDescriptionId;
      emulatorAndroidRight.LicenseFallbackDescription = licenseFallbackDescription;
      emulatorAndroidRight.LicenseSourceName = licenseSourceName;
      emulatorAndroidRight.LicenseUrl = licenseUrl;
      emulatorAndroidRight.m_attributes = attributes;
      return emulatorAndroidRight;
    }

    private VisualStudioEmulatorAndroidRight(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.m_name = name;
    }

    public override Dictionary<string, object> Attributes => this.m_attributes;

    public override string AuthorizedVSEdition => this.Edition.ToString();

    public override string Name => this.m_name;

    public override Version Version => VisualStudioEmulatorAndroidRight.s_currentVersion;

    [IgnoreDataMember]
    [JsonIgnore]
    public VisualStudioEdition Edition { get; set; }

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override int CompareTo(object obj) => this.CompareTo(obj as TestManagerRight);

    public int CompareTo(TestManagerRight right)
    {
      int num1 = VisualStudioEmulatorAndroidRight.CompareVisualStudioEdition(this.Edition, right.Edition);
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
      return VisualStudioEmulatorAndroidRight.s_visualStudioEditionOrder[(int) left].CompareTo(VisualStudioEmulatorAndroidRight.s_visualStudioEditionOrder[(int) right]);
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
  }
}
