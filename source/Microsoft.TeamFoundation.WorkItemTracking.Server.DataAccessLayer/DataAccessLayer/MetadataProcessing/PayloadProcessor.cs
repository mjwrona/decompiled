// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing.PayloadProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing
{
  internal abstract class PayloadProcessor
  {
    protected IVssRequestContext m_requestContext;

    public PayloadProcessor(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
    }

    internal PayloadProcessor()
    {
    }

    public abstract bool ProcessRow(PayloadTable.PayloadRow payloadRow);
  }
}
