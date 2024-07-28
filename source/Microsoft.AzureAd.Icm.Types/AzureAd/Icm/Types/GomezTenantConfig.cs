// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.GomezTenantConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class GomezTenantConfig
  {
    public GomezTenantConfig()
    {
      this.IncludeWarning = true;
      this.WarningSeverity = 2;
      this.SevereSeverity = 1;
      this.BadSeverity = 1;
    }

    [XmlElement]
    public string TenantShortName { get; set; }

    [XmlElement]
    public string UserName { get; set; }

    [XmlElement]
    public string PasswordFieldName { get; set; }

    [XmlElement]
    public string MonitoringType { get; set; }

    [XmlElement]
    public bool IncludeWarning { get; set; }

    [XmlElement]
    public string ServiceAddress { get; set; }

    [XmlElement]
    public Guid ConnectorId { get; set; }

    [XmlElement]
    public int WarningSeverity { get; set; }

    [XmlElement]
    public int BadSeverity { get; set; }

    [XmlElement]
    public int SevereSeverity { get; set; }
  }
}
