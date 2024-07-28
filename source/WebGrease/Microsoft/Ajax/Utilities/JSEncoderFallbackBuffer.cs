// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSEncoderFallbackBuffer
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  internal sealed class JSEncoderFallbackBuffer : EncoderFallbackBuffer
  {
    private string m_fallbackString;
    private int m_position;

    public override int Remaining => this.m_fallbackString.Length - this.m_position;

    public JSEncoderFallbackBuffer() => this.Reset();

    private static string GetEncoding(int charValue) => "\\u{0:x4}".FormatInvariant((object) charValue);

    public override bool Fallback(char charUnknown, int index)
    {
      if (this.m_position < this.m_fallbackString.Length)
        throw new ArgumentException(CommonStrings.FallbackEncodingFailed);
      this.m_fallbackString = JSEncoderFallbackBuffer.GetEncoding((int) charUnknown);
      this.m_position = 0;
      return this.m_fallbackString.Length > 0;
    }

    public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
    {
      if (this.m_position < this.m_fallbackString.Length)
        throw new ArgumentException(CommonStrings.FallbackEncodingFailed);
      this.m_fallbackString = JSEncoderFallbackBuffer.GetEncoding((int) charUnknownHigh) + JSEncoderFallbackBuffer.GetEncoding((int) charUnknownLow);
      this.m_position = 0;
      return this.m_fallbackString.Length > 0;
    }

    public override char GetNextChar() => this.m_position >= this.m_fallbackString.Length ? char.MinValue : this.m_fallbackString[this.m_position++];

    public override bool MovePrevious()
    {
      bool flag = this.m_position > 0;
      if (this.m_position > 0)
        --this.m_position;
      return flag;
    }

    public override void Reset()
    {
      this.m_fallbackString = string.Empty;
      this.m_position = 0;
      base.Reset();
    }

    public override string ToString() => this.m_fallbackString;
  }
}
