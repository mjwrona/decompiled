// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventSourceAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  [AttributeUsage(AttributeTargets.Class)]
  internal sealed class EventSourceAttribute : Attribute
  {
    public string Name { get; set; }

    public string Guid { get; set; }

    public string LocalizationResources { get; set; }
  }
}
