// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IAADAuthSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public interface IAADAuthSettings
  {
    bool Enabled { get; }

    string Authority { get; }

    string Issuer { get; }

    bool CreateAADTenant { get; }

    IEnumerable<string> BlockedAADAppIds { get; }

    IEnumerable<string> MsaPassthroughBlockedAADAppIds { get; }

    IEnumerable<string> OnBehalfOfAllowedAADAppIds { get; }

    string AADGraphResource { get; }
  }
}
