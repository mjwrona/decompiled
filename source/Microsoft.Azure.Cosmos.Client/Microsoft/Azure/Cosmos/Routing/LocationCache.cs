// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.LocationCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class LocationCache
  {
    private const string UnavailableLocationsExpirationTimeInSeconds = "UnavailableLocationsExpirationTimeInSeconds";
    private static int DefaultUnavailableLocationsExpirationTimeInSeconds = 300;
    private readonly bool enableEndpointDiscovery;
    private readonly Uri defaultEndpoint;
    private readonly bool useMultipleWriteLocations;
    private readonly object lockObject;
    private readonly TimeSpan unavailableLocationsExpirationTime;
    private readonly int connectionLimit;
    private readonly ConcurrentDictionary<Uri, LocationCache.LocationUnavailabilityInfo> locationUnavailablityInfoByEndpoint;
    private LocationCache.DatabaseAccountLocationsInfo locationInfo;
    private DateTime lastCacheUpdateTimestamp;
    private bool enableMultipleWriteLocations;

    public LocationCache(
      ReadOnlyCollection<string> preferredLocations,
      Uri defaultEndpoint,
      bool enableEndpointDiscovery,
      int connectionLimit,
      bool useMultipleWriteLocations)
    {
      this.locationInfo = new LocationCache.DatabaseAccountLocationsInfo(preferredLocations, defaultEndpoint);
      this.defaultEndpoint = defaultEndpoint;
      this.enableEndpointDiscovery = enableEndpointDiscovery;
      this.useMultipleWriteLocations = useMultipleWriteLocations;
      this.connectionLimit = connectionLimit;
      this.lockObject = new object();
      this.locationUnavailablityInfoByEndpoint = new ConcurrentDictionary<Uri, LocationCache.LocationUnavailabilityInfo>();
      this.lastCacheUpdateTimestamp = DateTime.MinValue;
      this.enableMultipleWriteLocations = false;
      this.unavailableLocationsExpirationTime = TimeSpan.FromSeconds((double) LocationCache.DefaultUnavailableLocationsExpirationTimeInSeconds);
      if (!(Assembly.GetEntryAssembly() != (Assembly) null))
        return;
      string appSetting = ConfigurationManager.AppSettings[nameof (UnavailableLocationsExpirationTimeInSeconds)];
      if (string.IsNullOrEmpty(appSetting))
        return;
      int result;
      if (!int.TryParse(appSetting, out result))
        this.unavailableLocationsExpirationTime = TimeSpan.FromSeconds((double) LocationCache.DefaultUnavailableLocationsExpirationTimeInSeconds);
      else
        this.unavailableLocationsExpirationTime = TimeSpan.FromSeconds((double) result);
    }

    public ReadOnlyCollection<Uri> ReadEndpoints
    {
      get
      {
        if (DateTime.UtcNow - this.lastCacheUpdateTimestamp > this.unavailableLocationsExpirationTime && this.locationUnavailablityInfoByEndpoint.Any<KeyValuePair<Uri, LocationCache.LocationUnavailabilityInfo>>())
          this.UpdateLocationCache();
        return this.locationInfo.ReadEndpoints;
      }
    }

    public ReadOnlyCollection<Uri> WriteEndpoints
    {
      get
      {
        if (DateTime.UtcNow - this.lastCacheUpdateTimestamp > this.unavailableLocationsExpirationTime && this.locationUnavailablityInfoByEndpoint.Any<KeyValuePair<Uri, LocationCache.LocationUnavailabilityInfo>>())
          this.UpdateLocationCache();
        return this.locationInfo.WriteEndpoints;
      }
    }

    public string GetLocation(Uri endpoint)
    {
      KeyValuePair<string, Uri> keyValuePair = this.locationInfo.AvailableWriteEndpointByLocation.FirstOrDefault<KeyValuePair<string, Uri>>((Func<KeyValuePair<string, Uri>, bool>) (uri => uri.Value == endpoint));
      string key = keyValuePair.Key;
      if (key == null)
      {
        keyValuePair = this.locationInfo.AvailableReadEndpointByLocation.FirstOrDefault<KeyValuePair<string, Uri>>((Func<KeyValuePair<string, Uri>, bool>) (uri => uri.Value == endpoint));
        key = keyValuePair.Key;
      }
      string location = key;
      if (location != null || !(endpoint == this.defaultEndpoint) || this.CanUseMultipleWriteLocations() || !this.locationInfo.AvailableWriteEndpointByLocation.Any<KeyValuePair<string, Uri>>())
        return location;
      keyValuePair = this.locationInfo.AvailableWriteEndpointByLocation.First<KeyValuePair<string, Uri>>();
      return keyValuePair.Key;
    }

    public bool TryGetLocationForGatewayDiagnostics(Uri endpoint, out string regionName)
    {
      if (Uri.Compare(endpoint, this.defaultEndpoint, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
      {
        regionName = (string) null;
        return false;
      }
      regionName = this.GetLocation(endpoint);
      return true;
    }

    public void MarkEndpointUnavailableForRead(Uri endpoint) => this.MarkEndpointUnavailable(endpoint, LocationCache.OperationType.Read);

    public void MarkEndpointUnavailableForWrite(Uri endpoint) => this.MarkEndpointUnavailable(endpoint, LocationCache.OperationType.Write);

    public void OnDatabaseAccountRead(AccountProperties databaseAccount) => this.UpdateLocationCache(databaseAccount.WritableRegions, databaseAccount.ReadableRegions, enableMultipleWriteLocations: new bool?(databaseAccount.EnableMultipleWriteLocations));

    public void OnLocationPreferenceChanged(ReadOnlyCollection<string> preferredLocations) => this.UpdateLocationCache(preferenceList: preferredLocations);

    public Uri ResolveServiceEndpoint(DocumentServiceRequest request)
    {
      if (request.RequestContext != null && request.RequestContext.LocationEndpointToRoute != (Uri) null)
        return request.RequestContext.LocationEndpointToRoute;
      int valueOrDefault = request.RequestContext.LocationIndexToRoute.GetValueOrDefault(0);
      Uri defaultEndpoint = this.defaultEndpoint;
      if (!request.RequestContext.UsePreferredLocations.GetValueOrDefault(true) || request.OperationType.IsWriteOperation() && !this.CanUseMultipleWriteLocations(request))
      {
        LocationCache.DatabaseAccountLocationsInfo locationInfo = this.locationInfo;
        if (this.enableEndpointDiscovery && locationInfo.AvailableWriteLocations.Count > 0)
        {
          int index = Math.Min(valueOrDefault % 2, locationInfo.AvailableWriteLocations.Count - 1);
          string availableWriteLocation = locationInfo.AvailableWriteLocations[index];
          defaultEndpoint = locationInfo.AvailableWriteEndpointByLocation[availableWriteLocation];
        }
      }
      else
      {
        ReadOnlyCollection<Uri> readOnlyCollection = request.OperationType.IsWriteOperation() ? this.WriteEndpoints : this.ReadEndpoints;
        defaultEndpoint = readOnlyCollection[valueOrDefault % readOnlyCollection.Count];
      }
      request.RequestContext.RouteToLocation(defaultEndpoint);
      return defaultEndpoint;
    }

    public bool ShouldRefreshEndpoints(out bool canRefreshInBackground)
    {
      canRefreshInBackground = true;
      LocationCache.DatabaseAccountLocationsInfo locationInfo = this.locationInfo;
      string key = locationInfo.PreferredLocations.FirstOrDefault<string>();
      if (!this.enableEndpointDiscovery)
        return false;
      bool flag1 = this.useMultipleWriteLocations && !this.enableMultipleWriteLocations;
      ReadOnlyCollection<Uri> readEndpoints = locationInfo.ReadEndpoints;
      if (this.IsEndpointUnavailable(readEndpoints[0], LocationCache.OperationType.Read))
      {
        canRefreshInBackground = readEndpoints.Count > 1;
        DefaultTrace.TraceInformation("ShouldRefreshEndpoints = true since the first read endpoint {0} is not available for read. canRefreshInBackground = {1}", (object) readEndpoints[0], (object) canRefreshInBackground);
        return true;
      }
      if (!string.IsNullOrEmpty(key))
      {
        Uri uri;
        if (locationInfo.AvailableReadEndpointByLocation.TryGetValue(key, out uri))
        {
          if (uri != readEndpoints[0])
          {
            DefaultTrace.TraceInformation("ShouldRefreshEndpoints = true since most preferred location {0} is not available for read.", (object) key);
            return true;
          }
        }
        else
        {
          DefaultTrace.TraceInformation("ShouldRefreshEndpoints = true since most preferred location {0} is not in available read locations.", (object) key);
          return true;
        }
      }
      ReadOnlyCollection<Uri> writeEndpoints = locationInfo.WriteEndpoints;
      if (!this.CanUseMultipleWriteLocations())
      {
        if (!this.IsEndpointUnavailable(writeEndpoints[0], LocationCache.OperationType.Write))
          return flag1;
        canRefreshInBackground = writeEndpoints.Count > 1;
        DefaultTrace.TraceInformation("ShouldRefreshEndpoints = true since most preferred location {0} endpoint {1} is not available for write. canRefreshInBackground = {2}", (object) key, (object) writeEndpoints[0], (object) canRefreshInBackground);
        return true;
      }
      if (string.IsNullOrEmpty(key))
        return flag1;
      Uri uri1;
      if (locationInfo.AvailableWriteEndpointByLocation.TryGetValue(key, out uri1))
      {
        bool flag2 = flag1 | uri1 != writeEndpoints[0];
        DefaultTrace.TraceInformation("ShouldRefreshEndpoints = {0} since most preferred location {1} is not available for write.", (object) flag2, (object) key);
        return flag2;
      }
      DefaultTrace.TraceInformation("ShouldRefreshEndpoints = true since most preferred location {0} is not in available write locations", (object) key);
      return true;
    }

    public bool CanUseMultipleWriteLocations(DocumentServiceRequest request)
    {
      if (!this.CanUseMultipleWriteLocations())
        return false;
      if (request.ResourceType == ResourceType.Document)
        return true;
      return request.ResourceType == ResourceType.StoredProcedure && request.OperationType == Microsoft.Azure.Documents.OperationType.ExecuteJavaScript;
    }

    private void ClearStaleEndpointUnavailabilityInfo()
    {
      if (!this.locationUnavailablityInfoByEndpoint.Any<KeyValuePair<Uri, LocationCache.LocationUnavailabilityInfo>>())
        return;
      foreach (Uri key in this.locationUnavailablityInfoByEndpoint.Keys.ToList<Uri>())
      {
        LocationCache.LocationUnavailabilityInfo unavailabilityInfo;
        if (this.locationUnavailablityInfoByEndpoint.TryGetValue(key, out unavailabilityInfo) && DateTime.UtcNow - unavailabilityInfo.LastUnavailabilityCheckTimeStamp > this.unavailableLocationsExpirationTime && this.locationUnavailablityInfoByEndpoint.TryRemove(key, out LocationCache.LocationUnavailabilityInfo _))
          DefaultTrace.TraceInformation("Removed endpoint {0} unavailable for operations {1} from unavailableEndpoints", (object) key, (object) unavailabilityInfo.UnavailableOperations);
      }
    }

    private bool IsEndpointUnavailable(
      Uri endpoint,
      LocationCache.OperationType expectedAvailableOperations)
    {
      LocationCache.LocationUnavailabilityInfo unavailabilityInfo;
      if (expectedAvailableOperations == LocationCache.OperationType.None || !this.locationUnavailablityInfoByEndpoint.TryGetValue(endpoint, out unavailabilityInfo) || !unavailabilityInfo.UnavailableOperations.HasFlag((Enum) expectedAvailableOperations) || DateTime.UtcNow - unavailabilityInfo.LastUnavailabilityCheckTimeStamp > this.unavailableLocationsExpirationTime)
        return false;
      DefaultTrace.TraceInformation("Endpoint {0} unavailable for operations {1} present in unavailableEndpoints", (object) endpoint, (object) unavailabilityInfo.UnavailableOperations);
      return true;
    }

    private void MarkEndpointUnavailable(
      Uri unavailableEndpoint,
      LocationCache.OperationType unavailableOperationType)
    {
      DateTime currentTime = DateTime.UtcNow;
      LocationCache.LocationUnavailabilityInfo unavailabilityInfo = this.locationUnavailablityInfoByEndpoint.AddOrUpdate(unavailableEndpoint, (Func<Uri, LocationCache.LocationUnavailabilityInfo>) (endpoint => new LocationCache.LocationUnavailabilityInfo()
      {
        LastUnavailabilityCheckTimeStamp = currentTime,
        UnavailableOperations = unavailableOperationType
      }), (Func<Uri, LocationCache.LocationUnavailabilityInfo, LocationCache.LocationUnavailabilityInfo>) ((endpoint, info) =>
      {
        info.LastUnavailabilityCheckTimeStamp = currentTime;
        info.UnavailableOperations |= unavailableOperationType;
        return info;
      }));
      this.UpdateLocationCache();
      DefaultTrace.TraceInformation("Endpoint {0} unavailable for {1} added/updated to unavailableEndpoints with timestamp {2}", (object) unavailableEndpoint, (object) unavailableOperationType, (object) unavailabilityInfo.LastUnavailabilityCheckTimeStamp);
    }

    private void UpdateLocationCache(
      IEnumerable<AccountRegion> writeLocations = null,
      IEnumerable<AccountRegion> readLocations = null,
      ReadOnlyCollection<string> preferenceList = null,
      bool? enableMultipleWriteLocations = null)
    {
      lock (this.lockObject)
      {
        LocationCache.DatabaseAccountLocationsInfo accountLocationsInfo = new LocationCache.DatabaseAccountLocationsInfo(this.locationInfo);
        if (preferenceList != null)
          accountLocationsInfo.PreferredLocations = preferenceList;
        if (enableMultipleWriteLocations.HasValue)
          this.enableMultipleWriteLocations = enableMultipleWriteLocations.Value;
        this.ClearStaleEndpointUnavailabilityInfo();
        if (readLocations != null)
        {
          ReadOnlyCollection<string> orderedLocations;
          accountLocationsInfo.AvailableReadEndpointByLocation = this.GetEndpointByLocation(readLocations, out orderedLocations);
          accountLocationsInfo.AvailableReadLocations = orderedLocations;
        }
        if (writeLocations != null)
        {
          ReadOnlyCollection<string> orderedLocations;
          accountLocationsInfo.AvailableWriteEndpointByLocation = this.GetEndpointByLocation(writeLocations, out orderedLocations);
          accountLocationsInfo.AvailableWriteLocations = orderedLocations;
        }
        accountLocationsInfo.WriteEndpoints = this.GetPreferredAvailableEndpoints(accountLocationsInfo.AvailableWriteEndpointByLocation, accountLocationsInfo.AvailableWriteLocations, LocationCache.OperationType.Write, this.defaultEndpoint);
        accountLocationsInfo.ReadEndpoints = this.GetPreferredAvailableEndpoints(accountLocationsInfo.AvailableReadEndpointByLocation, accountLocationsInfo.AvailableReadLocations, LocationCache.OperationType.Read, accountLocationsInfo.WriteEndpoints[0]);
        this.lastCacheUpdateTimestamp = DateTime.UtcNow;
        DefaultTrace.TraceInformation("Current WriteEndpoints = ({0}) ReadEndpoints = ({1})", (object) string.Join(", ", accountLocationsInfo.WriteEndpoints.Select<Uri, string>((Func<Uri, string>) (endpoint => endpoint.ToString()))), (object) string.Join(", ", accountLocationsInfo.ReadEndpoints.Select<Uri, string>((Func<Uri, string>) (endpoint => endpoint.ToString()))));
        this.locationInfo = accountLocationsInfo;
      }
    }

    private ReadOnlyCollection<Uri> GetPreferredAvailableEndpoints(
      ReadOnlyDictionary<string, Uri> endpointsByLocation,
      ReadOnlyCollection<string> orderedLocations,
      LocationCache.OperationType expectedAvailableOperation,
      Uri fallbackEndpoint)
    {
      List<Uri> uriList = new List<Uri>();
      LocationCache.DatabaseAccountLocationsInfo locationInfo = this.locationInfo;
      if (this.enableEndpointDiscovery)
      {
        if (this.CanUseMultipleWriteLocations() || expectedAvailableOperation.HasFlag((Enum) LocationCache.OperationType.Read))
        {
          List<Uri> collection = new List<Uri>();
          foreach (string preferredLocation in locationInfo.PreferredLocations)
          {
            Uri endpoint;
            if (endpointsByLocation.TryGetValue(preferredLocation, out endpoint))
            {
              if (this.IsEndpointUnavailable(endpoint, expectedAvailableOperation))
                collection.Add(endpoint);
              else
                uriList.Add(endpoint);
            }
          }
          if (uriList.Count == 0)
          {
            uriList.Add(fallbackEndpoint);
            collection.Remove(fallbackEndpoint);
          }
          uriList.AddRange((IEnumerable<Uri>) collection);
        }
        else
        {
          foreach (string orderedLocation in orderedLocations)
          {
            Uri uri;
            if (!string.IsNullOrEmpty(orderedLocation) && endpointsByLocation.TryGetValue(orderedLocation, out uri))
              uriList.Add(uri);
          }
        }
      }
      if (uriList.Count == 0)
        uriList.Add(fallbackEndpoint);
      return uriList.AsReadOnly();
    }

    private ReadOnlyDictionary<string, Uri> GetEndpointByLocation(
      IEnumerable<AccountRegion> locations,
      out ReadOnlyCollection<string> orderedLocations)
    {
      Dictionary<string, Uri> dictionary = new Dictionary<string, Uri>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> stringList = new List<string>();
      foreach (AccountRegion location in locations)
      {
        Uri result;
        if (!string.IsNullOrEmpty(location.Name) && Uri.TryCreate(location.Endpoint, UriKind.Absolute, out result))
        {
          dictionary[location.Name] = result;
          stringList.Add(location.Name);
          this.SetServicePointConnectionLimit(result);
        }
        else
          DefaultTrace.TraceInformation("GetAvailableEndpointsByLocation() - skipping add for location = {0} as it is location name is either empty or endpoint is malformed {1}", (object) location.Name, (object) location.Endpoint);
      }
      orderedLocations = stringList.AsReadOnly();
      return new ReadOnlyDictionary<string, Uri>((IDictionary<string, Uri>) dictionary);
    }

    private bool CanUseMultipleWriteLocations() => this.useMultipleWriteLocations && this.enableMultipleWriteLocations;

    private void SetServicePointConnectionLimit(Uri endpoint) => ServicePointAccessor.FindServicePoint(endpoint).ConnectionLimit = this.connectionLimit;

    private sealed class LocationUnavailabilityInfo
    {
      public DateTime LastUnavailabilityCheckTimeStamp { get; set; }

      public LocationCache.OperationType UnavailableOperations { get; set; }
    }

    private sealed class DatabaseAccountLocationsInfo
    {
      public DatabaseAccountLocationsInfo(
        ReadOnlyCollection<string> preferredLocations,
        Uri defaultEndpoint)
      {
        this.PreferredLocations = preferredLocations;
        this.AvailableWriteLocations = new List<string>().AsReadOnly();
        this.AvailableReadLocations = new List<string>().AsReadOnly();
        this.AvailableWriteEndpointByLocation = new ReadOnlyDictionary<string, Uri>((IDictionary<string, Uri>) new Dictionary<string, Uri>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        this.AvailableReadEndpointByLocation = new ReadOnlyDictionary<string, Uri>((IDictionary<string, Uri>) new Dictionary<string, Uri>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        this.WriteEndpoints = new List<Uri>()
        {
          defaultEndpoint
        }.AsReadOnly();
        this.ReadEndpoints = new List<Uri>()
        {
          defaultEndpoint
        }.AsReadOnly();
      }

      public DatabaseAccountLocationsInfo(LocationCache.DatabaseAccountLocationsInfo other)
      {
        this.PreferredLocations = other.PreferredLocations;
        this.AvailableWriteLocations = other.AvailableWriteLocations;
        this.AvailableReadLocations = other.AvailableReadLocations;
        this.AvailableWriteEndpointByLocation = other.AvailableWriteEndpointByLocation;
        this.AvailableReadEndpointByLocation = other.AvailableReadEndpointByLocation;
        this.WriteEndpoints = other.WriteEndpoints;
        this.ReadEndpoints = other.ReadEndpoints;
      }

      public ReadOnlyCollection<string> PreferredLocations { get; set; }

      public ReadOnlyCollection<string> AvailableWriteLocations { get; set; }

      public ReadOnlyCollection<string> AvailableReadLocations { get; set; }

      public ReadOnlyDictionary<string, Uri> AvailableWriteEndpointByLocation { get; set; }

      public ReadOnlyDictionary<string, Uri> AvailableReadEndpointByLocation { get; set; }

      public ReadOnlyCollection<Uri> WriteEndpoints { get; set; }

      public ReadOnlyCollection<Uri> ReadEndpoints { get; set; }
    }

    [Flags]
    private enum OperationType
    {
      None = 0,
      Read = 1,
      Write = 2,
    }
  }
}
