// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.FlakySettings
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class FlakySettings : TestManagementBaseSecuredObject
  {
    public static readonly FlakySettings DefaultForExistingAccounts = new FlakySettings()
    {
      FlakyDetection = new FlakyDetection()
      {
        FlakyDetectionType = FlakyDetectionType.Custom
      },
      FlakyInSummaryReport = new bool?(false),
      ManualMarkUnmarkFlaky = new bool?(false)
    };
    public static readonly FlakySettings DefaultForNewAccounts = new FlakySettings()
    {
      FlakyDetection = new FlakyDetection()
      {
        FlakyDetectionType = FlakyDetectionType.System,
        FlakyDetectionPipelines = new FlakyDetectionPipelines()
        {
          IsAllPipelinesAllowed = true
        }
      },
      FlakyInSummaryReport = new bool?(true),
      ManualMarkUnmarkFlaky = new bool?(false)
    };
    public static readonly FlakySettings DefaultForExistingAccountsWithManualMarkEnabled = new FlakySettings()
    {
      FlakyDetection = new FlakyDetection()
      {
        FlakyDetectionType = FlakyDetectionType.Custom
      },
      FlakyInSummaryReport = new bool?(false),
      ManualMarkUnmarkFlaky = new bool?(true)
    };

    [DataMember(IsRequired = false)]
    public FlakyDetection FlakyDetection { get; set; }

    [DataMember(IsRequired = false)]
    public bool? FlakyInSummaryReport { get; set; }

    [DataMember(IsRequired = false)]
    public bool? ManualMarkUnmarkFlaky { get; set; }

    [DataMember(IsRequired = false)]
    public bool? IsFlakyBugCreated { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.FlakyDetection == null)
        return;
      this.FlakyDetection.InitializeSecureObject(securedObject);
    }
  }
}
