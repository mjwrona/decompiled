// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RootCauseStatusText
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AzureAd.Icm.Types
{
  [SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed", Justification = "This class is used as a type parameter.")]
  public sealed class RootCauseStatusText
  {
    [EnumValue(1)]
    public const string NeedsInvestigation = "Needs Investigation";
    [EnumValue(2)]
    public const string NotRequired = "Not Required";
    [EnumValue(3)]
    public const string Specified = "Specified";

    private RootCauseStatusText()
    {
    }

    public static RootCauseStatus GetEnumValue(string value)
    {
      RootCauseStatus result;
      if (string.IsNullOrWhiteSpace(value) || !EnumTextMapper<RootCauseStatusText, RootCauseStatus>.TryGetEnumValue(value, out result))
        result = RootCauseStatus.Unknown;
      return result;
    }
  }
}
