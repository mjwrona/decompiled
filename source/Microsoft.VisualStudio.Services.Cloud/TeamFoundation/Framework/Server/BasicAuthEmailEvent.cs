// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthEmailEvent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class BasicAuthEmailEvent
  {
    public BasicAuthEmailEvent()
    {
    }

    public BasicAuthEmailEvent(string displayName, string changeDate)
      : this(displayName, changeDate, (string) null)
    {
    }

    public BasicAuthEmailEvent(string displayName, string changeDate, string changeTime)
    {
      this.DisplayNameText = displayName;
      this.ChangeDate = changeDate;
      this.ChangeTime = changeTime;
    }

    public string DisplayNameText { get; set; }

    public string ChangeDate { get; set; }

    public string ChangeTime { get; set; }
  }
}
