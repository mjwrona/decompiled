// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.WindowsEventLogRecordUserDataInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public class WindowsEventLogRecordUserDataInfo
  {
    [XmlAttribute("TraceId")]
    public Guid TraceId { get; set; }

    public int Tracepoint { get; set; }

    public Guid ServiceHost { get; set; }

    public long ContextId { get; set; }

    public string ProcessName { get; set; }

    public string Username { get; set; }

    public string Service { get; set; }

    public string Method { get; set; }

    public string Area { get; set; }

    public string Layer { get; set; }

    public string UserAgent { get; set; }

    public string Uri { get; set; }

    public string Path { get; set; }

    public string UserDefined { get; set; }

    public string Message { get; set; }
  }
}
