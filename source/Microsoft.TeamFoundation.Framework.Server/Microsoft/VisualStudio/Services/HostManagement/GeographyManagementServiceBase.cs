// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.GeographyManagementServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  public abstract class GeographyManagementServiceBase : 
    IGeographyManagementService,
    IVssFrameworkService
  {
    private const int c_maxGeographySize = 20;

    protected static void VerifyValidGeographyCode(string code)
    {
      if (string.IsNullOrWhiteSpace(code))
        throw new InvalidGeographyCodeException(FrameworkResources.GeographyCodeIsNullOrOnlyContainsWhiteSpace((object) code));
      if (code.Length > 20)
        throw new InvalidGeographyCodeException(FrameworkResources.GeographyCodeTooLong((object) code, (object) 20));
      foreach (char c in code)
      {
        if (!char.IsLetterOrDigit(c))
          throw new InvalidGeographyCodeException(FrameworkResources.GeographyCodeContainsInvalidCharacter((object) code));
      }
    }

    public abstract Geography GetGeography(IVssRequestContext requestContext, string code);

    public abstract void ServiceStart(IVssRequestContext systemRequestContext);

    public abstract void ServiceEnd(IVssRequestContext systemRequestContext);
  }
}
