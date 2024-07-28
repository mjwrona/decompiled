// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.WindowsEventLogRecordSystemData
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public class WindowsEventLogRecordSystemData
  {
    [XmlElement("EventID")]
    public int EventId { get; set; }

    [XmlElement("Version")]
    public int Version { get; set; }

    [XmlElement("Level")]
    public int Level { get; set; }

    [XmlElement("TimeCreated")]
    public WindowsEventLogRecordTimeCreated TimeCreated { get; set; }

    [XmlElement("Correlation")]
    public WindowsEventLogCorrelation Correlation { get; set; }

    [XmlElement("Computer")]
    public string Computer { get; set; }

    [XmlElement("Execution")]
    public WindowsEventLogExecution Execution { get; set; }
  }
}
