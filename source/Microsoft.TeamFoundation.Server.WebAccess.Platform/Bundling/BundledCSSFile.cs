// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundledCSSFile
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundledCSSFile : BundledFile
  {
    public BundledCSSFile() => this.UsesVersionBasedPathForCdn = true;

    public CSSBundleDefinition Definition { get; set; }

    public ISet<string> IncludedCssFiles { get; internal set; }

    internal string FallbackThemeLocalUri { get; set; }

    public string FallbackThemeCDNRelativeUri { get; internal set; }
  }
}
