// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.RunOptionsConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  public static class RunOptionsConstants
  {
    public const string EnvironmentOwnerEmailNotificationValueAlways = "Always";
    public const string EnvironmentOwnerEmailNotificationValueTypeOnlyOnFailure = "OnlyOnFailure";
    public const string EnvironmentOwnerEmailNotificationValueNever = "Never";
    public const string EnvironmentOwnerEmailNotificationTypeDefaultValue = "OnlyOnFailure";
    public const string ReleaseCreator = "release.creator";
    public const string EnvironmentOwner = "release.environment.owner";
    public const char EmailRecipientsSeparator = ';';

    public static string DefaultEmailRecipients => "release.environment.owner;release.creator";
  }
}
