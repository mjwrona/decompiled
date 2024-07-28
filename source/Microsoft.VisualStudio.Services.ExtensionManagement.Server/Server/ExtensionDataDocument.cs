// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionDataDocument
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [DataContract]
  internal sealed class ExtensionDataDocument
  {
    [DataMember]
    public byte[] DocumentId { get; set; }

    [DataMember]
    public string DocumentValue { get; set; }

    [DataMember]
    public long Size { get; set; }

    [DataMember]
    public byte Location { get; set; }

    [DataMember]
    public int Version { get; set; }
  }
}
