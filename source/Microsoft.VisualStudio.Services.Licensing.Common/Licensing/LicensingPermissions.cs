// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingPermissions
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingPermissions
  {
    public const int Read = 1;
    public const int Create = 2;
    public const int Modify = 4;
    public const int Delete = 8;
    public const int Assign = 16;
    public const int Revoke = 32;
    public const int All = 63;
  }
}
