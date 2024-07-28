// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingCommonConstants
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingCommonConstants
  {
    public const string LicensingInstanceTypeString = "00000043-0000-8888-8000-000000000000";
    public static readonly Guid LicensingInstanceType = new Guid("00000043-0000-8888-8000-000000000000");
    public const string RegisterAreaFeatureFlag = "AzureDevOps.Services.Licensing.RegisterLicensingResourceAreas";
    public static Guid SpsPrincipalId = new Guid("00000001-0000-8888-8000-000000000000");
  }
}
