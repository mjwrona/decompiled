// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.CodePointCharStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Antlr4.Runtime
{
  internal class CodePointCharStream : BaseInputCharStream
  {
    private int[] data;

    public CodePointCharStream(string input)
    {
      this.data = new int[input.Length];
      int num = 0;
      int utf32;
      for (int index = 0; index < input.Length; index += utf32 <= (int) ushort.MaxValue ? 1 : 2)
      {
        utf32 = char.ConvertToUtf32(input, index);
        this.data[num++] = utf32;
        if (num > this.data.Length)
          Array.Resize<int>(ref this.data, this.data.Length * 2);
      }
      this.n = num;
    }

    protected override int ValueAt(int i) => this.data[i];

    protected override string ConvertDataToString(int start, int count)
    {
      StringBuilder stringBuilder = new StringBuilder(count);
      for (int index = start; index < start + count; ++index)
        stringBuilder.Append(char.ConvertFromUtf32(this.data[index]));
      return stringBuilder.ToString();
    }
  }
}
