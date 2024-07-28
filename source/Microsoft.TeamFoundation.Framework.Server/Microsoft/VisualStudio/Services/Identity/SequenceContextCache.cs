// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SequenceContextCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class SequenceContextCache
  {
    private SequenceContext m_sequenceContext;
    private readonly object m_sequenceContextLock = new object();

    internal SequenceContextCache(SequenceContext sequenceContext, DateTimeOffset time)
    {
      ArgumentUtility.CheckForNull<SequenceContext>(sequenceContext, nameof (sequenceContext));
      this.m_sequenceContext = sequenceContext.Clone();
      this.Time = time;
    }

    internal SequenceContext SequenceContext => this.m_sequenceContext.Clone();

    internal DateTimeOffset Time { get; }

    internal bool IsNullOrExpired(TimeSpan timeToLive) => this.m_sequenceContext == null || this.IsExpired(timeToLive, DateTimeOffset.UtcNow);

    internal SequenceContext CompareAndSwapSequenceContextIfGreater(SequenceContext sequenceContext)
    {
      ArgumentUtility.CheckForNull<SequenceContext>(sequenceContext, nameof (sequenceContext));
      if (sequenceContext.GroupSequenceId <= this.m_sequenceContext.GroupSequenceId && sequenceContext.IdentitySequenceId <= this.m_sequenceContext.IdentitySequenceId && sequenceContext.OrganizationIdentitySequenceId <= this.m_sequenceContext.OrganizationIdentitySequenceId)
        return this.SequenceContext;
      lock (this.m_sequenceContextLock)
      {
        bool flag = false;
        long identitySequenceId1 = this.m_sequenceContext.IdentitySequenceId;
        long groupSequenceId = this.m_sequenceContext.GroupSequenceId;
        long identitySequenceId2 = this.m_sequenceContext.OrganizationIdentitySequenceId;
        if (sequenceContext.IdentitySequenceId > this.m_sequenceContext.IdentitySequenceId)
        {
          identitySequenceId1 = sequenceContext.IdentitySequenceId;
          flag = true;
        }
        if (sequenceContext.GroupSequenceId > this.m_sequenceContext.GroupSequenceId)
        {
          groupSequenceId = sequenceContext.GroupSequenceId;
          flag = true;
        }
        if (sequenceContext.OrganizationIdentitySequenceId > this.m_sequenceContext.OrganizationIdentitySequenceId)
        {
          identitySequenceId2 = sequenceContext.OrganizationIdentitySequenceId;
          flag = true;
        }
        if (flag)
          this.m_sequenceContext = new SequenceContext(identitySequenceId1, groupSequenceId, identitySequenceId2);
      }
      return this.SequenceContext;
    }

    private bool IsExpired(TimeSpan timeToLive, DateTimeOffset now) => this.Time + timeToLive < now;
  }
}
