// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.DataImportTestMilestones
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class DataImportTestMilestones
  {
    public const string SmallMostRecentImportMilestone = "smalldev19-m225-3";
    public const string MostRecentImportMilestone = "dev19-m225-3";
    public const string TurkishCollation = "dev19-m225-3-turkish";
    public const string SmallMostRecentImportMilestoneMinusOne = "smalldev19-m205-5";
    public const string MostRecentImportMilestoneMinusOne = "dev19-m205-5";
    public const string SmallMostRecentImportMilestoneMinusTwo = "smalldev19-m205-5";
    public const string OldestServiceLevel = "Dev18.M181.9";
    public const string OldestImportMilestone = "smalldev18-m181-9";
    public const string StretchServiceLevel = "Dev19.M225.4";
    public const string StretchMilestone = "smalldev19-m225-3-stretch";
    public const string OldStretchServiceLevel = "Dev19.M205.6";
    public const string OldStretchMilestone = "smalldev19-m205-5-stretch";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string MostRecent = "Obsolete";
  }
}
