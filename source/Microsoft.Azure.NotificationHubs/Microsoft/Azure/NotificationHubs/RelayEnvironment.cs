// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RelayEnvironment
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  internal class RelayEnvironment
  {
    public const string RelayEnvEnvironmentVariable = "RELAYENV";
    public const string StsEnabledEnvironmentVariable = "RELAYSTSENABLED";
    public const string AcsVersionVariable = "ACSVERSION";
    private const int DefaultHttpPort = 80;
    private const int DefaultHttpsPort = 443;
    private const int DefaultNmfPort = 9354;
    private static readonly RelayEnvironment.MutableEnvironment Environment;

    static RelayEnvironment()
    {
      string environmentVariable = System.Environment.GetEnvironmentVariable("RELAYENV");
      if (environmentVariable != null)
      {
        switch (environmentVariable.ToUpperInvariant())
        {
          case "LIVE":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.LiveEnvironment());
            break;
          case "PPE":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.PpeEnvironment());
            break;
          case "BVT":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.BvtEnvironment());
            break;
          case "INT":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.IntEnvironment());
            break;
          case "LOCAL":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.LocalEnvironment());
            break;
          case "CUSTOM":
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.CustomEnvironment());
            break;
          default:
            RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.LiveEnvironment());
            EventLog.WriteEntry("MSCSH", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid RELAYENV value: {0}, valid values = LIVE, PPE, INT", new object[1]
            {
              (object) environmentVariable
            }), EventLogEntryType.Error, 0);
            break;
        }
      }
      else
      {
        RelayEnvironment.ConfigSettings configSettings = new RelayEnvironment.ConfigSettings();
        if (configSettings.HaveSettings)
          RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) configSettings);
        else
          RelayEnvironment.Environment = new RelayEnvironment.MutableEnvironment((RelayEnvironment.IEnvironment) new RelayEnvironment.LiveEnvironment());
      }
    }

    public static string RelayHostRootName
    {
      get => RelayEnvironment.Environment.RelayHostRootName;
      set => RelayEnvironment.Environment.RelayHostRootName = value;
    }

    public static int RelayHttpPort => RelayEnvironment.Environment.RelayHttpPort;

    public static int RelayHttpsPort => RelayEnvironment.Environment.RelayHttpsPort;

    public static string StsHostName => RelayEnvironment.Environment.StsHostName;

    public static bool StsEnabled
    {
      get => RelayEnvironment.Environment.StsEnabled;
      set => RelayEnvironment.Environment.StsEnabled = value;
    }

    public static int StsHttpPort => RelayEnvironment.Environment.StsHttpPort;

    public static int StsHttpsPort => RelayEnvironment.Environment.StsHttpsPort;

    public static int RelayNmfPort => RelayEnvironment.Environment.RelayNmfPort;

    internal static string RelayPathPrefix => RelayEnvironment.Environment.RelayPathPrefix;

    public static bool GetEnvironmentVariable(string variable, bool defaultValue)
    {
      string environmentVariable = System.Environment.GetEnvironmentVariable(variable);
      bool result;
      return environmentVariable != null && bool.TryParse(environmentVariable, out result) ? result : defaultValue;
    }

    public static int GetEnvironmentVariable(string variable, int defaultValue)
    {
      string environmentVariable = System.Environment.GetEnvironmentVariable(variable);
      int result;
      return !string.IsNullOrEmpty(environmentVariable) && int.TryParse(environmentVariable, out result) ? result : defaultValue;
    }

    private interface IEnvironment
    {
      string RelayHostRootName { get; }

      int RelayHttpPort { get; }

      int RelayHttpsPort { get; }

      string RelayPathPrefix { get; }

      string StsHostName { get; }

      bool StsEnabled { get; }

      int StsHttpPort { get; }

      int StsHttpsPort { get; }

      int RelayNmfPort { get; }
    }

    private class MutableEnvironment : RelayEnvironment.IEnvironment
    {
      private string relayHostRootName;
      private int relayHttpPort;
      private int relayHttpsPort;
      private string relayPathPrefix;
      private string stsHostName;
      private bool stsEnabled;
      private int stsHttpPort;
      private int stsHttpsPort;
      private int relayNmfPort;

      public MutableEnvironment(RelayEnvironment.IEnvironment environment)
      {
        this.relayHostRootName = environment.RelayHostRootName;
        this.relayHttpPort = environment.RelayHttpPort;
        this.relayHttpsPort = environment.RelayHttpsPort;
        this.relayPathPrefix = environment.RelayPathPrefix;
        this.stsHostName = environment.StsHostName;
        this.stsEnabled = environment.StsEnabled;
        this.stsHttpPort = environment.StsHttpPort;
        this.stsHttpsPort = environment.StsHttpsPort;
        this.relayNmfPort = environment.RelayNmfPort;
      }

      public string RelayHostRootName
      {
        get => this.relayHostRootName;
        set => this.relayHostRootName = value;
      }

      public int RelayHttpPort => this.relayHttpPort;

      public int RelayHttpsPort => this.relayHttpsPort;

      public string RelayPathPrefix => this.relayPathPrefix;

      public string StsHostName => this.stsHostName;

      public bool StsEnabled
      {
        get => this.stsEnabled;
        set => this.stsEnabled = value;
      }

      public int StsHttpPort => this.stsHttpPort;

      public int StsHttpsPort => this.stsHttpsPort;

      public int RelayNmfPort => this.relayNmfPort;
    }

    private abstract class EnvironmentBase : RelayEnvironment.IEnvironment
    {
      public abstract string RelayHostRootName { get; }

      public virtual int RelayHttpPort => 80;

      public virtual int RelayHttpsPort => 443;

      public virtual string RelayPathPrefix => string.Empty;

      public abstract string StsHostName { get; }

      public virtual bool StsEnabled => true;

      public virtual int StsHttpPort => 80;

      public virtual int StsHttpsPort => 443;

      public int RelayNmfPort => 9354;
    }

    private class LabsEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.appfabriclabs.com";

      public override string StsHostName => "accesscontrol.appfabriclabs.com";
    }

    private class LiveEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.windows.net";

      public override string StsHostName => "accesscontrol.windows.net";
    }

    private class PpeEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.windows-ppe.net";

      public override string StsHostName => "accesscontrol.windows-ppe.net";
    }

    private class BvtEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.windows-bvt.net";

      public override string StsHostName => "accesscontrol.windows-ppe.net";
    }

    private class IntEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.windows-int.net";

      public override string StsHostName => "accesscontrol.windows-ppe.net";
    }

    private class LocalEnvironment : RelayEnvironment.EnvironmentBase
    {
      public override string RelayHostRootName => "servicebus.onebox.windows-int.net";

      public override string StsHostName => "servicebus.onebox.windows-int.net";

      public override bool StsEnabled => false;
    }

    private class CustomEnvironment : RelayEnvironment.IEnvironment
    {
      private const string RelayHostEnvironmentVariable = "RELAYHOST";
      private const string RelayHttpPortEnvironmentVariable = "RELAYHTTPPORT";
      private const string RelayHttpsPortEnvironmentVariable = "RELAYHTTPSPORT";
      private const string RelayNmfPortEnvironmentVariable = "RELAYNMFPORT";
      private const string RelayPathPrefixEnvironmentVariable = "RELAYPATHPREFIX";
      private const string StsHostEnvironmentVariable = "STSHOST";
      private const string StsHttpPortEnvironmentVariable = "STSHTTPPORT";
      private const string StsHttpsPortEnvironmentVariable = "STSHTTPSPORT";
      private string relayHostRootName;
      private int relayHttpPort;
      private int relayHttpsPort;
      private string relayPathPrefix;
      private string stsHostName;
      private bool stsEnabled;
      private int stsHttpPort;
      private int stsHttpsPort;
      private int relayNmfPort;

      public CustomEnvironment()
      {
        this.relayHostRootName = System.Environment.GetEnvironmentVariable("RELAYHOST");
        this.relayHttpPort = RelayEnvironment.GetEnvironmentVariable("RELAYHTTPPORT", 80);
        this.relayHttpsPort = RelayEnvironment.GetEnvironmentVariable("RELAYHTTPSPORT", 443);
        this.relayPathPrefix = System.Environment.GetEnvironmentVariable("RELAYPATHPREFIX");
        this.stsHostName = System.Environment.GetEnvironmentVariable("STSHOST");
        this.stsEnabled = true;
        this.stsHttpPort = RelayEnvironment.GetEnvironmentVariable("STSHTTPPORT", 80);
        this.stsHttpsPort = RelayEnvironment.GetEnvironmentVariable("STSHTTPSPORT", 443);
        this.relayNmfPort = RelayEnvironment.GetEnvironmentVariable("RELAYNMFPORT", 9354);
      }

      public string RelayHostRootName => this.relayHostRootName;

      public int RelayHttpPort => this.relayHttpPort;

      public int RelayHttpsPort => this.relayHttpsPort;

      public string RelayPathPrefix => this.relayPathPrefix;

      public string StsHostName => this.stsHostName;

      public bool StsEnabled => this.stsEnabled;

      public int StsHttpPort => this.stsHttpPort;

      public int StsHttpsPort => this.stsHttpsPort;

      public int RelayNmfPort => this.relayNmfPort;
    }

    private class ConfigSettings : RelayEnvironment.IEnvironment
    {
      private const string RelayHostNameElement = "relayHostName";
      private const string RelayHttpPortNameElement = "relayHttpPort";
      private const string RelayHttpsPortNameElement = "relayHttpsPort";
      private const string RelayNmfPortNameElement = "relayNmfPort";
      private const string RelayPathPrefixElement = "relayPathPrefix";
      private const string StsHostNameElement = "stsHostName";
      private const string StsEnabledElement = "stsEnabled";
      private const string StsHttpPortNameElement = "stsHttpPort";
      private const string StsHttpsPortNameElement = "stsHttpsPort";
      private const string V1ConfigFileName = "notificationhub.config";
      private const string WebRootPath = "approot\\";
      private readonly string configFileName;
      private bool haveSettings;
      private string relayHostName;
      private int relayHttpPort;
      private int relayHttpsPort;
      private int relayNmfPort;
      private string relayPathPrefix = string.Empty;
      private string stsHostName;
      private bool stsEnabled;
      private int stsHttpPort;
      private int stsHttpsPort;

      public ConfigSettings()
      {
        this.configFileName = "notificationhub.config";
        this.ReadConfigSettings();
      }

      public bool HaveSettings => this.haveSettings;

      public string RelayHostRootName => this.relayHostName;

      public int RelayHttpPort => this.relayHttpPort;

      public int RelayHttpsPort => this.relayHttpsPort;

      public int RelayNmfPort => this.relayNmfPort;

      public string RelayPathPrefix => this.relayPathPrefix;

      public string StsHostName => this.stsHostName;

      public bool StsEnabled => this.stsEnabled;

      public int StsHttpPort => this.stsHttpPort;

      public int StsHttpsPort => this.stsHttpsPort;

      private void ReadConfigSettings()
      {
        this.haveSettings = false;
        string configFileName = this.configFileName;
        string path1 = Path.Combine("approot\\", this.configFileName);
        string path2 = !File.Exists(configFileName) ? (!File.Exists(path1) ? Path.Combine(Path.GetDirectoryName(ConfigurationManager.OpenMachineConfiguration().FilePath), this.configFileName) : path1) : configFileName;
        RelayEnvironment.LiveEnvironment liveEnvironment = new RelayEnvironment.LiveEnvironment();
        this.relayHostName = liveEnvironment.RelayHostRootName;
        this.relayHttpPort = liveEnvironment.RelayHttpPort;
        this.relayHttpsPort = liveEnvironment.RelayHttpsPort;
        this.relayNmfPort = liveEnvironment.RelayNmfPort;
        this.relayPathPrefix = liveEnvironment.RelayPathPrefix;
        this.stsHostName = liveEnvironment.StsHostName;
        this.stsEnabled = liveEnvironment.StsEnabled;
        this.stsHttpPort = liveEnvironment.StsHttpPort;
        this.stsHttpsPort = liveEnvironment.StsHttpsPort;
        if (!File.Exists(path2))
          return;
        Stream input = (Stream) File.OpenRead(path2);
        XmlReader xmlReader = XmlReader.Create(input);
        xmlReader.ReadStartElement("configuration");
        xmlReader.ReadStartElement("Microsoft.Azure.NotificationHubs");
        while (xmlReader.IsStartElement())
        {
          string name = xmlReader.Name;
          string s = xmlReader.ReadElementString();
          switch (name)
          {
            case "relayHostName":
              this.relayHostName = s;
              continue;
            case "relayHttpPort":
              this.relayHttpPort = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            case "relayHttpsPort":
              this.relayHttpsPort = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            case "relayNmfPort":
              this.relayNmfPort = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            case "relayPathPrefix":
              this.relayPathPrefix = s;
              if (!this.relayPathPrefix.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                this.relayPathPrefix = "/" + this.relayPathPrefix;
              if (this.relayPathPrefix.EndsWith("/", StringComparison.Ordinal))
              {
                this.relayPathPrefix = this.relayPathPrefix.Substring(0, this.relayPathPrefix.Length - 1);
                continue;
              }
              continue;
            case "stsHostName":
              this.stsHostName = s;
              continue;
            case "stsHttpPort":
              this.stsHttpPort = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            case "stsHttpsPort":
              this.stsHttpsPort = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            default:
              continue;
          }
        }
        xmlReader.ReadEndElement();
        xmlReader.ReadEndElement();
        input.Close();
        this.haveSettings = true;
      }
    }
  }
}
