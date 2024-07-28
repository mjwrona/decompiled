// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.ServiceBusUriManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal sealed class ServiceBusUriManager
  {
    private readonly List<Uri> uriAddresses;
    private readonly bool roundRobin;
    private int currentUriIndex;
    private int firstUriIndex;

    public Uri Current => this.uriAddresses[this.currentUriIndex];

    public ServiceBusUriManager(List<Uri> uriAddresses, bool roundRobin = false)
    {
      this.uriAddresses = uriAddresses;
      this.firstUriIndex = -1;
      this.roundRobin = roundRobin;
    }

    public bool MoveNextUri()
    {
      if (this.firstUriIndex == -1)
      {
        this.firstUriIndex = this.currentUriIndex = ConcurrentRandom.Next(0, this.uriAddresses.Count);
      }
      else
      {
        if (!this.roundRobin && this.IsLastUri())
          return false;
        this.currentUriIndex = this.GetNextUriValue();
      }
      return true;
    }

    public bool CanRetry() => this.roundRobin || !this.IsLastUri();

    private int GetNextUriValue() => (this.currentUriIndex + 1) % this.uriAddresses.Count;

    private bool IsLastUri() => this.GetNextUriValue() == this.firstUriIndex;
  }
}
