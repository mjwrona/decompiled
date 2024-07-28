// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public struct SessionId : IEquatable<SessionId>
  {
    private SessionId(string name)
      : this()
    {
      this.Name = name;
      this.Id = GuidUtils.GenerateGuidFromString(PackagingProvenanceConstants.SessionNamespaceBytes, name);
    }

    private SessionId(Guid id)
    {
      this.Name = id.ToString();
      this.Id = id;
    }

    public string Name { get; }

    public Guid Id { get; }

    public static SessionId Empty => new SessionId(Guid.Empty);

    public static SessionId CreateNew(string feedId) => !string.IsNullOrWhiteSpace(feedId) ? new SessionId("@" + Path.GetRandomFileName().Substring(0, 8) + "_" + feedId) : throw new ArgumentNullException(nameof (feedId));

    public static bool TryParse(string sessionId, out SessionId parsedSessionId)
    {
      if (sessionId.StartsWith("@"))
      {
        parsedSessionId = new SessionId(sessionId);
        return true;
      }
      Guid result;
      if (Guid.TryParse(sessionId, out result) && SessionId.IsV7Guid(result))
      {
        parsedSessionId = new SessionId(result);
        return true;
      }
      parsedSessionId = SessionId.Empty;
      return false;
    }

    public bool Equals(SessionId other) => string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj is SessionId other && this.Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name);

    public override string ToString() => this.Name;

    private static bool IsV7Guid(Guid g) => (int) g.ToByteArray()[7] >> 4 == 7;

    public static bool operator ==(SessionId a, SessionId b) => a.Equals(b);

    public static bool operator !=(SessionId a, SessionId b) => !a.Equals(b);
  }
}
