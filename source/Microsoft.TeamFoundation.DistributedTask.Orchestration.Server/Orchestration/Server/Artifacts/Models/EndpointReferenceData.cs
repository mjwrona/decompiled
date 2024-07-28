// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.EndpointReferenceData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class EndpointReferenceData
  {
    public Guid Id { get; set; }

    public Uri Url { get; set; }

    public override string ToString() => string.Format("Id : {0}, Url : {1}", (object) this.Id.ToString(), (object) this.Url?.ToString());
  }
}
