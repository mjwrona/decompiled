// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogDownloadOptions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.DataMovement;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogDownloadOptions : ITestLogDownloadOptions
  {
    private DownloadOptions _downloadOptions;

    public TestLogDownloadOptions(bool enableContentMD5Validation = true) => this._downloadOptions = new DownloadOptions()
    {
      DisableContentMD5Validation = !enableContentMD5Validation
    };

    public DownloadOptions GetDownloadOptions() => this._downloadOptions;
  }
}
