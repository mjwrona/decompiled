// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AuthorizationException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class AuthorizationException : SecurityException
  {
    public AuthorizationException()
      : base(Resources.Get(nameof (AuthorizationException)))
    {
      this.EventId = TeamFoundationEventId.AuthorizationException;
    }

    public AuthorizationException(string identityName)
      : base(Resources.Format("AuthorizationDeniedException", (object) identityName))
    {
      this.EventId = TeamFoundationEventId.AuthorizationException;
    }
  }
}
