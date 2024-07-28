// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AnalyticsViewsSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public static class AnalyticsViewsSecurityNamespace
  {
    public static readonly Guid SecurityNamespaceId = new Guid("D34D3680-DFE5-4CC6-A949-7D9C68F73CBA");
    public static readonly char NamespaceSeparator = '/';

    public static string GetSecurityToken(
      AnalyticsViewVisibility viewVisibility,
      Guid projectId,
      Guid viewId)
    {
      return string.Format("$/{0}{1}{2}{3}{4}", (object) Enum.GetName(typeof (AnalyticsViewVisibility), (object) viewVisibility), (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) projectId, (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) viewId);
    }

    public static string GetSecurityToken(AnalyticsViewVisibility viewVisibility, Guid projectId) => string.Format("$/{0}{1}{2}", (object) Enum.GetName(typeof (AnalyticsViewVisibility), (object) viewVisibility), (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) projectId);

    public class Permissions
    {
      public const int Read = 1;
      public const int Edit = 2;
      public const int Delete = 4;
      public const int ManagePermissions = 1024;
    }
  }
}
