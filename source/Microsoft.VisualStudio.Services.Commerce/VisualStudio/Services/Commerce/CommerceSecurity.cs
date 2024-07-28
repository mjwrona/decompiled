// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSecurity
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceSecurity
  {
    public static readonly Guid CommerceSecurityNamespaceId = new Guid("0a7156a6-a70a-4e61-884b-1b2e602ffbb8");
    public static readonly Guid MeteringSecurityNamespaceId = new Guid("49b585d1-e748-4dd4-908d-a7aa20f0f17b");
    public static readonly Guid ReportingSecurityNamespaceId = new Guid("810944ec-b061-464a-bb2b-86cef1cf2f60");
    public const string CommercePathSeparator = "/";
    public const string CommerceSecurityNamespaceToken = "AllAccounts";
  }
}
