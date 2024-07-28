// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeRuleElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeRuleElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      bool grantRead,
      bool denyRead,
      bool grantWrite,
      bool denyWrite,
      bool grantAdmin,
      bool denyAdmin,
      bool defaultAttrib,
      bool suggestion,
      bool helpText,
      bool helpTextChanged,
      bool grantReadChanged,
      bool denyReadChanged,
      bool grantWriteChanged,
      bool denyWriteChanged,
      bool grantAdminChanged,
      bool denyAdminChanged,
      bool defaultChanged,
      bool suggestionChanged,
      string cacheStamp)
    {
      this.SqlBatch.RequestContext.Trace(908832, TraceLevel.Info, "DalElements", nameof (DalChangeRuleElement), "Changing rule ID={0}, grantRead={1}, denyRead={2}, grantWrite={3}, denyWrite={4}, grantAdmin={5}, denyAdmin={6}, defaultAttrib={7}, suggestion={8}, helpText={9}", (object) actionId, (object) grantRead, (object) denyRead, (object) grantWrite, (object) denyWrite, (object) grantAdmin, (object) denyAdmin, (object) defaultAttrib, (object) suggestion, (object) helpText);
      this.m_elementGroup = group;
      this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.");
      this.AppendSql("ChangeRule");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      if (cacheStamp.Length > 0)
      {
        this.AppendSql(this.SqlBatch.AddParameterBinary(DalMetadataSelectElement.ConvertLongToByteArray(Convert.ToInt64("0x" + cacheStamp, 16))));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      this.AppendSql("@O");
      this.AppendSql(actionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AppendSql(",");
      if (grantReadChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(grantRead));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (denyReadChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(denyRead));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (grantWriteChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(grantWrite));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (denyWriteChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(denyWrite));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (suggestionChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(suggestion));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (grantAdminChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(grantAdmin));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (denyAdminChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(denyAdmin));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (defaultChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(defaultAttrib));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (helpTextChanged)
        this.AppendSql(DalSqlElement.InlineBit(helpText));
      else
        this.AppendSql("default");
      this.AppendSql(";if ");
      this.AppendSql("@O");
      this.AppendSql(actionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AppendSql(" is null begin rollback transaction; return; end;");
      this.AppendSql(Environment.NewLine);
    }
  }
}
