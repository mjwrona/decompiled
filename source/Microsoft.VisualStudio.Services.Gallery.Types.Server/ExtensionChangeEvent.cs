// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.ExtensionChangeEvent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  [DataContract]
  [ServiceEventObject]
  public class ExtensionChangeEvent
  {
    [DataMember]
    public ExtensionEventType EventType { get; set; }

    [DataMember]
    public string PublisherName { get; set; }

    [DataMember]
    public string ExtensionName { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public int Flags { get; set; }

    [DataMember]
    public Guid HostId { get; set; }
  }
}
