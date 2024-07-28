// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.XmlDictionaryReaderQuotasElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public sealed class XmlDictionaryReaderQuotasElement : ConfigurationElement
  {
    private ConfigurationPropertyCollection properties;

    [ConfigurationProperty("maxDepth", DefaultValue = 0)]
    [IntegerValidator(MinValue = 0)]
    public int MaxDepth
    {
      get => (int) this["maxDepth"];
      set => this["maxDepth"] = (object) value;
    }

    [ConfigurationProperty("maxStringContentLength", DefaultValue = 0)]
    [IntegerValidator(MinValue = 0)]
    public int MaxStringContentLength
    {
      get => (int) this["maxStringContentLength"];
      set => this["maxStringContentLength"] = (object) value;
    }

    [ConfigurationProperty("maxArrayLength", DefaultValue = 0)]
    [IntegerValidator(MinValue = 0)]
    public int MaxArrayLength
    {
      get => (int) this["maxArrayLength"];
      set => this["maxArrayLength"] = (object) value;
    }

    [ConfigurationProperty("maxBytesPerRead", DefaultValue = 0)]
    [IntegerValidator(MinValue = 0)]
    public int MaxBytesPerRead
    {
      get => (int) this["maxBytesPerRead"];
      set => this["maxBytesPerRead"] = (object) value;
    }

    [ConfigurationProperty("maxNameTableCharCount", DefaultValue = 0)]
    [IntegerValidator(MinValue = 0)]
    public int MaxNameTableCharCount
    {
      get => (int) this["maxNameTableCharCount"];
      set => this["maxNameTableCharCount"] = (object) value;
    }

    internal void ApplyConfiguration(XmlDictionaryReaderQuotas readerQuotas)
    {
      if (readerQuotas == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (readerQuotas));
      if (this.MaxDepth != 0)
        readerQuotas.MaxDepth = this.MaxDepth;
      if (this.MaxStringContentLength != 0)
        readerQuotas.MaxStringContentLength = this.MaxStringContentLength;
      if (this.MaxArrayLength != 0)
        readerQuotas.MaxArrayLength = this.MaxArrayLength;
      if (this.MaxBytesPerRead != 0)
        readerQuotas.MaxBytesPerRead = this.MaxBytesPerRead;
      if (this.MaxNameTableCharCount == 0)
        return;
      readerQuotas.MaxNameTableCharCount = this.MaxNameTableCharCount;
    }

    internal void InitializeFrom(XmlDictionaryReaderQuotas readerQuotas)
    {
      this.MaxDepth = readerQuotas != null ? readerQuotas.MaxDepth : throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (readerQuotas));
      this.MaxStringContentLength = readerQuotas.MaxStringContentLength;
      this.MaxArrayLength = readerQuotas.MaxArrayLength;
      this.MaxBytesPerRead = readerQuotas.MaxBytesPerRead;
      this.MaxNameTableCharCount = readerQuotas.MaxNameTableCharCount;
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = new ConfigurationPropertyCollection()
          {
            new ConfigurationProperty("maxDepth", typeof (int), (object) 0, (TypeConverter) null, (ConfigurationValidatorBase) new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
            new ConfigurationProperty("maxStringContentLength", typeof (int), (object) 0, (TypeConverter) null, (ConfigurationValidatorBase) new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
            new ConfigurationProperty("maxArrayLength", typeof (int), (object) 0, (TypeConverter) null, (ConfigurationValidatorBase) new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
            new ConfigurationProperty("maxBytesPerRead", typeof (int), (object) 0, (TypeConverter) null, (ConfigurationValidatorBase) new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None),
            new ConfigurationProperty("maxNameTableCharCount", typeof (int), (object) 0, (TypeConverter) null, (ConfigurationValidatorBase) new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None)
          };
        return this.properties;
      }
    }
  }
}
