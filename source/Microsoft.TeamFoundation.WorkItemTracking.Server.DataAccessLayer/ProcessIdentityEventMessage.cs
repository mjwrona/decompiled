// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessIdentityEventMessage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ProcessIdentityEventMessage : DalSqlElement
  {
    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> m_identities;
    private ElementGroup m_identityGroup;
    private ElementGroup m_updateSeqIdGroup;
    private int m_batchCounter;
    private SequenceIds m_sequenceIds;
    private DateTime m_syncTime;
    private int m_syncBatchSize;

    public void InitializeParams(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities, SequenceIds sequenceIds)
    {
      this.m_identities = identities;
      this.m_sequenceIds = sequenceIds;
      this.m_syncTime = DateTime.Now.ToUniversalTime();
      this.m_syncBatchSize = this.SqlBatch.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.SqlBatch.RequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/IMSSyncBatchSize", 250);
      this.ProcessedGroups = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ProcessedGroups { get; private set; }

    public void Process()
    {
      this.SqlBatch.RequestContext.TraceEnter(900240, "Sync", nameof (ProcessIdentityEventMessage), nameof (Process));
      this.SqlBatch.RequestContext.Trace(900241, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), "Intializing the batch");
      this.InitializeBatch();
      bool premisesDeployment = this.SqlBatch.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      this.SqlBatch.RequestContext.Trace(900242, TraceLevel.Info, "Sync", nameof (ProcessIdentityEventMessage), "Processing Identities");
      StringBuilder stringBuilder1 = new StringBuilder();
      List<StringBuilder> source = new List<StringBuilder>();
      string instanceId = this.SqlBatch.RequestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString("D");
      string collectionId = this.SqlBatch.RequestContext.ServiceHost.InstanceId.ToString("D");
      try
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in this.m_identities)
        {
          this.SqlBatch.RequestContext.Trace(900408, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), string.Format("Processing identity: {0}", (object) identity));
          if (this.SqlBatch.RequestContext.IsCanceled)
          {
            this.SqlBatch.RequestContext.Trace(900243, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), "Process() : Cancel was called, returning midway");
            return;
          }
          if (IdentityConstantsNormalizer.CanSyncIdentity(identity))
          {
            IdentityConstantsNormalizer.NormalizeIdentity(identity, instanceId, collectionId);
            this.SqlBatch.RequestContext.Trace(900409, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), string.Format("Adding identity to batch: {0}", (object) identity));
            DalSqlElement.GetElement<DalAddIdentityElement>(this.SqlBatch).JoinBatch(this.m_identityGroup, identity, premisesDeployment);
            string str;
            if (identity.IsContainer)
            {
              this.ProcessedGroups.Add(identity);
              str = !this.SqlBatch.RequestContext.ExecutionEnvironment.IsDevFabricDeployment ? string.Format("{0}; Membership Count:{1}", (object) identity, (object) identity.Members.Count) : string.Format("{0}; Membership Details: {1}", (object) identity, (object) string.Join<IdentityDescriptor>(",", (IEnumerable<IdentityDescriptor>) identity.Members));
            }
            else
              str = string.Format("{0}", (object) identity);
            if (str.Length + stringBuilder1.Length > 30000)
            {
              source.Add(stringBuilder1);
              stringBuilder1 = new StringBuilder();
            }
            stringBuilder1.AppendLine(str);
            this.IncrementBatchCounter();
          }
        }
        source.Add(stringBuilder1);
        bool aadBackedAccount = this.SqlBatch.RequestContext.WitContext().IsAadBackedAccount;
        DalUpdateSequenceIdElement element = DalSqlElement.GetElement<DalUpdateSequenceIdElement>(this.SqlBatch);
        element.JoinBatch(this.m_updateSeqIdGroup, EventType.Identity, this.m_sequenceIds.IdentitySequenceId, this.m_syncTime, aadBackedAccount);
        element.JoinBatch(this.m_updateSeqIdGroup, EventType.Group, this.m_sequenceIds.IdentityGroupSequenceId, this.m_syncTime, aadBackedAccount);
        element.JoinBatch(this.m_updateSeqIdGroup, EventType.Organization, this.m_sequenceIds.IdentityOrganizationSequenceId, this.m_syncTime, aadBackedAccount);
        this.ExecuteBatch();
      }
      finally
      {
        try
        {
          EuiiTracingService service = this.SqlBatch.RequestContext.GetService<EuiiTracingService>();
          foreach (StringBuilder stringBuilder2 in source.Where<StringBuilder>((Func<StringBuilder, bool>) (c => c.Length > 0)))
            service.TraceEuii(this.SqlBatch.RequestContext, "Sync", "Sync", "SyncIdentitiesDetails", stringBuilder2.ToString());
        }
        catch (Exception ex)
        {
          this.SqlBatch.RequestContext.TraceException(900574, "Sync", nameof (ProcessIdentityEventMessage), ex);
        }
        this.SqlBatch.RequestContext.TraceLeave(900573, "Sync", nameof (ProcessIdentityEventMessage), nameof (Process));
      }
    }

    private void InitializeBatch()
    {
      this.SqlBatch.RequestContext.TraceEnter(900244, "Sync", nameof (ProcessIdentityEventMessage), nameof (InitializeBatch));
      int defaultValue = this.SqlBatch.RequestContext.ExecutionEnvironment.IsHostedDeployment ? 600 : 3600;
      this.SqlBatch.ExecutionTimeout = new int?(this.SqlBatch.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.SqlBatch.RequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/IMSSyncTimeOutInSeconds", false, defaultValue));
      this.SqlBatch.ReInitializeBatch();
      this.m_identityGroup = new ElementGroup();
      this.m_updateSeqIdGroup = new ElementGroup();
      this.m_batchCounter = 0;
      this.SqlBatch.RequestContext.TraceLeave(900246, "Sync", nameof (ProcessIdentityEventMessage), nameof (InitializeBatch));
    }

    private void ExecuteBatch()
    {
      if (this.SqlBatch.RequestContext.IsCanceled)
      {
        this.SqlBatch.RequestContext.Trace(900247, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), "Process() : Cancel was called, returning midway");
      }
      else
      {
        this.SqlBatch.RequestContext.Trace(900508, TraceLevel.Verbose, "Sync", nameof (ProcessIdentityEventMessage), "BatchCounter = " + this.m_batchCounter.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.SqlBatch.AppendSql(Environment.NewLine);
        this.SqlBatch.AppendSql("declare ");
        this.SqlBatch.AppendSql("@SyncTime");
        this.SqlBatch.AppendSql(" as datetime; set ");
        this.SqlBatch.AppendSql("@SyncTime");
        this.SqlBatch.AppendSql("='");
        this.SqlBatch.AppendSql(this.m_syncTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
        this.SqlBatch.AppendSql("' ");
        this.SqlBatch.AppendSql(Environment.NewLine);
        this.SqlBatch.AppendSql("declare ");
        this.SqlBatch.AppendSql("@fRollback");
        this.SqlBatch.AppendSql(" as int");
        this.SqlBatch.AppendSql(Environment.NewLine);
        if (this.m_identityGroup.ElementCount > 0)
        {
          for (int index = 0; index < this.m_identityGroup.ElementCount; ++index)
            this.SqlBatch.AppendSql(this.m_identityGroup.GetSql(index));
        }
        if (this.m_updateSeqIdGroup.ElementCount > 0)
        {
          for (int index = 0; index < this.m_updateSeqIdGroup.ElementCount; ++index)
            this.SqlBatch.AppendSql(this.m_updateSeqIdGroup.GetSql(index));
        }
        TeamFoundationTrace.Info(TraceKeywordSets.Database, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExecuteBatch() : [Batch Count = {0}]", (object) this.m_batchCounter.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        this.SqlBatch.ExecuteBatch();
      }
    }

    private void IncrementBatchCounter()
    {
      ++this.m_batchCounter;
      if (this.m_batchCounter != this.m_syncBatchSize)
        return;
      this.ExecuteBatch();
      this.InitializeBatch();
    }
  }
}
