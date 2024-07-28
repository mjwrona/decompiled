// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.ToastType
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract]
  public enum ToastType
  {
    [EnumMember] ToastImageAndText01,
    [EnumMember] ToastImageAndText02,
    [EnumMember] ToastImageAndText03,
    [EnumMember] ToastImageAndText04,
    [EnumMember] ToastSmallImageAndText01,
    [EnumMember] ToastSmallImageAndText02,
    [EnumMember] ToastSmallImageAndText03,
    [EnumMember] ToastSmallImageAndText04,
    [EnumMember] ToastText01,
    [EnumMember] ToastText02,
    [EnumMember] ToastText03,
    [EnumMember] ToastText04,
  }
}
