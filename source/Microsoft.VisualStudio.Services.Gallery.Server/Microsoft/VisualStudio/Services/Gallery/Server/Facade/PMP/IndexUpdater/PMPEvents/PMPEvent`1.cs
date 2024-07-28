// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.PMPEvent`1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents
{
  public abstract class PMPEvent<T>
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Source { get; set; }

    public abstract string Type { get; }

    public Guid CorrelationId { get; set; } = Guid.Empty;

    public string Version { get; set; } = "1.0";

    public DateTimeOffset CreatedDateTime { get; } = (DateTimeOffset) DateTime.UtcNow;

    public T Data { get; set; }

    public PMPEvent(string source, T data)
    {
      this.Source = source;
      this.Data = data;
    }
  }
}
