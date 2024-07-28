// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestManagementSecurityNamespaceConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestManagementSecurityNamespaceConstants
  {
    public static readonly Guid NamespaceId = new Guid("E06E1C24-E93D-4E4A-908A-7D951187B483");
    public const string RootToken = "TestManagement";
    public const char PathSeparator = '/';
    public const string UserSettings = "TestManagementUserSettings";
    public const int ReadPermission = 1;
  }
}
