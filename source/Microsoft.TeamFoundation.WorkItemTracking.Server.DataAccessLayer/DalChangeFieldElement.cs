// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeFieldElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeFieldElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      string nodeName,
      bool changeName,
      int type,
      bool changedFieldAttributeType,
      int reportingType,
      bool reportingTypeChanged,
      int reportingFormula,
      bool reportingFormulaChanged,
      bool reportingEnabled,
      bool reportingEnabledChanged,
      string reportingName,
      string reportingReferenceName,
      string referenceName,
      bool referenceNameChanged,
      string cacheStamp,
      bool newItem)
    {
      this.m_elementGroup = group;
      this.m_outputs = 1;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.");
      this.AppendSql("ChangeField");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      if (newItem || cacheStamp == null || cacheStamp.Trim().Length == 0)
      {
        this.AppendSql("default,");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterBinary(DalMetadataSelectElement.ConvertLongToByteArray(Convert.ToInt64("0x" + cacheStamp, 16))));
        this.AppendSql(",");
      }
      this.AppendSql("@O");
      this.AppendSql(DalSqlElement.InlineInt(actionId));
      this.AppendSql(" output,");
      this.AppendSql("default,");
      if (changeName)
      {
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(nodeName));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (referenceNameChanged)
      {
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(referenceName));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (changedFieldAttributeType)
      {
        this.AppendSql(DalSqlElement.InlineInt(type));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (reportingTypeChanged)
      {
        this.AppendSql(DalSqlElement.InlineInt(reportingType));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (reportingFormulaChanged)
      {
        this.AppendSql(DalSqlElement.InlineInt(reportingFormula));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (reportingEnabledChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(reportingEnabled));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (!string.IsNullOrEmpty(reportingName))
      {
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(reportingName));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (!string.IsNullOrEmpty(reportingReferenceName))
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(reportingReferenceName));
      else
        this.AppendSql("default");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("insert into ");
      this.AppendSql("@tempIdMap");
      this.AppendSql(" select ");
      string parameterName = "@TO" + DalSqlElement.InlineInt(actionId);
      this.AppendSql(this.SqlBatch.AddParameterInt(-actionId - 20000, parameterName));
      this.AppendSql(",");
      this.AppendSql("@O");
      this.AppendSql(DalSqlElement.InlineInt(actionId));
      this.AppendSql(Environment.NewLine);
    }
  }
}
