// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.EncodingType
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [TypeIdentifier("728AB348-217D-11DA-B2A4-000E7BBB2B09", "Windows.CertEntroll.EncodingType")]
  public enum EncodingType
  {
    XCN_CRYPT_STRING_NOCR = -2147483648, // 0x80000000
    XCN_CRYPT_STRING_BASE64HEADER = 0,
    XCN_CRYPT_STRING_BASE64 = 1,
    XCN_CRYPT_STRING_BINARY = 2,
    XCN_CRYPT_STRING_BASE64REQUESTHEADER = 3,
    XCN_CRYPT_STRING_HEX = 4,
    XCN_CRYPT_STRING_HEXASCII = 5,
    XCN_CRYPT_STRING_BASE64_ANY = 6,
    XCN_CRYPT_STRING_ANY = 7,
    XCN_CRYPT_STRING_HEX_ANY = 8,
    XCN_CRYPT_STRING_BASE64X509CRLHEADER = 9,
    XCN_CRYPT_STRING_HEXADDR = 10, // 0x0000000A
    XCN_CRYPT_STRING_HEXASCIIADDR = 11, // 0x0000000B
    XCN_CRYPT_STRING_HEXRAW = 12, // 0x0000000C
    XCN_CRYPT_STRING_HASHDATA = 268435456, // 0x10000000
    XCN_CRYPT_STRING_STRICT = 536870912, // 0x20000000
    XCN_CRYPT_STRING_NOCRLF = 1073741824, // 0x40000000
  }
}
