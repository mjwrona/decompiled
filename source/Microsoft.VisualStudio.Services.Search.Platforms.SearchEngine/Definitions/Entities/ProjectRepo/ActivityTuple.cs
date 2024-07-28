// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ActivityTuple
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class ActivityTuple
  {
    public ActivityTuple(DateTime date, int value)
    {
      this.ActivityDate = date;
      this.ActivityValue = value;
    }

    [Date(Name = "activityDate")]
    public DateTime ActivityDate { get; set; }

    [Number(Name = "activityValue")]
    public int ActivityValue { get; set; }
  }
}
