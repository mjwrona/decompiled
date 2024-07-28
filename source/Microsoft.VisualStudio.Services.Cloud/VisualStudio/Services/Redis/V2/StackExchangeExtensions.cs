// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.StackExchangeExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using StackExchange.Redis;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal static class StackExchangeExtensions
  {
    public static int GetSize(this RedisKey key) => RedisKey.op_Implicit(key).Length;

    public static int GetSize(this RedisValue value)
    {
      if (((RedisValue) ref value).IsInteger)
        return 8;
      byte[] numArray = RedisValue.op_Implicit(value);
      return numArray != null ? numArray.Length : 0;
    }
  }
}
