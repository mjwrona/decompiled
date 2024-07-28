// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.InstrumentationManifest
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public sealed class InstrumentationManifest
  {
    private XDocument manifestXml;
    private XmlNamespaceManager manifestNamespaces;
    private IEnumerable<XElement> counterSets;

    public InstrumentationManifest()
      : this(Assembly.GetCallingAssembly())
    {
    }

    public InstrumentationManifest(string manifestPath)
    {
      this.SourceFile = !string.IsNullOrEmpty(manifestPath) ? manifestPath : throw new ArgumentNullException("path");
      this.ManifestName = Path.GetFileName(manifestPath);
      this.ReadManifestXml(File.ReadAllText(manifestPath));
    }

    internal InstrumentationManifest(Assembly assembly)
    {
      this.SourceAssembly = !(assembly == (Assembly) null) ? assembly : throw new ArgumentNullException(nameof (assembly));
      this.ManifestName = InstrumentationManifest.GetEmbeddedInstrumentationManifestName(assembly);
      this.ReadManifestXml(InstrumentationManifest.ReadEmbeddedInstrumentationManifest(assembly, this.ManifestName));
    }

    internal InstrumentationManifest(string manifestName, string manifestContent)
    {
      this.ManifestName = manifestName;
      this.ReadManifestXml(manifestContent);
    }

    public Guid CounterProviderGuid { get; private set; }

    public string CounterProviderName { get; private set; }

    public IEnumerable<string> CounterSetNames { get; private set; }

    public bool HasCounters { get; private set; }

    public bool HasEvents { get; private set; }

    public string ManifestName { get; private set; }

    public Assembly SourceAssembly { get; private set; }

    public string SourceFile { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.ManifestName);
      if (this.HasCounters)
        stringBuilder.AppendFormat("HasCounters, ProviderName={0}, ProviderGuid={1}", (object) this.CounterProviderName, (object) this.CounterProviderGuid);
      if (this.HasEvents)
        stringBuilder.AppendFormat("HasEvents");
      return stringBuilder.ToString();
    }

    internal IPerformanceDataFacade InitializeCounterFacade(bool enableRealCounters)
    {
      IPerformanceDataFacade performanceDataFacade = !enableRealCounters ? (IPerformanceDataFacade) new NoopPerformanceDataFacade() : (IPerformanceDataFacade) PerformanceDataFacade.GetExistingOrCreate(this.CounterProviderGuid);
      if (!performanceDataFacade.IsInitialized)
      {
        XDocument manifestXml = this.manifestXml;
        XmlNamespaceManager manifestNamespaces = this.manifestNamespaces;
        IEnumerable<XElement> counterSets = this.counterSets;
        Dictionary<string, CounterSetInstanceType> dictionary1 = new Dictionary<string, CounterSetInstanceType>()
        {
          {
            "globalAggregate",
            CounterSetInstanceType.GlobalAggregate
          },
          {
            "multipleAggregate",
            CounterSetInstanceType.MultipleAggregate
          },
          {
            "multiple",
            CounterSetInstanceType.Multiple
          },
          {
            "single",
            CounterSetInstanceType.Single
          }
        };
        Dictionary<string, CounterType> dictionary2 = new Dictionary<string, CounterType>()
        {
          {
            "perf_counter_rawcount_hex",
            CounterType.RawDataHex32
          },
          {
            "perf_counter_large_rawcount_hex",
            CounterType.RawDataHex64
          },
          {
            "perf_counter_rawcount",
            CounterType.RawData32
          },
          {
            "perf_counter_large_rawcount",
            CounterType.RawData64
          },
          {
            "perf_counter_delta",
            CounterType.Delta32
          },
          {
            "perf_counter_large_delta",
            CounterType.Delta64
          },
          {
            "perf_sample_counter",
            CounterType.SampleCounter
          },
          {
            "perf_counter_queuelen_type",
            CounterType.QueueLength
          },
          {
            "perf_counter_large_queuelen_type",
            CounterType.LargeQueueLength
          },
          {
            "perf_counter_100ns_queuelen_type",
            CounterType.QueueLength100Ns
          },
          {
            "perf_counter_obj_queuelen_type",
            CounterType.QueueLengthObjectTime
          },
          {
            "perf_counter_counter",
            CounterType.RateOfCountPerSecond32
          },
          {
            "perf_counter_bulk_count",
            CounterType.RateOfCountPerSecond64
          },
          {
            "perf_raw_fraction",
            CounterType.RawFraction32
          },
          {
            "perf_counter_timer",
            CounterType.PercentageActive
          },
          {
            "perf_precision_timer",
            CounterType.PrecisionSystemTimer
          },
          {
            "perf_100nsec_timer",
            CounterType.PercentageActive100Ns
          },
          {
            "perf_precision_100ns_timer",
            CounterType.PrecisionTimer100Ns
          },
          {
            "perf_obj_time_timer",
            CounterType.ObjectSpecificTimer
          },
          {
            "perf_precision_object_timer",
            CounterType.PrecisionObjectSpecificTimer
          },
          {
            "perf_sample_fraction",
            CounterType.SampleFraction
          },
          {
            "perf_counter_timer_inv",
            CounterType.PercentageNotActive
          },
          {
            "perf_100nsec_timer_inv",
            CounterType.PercentageNotActive100Ns
          },
          {
            "perf_counter_multi_timer",
            CounterType.MultiTimerPercentageActive
          },
          {
            "perf_100nsec_multi_timer",
            CounterType.MultiTimerPercentageActive100Ns
          },
          {
            "perf_counter_multi_timer_inv",
            CounterType.MultiTimerPercentageNotActive
          },
          {
            "perf_100nsec_multi_timer_inv",
            CounterType.MultiTimerPercentageNotActive100Ns
          },
          {
            "perf_average_timer",
            CounterType.AverageTimer32
          },
          {
            "perf_elapsed_time",
            CounterType.ElapsedTime
          },
          {
            "perf_average_bulk",
            CounterType.AverageCount64
          },
          {
            "perf_average_base",
            CounterType.AverageBase
          },
          {
            "perf_raw_base",
            CounterType.RawBase32
          },
          {
            "perf_large_raw_base",
            CounterType.RawBase64
          }
        };
        foreach (XElement node in counterSets)
        {
          Guid counterSetGuid = new Guid(node.Attribute((XName) "guid").Value.Trim('{', '}'));
          string counterSetName = node.Attribute((XName) "name").Value;
          string key = node.Attribute((XName) "instances").Value;
          CounterSetInstanceType instanceType = dictionary1[key];
          performanceDataFacade.RegisterCounterSet(counterSetGuid, counterSetName, instanceType);
          IEnumerable<XElement> xelements = node.XPathSelectElements("c:counter", (IXmlNamespaceResolver) manifestNamespaces);
          if (xelements == null)
            throw new InstrumentationManifestException(string.Format("<counterSet name=\"{0}\"> doesn't contain <counter>s", (object) counterSetName));
          foreach (XElement xelement in xelements)
          {
            int counterId = int.Parse(xelement.Attribute((XName) "id").Value);
            string counterName = xelement.Attribute((XName) "name").Value;
            CounterType counterType = dictionary2[xelement.Attribute((XName) "type").Value.ToLower()];
            int baseId = 0;
            XAttribute xattribute = xelement.Attribute((XName) "baseID");
            if (xattribute != null)
              baseId = int.Parse(xattribute.Value);
            performanceDataFacade.AddCounterToSet(counterSetGuid, counterId, counterType, counterName, baseId);
          }
          performanceDataFacade.CreateCounterSetInstance(counterSetGuid);
        }
        performanceDataFacade.CompleteInitialization();
      }
      return performanceDataFacade;
    }

    private static string GetEmbeddedInstrumentationManifestName(Assembly assembly)
    {
      string[] manifestResourceNames = assembly.GetManifestResourceNames();
      Regex manifestResourceNameRegex = new Regex(".man(.template)?$", RegexOptions.IgnoreCase);
      Func<string, bool> predicate = (Func<string, bool>) (n => manifestResourceNameRegex.IsMatch(n));
      string[] array = ((IEnumerable<string>) manifestResourceNames).Where<string>(predicate).ToArray<string>();
      if (array.Length == 0)
        throw new ArgumentException(string.Format("Assembly \"{0}\" doesn't contain an instrumentationManifest as an embedded resource ending in .man.template. We require this manifest to be embedded in the assembly because it allows the native resources (which Windows requires for PerfLib v2 counter registration) to be verified for consistency.", (object) assembly));
      return array.Length <= 1 ? ((IEnumerable<string>) array).Single<string>() : throw new NotSupportedException(string.Format("Assembly \"{0}\" contains multiple instrumentationManifests as embedded resources, and this class cannot disambiguate them.", (object) assembly));
    }

    private static string ReadEmbeddedInstrumentationManifest(
      Assembly assembly,
      string manifestResourceName)
    {
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(manifestResourceName))
      {
        using (StreamReader streamReader = new StreamReader(manifestResourceStream))
          return streamReader.ReadToEnd();
      }
    }

    private void ReadManifestXml(string manifestContent)
    {
      this.manifestXml = XDocument.Parse(manifestContent, LoadOptions.SetLineInfo);
      this.manifestNamespaces = new XmlNamespaceManager((XmlNameTable) new NameTable());
      this.manifestNamespaces.AddNamespace("e", "http://schemas.microsoft.com/win/2004/08/events");
      this.manifestNamespaces.AddNamespace("c", "http://schemas.microsoft.com/win/2005/12/counters");
      XElement node1 = this.manifestXml.Root.XPathSelectElement("/e:instrumentationManifest/e:instrumentation/c:counters", (IXmlNamespaceResolver) this.manifestNamespaces);
      if (node1 != null)
      {
        this.HasCounters = true;
        XElement node2 = node1.XPathSelectElement("c:provider", (IXmlNamespaceResolver) this.manifestNamespaces);
        this.CounterProviderGuid = node2 != null ? new Guid(node2.Attribute((XName) "providerGuid").Value.Trim('{', '}')) : throw new InstrumentationManifestException("<counters> doesn't contain <provider>");
        this.CounterProviderName = node2.Attribute((XName) "providerName").Value;
        IEnumerable<XElement> source = node2.XPathSelectElements("c:counterSet", (IXmlNamespaceResolver) this.manifestNamespaces);
        this.counterSets = source != null ? source : throw new InstrumentationManifestException("<provider> doesn't contain any <counterSet>s");
        this.CounterSetNames = (IEnumerable<string>) new HashSet<string>((IEnumerable<string>) source.Select<XElement, string>((Func<XElement, string>) (s => s.Attribute((XName) "name").Value)).OrderBy<string, string>((Func<string, string>) (s => s)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      if (this.manifestXml.Root.XPathSelectElement("/e:instrumentationManifest/e:instrumentation/e:events", (IXmlNamespaceResolver) this.manifestNamespaces) == null)
        return;
      this.HasEvents = true;
    }
  }
}
