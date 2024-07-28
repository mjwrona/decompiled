// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IdentityEventMessage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class IdentityEventMessage
  {
    private IVssRequestContext m_requestContext;
    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> m_identities;

    public IdentityEventMessage(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      SequenceIds sequenceIds)
    {
      this.m_requestContext = requestContext;
      this.m_identities = identities;
      this.SequenceIds = sequenceIds;
    }

    public void Process(out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> processedGroups)
    {
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext, false, DatabaseConnectionType.Default))
      {
        ProcessIdentityEventMessage element = DalSqlElement.GetElement<ProcessIdentityEventMessage>(sqlBatch);
        element.InitializeParams(this.m_identities, this.SequenceIds);
        element.Process();
        processedGroups = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) element.ProcessedGroups;
      }
    }

    public SequenceIds SequenceIds { get; private set; }

    public string Type => "IDENTITY";
  }
}
