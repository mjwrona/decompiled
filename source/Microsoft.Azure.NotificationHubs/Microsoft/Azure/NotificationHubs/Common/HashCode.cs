// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.HashCode
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class HashCode
  {
    public static int CombineHashCodes(int h1, int h2) => (h1 << 5) + h1 ^ h2;

    public static int CombineHashCodes(int h1, int h2, int h3) => HashCode.CombineHashCodes(HashCode.CombineHashCodes(h1, h2), h3);

    public static int CombineHashCodes(int h1, int h2, int h3, int h4) => HashCode.CombineHashCodes(HashCode.CombineHashCodes(h1, h2), HashCode.CombineHashCodes(h3, h4));
  }
}
