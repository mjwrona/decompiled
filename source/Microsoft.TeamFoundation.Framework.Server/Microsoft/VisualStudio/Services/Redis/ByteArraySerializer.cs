// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.ByteArraySerializer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class ByteArraySerializer : IValueSerializer
  {
    public T Deserialize<T>(byte[] data)
    {
      if (typeof (T) == typeof (byte[]))
        return (T) data;
      throw new InvalidOperationException();
    }

    public byte[] Serialize<T>(T value) => value is byte[] numArray ? numArray : throw new InvalidOperationException();
  }
}
