// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory.DynamicByteArray
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory
{
  public class DynamicByteArray : ByteArray
  {
    private const int MaxArraySize = 2147483587;

    public DynamicByteArray(uint initialCapacity)
      : base(initialCapacity)
    {
    }

    private void Grow(uint minGrowBy)
    {
      long val1 = (long) this.BackingArray.Length + (long) minGrowBy;
      if (val1 > 2147483587L)
        throw new ArrayTooLargeException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Array cannot grow to {0} bytes.", (object) val1));
      long val2 = (long) this.BackingArray.Length * 2L;
      int length = val2 > 2147483587L ? 2147483587 : (int) Math.Max(val1, val2);
      if (length <= this.BackingArray.Length)
        length = length != 2147483587 ? 2147483587 : throw new ArrayTooLargeException();
      byte[] destinationArray = new byte[length];
      Array.Copy((Array) this.BackingArray, (Array) destinationArray, this.BackingArray.Length);
      uint position = this.Position;
      this.ArrayPinHandle.Free();
      this.BackingArray = destinationArray;
      this.InitializePointers();
      this.Position = position;
    }

    protected override void FillWriteBuffer(uint size) => this.Grow(size);
  }
}
