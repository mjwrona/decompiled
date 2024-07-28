// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing.ConstantsProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing
{
  internal class ConstantsProcessor : PayloadProcessor
  {
    public ConstantsProcessor(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    public override bool ProcessRow(PayloadTable.PayloadRow payloadRow)
    {
      try
      {
        if (payloadRow != null)
        {
          if (this.IsDeletedIdentityRow(payloadRow))
            this.ObfuscateIdentityFields(payloadRow);
        }
      }
      catch
      {
      }
      return true;
    }

    private bool IsDeletedIdentityRow(PayloadTable.PayloadRow row) => (bool) row["fDeleted"] && (string) row["Sid"] != null;

    private void ObfuscateIdentityFields(PayloadTable.PayloadRow row)
    {
      row.SetValue("String", (object) InternalsResourceStrings.Get("UnknownUser"));
      row.SetValue("DisplayName", (object) InternalsResourceStrings.Get("UnknownUser"));
    }
  }
}
