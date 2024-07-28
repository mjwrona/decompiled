// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceBillingAccountInfo
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceBillingAccountInfo
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string NotificationEmailAddress { get; set; }

    public string CommunicationLanguage { get; set; }

    public string CommunicationCulture { get; set; }

    public string CurrencyCode { get; set; }

    public bool FromCache { get; set; }

    public string Region { get; set; }
  }
}
