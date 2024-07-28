// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionKey
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionKey
  {
    public SessionKey(SessionId sessionId, string protocol)
    {
      this.Protocol = protocol;
      this.SessionId = sessionId;
    }

    public SessionId SessionId { get; }

    public string Protocol { get; }

    public override string ToString()
    {
      string protocol = this.Protocol;
      SessionId sessionId = this.SessionId;
      string name = sessionId.Name;
      sessionId = this.SessionId;
      // ISSUE: variable of a boxed type
      __Boxed<Guid> id = (ValueType) sessionId.Id;
      return string.Format("{0} {1} ({2})", (object) protocol, (object) name, (object) id);
    }
  }
}
