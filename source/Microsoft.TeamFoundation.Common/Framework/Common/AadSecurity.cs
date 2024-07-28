// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.AadSecurity
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class AadSecurity
  {
    public static readonly Guid NamespaceId = new Guid("0AB23938-CC39-4EEE-8541-F5FDF15DC345");
    public static readonly string RootToken = "/";
    public static readonly char PathSeparator = '/';
    public static readonly string GraphToken = AadSecurity.RootToken + "Graph";
    public static readonly string TenantsToken = AadSecurity.GraphToken + AadSecurity.PathSeparator.ToString() + "Tenants";

    public static class Permissions
    {
      public const int AllowApplicationImpersonation = 1;
      public const int All = 255;
    }
  }
}
