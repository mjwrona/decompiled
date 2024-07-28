// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AexServiceConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class AexServiceConstants
  {
    public static readonly Guid ServiceInstanceTypeId = new Guid("00000041-0000-8888-8000-000000000000");
    public const string UrlParamCampaignKey = "campaign";
    public const string UrlParamCampaignValue = "o~msft~vsts~usercard";
    public const string GoProfilePath = "go/profile/";
    public const string AexLocationTimeout = "/Service/Profile/AexLocation/Timeout";
    public const string GitHubConnectContinuationToken = "githubconnect.wizard-continuation-token";
    public const string AuthRegisterAppPath = "app/register/";
  }
}
