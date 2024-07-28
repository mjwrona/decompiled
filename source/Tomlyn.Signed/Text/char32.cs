// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.char32
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Text
{
  internal readonly struct char32
  {
    public char32(int code) => this.Code = code;

    public int Code { get; }

    public static implicit operator int(char32 c) => c.Code;

    public static implicit operator char32(int c) => new char32(c);

    public override string ToString() => char.ConvertFromUtf32(this.Code);
  }
}
