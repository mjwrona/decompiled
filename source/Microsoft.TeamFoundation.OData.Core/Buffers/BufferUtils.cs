// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Buffers.BufferUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.Buffers
{
  internal static class BufferUtils
  {
    private const int BufferLength = 128;

    public static char[] InitializeBufferIfRequired(char[] buffer) => BufferUtils.InitializeBufferIfRequired((ICharArrayPool) null, buffer);

    public static char[] InitializeBufferIfRequired(ICharArrayPool bufferPool, char[] buffer) => buffer != null ? buffer : BufferUtils.RentFromBuffer(bufferPool, 128);

    public static char[] RentFromBuffer(ICharArrayPool bufferPool, int minSize)
    {
      if (bufferPool == null)
        return new char[minSize];
      char[] chArray = bufferPool.Rent(minSize);
      if (chArray == null || chArray.Length < minSize)
        throw new ODataException(Strings.BufferUtils_InvalidBufferOrSize((object) minSize));
      return chArray;
    }

    public static void ReturnToBuffer(ICharArrayPool bufferPool, char[] buffer) => bufferPool?.Return(buffer);
  }
}
