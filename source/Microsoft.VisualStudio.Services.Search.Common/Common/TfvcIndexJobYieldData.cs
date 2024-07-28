// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcIndexJobYieldData : AbstractJobYieldData
  {
    private readonly TraceMetaData m_traceMetaData;

    [DataMember]
    public string BaseChangeSetId { get; set; }

    [DataMember]
    public string TargetChangeSetId { get; set; }

    [DataMember]
    public DateTime TargetChangeSetDate { get; set; }

    [DataMember]
    public string ContinuationToken { get; set; }

    [DataMember]
    public TfvcCrawlType TfvcCrawlType { get; set; }

    [DataMember]
    public TfvcBranchData BranchData { get; set; }

    [DataMember]
    public string IntermediateChangeSetId { get; set; }

    [DataMember]
    public string LastAttemptedTargetChangesetId { get; set; }

    public TfvcIndexJobYieldData()
    {
      this.BaseChangeSetId = string.Empty;
      this.IntermediateChangeSetId = string.Empty;
      this.TargetChangeSetId = string.Empty;
      this.ContinuationToken = string.Empty;
      this.LastAttemptedTargetChangesetId = string.Empty;
      this.TargetChangeSetDate = RepositoryConstants.DefaultLastIndexChangeUtcTime;
      this.TfvcCrawlType = TfvcCrawlType.UnkownCrawl;
      this.BranchData = new TfvcBranchData();
      this.m_traceMetaData = new TraceMetaData(1080135, "Indexing Pipeline", "Indexer");
    }

    public override bool HasData() => this.HasEntireRepoCrawlData() || this.HasBranchWisePhase1CrawlData();

    public virtual bool HasEntireRepoCrawlData()
    {
      bool flag = (this.TfvcCrawlType == TfvcCrawlType.UnkownCrawl || this.TfvcCrawlType == TfvcCrawlType.NonBranchCrawl || this.TfvcCrawlType == TfvcCrawlType.BranchWiseCrawlPhase2) && !string.IsNullOrWhiteSpace(this.TargetChangeSetId) && int.Parse(this.TargetChangeSetId, (IFormatProvider) CultureInfo.InvariantCulture) > -1;
      if (this.TfvcCrawlType == TfvcCrawlType.BranchWiseCrawlPhase2 && !flag)
        Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Unexpected yield data present. Assuming no yield data present. yield data: {0}", (object) this.ToString())));
      return flag;
    }

    public virtual bool HasBranchWisePhase1CrawlData()
    {
      bool flag = false;
      if (this.TfvcCrawlType == TfvcCrawlType.BranchWiseCrawlPhase1)
      {
        flag = this.BranchData != null && this.BranchData.CurrentGroupInProgress > 0 && this.BranchData.AllBranchGroups != null && this.BranchData.AllBranchGroups.Any<TfvcBranchGroup>((Func<TfvcBranchGroup, bool>) (group => group.GroupId == this.BranchData.CurrentGroupInProgress)) && !string.IsNullOrWhiteSpace(this.TargetChangeSetId) && int.Parse(this.TargetChangeSetId, (IFormatProvider) CultureInfo.InvariantCulture) > -1;
        if (!flag)
          Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("HasBranchWisePhase1CrawlData: Unexpected yield data present. Discarding this returning as if no data was present. It should trigger fresh BI. yield data: {0}", (object) this.ToString())));
      }
      return flag;
    }

    public override AbstractJobYieldData Clone()
    {
      TfvcIndexJobYieldData indexJobYieldData = (TfvcIndexJobYieldData) this.MemberwiseClone();
      indexJobYieldData.BranchData = this.BranchData?.Clone();
      return (AbstractJobYieldData) indexJobYieldData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[Base Changeset Id: ");
      stringBuilder.Append(this.BaseChangeSetId);
      stringBuilder.Append(", Intermediate Changeset Id: ");
      stringBuilder.Append(this.IntermediateChangeSetId);
      stringBuilder.Append(", Target Changeset Id: ");
      stringBuilder.Append(this.TargetChangeSetId);
      stringBuilder.Append(", Last Attempted Changeset Id: ");
      stringBuilder.Append(this.LastAttemptedTargetChangesetId);
      stringBuilder.Append(", Target Changeset Time: ");
      stringBuilder.Append((object) this.TargetChangeSetDate);
      stringBuilder.Append(", Continuation token: ");
      stringBuilder.Append(this.ContinuationToken);
      stringBuilder.Append(", Crawl Type: ");
      stringBuilder.Append((object) this.TfvcCrawlType);
      stringBuilder.Append(", BranchData: ");
      stringBuilder.Append((object) this.BranchData);
      stringBuilder.Append("]");
      stringBuilder.Append(base.ToString());
      return stringBuilder.ToString();
    }

    internal virtual List<string> GetBranchesList()
    {
      List<string> branchesList = new List<string>();
      if (this.BranchData?.AllBranchGroups != null)
      {
        foreach (TfvcBranchGroup allBranchGroup in this.BranchData.AllBranchGroups)
        {
          if (allBranchGroup.BranchesInfo != null)
            branchesList.AddRange(allBranchGroup.BranchesInfo.Select<TfvcBranchInfo, string>((Func<TfvcBranchInfo, string>) (branchInfo => branchInfo.BranchName)));
        }
      }
      return branchesList;
    }
  }
}
