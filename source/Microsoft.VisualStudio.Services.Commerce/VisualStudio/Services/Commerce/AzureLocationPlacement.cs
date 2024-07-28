// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureLocationPlacement
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class AzureLocationPlacement
  {
    private static readonly Dictionary<string, AzureLocationPlacementSettings> LocationPlacementIdMappings = new Dictionary<string, AzureLocationPlacementSettings>();

    public static bool TryMap(
      string locationPlacementId,
      out AzureLocationPlacementSettings settings)
    {
      if (!string.IsNullOrEmpty(locationPlacementId))
        return AzureLocationPlacement.LocationPlacementIdMappings.TryGetValue(locationPlacementId, out settings);
      settings = (AzureLocationPlacementSettings) null;
      return false;
    }

    public static AzureLocationPlacementSettings GetPlacementSettingsForRegion(string regionCode) => AzureLocationPlacement.LocationPlacementIdMappings.Values.FirstOrDefault<AzureLocationPlacementSettings>((Func<AzureLocationPlacementSettings, bool>) (settings => settings.AllowedRegionCodes.Contains(regionCode)));

    internal static void Add(AzureLocationPlacementSettings settings)
    {
      if (AzureLocationPlacement.LocationPlacementIdMappings.ContainsKey(settings.LocationPlacementId))
        return;
      foreach (string allowedRegionCode in (IEnumerable<string>) settings.AllowedRegionCodes)
      {
        AzureLocationPlacementSettings settingsForRegion = AzureLocationPlacement.GetPlacementSettingsForRegion(allowedRegionCode);
        if (settingsForRegion != null)
          throw new ArgumentException("Regions cannot overlap between multiple location placement settings. Region code " + allowedRegionCode + " already exists in " + settingsForRegion.LocationPlacementId);
      }
      AzureLocationPlacement.LocationPlacementIdMappings.Add(settings.LocationPlacementId, settings);
    }
  }
}
