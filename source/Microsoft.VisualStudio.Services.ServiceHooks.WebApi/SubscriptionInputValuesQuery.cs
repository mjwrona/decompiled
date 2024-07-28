// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionInputValuesQuery
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class SubscriptionInputValuesQuery
  {
    [DataMember]
    public Subscription Subscription { get; set; }

    [DataMember]
    public SubscriptionInputScope Scope { get; set; }

    [DataMember]
    public IList<Microsoft.VisualStudio.Services.FormInput.InputValues> InputValues { get; set; }
  }
}
