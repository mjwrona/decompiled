// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingConfigurationSecurityConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemTrackingConfigurationSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("35E35E8E-686D-4B01-AFF6-C369D6E36CE0");
    public const string RootToken = "WorkItemTrackingConfiguration";
    public const string CommonConfigurationToken = "Common";
    public const char PathSeparator = '/';
    public const int ReadPermission = 1;
  }
}
