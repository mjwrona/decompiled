// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PirStatusText
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types
{
  public sealed class PirStatusText
  {
    [EnumValue(1)]
    public const string NotStarted = "Not Started";
    [EnumValue(2)]
    public const string Abandoned = "Abandoned";
    [EnumValue(4)]
    public const string InProgress = "In Progress";
    [EnumValue(8)]
    public const string ReadyForReview = "Ready For Review";
    [EnumValue(16)]
    public const string Completed = "Completed";
    [EnumValue(32)]
    public const string Approved = "Approved";
    [EnumValue(64)]
    public const string Published = "Published";

    public static PirStatus GetEnumValue(string value)
    {
      PirStatus result;
      if (string.IsNullOrEmpty(value) || value.Trim().Length == 0 || !EnumTextMapper<PirStatusText, PirStatus>.TryGetEnumValue(value, out result))
        result = PirStatus.Unknown;
      return result;
    }
  }
}
