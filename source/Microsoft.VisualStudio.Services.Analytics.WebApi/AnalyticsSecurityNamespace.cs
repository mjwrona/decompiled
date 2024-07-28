// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsSecurityNamespace
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  public static class AnalyticsSecurityNamespace
  {
    public const string Root = "$";
    public static readonly Guid Id = new Guid("58450C49-B02D-465A-AB12-59AE512D6531");

    public static string GetSecurityToken(Guid projectId) => string.Format("$/{0}", (object) projectId);

    public static string GetODataPermissionAlreadySetRegistryKey(string scope) => "/Configuration/AnalyticsService/ProjectServiceBusReceiver/ProjectPermissionApplied/" + scope;

    [GenerateAllConstants(null)]
    public class Permissions
    {
      public const int Read = 1;
      public const int Administer = 2;
      public const int Staging = 4;
      public const int ExecuteUnrestrictedQuery = 8;
      public const int ReadEuii = 16;
    }
  }
}
