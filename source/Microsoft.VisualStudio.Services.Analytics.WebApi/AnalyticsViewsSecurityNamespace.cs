// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsViewsSecurityNamespace
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  public static class AnalyticsViewsSecurityNamespace
  {
    public static readonly Guid Id = new Guid("D34D3680-DFE5-4CC6-A949-7D9C68F73CBA");

    public static string GetSecurityToken(Guid projectId) => string.Format("$/{0}", (object) projectId);

    [GenerateAllConstants(null)]
    public class Permissions
    {
      public const int Read = 1;
      public const int Edit = 2;
      public const int Delete = 4;
      public const int Execute = 8;
      public const int ManagePermissions = 1024;
    }
  }
}
