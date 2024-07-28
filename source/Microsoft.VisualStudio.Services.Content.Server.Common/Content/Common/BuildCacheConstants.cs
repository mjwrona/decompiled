// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.BuildCacheConstants
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class BuildCacheConstants
  {
    public static Guid SecurityNamespaceId = new Guid("13c90714-5b92-4c6d-aaf2-e89451e38b5e");
    public static string Token = "/Cache";

    public enum BuildCachePermissions
    {
      Read = 1,
      ManagePermissions = 2,
      WriteCache = 8,
      CacheReadWrite = 9,
      Enumerate = 16, // 0x00000010
      AllPermissions = 27, // 0x0000001B
    }
  }
}
