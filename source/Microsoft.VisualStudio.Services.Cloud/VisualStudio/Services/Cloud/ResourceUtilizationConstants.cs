// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceUtilizationConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ResourceUtilizationConstants
  {
    private static readonly string RegistryRoot = "/Service/ResourceUtilization/";
    public static readonly string ServiceSettingsRegistryRoot = ResourceUtilizationConstants.RegistryRoot + "Settings/";
    public const string QueueIncrement = "QueueIncrement";
    public const string ShouldOutputTelemetry = "ShouldOutputTelemetry";
    public const int UnlimitedThreshold = -1;
    public const string RU2ReadScaleOutXeventsFF = "VisualStudio.FrameworkService.ResourceUtilization2.ReadScaleOutXevents";
    public static readonly Guid RulesOrThresholdsChanged = new Guid("5BD9D779-B847-4678-BFE7-13335D8C86A1");
    internal const double TarpitThresholdTSTUsPerSecond = 0.66666666666666663;
    internal const string RU2FF = "VisualStudio.FrameworkService.ResourceUtilization2";
    internal const string ThrottlingNotificationEmailFeatureName = "VisualStudio.FrameworkService.ThrottlingNotification.Email";
    internal const string XEventJobNotificationsFeatureName = "VisualStudio.FrameworkService.XEventProcessorJob.SqlNotifications";
    internal const string AccountLicenseTypeBasicTestPlanFeatureName = "VisualStudio.FrameworkService.ResourceUtilization2.AccountLicenseTypeBasicTestPlan";
    internal const string ThrottleReasonWithRuleFeatureName = "VisualStudio.FrameworkService.ResourceUtilization2.ThrottleReasonWithRule";
    internal const string UseResourceUtilization21FeatureName = "VisualStudio.FrameworkService.ResourceUtilization2.UseResourceUtilization21";
    internal static readonly Guid ResourceStateChanged = new Guid("A49ED993-8C9A-4D77-9450-798AE7EABB8E");
    internal static readonly Guid DeleteExpiredThresholdsJobId = new Guid("8D8E1D24-E7A5-4CF0-9661-D7872BD0EAAB");
    internal static readonly Guid PollSettingsForExpiredThresholdsJobId = new Guid("53347EF3-74FB-4F73-92DF-EDE1096AA466");
  }
}
