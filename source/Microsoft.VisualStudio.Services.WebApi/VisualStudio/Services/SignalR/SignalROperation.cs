// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.SignalROperation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.SignalR
{
  [DataContract]
  public sealed class SignalROperation : ISignalRObjectContainer
  {
    [DataMember(Name = "objects")]
    private List<SignalRObject> m_objects;

    public List<SignalRObject> Operations
    {
      get
      {
        if (this.m_objects == null)
          this.m_objects = new List<SignalRObject>();
        return this.m_objects;
      }
    }
  }
}
