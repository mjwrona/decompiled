// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InvalidIdentityException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class InvalidIdentityException : SecurityException
  {
    internal string m_identityName;

    public InvalidIdentityException(string identityName, string resourceName)
      : base(Resources.Format(resourceName, (object) identityName))
    {
      this.m_identityName = identityName;
      this.EventId = TeamFoundationEventId.InvalidIdentityException;
    }

    public InvalidIdentityException(string identityName)
      : this(identityName, nameof (InvalidIdentityException))
    {
    }

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.IdentityName = this.m_identityName;
    }
  }
}
