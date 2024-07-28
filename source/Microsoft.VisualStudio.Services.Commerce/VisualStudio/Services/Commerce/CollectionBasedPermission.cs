// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CollectionBasedPermission
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CollectionBasedPermission
  {
    public static readonly Guid NamespaceId = new Guid("307BE2D3-12ED-45C2-AACF-6598760EFCA7");

    public static class Permissions
    {
      public const int Read = 1;
      public const int Write = 2;
      public const int Update = 4;
      public const int All = 7;
    }

    public static class Tokens
    {
      public const string AccountLinkSecurityNamespaceToken = "/AccountLink";
      public const string OfferSubscriptionCommittedQuantitySecurityNamespaceToken = "/OfferSubscriptionCommittedQuantity";
      public const string OfferSubscriptionTrial = "/Trial";
      public const string Metering = "/Meter";
      public const string Account = "/Account";
    }
  }
}
