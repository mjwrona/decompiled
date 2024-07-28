// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WhitelistedCommandKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct WhitelistedCommandKey : IEquatable<WhitelistedCommandKey>
  {
    public string Application { get; private set; }

    public string Command { get; private set; }

    public WhitelistedCommandKey(string application, string command)
      : this()
    {
      this.Application = application;
      this.Command = command;
    }

    public bool Equals(WhitelistedCommandKey other) => string.Equals(this.Application, other.Application, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Command, other.Command, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj != null && obj is WhitelistedCommandKey other && this.Equals(other);

    public override int GetHashCode() => (this.Application + this.Command).ToLower().GetHashCode();

    public static bool operator ==(WhitelistedCommandKey left, WhitelistedCommandKey right) => left.Equals(right);

    public static bool operator !=(WhitelistedCommandKey left, WhitelistedCommandKey right) => !left.Equals(right);
  }
}
