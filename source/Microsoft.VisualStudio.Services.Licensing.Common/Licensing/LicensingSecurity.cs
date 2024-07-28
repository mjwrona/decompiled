// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingSecurity
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingSecurity
  {
    public static readonly Guid NamespaceId = new Guid("453E2DB3-2E81-474F-874D-3BF51027F2EE");
    public static readonly string RootToken = "/";
    public static readonly char PathSeparator = '/';
    public static readonly string EntitlementsToken = LicensingSecurity.RootToken + "Entitlements";
    public static readonly string AccountEntitlementsToken = LicensingSecurity.EntitlementsToken + LicensingSecurity.PathSeparator.ToString() + "AccountEntitlements";
    public static readonly string MembershipsToken = LicensingSecurity.RootToken + "Memberships";
  }
}
