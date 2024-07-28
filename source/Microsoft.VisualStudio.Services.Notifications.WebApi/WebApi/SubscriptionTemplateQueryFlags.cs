// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionTemplateQueryFlags
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [Flags]
  [DataContract]
  public enum SubscriptionTemplateQueryFlags
  {
    None = 0,
    IncludeUser = 1,
    IncludeGroup = 2,
    IncludeUserAndGroup = 4,
    IncludeEventTypeInformation = 22, // 0x00000016
  }
}
