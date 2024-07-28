// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountAdminSecurity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Account
{
  public static class AccountAdminSecurity
  {
    public static readonly Guid NamespaceId = new Guid("11238E09-49F2-40C7-94D0-8F0307204CE4");
    public static readonly string RootToken = "/";
    public static readonly char PathSeparator = '/';
    public static readonly string OwnershipToken = AccountAdminSecurity.RootToken + "Ownership";

    public static class Permissions
    {
      public const int Read = 1;
      public const int Create = 2;
      public const int Modify = 4;
      public const int All = 7;
    }
  }
}
