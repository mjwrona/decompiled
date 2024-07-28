// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing.ClassificationNodeProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing
{
  internal class ClassificationNodeProcessor : PayloadProcessor
  {
    public ClassificationNodeProcessor(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    public override bool ProcessRow(PayloadTable.PayloadRow payloadRow)
    {
      try
      {
        if (payloadRow != null)
        {
          if (this.IsProjectNode(payloadRow))
            this.FixProjectName(payloadRow, this.m_requestContext);
        }
      }
      catch
      {
      }
      return true;
    }

    private bool IsProjectNode(PayloadTable.PayloadRow row) => (int) row["TypeID"] == -42;

    private void FixProjectName(PayloadTable.PayloadRow row, IVssRequestContext requestContext)
    {
      Guid projectId = Guid.Parse((string) row["GUID"]);
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
      row.SetValue("Name", (object) projectName);
    }
  }
}
