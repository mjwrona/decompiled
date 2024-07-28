// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterCacheContainer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferMeterCacheContainer
  {
    private Dictionary<int, OfferMeter> meterConfiguration;
    private Dictionary<string, int> meterNameMapping;
    private Dictionary<string, int> galleryIdMapping;

    public OfferMeterCacheContainer()
    {
      this.meterConfiguration = new Dictionary<int, OfferMeter>();
      this.meterNameMapping = new Dictionary<string, int>();
      this.galleryIdMapping = new Dictionary<string, int>();
    }

    public void AddOfferConfiguration(IEnumerable<OfferMeter> meterConfiguration)
    {
      this.meterConfiguration.Clear();
      this.meterNameMapping.Clear();
      this.galleryIdMapping.Clear();
      foreach (OfferMeter offerMeter in meterConfiguration)
      {
        this.meterConfiguration.Add(offerMeter.MeterId, offerMeter);
        if (!string.IsNullOrEmpty(offerMeter.Name))
          this.meterNameMapping.Add(offerMeter.Name.ToLowerInvariant(), offerMeter.MeterId);
        if (!string.IsNullOrEmpty(offerMeter.GalleryId))
          this.galleryIdMapping.Add(offerMeter.GalleryId.ToLowerInvariant(), offerMeter.MeterId);
      }
    }

    public int? GetOfferMeterId(string meterName)
    {
      if (string.IsNullOrEmpty(meterName))
        return new int?();
      meterName = meterName.ToLowerInvariant();
      return this.meterNameMapping.ContainsKey(meterName) ? new int?(this.meterNameMapping[meterName]) : new int?();
    }

    public OfferMeter GetOfferMeter(int meterId) => this.meterConfiguration[meterId];

    public OfferMeter GetOfferMeter(string meterNameOrGalleryId)
    {
      int? nullable = this.GetOfferMeterId(meterNameOrGalleryId);
      if (!nullable.HasValue)
        nullable = this.GetMeterIdFromGalleryId(meterNameOrGalleryId);
      return nullable.HasValue ? this.GetOfferMeter(nullable.Value) : (OfferMeter) null;
    }

    public int? GetMeterIdFromGalleryId(string galleryId)
    {
      if (string.IsNullOrEmpty(galleryId))
        return new int?();
      galleryId = galleryId.ToLowerInvariant();
      return this.galleryIdMapping.ContainsKey(galleryId) ? new int?(this.galleryIdMapping[galleryId]) : new int?();
    }

    public IEnumerable<OfferMeter> GetOfferMeters() => (IEnumerable<OfferMeter>) this.meterConfiguration.Values.ToList<OfferMeter>();
  }
}
