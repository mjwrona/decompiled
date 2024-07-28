// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.TestNotification
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  internal class TestNotification : Notification
  {
    public List<Tuple<string, string, SubscriptionInputScope>> OverriddenConfidentialInputs { get; set; }

    public Guid TesterUserId { get; set; }
  }
}
