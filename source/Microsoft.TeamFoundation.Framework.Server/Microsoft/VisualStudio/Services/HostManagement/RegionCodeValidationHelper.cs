// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.RegionCodeValidationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  public class RegionCodeValidationHelper
  {
    private const int c_maxRegionSize = 20;

    public static void VerifyValidRegionCode(string regionCode)
    {
      if (string.IsNullOrWhiteSpace(regionCode))
        throw new InvalidRegionCodeException(FrameworkResources.RegionCodeIsNullOrOnlyContainsWhiteSpacce((object) regionCode));
      if (regionCode.Length > 20)
        throw new InvalidRegionCodeException(FrameworkResources.RegionCodeTooLong((object) regionCode, (object) 20));
      foreach (char ch in regionCode)
      {
        if ((ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9'))
          throw new InvalidRegionCodeException(FrameworkResources.RegionCodeContainsInvalidCharacter((object) regionCode));
      }
    }
  }
}
