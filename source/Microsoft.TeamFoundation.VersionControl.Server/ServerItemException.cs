// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerItemException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public abstract class ServerItemException : ServerException
  {
    internal string m_serverItem;

    protected ServerItemException(
      string resourceKey,
      string serverItem,
      string additionalParameter)
      : base(Resources.Format(resourceKey, (object) serverItem, (object) additionalParameter))
    {
      this.m_serverItem = serverItem;
      this.EventId = TeamFoundationEventId.ServerItemException;
    }

    protected ServerItemException(string resourceKey, string serverItem)
      : base(Resources.Format(resourceKey, (object) serverItem))
    {
      this.m_serverItem = serverItem;
      this.EventId = TeamFoundationEventId.ServerItemException;
    }

    protected ServerItemException(string errorMessage)
      : base(errorMessage)
    {
      this.EventId = TeamFoundationEventId.ServerItemException;
    }

    public string ServerItem => this.m_serverItem;

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      if (this.m_serverItem == null)
        return;
      if (VersionControlPath.IsServerItem(this.m_serverItem))
        failure.ServerItem = this.m_serverItem;
      else
        failure.LocalItem = this.m_serverItem;
    }
  }
}
