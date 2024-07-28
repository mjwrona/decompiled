// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.WindowsEventLogRecord
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  [XmlRoot("Event", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
  public class WindowsEventLogRecord
  {
    [XmlElement("System")]
    public WindowsEventLogRecordSystemData SystemData { get; set; }

    [XmlElement("UserData")]
    public WindowsEventLogRecordUserData UserData { get; set; }
  }
}
