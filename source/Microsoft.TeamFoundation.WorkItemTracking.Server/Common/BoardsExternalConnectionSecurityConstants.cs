// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.BoardsExternalConnectionSecurityConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class BoardsExternalConnectionSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("5AB15BC8-4EA1-D0F3-8344-CAB8FE976877");
    public const string RootToken = "BoardsExternalIntegration";
    public const char PathSeparator = '/';
    public const int None = 0;
    public const int ReadPermission = 1;
    public const int WritePermission = 2;
  }
}
