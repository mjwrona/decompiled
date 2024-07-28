// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskPackageResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class TaskPackageResource : IDisposable
  {
    private bool m_disposed;

    public string Name { get; set; }

    public Stream Stream { get; set; }

    public long Length { get; set; }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing && this.Stream != null)
      {
        this.Stream.Dispose();
        this.Stream = (Stream) null;
      }
      this.m_disposed = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
