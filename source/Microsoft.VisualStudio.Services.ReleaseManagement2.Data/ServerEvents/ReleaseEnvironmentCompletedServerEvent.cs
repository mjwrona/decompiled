// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents.ReleaseEnvironmentCompletedServerEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents
{
  [ServiceEventObject]
  public class ReleaseEnvironmentCompletedServerEvent
  {
    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release Release { get; set; }

    public int CurrentEnvironmentId { get; set; }

    public Guid ProjectId { get; set; }

    public EnvironmentStatus Status { get; set; }

    public string Comment { get; set; }
  }
}
