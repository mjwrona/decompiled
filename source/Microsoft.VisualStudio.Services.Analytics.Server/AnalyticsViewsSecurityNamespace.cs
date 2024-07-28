// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsSecurityNamespace
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsSecurityNamespace
  {
    public static readonly Guid Id = new Guid("D34D3680-DFE5-4CC6-A949-7D9C68F73CBA");
    public static readonly char NamespaceSeparator = '/';
    public static readonly string SharedRoot = Enum.GetName(typeof (AnalyticsViewVisibility), (object) AnalyticsViewVisibility.Shared);
    public static readonly string PrivateRoot = Enum.GetName(typeof (AnalyticsViewVisibility), (object) AnalyticsViewVisibility.Private);
    public static readonly int DefaultProjectAdminAllowPermissions = 1031;
    public static readonly int DefaultViewOwnerAllowPermissions = AnalyticsViewsSecurityNamespace.DefaultProjectAdminAllowPermissions;
    public static readonly int DefaultProjectValidUsersAllowPermissions = 7;
    public static readonly int DefaultProjectAdminDenyPermissions = 0;
    public static readonly int DefaultProjectValidUsersDenyPermissions = 0;
    public static readonly int DefaultViewOwnerDenyPermissions = AnalyticsViewsSecurityNamespace.DefaultProjectAdminDenyPermissions;

    public static string GetSecurityToken(
      AnalyticsViewVisibility viewVisibility,
      Guid projectId,
      Guid viewId)
    {
      return string.Format("$/{0}{1}{2}{3}{4}", (object) Enum.GetName(typeof (AnalyticsViewVisibility), (object) viewVisibility), (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) projectId, (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) viewId);
    }

    public static string GetSecurityToken(AnalyticsViewVisibility viewVisibility, Guid projectId) => string.Format("$/{0}{1}{2}", (object) Enum.GetName(typeof (AnalyticsViewVisibility), (object) viewVisibility), (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) projectId);

    public static string GetSecurityToken(AnalyticsViewVisibility viewVisibility) => "$/" + Enum.GetName(typeof (AnalyticsViewVisibility), (object) viewVisibility);

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
