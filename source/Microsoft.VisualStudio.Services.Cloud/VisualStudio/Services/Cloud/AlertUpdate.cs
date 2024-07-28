// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertUpdate
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AlertUpdate
  {
    public int EventId { get; set; }

    public string EventName { get; set; }

    public int EventIndex { get; set; }

    public bool DeleteEntry { get; set; }

    public int Version { get; set; }

    public bool Enabled { get; set; }

    public string Area { get; set; }

    public string Description { get; set; }

    public string AreaPath { get; set; }

    public string IterationPath { get; set; }

    public bool MdmEventEnabled { get; set; }

    public int ScopeIndex { get; set; }
  }
}
