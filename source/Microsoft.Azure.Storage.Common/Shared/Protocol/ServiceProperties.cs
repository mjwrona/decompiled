// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.ServiceProperties
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public sealed class ServiceProperties
  {
    internal const string StorageServicePropertiesName = "StorageServiceProperties";
    internal const string LoggingName = "Logging";
    internal const string HourMetricsName = "HourMetrics";
    internal const string CorsName = "Cors";
    internal const string MinuteMetricsName = "MinuteMetrics";
    internal const string DeleteRetentionPolicyName = "DeleteRetentionPolicy";
    internal const string VersionName = "Version";
    internal const string DeleteName = "Delete";
    internal const string ReadName = "Read";
    internal const string WriteName = "Write";
    internal const string RetentionPolicyName = "RetentionPolicy";
    internal const string EnabledName = "Enabled";
    internal const string DaysName = "Days";
    internal const string IncludeApisName = "IncludeAPIs";
    internal const string DefaultServiceVersionName = "DefaultServiceVersion";
    internal const string CorsRuleName = "CorsRule";
    internal const string AllowedOriginsName = "AllowedOrigins";
    internal const string AllowedMethodsName = "AllowedMethods";
    internal const string MaxAgeInSecondsName = "MaxAgeInSeconds";
    internal const string ExposedHeadersName = "ExposedHeaders";
    internal const string AllowedHeadersName = "AllowedHeaders";
    internal const string StaticWebsiteName = "StaticWebsite";
    internal const string StaticWebsiteEnabledName = "Enabled";
    internal const string StaticWebsiteIndexDocumentName = "IndexDocument";
    internal const string StaticWebsiteErrorDocument404PathName = "ErrorDocument404Path";
    internal const string RetainedVersionsPerBlob = "RetainedVersionsPerBlob";

    public ServiceProperties()
    {
    }

    public ServiceProperties(
      LoggingProperties logging = null,
      MetricsProperties hourMetrics = null,
      MetricsProperties minuteMetrics = null,
      CorsProperties cors = null,
      DeleteRetentionPolicy deleteRetentionPolicy = null)
      : this(logging, hourMetrics, minuteMetrics, cors, deleteRetentionPolicy, (StaticWebsiteProperties) null)
    {
    }

    public ServiceProperties(
      LoggingProperties logging,
      MetricsProperties hourMetrics,
      MetricsProperties minuteMetrics,
      CorsProperties cors,
      DeleteRetentionPolicy deleteRetentionPolicy,
      StaticWebsiteProperties staticWebsite = null)
    {
      this.Logging = logging;
      this.HourMetrics = hourMetrics;
      this.MinuteMetrics = minuteMetrics;
      this.Cors = cors;
      this.DeleteRetentionPolicy = deleteRetentionPolicy;
      this.StaticWebsite = staticWebsite;
    }

    public LoggingProperties Logging { get; set; }

    public MetricsProperties HourMetrics { get; set; }

    public CorsProperties Cors { get; set; }

    public MetricsProperties MinuteMetrics { get; set; }

    public string DefaultServiceVersion { get; set; }

    public DeleteRetentionPolicy DeleteRetentionPolicy { get; set; }

    public StaticWebsiteProperties StaticWebsite { get; set; }

    internal static ServiceProperties FromServiceXml(XDocument servicePropertiesDocument)
    {
      XElement xelement1 = servicePropertiesDocument.Element((XName) "StorageServiceProperties");
      ServiceProperties serviceProperties = new ServiceProperties()
      {
        Logging = ServiceProperties.ReadLoggingPropertiesFromXml(xelement1.Element((XName) "Logging")),
        HourMetrics = ServiceProperties.ReadMetricsPropertiesFromXml(xelement1.Element((XName) "HourMetrics")),
        MinuteMetrics = ServiceProperties.ReadMetricsPropertiesFromXml(xelement1.Element((XName) "MinuteMetrics")),
        Cors = ServiceProperties.ReadCorsPropertiesFromXml(xelement1.Element((XName) "Cors")),
        DeleteRetentionPolicy = ServiceProperties.ReadDeleteRetentionPolicyFromXml(xelement1.Element((XName) "DeleteRetentionPolicy")),
        StaticWebsite = ServiceProperties.ReadStaticWebsitePropertiesFromXml(xelement1.Element((XName) "StaticWebsite"))
      };
      XElement xelement2 = xelement1.Element((XName) "DefaultServiceVersion");
      if (xelement2 != null)
        serviceProperties.DefaultServiceVersion = xelement2.Value;
      return serviceProperties;
    }

    internal XDocument ToServiceXml()
    {
      if (this.Logging == null && this.HourMetrics == null && this.MinuteMetrics == null && this.Cors == null && this.DeleteRetentionPolicy == null && this.DefaultServiceVersion == null && this.StaticWebsite == null)
        throw new InvalidOperationException("At least one service property needs to be non-null for SetServiceProperties API.");
      XElement xelement = new XElement((XName) "StorageServiceProperties");
      if (this.Logging != null)
        xelement.Add((object) ServiceProperties.GenerateLoggingXml(this.Logging));
      if (this.HourMetrics != null)
        xelement.Add((object) ServiceProperties.GenerateMetricsXml(this.HourMetrics, "HourMetrics"));
      if (this.MinuteMetrics != null)
        xelement.Add((object) ServiceProperties.GenerateMetricsXml(this.MinuteMetrics, "MinuteMetrics"));
      if (this.Cors != null)
        xelement.Add((object) ServiceProperties.GenerateCorsXml(this.Cors));
      if (this.DefaultServiceVersion != null)
        xelement.Add((object) new XElement((XName) "DefaultServiceVersion", (object) this.DefaultServiceVersion));
      if (this.DeleteRetentionPolicy != null)
        xelement.Add((object) ServiceProperties.GenerateDeleteRetentionPolicyXml(this.DeleteRetentionPolicy));
      if (this.StaticWebsite != null)
        xelement.Add((object) ServiceProperties.GenerateStaticWebsitePropertiesXml(this.StaticWebsite));
      return new XDocument(new object[1]
      {
        (object) xelement
      });
    }

    private static XElement GenerateRetentionPolicyXml(int? retentionDays)
    {
      bool hasValue = retentionDays.HasValue;
      XElement retentionPolicyXml = new XElement((XName) "RetentionPolicy", (object) new XElement((XName) "Enabled", (object) hasValue));
      if (hasValue)
        retentionPolicyXml.Add((object) new XElement((XName) "Days", (object) retentionDays.Value));
      return retentionPolicyXml;
    }

    private static XElement GenerateMetricsXml(MetricsProperties metrics, string metricsName)
    {
      if (!Enum.IsDefined(typeof (MetricsLevel), (object) metrics.MetricsLevel))
        throw new InvalidOperationException("Invalid metrics level specified.");
      if (string.IsNullOrEmpty(metrics.Version))
        throw new InvalidOperationException("The metrics version is null or empty.");
      bool content = metrics.MetricsLevel != 0;
      XElement metricsXml = new XElement((XName) metricsName, new object[3]
      {
        (object) new XElement((XName) "Version", (object) metrics.Version),
        (object) new XElement((XName) "Enabled", (object) content),
        (object) ServiceProperties.GenerateRetentionPolicyXml(metrics.RetentionDays)
      });
      if (content)
        metricsXml.Add((object) new XElement((XName) "IncludeAPIs", (object) (metrics.MetricsLevel == MetricsLevel.ServiceAndApi)));
      return metricsXml;
    }

    private static XElement GenerateLoggingXml(LoggingProperties logging)
    {
      if ((LoggingOperations.All & logging.LoggingOperations) != logging.LoggingOperations)
        throw new InvalidOperationException("Invalid logging operations specified.");
      if (string.IsNullOrEmpty(logging.Version))
        throw new InvalidOperationException("The logging version is null or empty.");
      return new XElement((XName) "Logging", new object[5]
      {
        (object) new XElement((XName) "Version", (object) logging.Version),
        (object) new XElement((XName) "Delete", (object) ((logging.LoggingOperations & LoggingOperations.Delete) != 0)),
        (object) new XElement((XName) "Read", (object) ((logging.LoggingOperations & LoggingOperations.Read) != 0)),
        (object) new XElement((XName) "Write", (object) ((logging.LoggingOperations & LoggingOperations.Write) != 0)),
        (object) ServiceProperties.GenerateRetentionPolicyXml(logging.RetentionDays)
      });
    }

    private static XElement GenerateCorsXml(CorsProperties cors)
    {
      CommonUtility.AssertNotNull(nameof (cors), (object) cors);
      IList<CorsRule> corsRules = cors.CorsRules;
      XElement corsXml = new XElement((XName) "Cors");
      foreach (CorsRule corsRule in (IEnumerable<CorsRule>) corsRules)
      {
        if (corsRule.AllowedOrigins.Count < 1 || corsRule.AllowedMethods == CorsHttpMethods.None || corsRule.MaxAgeInSeconds < 0)
          throw new InvalidOperationException("A CORS rule must contain at least one allowed origin and allowed method, and MaxAgeInSeconds cannot have a value less than zero.");
        XElement content = new XElement((XName) "CorsRule", new object[5]
        {
          (object) new XElement((XName) "AllowedOrigins", (object) string.Join(",", corsRule.AllowedOrigins.ToArray<string>())),
          (object) new XElement((XName) "AllowedMethods", (object) corsRule.AllowedMethods.ToString().Replace(" ", string.Empty).ToUpperInvariant()),
          (object) new XElement((XName) "ExposedHeaders", (object) string.Join(",", corsRule.ExposedHeaders.ToArray<string>())),
          (object) new XElement((XName) "AllowedHeaders", (object) string.Join(",", corsRule.AllowedHeaders.ToArray<string>())),
          (object) new XElement((XName) "MaxAgeInSeconds", (object) corsRule.MaxAgeInSeconds)
        });
        corsXml.Add((object) content);
      }
      return corsXml;
    }

    private static XElement GenerateDeleteRetentionPolicyXml(
      DeleteRetentionPolicy deleteRetentionPolicy)
    {
      CommonUtility.AssertNotNull(nameof (deleteRetentionPolicy), (object) deleteRetentionPolicy);
      bool enabled = deleteRetentionPolicy.Enabled;
      XElement retentionPolicyXml = new XElement((XName) "DeleteRetentionPolicy", (object) new XElement((XName) "Enabled", (object) enabled));
      if (!enabled)
        return retentionPolicyXml;
      if (deleteRetentionPolicy.RetentionDays.HasValue && deleteRetentionPolicy.RetentionDays.Value >= 1)
      {
        int? retentionDays = deleteRetentionPolicy.RetentionDays;
        int allowedRetentionDays = Constants.MaximumAllowedRetentionDays;
        if (!(retentionDays.GetValueOrDefault() > allowedRetentionDays & retentionDays.HasValue))
        {
          XElement xelement = retentionPolicyXml;
          XName name = (XName) "Days";
          retentionDays = deleteRetentionPolicy.RetentionDays;
          // ISSUE: variable of a boxed type
          __Boxed<int> content1 = (ValueType) retentionDays.Value;
          XElement content2 = new XElement(name, (object) content1);
          xelement.Add((object) content2);
          return retentionPolicyXml;
        }
      }
      throw new ArgumentException("The delete retention policy is enabled but the RetentionDays property is not specified or has an invalid value. RetentionDays must be greater than 0 and less than or equal to 365 days.");
    }

    private static XElement GenerateStaticWebsitePropertiesXml(
      StaticWebsiteProperties staticWebsiteProperties)
    {
      CommonUtility.AssertNotNull(nameof (staticWebsiteProperties), (object) staticWebsiteProperties);
      bool enabled = staticWebsiteProperties.Enabled;
      XElement websitePropertiesXml = new XElement((XName) "StaticWebsite", (object) new XElement((XName) "Enabled", (object) enabled));
      if (!enabled)
        return websitePropertiesXml;
      if (!string.IsNullOrWhiteSpace(staticWebsiteProperties.IndexDocument))
        websitePropertiesXml.Add((object) new XElement((XName) "IndexDocument", (object) staticWebsiteProperties.IndexDocument));
      if (!string.IsNullOrWhiteSpace(staticWebsiteProperties.ErrorDocument404Path))
        websitePropertiesXml.Add((object) new XElement((XName) "ErrorDocument404Path", (object) staticWebsiteProperties.ErrorDocument404Path));
      return websitePropertiesXml;
    }

    private static LoggingProperties ReadLoggingPropertiesFromXml(XElement element)
    {
      if (element == null)
        return (LoggingProperties) null;
      LoggingOperations loggingOperations = LoggingOperations.None;
      if (bool.Parse(element.Element((XName) "Delete").Value))
        loggingOperations |= LoggingOperations.Delete;
      if (bool.Parse(element.Element((XName) "Read").Value))
        loggingOperations |= LoggingOperations.Read;
      if (bool.Parse(element.Element((XName) "Write").Value))
        loggingOperations |= LoggingOperations.Write;
      return new LoggingProperties()
      {
        Version = element.Element((XName) "Version").Value,
        LoggingOperations = loggingOperations,
        RetentionDays = ServiceProperties.ReadRetentionPolicyFromXml(element.Element((XName) "RetentionPolicy"))
      };
    }

    internal static MetricsProperties ReadMetricsPropertiesFromXml(XElement element)
    {
      if (element == null)
        return (MetricsProperties) null;
      MetricsLevel metricsLevel = MetricsLevel.None;
      if (bool.Parse(element.Element((XName) "Enabled").Value))
      {
        metricsLevel = MetricsLevel.Service;
        if (bool.Parse(element.Element((XName) "IncludeAPIs").Value))
          metricsLevel = MetricsLevel.ServiceAndApi;
      }
      return new MetricsProperties()
      {
        Version = element.Element((XName) "Version").Value,
        MetricsLevel = metricsLevel,
        RetentionDays = ServiceProperties.ReadRetentionPolicyFromXml(element.Element((XName) "RetentionPolicy"))
      };
    }

    private static int? ReadRetentionPolicyFromXml(XElement element) => !bool.Parse(element.Element((XName) "Enabled").Value) ? new int?() : new int?(int.Parse(element.Element((XName) "Days").Value, (IFormatProvider) CultureInfo.InvariantCulture));

    internal static CorsProperties ReadCorsPropertiesFromXml(XElement element)
    {
      if (element == null)
        return (CorsProperties) null;
      return new CorsProperties()
      {
        CorsRules = (IList<CorsRule>) element.Descendants((XName) "CorsRule").Select<XElement, CorsRule>((Func<XElement, CorsRule>) (rule => new CorsRule()
        {
          AllowedOrigins = (IList<string>) ((IEnumerable<string>) rule.Element((XName) "AllowedOrigins").Value.Split(',')).ToList<string>(),
          AllowedMethods = (CorsHttpMethods) Enum.Parse(typeof (CorsHttpMethods), rule.Element((XName) "AllowedMethods").Value, true),
          AllowedHeaders = (IList<string>) ((IEnumerable<string>) rule.Element((XName) "AllowedHeaders").Value.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>(),
          ExposedHeaders = (IList<string>) ((IEnumerable<string>) rule.Element((XName) "ExposedHeaders").Value.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>(),
          MaxAgeInSeconds = int.Parse(rule.Element((XName) "MaxAgeInSeconds").Value, (IFormatProvider) CultureInfo.InvariantCulture)
        })).ToList<CorsRule>()
      };
    }

    internal static DeleteRetentionPolicy ReadDeleteRetentionPolicyFromXml(XElement element)
    {
      if (element == null)
        return (DeleteRetentionPolicy) null;
      DeleteRetentionPolicy deleteRetentionPolicy = new DeleteRetentionPolicy()
      {
        Enabled = false,
        RetentionDays = new int?()
      };
      deleteRetentionPolicy.Enabled = bool.Parse(element.Element((XName) "Enabled").Value);
      if (deleteRetentionPolicy.Enabled)
        deleteRetentionPolicy.RetentionDays = new int?(int.Parse(element.Element((XName) "Days").Value, (IFormatProvider) CultureInfo.InvariantCulture));
      return deleteRetentionPolicy;
    }

    internal static StaticWebsiteProperties ReadStaticWebsitePropertiesFromXml(XElement element)
    {
      if (element == null)
        return (StaticWebsiteProperties) null;
      XElement xelement1 = element.Element((XName) "Enabled");
      bool flag = xelement1 != null && bool.Parse(xelement1.Value);
      XElement xelement2 = element.Element((XName) "IndexDocument");
      XElement xelement3 = element.Element((XName) "ErrorDocument404Path");
      return new StaticWebsiteProperties()
      {
        Enabled = flag,
        IndexDocument = !flag || xelement2 == null ? (string) null : xelement2.Value,
        ErrorDocument404Path = !flag || xelement3 == null ? (string) null : xelement3.Value
      };
    }

    internal void WriteServiceProperties(Stream outputStream)
    {
      using (XmlWriter writer = XmlWriter.Create(outputStream, new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8,
        NewLineHandling = NewLineHandling.Entitize
      }))
        this.ToServiceXml().Save(writer);
    }
  }
}
