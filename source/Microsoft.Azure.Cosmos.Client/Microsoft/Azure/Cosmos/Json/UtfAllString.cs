// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.UtfAllString
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal sealed class UtfAllString : IEquatable<UtfAllString>
  {
    private UtfAllString(
      Utf8Memory utf8String,
      string utf16String,
      Utf8Memory utf8EscapedString,
      string utf16EscapedString)
    {
      this.Utf8String = utf8String;
      this.Utf16String = utf16String;
      this.Utf8EscapedString = utf8EscapedString;
      this.Utf16EscapedString = utf16EscapedString;
    }

    public Utf8Memory Utf8String { get; }

    public string Utf16String { get; }

    public Utf8Memory Utf8EscapedString { get; }

    public string Utf16EscapedString { get; }

    public static UtfAllString Create(string utf16String)
    {
      Utf8Memory utf8String = utf16String != null ? Utf8Memory.UnsafeCreateNoValidation((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(utf16String)) : throw new ArgumentNullException(nameof (utf16String));
      string str = JsonConvert.ToString(utf16String);
      string s = str.Substring(1, str.Length - 2);
      Utf8Memory noValidation = Utf8Memory.UnsafeCreateNoValidation((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(s));
      string utf16String1 = utf16String;
      Utf8Memory utf8EscapedString = noValidation;
      string utf16EscapedString = s;
      return new UtfAllString(utf8String, utf16String1, utf8EscapedString, utf16EscapedString);
    }

    public static UtfAllString Create(Utf8Memory utf8String)
    {
      string utf16String = utf8String.ToString();
      string str1 = JsonConvert.ToString(utf16String);
      string str2 = str1.Substring(1, str1.Length - 2);
      Utf8Memory noValidation = Utf8Memory.UnsafeCreateNoValidation((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(str2));
      return new UtfAllString(utf8String, utf16String, noValidation, str2);
    }

    public bool Equals(UtfAllString other)
    {
      if (this == other)
        return true;
      if (other == null)
        return false;
      Utf8Span span1 = this.Utf8String.Span;
      ReadOnlySpan<byte> span2 = ((Utf8Span) ref span1).Span;
      span1 = other.Utf8String.Span;
      ReadOnlySpan<byte> span3 = ((Utf8Span) ref span1).Span;
      return span2.SequenceEqual<byte>(span3);
    }
  }
}
