// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostModifiedMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class HostModifiedMessage
  {
    public Guid HostId;
    public string HostName;
    public string PreviousHostName;
    public HostModificationType ModificationType;

    public static HostModifiedMessage CreateHostModifiedMessage(TeamFoundationHostType hostType)
    {
      if (hostType == TeamFoundationHostType.Application)
        return (HostModifiedMessage) new OrganizationHostModifiedMessage();
      if (hostType == TeamFoundationHostType.ProjectCollection)
        return (HostModifiedMessage) new CollectionHostModifiedMessage();
      throw new ArgumentException(string.Format("Cannot create HostModifiedMessage for host type '{0}'.", (object) hostType));
    }
  }
}
