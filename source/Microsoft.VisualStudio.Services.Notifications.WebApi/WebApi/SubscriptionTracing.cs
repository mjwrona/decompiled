// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionTracing
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class SubscriptionTracing
  {
    [DataMember(IsRequired = true)]
    public bool Enabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime EndDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime StartDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int MaxTracedEntries { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int TracedEntries { get; set; }
  }
}
