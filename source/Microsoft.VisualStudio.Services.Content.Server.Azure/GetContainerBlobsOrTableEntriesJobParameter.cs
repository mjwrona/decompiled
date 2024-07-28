// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.GetContainerBlobsOrTableEntriesJobParameter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Cloud;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  [Serializable]
  public class GetContainerBlobsOrTableEntriesJobParameter
  {
    public StorageType StorageType { get; set; }

    public string ResourceName { get; set; }

    public string DiagnosticConnectionString { get; set; }

    public string ServiceName { get; set; }

    public string DeploymentName { get; set; }
  }
}
