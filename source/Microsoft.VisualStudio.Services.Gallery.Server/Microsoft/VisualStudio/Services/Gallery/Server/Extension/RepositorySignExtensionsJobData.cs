// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.RepositorySignExtensionsJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class RepositorySignExtensionsJobData
  {
    public RepositorySignFilterType RepositorySignFilterType { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public List<ExtensionInformation> ArtifactFilesToSign { get; set; } = new List<ExtensionInformation>();

    public bool LatestVersionsOnly { get; set; } = true;

    public int TotalExtensionsToSign { get; set; } = 10;

    public int TotalExtensionsSignedSuccessFully { get; set; }

    public int TotalExtensionsFailedSigning { get; set; }

    public bool RequeueForNextIteration { get; set; }

    public int MaxJobRuntimeInMinutes { get; set; } = 60;

    public int CurrentPageNumber { get; set; }

    public int SigningTimeOutDuration { get; set; } = 15;

    public bool IncrementPage { get; set; } = true;

    public bool SingleRequest { get; set; }

    public string SingleRequestExtensionName { get; set; }

    public string SingleRequestPublisherName { get; set; }

    public string SingleRequestVersion { get; set; }

    public string SingleRequestTargetPlatform { get; set; }

    public string SingleRequestCDNRootDirectory { get; set; }
  }
}
