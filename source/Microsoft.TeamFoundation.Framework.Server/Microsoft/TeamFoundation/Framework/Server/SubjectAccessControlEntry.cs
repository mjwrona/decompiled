// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SubjectAccessControlEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete("This type is no longer used.")]
  public class SubjectAccessControlEntry : ICloneable
  {
    public SubjectAccessControlEntry()
    {
    }

    public SubjectAccessControlEntry(string subject, string token, int allow, int deny)
    {
      this.Subject = subject;
      this.Token = token;
      this.Allow = allow;
      this.Deny = deny;
    }

    public string Subject { get; set; }

    public string Token { get; set; }

    public int Allow { get; set; }

    public int Deny { get; set; }

    public override bool Equals(object o)
    {
      if (this == o)
        return true;
      return o is SubjectAccessControlEntry accessControlEntry && string.Equals(this.Token, accessControlEntry.Token, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.Subject, accessControlEntry.Subject, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode() => (this.Subject == null ? 0 : this.Subject.GetHashCode()) + 23 * (this.Token == null ? 0 : this.Token.GetHashCode()) + this.Allow + this.Deny;

    public object Clone() => (object) new SubjectAccessControlEntry(this.Subject, this.Token, this.Allow, this.Deny);
  }
}
