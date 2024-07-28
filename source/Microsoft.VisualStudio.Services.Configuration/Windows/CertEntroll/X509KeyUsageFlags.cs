// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.X509KeyUsageFlags
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [TypeIdentifier("728AB348-217D-11DA-B2A4-000E7BBB2B09", "Windows.CertEntroll.X509KeyUsageFlags")]
  public enum X509KeyUsageFlags
  {
    XCN_CERT_NO_KEY_USAGE = 0,
    XCN_CERT_ENCIPHER_ONLY_KEY_USAGE = 1,
    XCN_CERT_CRL_SIGN_KEY_USAGE = 2,
    XCN_CERT_OFFLINE_CRL_SIGN_KEY_USAGE = 2,
    XCN_CERT_KEY_CERT_SIGN_KEY_USAGE = 4,
    XCN_CERT_KEY_AGREEMENT_KEY_USAGE = 8,
    XCN_CERT_DATA_ENCIPHERMENT_KEY_USAGE = 16, // 0x00000010
    XCN_CERT_KEY_ENCIPHERMENT_KEY_USAGE = 32, // 0x00000020
    XCN_CERT_NON_REPUDIATION_KEY_USAGE = 64, // 0x00000040
    XCN_CERT_DIGITAL_SIGNATURE_KEY_USAGE = 128, // 0x00000080
    XCN_CERT_DECIPHER_ONLY_KEY_USAGE = 32768, // 0x00008000
  }
}
