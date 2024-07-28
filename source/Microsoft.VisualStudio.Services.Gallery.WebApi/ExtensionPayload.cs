// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionPayload
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionPayload
  {
    public string FileName { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public ExtensionDeploymentTechnology Type { get; set; }

    public List<KeyValuePair<string, string>> Metadata { get; set; }

    public List<InstallationTarget> InstallationTargets { get; set; }

    public bool IsValid { get; set; }

    public bool IsSignedByMicrosoft { get; set; }

    public bool IsPreview { get; set; }
  }
}
