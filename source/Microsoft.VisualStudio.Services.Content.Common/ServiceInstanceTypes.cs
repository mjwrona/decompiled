// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ServiceInstanceTypes
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ServiceInstanceTypes
  {
    private const string BlobStoreString = "00000019-0000-8888-8000-000000000000";
    private const string ArtifactString = "00000016-0000-8888-8000-000000000000";
    private const string TFSPrincipalStr = "00000002-0000-8888-8000-000000000000";
    public static readonly Guid BlobStore = new Guid("00000019-0000-8888-8000-000000000000");
    public static readonly Guid Artifact = new Guid("00000016-0000-8888-8000-000000000000");
    public static readonly Guid TFS = new Guid("00000002-0000-8888-8000-000000000000");
  }
}
