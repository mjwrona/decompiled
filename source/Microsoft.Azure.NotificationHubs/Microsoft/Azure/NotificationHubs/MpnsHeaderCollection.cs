// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MpnsHeaderCollection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [CollectionDataContract(Name = "MpnsHeaders", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect", ItemName = "MpnsHeader", KeyName = "Header", ValueName = "Value")]
  public sealed class MpnsHeaderCollection : SortedDictionary<string, string>
  {
    public MpnsHeaderCollection()
      : base((IComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }
  }
}
