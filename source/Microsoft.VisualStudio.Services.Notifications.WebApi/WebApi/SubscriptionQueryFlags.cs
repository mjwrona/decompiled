// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionQueryFlags
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [Flags]
  public enum SubscriptionQueryFlags
  {
    None = 0,
    IncludeInvalidSubscriptions = 2,
    IncludeDeletedSubscriptions = 4,
    IncludeFilterDetails = 8,
    AlwaysReturnBasicInformation = 16, // 0x00000010
    IncludeSystemSubscriptions = 32, // 0x00000020
  }
}
