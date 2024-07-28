// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.BasicPduNotificationDetector`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public abstract class BasicPduNotificationDetector<TOp> : IPduNotificationDetector where TOp : class, IPackageVersionOperationData
  {
    public IEnumerable<PendingPduNotification> GetNotificationsForCommits(
      IEnumerable<ICommitLogEntry> flattenedCommits)
    {
      TriggerCommitType triggerType = this.TriggerCommitType;
      return this.FilterCommits(flattenedCommits.FilterToOpType<TOp>()).Select<CastedOpCommit<TOp>, PendingPduNotification>((Func<CastedOpCommit<TOp>, PendingPduNotification>) (op => new PendingPduNotification(op.Commit, triggerType, op.CommitOperationData.Identity, this.GetAffectedViews(op.CommitOperationData))));
    }

    protected abstract CommitAffectsViews GetAffectedViews(TOp opData);

    protected abstract TriggerCommitType TriggerCommitType { get; }

    protected virtual IEnumerable<CastedOpCommit<TOp>> FilterCommits(
      IEnumerable<CastedOpCommit<TOp>> commits)
    {
      return commits;
    }
  }
}
