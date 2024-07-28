// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventOpcode
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  public enum EventOpcode
  {
    Info = 0,
    Start = 1,
    Stop = 2,
    DataCollectionStart = 3,
    DataCollectionStop = 4,
    Extension = 5,
    Reply = 6,
    Resume = 7,
    Suspend = 8,
    Send = 9,
    Receive = 240, // 0x000000F0
  }
}
