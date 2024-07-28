// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreCloudBlockBlob
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreCloudBlockBlob : ILogStoreCloudBlockBlob
  {
    private CloudBlockBlob _cloudBlockBlob;

    public LogStoreCloudBlockBlob(CloudBlockBlob cloudBlockBlob) => this._cloudBlockBlob = cloudBlockBlob;

    public CloudBlockBlob GetCloudBlockBlob() => this._cloudBlockBlob;
  }
}
