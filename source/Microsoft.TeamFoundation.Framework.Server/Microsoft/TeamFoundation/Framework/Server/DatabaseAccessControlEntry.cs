// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseAccessControlEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct DatabaseAccessControlEntry
  {
    public readonly Guid SubjectId;
    public readonly string Token;
    public readonly int Allow;
    public readonly int Deny;
    public readonly bool IsDeleted;

    public DatabaseAccessControlEntry(
      Guid subjectId,
      string token,
      int allow,
      int deny,
      bool isDeleted)
    {
      this.SubjectId = subjectId;
      this.Token = token;
      this.Allow = allow;
      this.Deny = deny;
      this.IsDeleted = isDeleted;
    }

    public override string ToString() => string.Format("[SubjectId={0}, Token={1}, Allow={2}, Deny={3}]", (object) this.SubjectId, (object) this.Token, (object) this.Allow, (object) this.Deny);
  }
}
