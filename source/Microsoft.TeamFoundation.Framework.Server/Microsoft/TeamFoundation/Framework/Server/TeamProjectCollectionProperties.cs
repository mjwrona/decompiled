// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamProjectCollectionProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Private)]
  public class TeamProjectCollectionProperties
  {
    private IDictionary<string, string> m_extendedSettings = (IDictionary<string, string>) new Dictionary<string, string>();
    private List<ServicingJobDetail> m_servicingDetails = new List<ServicingJobDetail>();
    private static readonly string[] m_ignoredServicingOperationClasses = new string[2]
    {
      "CreateProject",
      "DeleteProject"
    };
    private static readonly ServicingJobStatus[] m_blockingStatuses = new ServicingJobStatus[2]
    {
      ServicingJobStatus.Queued,
      ServicingJobStatus.Running
    };

    public TeamProjectCollectionProperties()
    {
    }

    internal TeamProjectCollectionProperties(
      HostProperties serviceHostProperties,
      ISqlConnectionInfo frameworkConnectionInfo)
    {
      this.Id = serviceHostProperties.Id;
      this.Name = serviceHostProperties.Name;
      this.Description = serviceHostProperties.Description;
      this.StateValue = (int) serviceHostProperties.Status;
      this.Registered = serviceHostProperties.Registered;
      this.IsDefault = false;
      this.DatabaseId = serviceHostProperties.DatabaseId;
      this.VirtualDirectory = "~/" + serviceHostProperties.Name + "/";
      this.m_servicingDetails = new List<ServicingJobDetail>();
      this.FrameworkConnectionInfo = frameworkConnectionInfo;
    }

    public TeamProjectCollectionProperties(TeamProjectCollectionProperties properties)
    {
      this.Id = properties.Id;
      this.Name = properties.Name;
      this.Description = properties.Description;
      this.StateValue = properties.StateValue;
      this.Registered = properties.Registered;
      this.IsDefault = properties.IsDefault;
      this.DatabaseId = properties.DatabaseId;
      this.VirtualDirectory = properties.VirtualDirectory;
      this.FrameworkConnectionInfo = properties.FrameworkConnectionInfo;
      this.Authority = properties.Authority;
      this.DatabaseCategoryConnectionStringsValue = properties.DatabaseCategoryConnectionStringsValue;
      this.DefaultConnectionInfo = properties.DefaultConnectionInfo;
      this.DefaultConnectionString = properties.DefaultConnectionString;
      this.State = properties.State;
      this.m_servicingDetails = properties.ServicingDetails;
    }

    internal TeamProjectCollectionProperties(
      TeamFoundationServiceHostProperties serviceHostProperties,
      ISqlConnectionInfo frameworkConnectionInfo,
      bool isDefault)
      : this((HostProperties) serviceHostProperties, frameworkConnectionInfo)
    {
      this.IsDefault = isDefault;
      this.m_servicingDetails = serviceHostProperties.ServicingDetails;
    }

    [XmlAttribute("id")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public Guid Id { get; set; }

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute("default")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool IsDefault { get; set; }

    [XmlAttribute("registered")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public bool Registered { get; set; }

    [XmlAttribute("authority")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Authority { get; set; }

    [XmlAttribute("vdir")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string VirtualDirectory { get; set; }

    internal ISqlConnectionInfo DefaultConnectionInfo { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string DefaultConnectionString
    {
      get => this.DefaultConnectionInfo != null ? this.DefaultConnectionInfo.ConnectionString : (string) null;
      set => this.DefaultConnectionInfo = SqlConnectionInfoFactory.Create(value);
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int DatabaseId { get; set; }

    [XmlAttribute("state")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int StateValue { get; set; }

    [XmlIgnore]
    public TeamFoundationServiceHostStatus State
    {
      get => (TeamFoundationServiceHostStatus) this.StateValue;
      set => this.StateValue = (int) value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public List<ServicingJobDetail> ServicingDetails => this.m_servicingDetails;

    [XmlElement("ServicingTokens", typeof (KeyValue<string, string>[]))]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public KeyValue<string, string>[] ServicingTokensValue
    {
      get
      {
        List<KeyValue<string, string>> keyValueList = new List<KeyValue<string, string>>();
        KeyValue<string, string>[] servicingTokensValue = new KeyValue<string, string>[this.m_extendedSettings.Count];
        int num = 0;
        foreach (KeyValuePair<string, string> extendedSetting in (IEnumerable<KeyValuePair<string, string>>) this.m_extendedSettings)
          servicingTokensValue[num++] = new KeyValue<string, string>(extendedSetting);
        return servicingTokensValue;
      }
      set
      {
        this.m_extendedSettings = (IDictionary<string, string>) new Dictionary<string, string>();
        if (value == null)
          return;
        foreach (KeyValue<string, string> keyValue in value)
          this.m_extendedSettings.Add(keyValue.Key, keyValue.Value);
      }
    }

    [XmlElement("DatabaseCategoryConnectionStrings", typeof (KeyValue<string, string>[]))]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public KeyValue<string, string>[] DatabaseCategoryConnectionStringsValue
    {
      get
      {
        string connectionString = this.DefaultConnectionString;
        if (connectionString == null)
          return new KeyValue<string, string>[0];
        return new KeyValue<string, string>[1]
        {
          new KeyValue<string, string>("Framework", connectionString)
        };
      }
      set
      {
        if (this.DefaultConnectionString != null || value == null)
          return;
        foreach (KeyValue<string, string> keyValue in value)
        {
          if (VssStringComparer.DatabaseCategory.Equals(keyValue.Key, "Framework"))
            this.DefaultConnectionString = keyValue.Value;
        }
      }
    }

    [XmlIgnore]
    public bool IsBeingServiced => this.m_servicingDetails != null && this.m_servicingDetails.Any<ServicingJobDetail>((Func<ServicingJobDetail, bool>) (servicingJobDetail => ((IEnumerable<ServicingJobStatus>) TeamProjectCollectionProperties.m_blockingStatuses).Contains<ServicingJobStatus>(servicingJobDetail.JobStatus) && !((IEnumerable<string>) TeamProjectCollectionProperties.m_ignoredServicingOperationClasses).Contains<string>(servicingJobDetail.OperationClass)));

    public override string ToString() => TeamProjectCollectionProperties.ToString(this.Id, this.Name);

    private static string ToString(Guid hostId, string hostName) => !string.IsNullOrEmpty(hostName) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) hostId, (object) hostName) : hostId.ToString("D", (IFormatProvider) CultureInfo.CurrentCulture);

    internal ISqlConnectionInfo FrameworkConnectionInfo { get; set; }

    internal IDictionary<string, string> GetServicingTokens()
    {
      IDictionary<string, string> extendedSettings = this.m_extendedSettings;
      this.m_extendedSettings = (IDictionary<string, string>) new Dictionary<string, string>();
      return extendedSettings;
    }
  }
}
