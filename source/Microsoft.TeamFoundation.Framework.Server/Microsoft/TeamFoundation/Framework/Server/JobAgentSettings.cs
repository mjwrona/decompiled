// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobAgentSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class JobAgentSettings : IJobAgentSettings, IApplicationSettings
  {
    private readonly Lazy<XmlDocument> m_xmlDoc;

    public JobAgentSettings()
      : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\Web Services\\web.config"))
    {
    }

    public JobAgentSettings(string webConfigPath)
    {
      JobAgentSettings jobAgentSettings = this;
      this.m_xmlDoc = new Lazy<XmlDocument>((Func<XmlDocument>) (() => jobAgentSettings.LoadSettings(webConfigPath)));
    }

    public string this[string key] => this.GetSettingValue(key);

    public string ConfigDbConnectionString
    {
      get
      {
        string str = this[FrameworkServerConstants.ApplicationDatabaseAppSettingsKey];
        return !string.IsNullOrEmpty(str) ? str : throw new ApplicationException(FrameworkResources.JobAgentConfigurationError((object) FrameworkServerConstants.ApplicationDatabaseAppSettingsKey));
      }
    }

    public string ConfigDbUserId => this[FrameworkServerConstants.ApplicationDatabaseSqlUserKey];

    public string ConfigDbPassword => this[FrameworkServerConstants.ApplicationDatabaseSqlPasswordKey];

    public Guid InstanceId
    {
      get
      {
        Guid instanceId = Guid.Empty;
        string g = this[FrameworkServerConstants.ApplicationIdAppSettingsKey];
        if (!string.IsNullOrEmpty(g))
        {
          try
          {
            instanceId = new Guid(g);
          }
          catch (FormatException ex)
          {
            throw new ApplicationException(FrameworkResources.ConfigurationGuidError((object) FrameworkServerConstants.ApplicationIdAppSettingsKey), (Exception) ex);
          }
        }
        return instanceId;
      }
    }

    public TimeSpan MaximumStopTime => TimeSpan.FromMinutes(1.0);

    public TimeSpan ForceQueueCheckInterval
    {
      get
      {
        int result;
        if (!int.TryParse(this["forceQueueCheckSeconds"], out result))
          result = 300;
        return TimeSpan.FromSeconds((double) result);
      }
    }

    private string GetSettingValue(string settingKey)
    {
      string settingValue = ConfigurationManager.AppSettings[settingKey];
      if (string.IsNullOrEmpty(settingValue))
      {
        XmlNode xmlNode = this.m_xmlDoc.Value.SelectSingleNode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//appSettings/add[@key='{0}']/@value", (object) settingKey));
        if (xmlNode != null)
          settingValue = xmlNode.InnerText;
      }
      return settingValue;
    }

    private XmlDocument LoadSettings(string webConfigPath)
    {
      if (!string.IsNullOrEmpty(webConfigPath) && !Path.IsPathRooted(webConfigPath))
        webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webConfigPath);
      if (!File.Exists(webConfigPath))
        throw new ApplicationException(FrameworkResources.JobAgentConfigurationDoesntExistError((object) webConfigPath));
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (Stream input = (Stream) File.Open(webConfigPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          using (XmlReader reader = XmlReader.Create(input, settings))
            xmlDocument.Load(reader);
        }
      }
      catch (Exception ex)
      {
        throw new ApplicationException(FrameworkResources.JobAgentReadConfigurationError((object) webConfigPath), ex);
      }
      return xmlDocument;
    }
  }
}
