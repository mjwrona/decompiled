// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.X500NameFlags
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [TypeIdentifier("728AB348-217D-11DA-B2A4-000E7BBB2B09", "Windows.CertEntroll.X500NameFlags")]
  public enum X500NameFlags
  {
    XCN_CERT_NAME_STR_NONE = 0,
    XCN_CERT_SIMPLE_NAME_STR = 1,
    XCN_CERT_OID_NAME_STR = 2,
    XCN_CERT_X500_NAME_STR = 3,
    XCN_CERT_XML_NAME_STR = 4,
    XCN_CERT_NAME_STR_DISABLE_IE4_UTF8_FLAG = 65536, // 0x00010000
    XCN_CERT_NAME_STR_ENABLE_T61_UNICODE_FLAG = 131072, // 0x00020000
    XCN_CERT_NAME_STR_ENABLE_UTF8_UNICODE_FLAG = 262144, // 0x00040000
    XCN_CERT_NAME_STR_FORCE_UTF8_DIR_STR_FLAG = 524288, // 0x00080000
    XCN_CERT_NAME_STR_DISABLE_UTF8_DIR_STR_FLAG = 1048576, // 0x00100000
    XCN_CERT_NAME_STR_ENABLE_PUNYCODE_FLAG = 2097152, // 0x00200000
    XCN_CERT_NAME_STR_FORWARD_FLAG = 16777216, // 0x01000000
    XCN_CERT_NAME_STR_REVERSE_FLAG = 33554432, // 0x02000000
    XCN_CERT_NAME_STR_COMMA_FLAG = 67108864, // 0x04000000
    XCN_CERT_NAME_STR_CRLF_FLAG = 134217728, // 0x08000000
    XCN_CERT_NAME_STR_NO_QUOTING_FLAG = 268435456, // 0x10000000
    XCN_CERT_NAME_STR_NO_PLUS_FLAG = 536870912, // 0x20000000
    XCN_CERT_NAME_STR_SEMICOLON_FLAG = 1073741824, // 0x40000000
  }
}
