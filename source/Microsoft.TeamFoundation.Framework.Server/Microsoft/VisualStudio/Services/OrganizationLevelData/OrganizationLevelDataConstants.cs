// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OrganizationLevelData.OrganizationLevelDataConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.OrganizationLevelData
{
  public static class OrganizationLevelDataConstants
  {
    public static readonly Guid NamespaceId = new Guid("F0003BCE-5F45-4F93-A25D-90FC33FE3AA9");
    public static readonly string Token = "/";

    public static class Permissions
    {
      public const int View = 1;
    }
  }
}
