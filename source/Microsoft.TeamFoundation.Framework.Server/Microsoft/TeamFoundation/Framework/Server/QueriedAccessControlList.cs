// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueriedAccessControlList
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class QueriedAccessControlList
  {
    public readonly string Token;
    public readonly bool Inherit;
    public readonly IList<QueriedAccessControlEntry> AccessControlEntries;

    public QueriedAccessControlList(
      string token,
      bool inherit,
      IList<QueriedAccessControlEntry> accessControlEntries)
    {
      this.Token = token;
      this.Inherit = inherit;
      this.AccessControlEntries = accessControlEntries;
    }
  }
}
