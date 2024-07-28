// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskPackageResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class TaskPackageResources : IDisposable
  {
    private bool m_disposed;

    public TaskPackageResources(
      TaskDefinition definition,
      TaskPackageResource icon,
      Stream packageStream)
    {
      this.Definition = definition;
      this.Icon = icon;
      this.PackageStream = packageStream;
    }

    public TaskDefinition Definition { get; }

    public TaskPackageResource Icon { get; }

    public Stream PackageStream { get; }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.Icon?.Dispose();
      this.PackageStream?.Dispose();
      this.m_disposed = true;
    }
  }
}
