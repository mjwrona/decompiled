// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupMembershipChangeMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class GroupMembershipChangeMessage
  {
    public Guid InstanceId { get; set; }

    public long SequenceId { get; set; }

    public GroupMembershipChangeMessage.Change[] Changes { get; set; }

    public struct Change
    {
      public Guid ContainerId { get; set; }

      public Guid MemberId { get; set; }

      public bool Active { get; set; }
    }
  }
}
