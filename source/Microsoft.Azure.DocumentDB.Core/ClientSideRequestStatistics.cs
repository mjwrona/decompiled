// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientSideRequestStatistics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class ClientSideRequestStatistics : IClientSideRequestStatistics
  {
    private const int MaxSupplementalRequestsForToString = 10;
    private DateTime requestStartTime;
    private DateTime requestEndTime;
    private object lockObject = new object();
    private List<ClientSideRequestStatistics.StoreResponseStatistics> responseStatisticsList;
    private List<ClientSideRequestStatistics.StoreResponseStatistics> supplementalResponseStatisticsList;
    private Dictionary<string, ClientSideRequestStatistics.AddressResolutionStatistics> addressResolutionStatistics;

    public ClientSideRequestStatistics()
    {
      this.requestStartTime = DateTime.UtcNow;
      this.requestEndTime = DateTime.UtcNow;
      this.responseStatisticsList = new List<ClientSideRequestStatistics.StoreResponseStatistics>();
      this.supplementalResponseStatisticsList = new List<ClientSideRequestStatistics.StoreResponseStatistics>();
      this.addressResolutionStatistics = new Dictionary<string, ClientSideRequestStatistics.AddressResolutionStatistics>();
      this.ContactedReplicas = new List<Uri>();
      this.FailedReplicas = new HashSet<Uri>();
      this.RegionsContacted = new HashSet<Uri>();
    }

    public List<Uri> ContactedReplicas { get; set; }

    public HashSet<Uri> FailedReplicas { get; private set; }

    public HashSet<Uri> RegionsContacted { get; private set; }

    public TimeSpan RequestLatency => this.requestEndTime - this.requestStartTime;

    public bool IsCpuOverloaded
    {
      get
      {
        foreach (ClientSideRequestStatistics.StoreResponseStatistics responseStatistics in this.responseStatisticsList)
        {
          if (responseStatistics.StoreResult.IsClientCpuOverloaded)
            return true;
        }
        foreach (ClientSideRequestStatistics.StoreResponseStatistics responseStatistics in this.supplementalResponseStatisticsList)
        {
          if (responseStatistics.StoreResult.IsClientCpuOverloaded)
            return true;
        }
        return false;
      }
    }

    public void RecordRequest(DocumentServiceRequest request)
    {
    }

    public void RecordResponse(DocumentServiceRequest request, StoreResult storeResult)
    {
      DateTime utcNow = DateTime.UtcNow;
      ClientSideRequestStatistics.StoreResponseStatistics responseStatistics;
      responseStatistics.RequestResponseTime = utcNow;
      responseStatistics.StoreResult = storeResult;
      responseStatistics.RequestOperationType = request.OperationType;
      responseStatistics.RequestResourceType = request.ResourceType;
      Uri locationEndpointToRoute = request.RequestContext.LocationEndpointToRoute;
      lock (this.lockObject)
      {
        if (utcNow > this.requestEndTime)
          this.requestEndTime = utcNow;
        if (locationEndpointToRoute != (Uri) null)
          this.RegionsContacted.Add(locationEndpointToRoute);
        if (responseStatistics.RequestOperationType == OperationType.Head || responseStatistics.RequestOperationType == OperationType.HeadFeed)
          this.supplementalResponseStatisticsList.Add(responseStatistics);
        else
          this.responseStatisticsList.Add(responseStatistics);
      }
    }

    public string RecordAddressResolutionStart(Uri targetEndpoint)
    {
      string key = Guid.NewGuid().ToString();
      ClientSideRequestStatistics.AddressResolutionStatistics resolutionStatistics = new ClientSideRequestStatistics.AddressResolutionStatistics()
      {
        StartTime = DateTime.UtcNow,
        EndTime = DateTime.MaxValue,
        TargetEndpoint = targetEndpoint == (Uri) null ? "<NULL>" : targetEndpoint.ToString()
      };
      lock (this.lockObject)
        this.addressResolutionStatistics.Add(key, resolutionStatistics);
      return key;
    }

    public void RecordAddressResolutionEnd(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        return;
      DateTime utcNow = DateTime.UtcNow;
      lock (this.lockObject)
      {
        if (!this.addressResolutionStatistics.ContainsKey(identifier))
          throw new ArgumentException("Identifier {0} does not exist. Please call start before calling end.", identifier);
        if (utcNow > this.requestEndTime)
          this.requestEndTime = utcNow;
        this.addressResolutionStatistics[identifier].EndTime = utcNow;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.AppendToBuilder(stringBuilder);
      return stringBuilder.ToString();
    }

    public void AppendToBuilder(StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        throw new ArgumentNullException(nameof (stringBuilder));
      lock (this.lockObject)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "RequestStartTime: {0}, RequestEndTime: {1},  Number of regions attempted:{2}", (object) this.requestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.requestEndTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) (this.RegionsContacted.Count == 0 ? 1 : this.RegionsContacted.Count));
        stringBuilder.AppendLine();
        foreach (ClientSideRequestStatistics.StoreResponseStatistics responseStatistics in this.responseStatisticsList)
        {
          responseStatistics.AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
        foreach (ClientSideRequestStatistics.AddressResolutionStatistics resolutionStatistics in this.addressResolutionStatistics.Values)
        {
          resolutionStatistics.AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
        int count = this.supplementalResponseStatisticsList.Count;
        int num = Math.Max(count - 10, 0);
        if (num != 0)
        {
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "  -- Displaying only the last {0} head/headfeed requests. Total head/headfeed requests: {1}", (object) 10, (object) count);
          stringBuilder.AppendLine();
        }
        for (int index = num; index < count; ++index)
        {
          this.supplementalResponseStatisticsList[index].AppendToBuilder(stringBuilder);
          stringBuilder.AppendLine();
        }
      }
    }

    private struct StoreResponseStatistics
    {
      public DateTime RequestResponseTime;
      public StoreResult StoreResult;
      public ResourceType RequestResourceType;
      public OperationType RequestOperationType;

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.AppendToBuilder(stringBuilder);
        return stringBuilder.ToString();
      }

      public void AppendToBuilder(StringBuilder stringBuilder)
      {
        if (stringBuilder == null)
          throw new ArgumentNullException(nameof (stringBuilder));
        stringBuilder.Append("ResponseTime: " + this.RequestResponseTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ", ");
        stringBuilder.Append("StoreResult: ");
        if (this.StoreResult != null)
          this.StoreResult.AppendToBuilder(stringBuilder);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ", ResourceType: {0}, OperationType: {1}", (object) this.RequestResourceType, (object) this.RequestOperationType);
      }
    }

    private class AddressResolutionStatistics
    {
      public DateTime StartTime { get; set; }

      public DateTime EndTime { get; set; }

      public string TargetEndpoint { get; set; }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.AppendToBuilder(stringBuilder);
        return stringBuilder.ToString();
      }

      public void AppendToBuilder(StringBuilder stringBuilder)
      {
        StringBuilder stringBuilder1 = stringBuilder != null ? stringBuilder : throw new ArgumentNullException(nameof (stringBuilder));
        DateTime dateTime = this.StartTime;
        string str1 = "AddressResolution - StartTime: " + dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ", ";
        StringBuilder stringBuilder2 = stringBuilder1.Append(str1);
        dateTime = this.EndTime;
        string str2 = "EndTime: " + dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ", ";
        stringBuilder2.Append(str2).Append("TargetEndpoint: ").Append(this.TargetEndpoint);
      }
    }
  }
}
