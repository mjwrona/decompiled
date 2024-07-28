// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceHooksSecurityConstants
  {
    public const char TokenSeparator = '/';
    public const string PublisherTokenRoot = "PublisherSecurity";
    public const string ManagementTokenRoot = "ManagementSecurity";
    public static readonly Guid NamespaceId = new Guid("cb594ebe-87dd-4fc9-ac2c-6a10a4c92046");
    public const int DefaultAdminDenyPermissions = 0;
    public const int DefaultAdminAllowPermissions = 7;

    [GenerateAllConstants(null)]
    public static class Permissions
    {
      public const int None = 0;
      public const int ViewSubscriptions = 1;
      public const int EditSubscriptions = 2;
      public const int DeleteSubscriptions = 4;
      public const int PublishEvents = 8;
    }
  }
}
