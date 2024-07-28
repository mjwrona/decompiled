// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.SharedResourceData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class SharedResourceData
  {
    public Uri BuildUri { get; set; }

    public string ResourceName { get; set; }

    public string InstanceId { get; set; }

    public string MessageQueueUrl { get; set; }

    public string LockedBy { get; set; }

    public string EndpointUrl { get; set; }
  }
}
