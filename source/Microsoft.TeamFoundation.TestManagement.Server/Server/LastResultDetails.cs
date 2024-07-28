// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LastResultDetails
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class LastResultDetails
  {
    public LastResultDetails()
    {
    }

    public LastResultDetails(DateTime dateCompleted, long duration, Guid runBy)
    {
      this.DateCompleted = dateCompleted;
      this.Duration = duration;
      this.RunBy = runBy;
    }

    public DateTime DateCompleted { get; set; }

    public Guid RunBy { get; set; }

    public long Duration { get; set; }

    public string RunByName { get; set; }
  }
}
