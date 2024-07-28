// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServices.UxServicesRequestData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.UxServices
{
  public class UxServicesRequestData
  {
    public string Control { get; set; }

    public string Brand { get; set; }

    public string Site { get; set; }

    public string Locale { get; set; }

    public LoginContext LoginContext { get; set; }

    public string UxServiceHeaderUrl { get; set; }

    public string UxServiceFooterUrl { get; set; }
  }
}
