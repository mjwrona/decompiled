// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationSummary
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  [DebuggerDisplay("{Id} Status:{Status} Result:{Result}")]
  public class NotificationSummary
  {
    [DataMember]
    public Guid SubscriptionId { get; set; }

    [DataMember]
    public IList<NotificationResultsSummaryDetail> Results { get; set; }
  }
}
