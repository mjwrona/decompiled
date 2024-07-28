// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.WellKnownPublisherMetadataLinks
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class WellKnownPublisherMetadataLinks
  {
    public const string Company = "company";
    public const string Support = "support";
    public const string SourceCode = "sourceCode";
    public const string Twitter = "twitter";
    public const string LinkedIn = "linkedIn";
    public const string Profile = "profile";
    public const string Logo = "logo";
    public const string FallbackLogo = "fallbackLogo";
    public static readonly IList<string> PublisherMetadataInputLinks = (IList<string>) new List<string>()
    {
      "company",
      "support",
      "sourceCode",
      "twitter",
      "linkedIn"
    };
    public static readonly IList<string> PublisherMetadataLinks = (IList<string>) new List<string>()
    {
      "company",
      "support",
      "sourceCode",
      "twitter",
      "linkedIn",
      "profile",
      "logo",
      "fallbackLogo"
    };
  }
}
