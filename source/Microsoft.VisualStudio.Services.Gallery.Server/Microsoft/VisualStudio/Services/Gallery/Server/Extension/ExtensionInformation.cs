// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionInformation
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class ExtensionInformation
  {
    public string Name { get; set; }

    public string PublisherName { get; set; }

    public string Version { get; set; }

    public string TargetPlatform { get; set; }

    public string CDNRootDirectory { get; set; }

    public RepositorySigningInfo RepositorySigningInfo { get; set; }

    public ValidationStatus SigningStatus { get; set; }

    public DateTime StartTime { get; set; }

    public int SigningTimeOutDurationInMinutes { get; set; }
  }
}
