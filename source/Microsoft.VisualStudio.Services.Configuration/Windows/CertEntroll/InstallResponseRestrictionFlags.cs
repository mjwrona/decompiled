// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.InstallResponseRestrictionFlags
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [TypeIdentifier("728AB348-217D-11DA-B2A4-000E7BBB2B09", "Windows.CertEntroll.InstallResponseRestrictionFlags")]
  public enum InstallResponseRestrictionFlags
  {
    AllowNone = 0,
    AllowNoOutstandingRequest = 1,
    AllowUntrustedCertificate = 2,
    AllowUntrustedRoot = 4,
  }
}
