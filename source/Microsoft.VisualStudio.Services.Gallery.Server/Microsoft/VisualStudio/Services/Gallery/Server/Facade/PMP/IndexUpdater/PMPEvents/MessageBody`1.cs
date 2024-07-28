// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.MessageBody`1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents
{
  internal class MessageBody<T>
  {
    public Guid ID { get; set; }

    public string Source { get; set; }

    public string Type { get; set; }

    public string Version { get; set; }

    public T Data { get; set; }

    public DateTimeOffset Time { get; set; }

    public Guid CorrelationId { get; set; }
  }
}
