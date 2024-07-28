// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounterManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssPerformanceCounterManager
  {
    private static readonly ConcurrentDictionary<Guid, VssPerformanceCounterSet> CounterSets = new ConcurrentDictionary<Guid, VssPerformanceCounterSet>();
    private static readonly ConcurrentDictionary<string, KeyValue<Guid, int>> CounterUris = new ConcurrentDictionary<string, KeyValue<Guid, int>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly string ProcessInstanceName = VssPerformanceCounterManager.GetProcessInstanceName();
    private static bool? _shouldThrowExceptions;

    static VssPerformanceCounterManager() => VssPerformanceCounterManager.Initialize();

    private static void Initialize()
    {
      foreach (string file in Directory.GetFiles(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory, "*PerfCounters.man"))
      {
        try
        {
          XDocument node = XDocument.Load(file);
          XmlNamespaceManager resolver1 = new XmlNamespaceManager((XmlNameTable) new NameTable());
          resolver1.AddNamespace("e", "http://schemas.microsoft.com/win/2004/08/events");
          resolver1.AddNamespace("c", "http://schemas.microsoft.com/win/2005/12/counters");
          XmlNamespaceManager resolver2 = resolver1;
          foreach (XElement xpathSelectElement1 in node.XPathSelectElements("/e:instrumentationManifest/e:instrumentation/c:counters/c:provider", (IXmlNamespaceResolver) resolver2))
          {
            Guid providerGuid = Guid.Parse(xpathSelectElement1.Attribute((XName) "providerGuid").Value);
            foreach (XElement xpathSelectElement2 in xpathSelectElement1.XPathSelectElements("c:counterSet", (IXmlNamespaceResolver) resolver1))
            {
              Guid counterSetGuid = Guid.Parse(xpathSelectElement2.Attribute((XName) "guid").Value);
              string counterSetName = xpathSelectElement2.Attribute((XName) "name").Value;
              string str = xpathSelectElement2.Attribute((XName) "instances").Value;
              List<VssPerformanceCounterInfo> counters = new List<VssPerformanceCounterInfo>();
              foreach (XElement xpathSelectElement3 in xpathSelectElement2.XPathSelectElements("c:counter", (IXmlNamespaceResolver) resolver1))
              {
                int id = int.Parse(xpathSelectElement3.Attribute((XName) "id").Value);
                string counterType = xpathSelectElement3.Attribute((XName) "type").Value;
                string name = xpathSelectElement3.Attribute((XName) "name").Value;
                string uri = xpathSelectElement3.Attribute((XName) "uri").Value;
                counters.Add(new VssPerformanceCounterInfo(id, name, VssPerformanceCounterManager.ParseCounterType(counterType), uri));
              }
              bool isMultiInstance;
              switch ((CounterSetInstanceType) Enum.Parse(typeof (CounterSetInstanceType), str, true))
              {
                case CounterSetInstanceType.GlobalAggregate:
                  isMultiInstance = false;
                  break;
                case CounterSetInstanceType.MultipleAggregate:
                  isMultiInstance = true;
                  break;
                default:
                  throw new ArgumentException("Unknown counter instances type: " + str);
              }
              VssPerformanceCounterManager.RegisterCounterSet(providerGuid, counterSetGuid, counterSetName, (IReadOnlyCollection<VssPerformanceCounterInfo>) counters, isMultiInstance);
            }
          }
        }
        catch (Exception ex) when (!VssPerformanceCounterManager.ThrowExceptions())
        {
          TeamFoundationTrace.Error(new string[2]
          {
            "PerformanceCounters",
            nameof (VssPerformanceCounterManager)
          }, ex);
        }
      }
    }

    private static void RegisterCounterSet(
      Guid providerGuid,
      Guid counterSetGuid,
      string counterSetName,
      IReadOnlyCollection<VssPerformanceCounterInfo> counters,
      bool isMultiInstance)
    {
      ArgumentUtility.CheckForEmptyGuid(providerGuid, nameof (providerGuid));
      ArgumentUtility.CheckForEmptyGuid(counterSetGuid, nameof (counterSetGuid));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(counterSetName, nameof (counterSetName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) counters, nameof (counters));
      try
      {
        VssPerformanceCounterSet counterSet = new VssPerformanceCounterSet(providerGuid, counterSetGuid, counterSetName, counters, isMultiInstance);
        VssPerformanceCounterManager.CounterSets.AddOrUpdate(counterSetGuid, (Func<Guid, VssPerformanceCounterSet>) (id => counterSet), (Func<Guid, VssPerformanceCounterSet, VssPerformanceCounterSet>) ((id, c) => counterSet));
        foreach (VssPerformanceCounterInfo counter in (IEnumerable<VssPerformanceCounterInfo>) counters)
        {
          KeyValue<Guid, int> reference = new KeyValue<Guid, int>(counterSetGuid, counter.Id);
          VssPerformanceCounterManager.CounterUris.AddOrUpdate(counter.Uri, (Func<string, KeyValue<Guid, int>>) (s => reference), (Func<string, KeyValue<Guid, int>, KeyValue<Guid, int>>) ((s, id) => reference));
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Error(new string[2]
        {
          "PerformanceCounters",
          nameof (VssPerformanceCounterManager)
        }, ex);
      }
    }

    public static VssPerformanceCounter GetPerformanceCounter(
      string counterUri,
      string instanceName = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(counterUri, nameof (counterUri));
      KeyValue<Guid, int> keyValue;
      VssPerformanceCounterSet performanceCounterSet;
      if (VssPerformanceCounterManager.CounterUris.TryGetValue(counterUri, out keyValue) && VssPerformanceCounterManager.CounterSets.TryGetValue(keyValue.Key, out performanceCounterSet))
      {
        if (performanceCounterSet.IsMultiInstance)
        {
          if (!string.IsNullOrWhiteSpace(instanceName))
            return performanceCounterSet[keyValue.Value, instanceName, VssPerformanceCounterManager.ProcessInstanceName];
          if (VssPerformanceCounterManager.ThrowExceptions())
            throw new ArgumentException("'" + counterUri + "' is a multiinstance performance counter. Instance name must be specified.", nameof (instanceName));
          return new VssPerformanceCounter();
        }
        if (string.IsNullOrWhiteSpace(instanceName))
          return performanceCounterSet[keyValue.Value];
        if (VssPerformanceCounterManager.ThrowExceptions())
          throw new ArgumentException("'" + counterUri + "' is not a multiinstance performance counter. Instance name cannot be specified.", nameof (instanceName));
        return new VssPerformanceCounter();
      }
      if (!VssPerformanceCounterManager.ThrowExceptions() || VssPerformanceCounterManager.CounterSets.Count == 0)
        return new VssPerformanceCounter();
      throw new ArgumentException("Counter with uri '" + counterUri + "' has not been registered.", nameof (counterUri));
    }

    private static CounterType ParseCounterType(string counterType)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(counterType, nameof (counterType));
      string upper = counterType.ToUpper(CultureInfo.InvariantCulture);
      if (upper != null)
      {
        switch (upper.Length)
        {
          case 13:
            if (upper == "PERF_RAW_BASE")
              return CounterType.RawBase32;
            break;
          case 16:
            if (upper == "PERF_SAMPLE_BASE")
              return CounterType.SampleBase;
            break;
          case 17:
            switch (upper[15])
            {
              case 'L':
                if (upper == "PERF_AVERAGE_BULK")
                  return CounterType.AverageCount64;
                break;
              case 'M':
                if (upper == "PERF_ELAPSED_TIME")
                  return CounterType.ElapsedTime;
                break;
              case 'O':
                if (upper == "PERF_RAW_FRACTION")
                  return CounterType.RawFraction32;
                break;
              case 'S':
                if (upper == "PERF_AVERAGE_BASE")
                  return CounterType.AverageBase;
                break;
            }
            break;
          case 18:
            switch (upper[5])
            {
              case '1':
                if (upper == "PERF_100NSEC_TIMER")
                  return CounterType.PercentageActive100Ns;
                break;
              case 'A':
                if (upper == "PERF_AVERAGE_TIMER")
                  return CounterType.AverageTimer32;
                break;
              case 'C':
                switch (upper)
                {
                  case "PERF_COUNTER_DELTA":
                    return CounterType.Delta32;
                  case "PERF_COUNTER_TIMER":
                    return CounterType.PercentageActive;
                }
                break;
            }
            break;
          case 19:
            switch (upper[5])
            {
              case 'L':
                if (upper == "PERF_LARGE_RAW_BASE")
                  return CounterType.RawBase64;
                break;
              case 'O':
                if (upper == "PERF_OBJ_TIME_TIMER")
                  return CounterType.ObjectSpecificTimer;
                break;
              case 'S':
                if (upper == "PERF_SAMPLE_COUNTER")
                  return CounterType.SampleCounter;
                break;
            }
            break;
          case 20:
            switch (upper[5])
            {
              case 'C':
                if (upper == "PERF_COUNTER_COUNTER")
                  return CounterType.RateOfCountPerSecond32;
                break;
              case 'P':
                if (upper == "PERF_PRECISION_TIMER")
                  return CounterType.PrecisionSystemTimer;
                break;
              case 'S':
                if (upper == "PERF_SAMPLE_FRACTION")
                  return CounterType.SampleFraction;
                break;
            }
            break;
          case 21:
            if (upper == "PERF_COUNTER_RAWCOUNT")
              return CounterType.RawData32;
            break;
          case 22:
            switch (upper[5])
            {
              case '1':
                if (upper == "PERF_100NSEC_TIMER_INV")
                  return CounterType.PercentageNotActive100Ns;
                break;
              case 'C':
                if (upper == "PERF_COUNTER_TIMER_INV")
                  return CounterType.PercentageNotActive;
                break;
            }
            break;
          case 23:
            switch (upper[13])
            {
              case 'B':
                if (upper == "PERF_COUNTER_BULK_COUNT")
                  return CounterType.RateOfCountPerSecond64;
                break;
              case 'M':
                if (upper == "PERF_COUNTER_MULTI_BASE")
                  return CounterType.MultiTimerBase;
                break;
              case 'W':
                if (upper == "PERF_LARGE_RAW_FRACTION")
                  return CounterType.RawFraction64;
                break;
            }
            break;
          case 24:
            switch (upper[5])
            {
              case '1':
                if (upper == "PERF_100NSEC_MULTI_TIMER")
                  return CounterType.MultiTimerPercentageActive100Ns;
                break;
              case 'C':
                switch (upper)
                {
                  case "PERF_COUNTER_LARGE_DELTA":
                    return CounterType.Delta64;
                  case "PERF_COUNTER_MULTI_TIMER":
                    return CounterType.MultiTimerPercentageActive;
                }
                break;
            }
            break;
          case 25:
            if (upper == "PERF_COUNTER_RAWCOUNT_HEX")
              return CounterType.RawDataHex32;
            break;
          case 26:
            switch (upper[5])
            {
              case 'C':
                if (upper == "PERF_COUNTER_QUEUELEN_TYPE")
                  return CounterType.QueueLength;
                break;
              case 'P':
                if (upper == "PERF_PRECISION_100NS_TIMER")
                  return CounterType.PrecisionTimer100Ns;
                break;
            }
            break;
          case 27:
            switch (upper[5])
            {
              case 'C':
                if (upper == "PERF_COUNTER_LARGE_RAWCOUNT")
                  return CounterType.RawData64;
                break;
              case 'P':
                if (upper == "PERF_PRECISION_OBJECT_TIMER")
                  return CounterType.PrecisionObjectSpecificTimer;
                break;
            }
            break;
          case 28:
            switch (upper[5])
            {
              case '1':
                if (upper == "PERF_100NSEC_MULTI_TIMER_INV")
                  return CounterType.MultiTimerPercentageNotActive100Ns;
                break;
              case 'C':
                if (upper == "PERF_COUNTER_MULTI_TIMER_INV")
                  return CounterType.MultiTimerPercentageNotActive;
                break;
            }
            break;
          case 30:
            if (upper == "PERF_COUNTER_OBJ_QUEUELEN_TYPE")
              return CounterType.QueueLengthObjectTime;
            break;
          case 31:
            if (upper == "PERF_COUNTER_LARGE_RAWCOUNT_HEX")
              return CounterType.RawDataHex64;
            break;
          case 32:
            switch (upper[13])
            {
              case '1':
                if (upper == "PERF_COUNTER_100NS_QUEUELEN_TYPE")
                  return CounterType.QueueLength100Ns;
                break;
              case 'L':
                if (upper == "PERF_COUNTER_LARGE_QUEUELEN_TYPE")
                  return CounterType.LargeQueueLength;
                break;
            }
            break;
        }
      }
      throw new ArgumentException("Unknown counter type: " + counterType);
    }

    internal static bool ThrowExceptions()
    {
      if (VssPerformanceCounterManager._shouldThrowExceptions.HasValue)
        return VssPerformanceCounterManager._shouldThrowExceptions.Value;
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        return false;
      VssPerformanceCounterManager._shouldThrowExceptions = new bool?(((IDeploymentServiceHostInternal) TeamFoundationApplicationCore.DeploymentServiceHost).HostManagement.DeploymentType == DeploymentType.DevFabric);
      return VssPerformanceCounterManager._shouldThrowExceptions.Value;
    }

    private static string GetProcessInstanceName()
    {
      string processName = TFCommonUtil.CurrentProcess.ProcessName;
      if (!processName.StartsWith("w3wp", StringComparison.OrdinalIgnoreCase) && !processName.StartsWith("WaWorkerHost", StringComparison.OrdinalIgnoreCase) && !processName.StartsWith("TfsJobAgent", StringComparison.OrdinalIgnoreCase))
        return processName;
      int id = TFCommonUtil.CurrentProcess.Id;
      try
      {
        foreach (string instanceName in ((IEnumerable<string>) new PerformanceCounterCategory("Process").GetInstanceNames()).Where<string>((Func<string, bool>) (i => i.StartsWith(processName, StringComparison.OrdinalIgnoreCase))))
        {
          using (PerformanceCounter performanceCounter = new PerformanceCounter("Process", "ID Process", instanceName, true))
          {
            try
            {
              if ((int) performanceCounter.RawValue == id)
                return instanceName.Replace('#', '_');
            }
            catch
            {
            }
          }
        }
      }
      catch
      {
        return id.ToString();
      }
      return processName;
    }
  }
}
