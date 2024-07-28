// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundledFile
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public abstract class BundledFile
  {
    public string Name { get; internal set; }

    internal string LocalUri { get; set; }

    public string CDNRelativeUri { get; internal set; }

    public bool IsEmpty { get; internal set; }

    public int ContentLength { get; internal set; }

    internal bool UsesVersionBasedPathForCdn { get; set; }

    public string Integrity { get; internal set; }
  }
}
