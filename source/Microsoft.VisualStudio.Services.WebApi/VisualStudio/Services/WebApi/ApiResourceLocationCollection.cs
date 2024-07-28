// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ApiResourceLocationCollection
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class ApiResourceLocationCollection
  {
    private Dictionary<Guid, ApiResourceLocation> m_locationsById = new Dictionary<Guid, ApiResourceLocation>();
    private Dictionary<string, List<ApiResourceLocation>> m_locationsByKey = new Dictionary<string, List<ApiResourceLocation>>();

    public void AddResourceLocation(ApiResourceLocation location)
    {
      ApiResourceLocation other;
      if (this.m_locationsById.TryGetValue(location.Id, out other) && !location.Equals(other))
        throw new VssApiResourceDuplicateIdException(location.Id);
      this.m_locationsById[location.Id] = location;
      string locationCacheKey = this.GetLocationCacheKey(location.Area, location.ResourceName);
      List<ApiResourceLocation> source;
      if (!this.m_locationsByKey.TryGetValue(locationCacheKey, out source))
      {
        source = new List<ApiResourceLocation>();
        this.m_locationsByKey.Add(locationCacheKey, source);
      }
      if (source.Any<ApiResourceLocation>((Func<ApiResourceLocation, bool>) (x => x.Id.Equals(location.Id))))
        return;
      source.Add(location);
    }

    public void AddResourceLocations(IEnumerable<ApiResourceLocation> locations)
    {
      if (locations == null)
        return;
      foreach (ApiResourceLocation location in locations)
        this.AddResourceLocation(location);
    }

    private string GetLocationCacheKey(string area, string resourceName)
    {
      if (area == null)
        area = string.Empty;
      if (resourceName == null)
        resourceName = string.Empty;
      return string.Format("{0}:{1}", (object) area.ToLower(), (object) resourceName.ToLower());
    }

    public ApiResourceLocation TryGetLocationById(Guid locationId)
    {
      ApiResourceLocation locationById;
      this.m_locationsById.TryGetValue(locationId, out locationById);
      return locationById;
    }

    public ApiResourceLocation GetLocationById(Guid locationId) => this.TryGetLocationById(locationId) ?? throw new VssResourceNotFoundException(locationId);

    public IEnumerable<ApiResourceLocation> GetAllLocations() => (IEnumerable<ApiResourceLocation>) this.m_locationsById.Values;

    public IEnumerable<ApiResourceLocation> GetAreaLocations(string area) => this.m_locationsById.Values.Where<ApiResourceLocation>((Func<ApiResourceLocation, bool>) (l => string.Equals(area, l.Area, StringComparison.OrdinalIgnoreCase)));

    public IEnumerable<ApiResourceLocation> GetResourceLocations(string area, string resourceName)
    {
      List<ApiResourceLocation> resourceLocationList;
      return this.m_locationsByKey.TryGetValue(this.GetLocationCacheKey(area, resourceName), out resourceLocationList) ? (IEnumerable<ApiResourceLocation>) resourceLocationList : Enumerable.Empty<ApiResourceLocation>();
    }
  }
}
