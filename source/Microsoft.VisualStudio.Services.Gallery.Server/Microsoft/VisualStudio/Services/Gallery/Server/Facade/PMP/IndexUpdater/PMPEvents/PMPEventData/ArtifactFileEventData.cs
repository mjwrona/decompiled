// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.PMPEventData.ArtifactFileEventData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.PMPEventData
{
  public class ArtifactFileEventData : PackageEventData
  {
    public Dictionary<string, List<string>> ArtifactFileIdentifiers { get; set; }

    public ArtifactFileEventData(
      string extensionName,
      string version,
      string targetPlatform,
      string registryName)
      : base(registryName, extensionName, version)
    {
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
      List<string> stringList = new List<string>();
      if (!string.IsNullOrWhiteSpace(targetPlatform))
      {
        stringList.Add(targetPlatform);
        dictionary.Add("targetplatforms", stringList);
      }
      this.ArtifactFileIdentifiers = dictionary;
    }
  }
}
