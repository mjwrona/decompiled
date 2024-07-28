// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSaveRuleElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSaveRuleElement : DalObjectPersistenceElement<InsertRuleAttributes>
  {
    public DalSaveRuleElement()
      : this("@rules", false)
    {
    }

    protected DalSaveRuleElement(string rulesVariable, bool identityFieldsOnly)
    {
      this.AddPrerequisiteSingletonElement<DalGetTempIdMapElement>();
      this.RulesVariable = rulesVariable;
      this.IdentityFieldsOnly = identityFieldsOnly;
    }

    public string RulesVariable { get; private set; }

    public bool IdentityFieldsOnly { get; private set; }

    public virtual void JoinBatch(ElementGroup group, bool overwrite)
    {
      if (this.ValidObjects.Count < 1)
        return;
      this.TraceFormsDetails(this.ValidObjects);
      this.SetOutputs(0);
      this.SetGroup(group);
      if (this.Version >= 7)
      {
        this.AppendSql("insert into ");
        this.AppendSql("@tempIdMap");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("exec ");
        this.AppendSql("@status");
        this.AppendSql(" = dbo.");
        this.AppendSql("SaveRules");
        this.AppendSql(" ");
        this.AppendPartitionIdVariable();
        this.AppendSql("@PersonId");
        this.AppendSql(",");
        this.AppendSql("@NowUtc");
        this.AppendSql(",");
        this.AppendSql(this.SqlBatch.AddParameterTable<InsertRuleAttributes>((WorkItemTrackingTableValueParameter<InsertRuleAttributes>) new RuleTable((IEnumerable<InsertRuleAttributes>) this.ValidObjects), this.RulesVariable));
        this.AppendSql(",");
        this.AppendSql("@tempIdMap");
        if (this.Version >= 29)
        {
          this.AppendSql(",");
          this.AppendSql(DalSqlElement.InlineBit(this.IdentityFieldsOnly));
        }
        this.AppendSql(Environment.NewLine);
        this.AppendSql("if ");
        this.AppendSql("@status");
        this.AppendSql(" <> 0 begin rollback transaction; return; end");
        this.AppendSql(Environment.NewLine);
        if (!overwrite)
          return;
        this.AppendSql(Environment.NewLine);
        this.AppendSql("insert into ");
        this.AppendSql("#ruleIds");
        this.AppendSql(" select t.Id");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("from ");
        this.AppendSql(this.RulesVariable);
        this.AppendSql(" r join ");
        this.AppendSql("@tempIdMap");
        this.AppendSql(" t on t.TempId = r.Id + (");
        this.AppendSql(DalSqlElement.Inline((object) -20000));
        this.AppendSql(")");
        this.AppendSql(Environment.NewLine);
      }
      else
      {
        foreach (InsertRuleAttributes validObject in (IEnumerable<InsertRuleAttributes>) this.ValidObjects)
        {
          this.AppendSql("declare ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.Inline((object) -validObject.Id));
          this.AppendSql(" as int; ");
          this.AppendSql("exec dbo.");
          this.AppendSql("LookupRule");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.Inline((object) -validObject.Id));
          this.AppendSql(" output,");
          if (validObject.tempAreaId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempAreaId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.areaId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempPersonId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempPersonId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.personId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempObjectTypeScopeId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempObjectTypeScopeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.objectTypeScopeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.unless)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempIfFieldId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempIfFieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.ifFieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.ifNot)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempIfConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempIfConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.ifConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempThenFieldId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempThenFieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.thenFieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.thenNot)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenLike)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenTwoPlusLevels)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempThenConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempThenConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.thenConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.flowDownTree)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenOneLevel)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenInterior)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenConstLargeText)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenLeaf)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.reverse)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.inversePersonId)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenImplicitEmpty)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.thenImplicitUnchanged)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempRootTreeId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempRootTreeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.rootTreeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.flowAroundTree)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempField1Id != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField1Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field1Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField1IsConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField1IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field1IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField1WasConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField1WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field1WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField2Id != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField2Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field2Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField2IsConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField2IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field2IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField2WasConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField2WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field2WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField3Id != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField3Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field3Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField3IsConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField3IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field3IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField3WasConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField3WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field3WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField4Id != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField4Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field4Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField4IsConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField4IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field4IsConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.tempField4WasConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempField4WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.field4WasConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.fAcl)
            this.AppendSql("1");
          else
            this.AppendSql("0");
          this.AppendSql(",");
          if (validObject.tempIf2FieldId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempIf2FieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          else
          {
            this.AppendSql(validObject.if2FieldId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            this.AppendSql(",");
          }
          if (validObject.if2Not)
            this.AppendSql("1,");
          else
            this.AppendSql("0,");
          if (validObject.tempIf2ConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(validObject.tempIf2ConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          else
            this.AppendSql(validObject.if2ConstId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          this.AppendSql(";if ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.Id));
          this.AppendSql(" is null begin rollback transaction; return; end;");
          this.AppendSql("insert into ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(" select ");
          this.AppendSql(DalSqlElement.InlineInt(validObject.Id - 20000));
          this.AppendSql(",");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.Id));
          if (overwrite)
          {
            this.AppendSql(Environment.NewLine);
            this.AppendSql(";insert into ");
            this.AppendSql("#ruleIds");
            this.AppendSql(" select ");
            this.AppendSql("@O");
            this.AppendSql(DalSqlElement.InlineInt(-validObject.Id));
          }
          this.AppendSql(Environment.NewLine);
          this.AppendSql("exec dbo.");
          this.AppendSql("ChangeRule");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@PersonId");
          this.AppendSql(",");
          this.AppendSql("@NowUtc");
          this.AppendSql(",");
          this.AppendSql("default,");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.Id));
          this.AppendSql(",");
          this.AppendNullable(validObject.grantRead);
          this.AppendSql(",");
          this.AppendNullable(validObject.denyRead);
          this.AppendSql(",");
          this.AppendNullable(validObject.grantWrite);
          this.AppendSql(",");
          this.AppendNullable(validObject.denyWrite);
          this.AppendSql(",");
          this.AppendNullable(validObject.suggestion);
          this.AppendSql(",");
          this.AppendNullable(validObject.grantAdmin);
          this.AppendSql(",");
          this.AppendNullable(validObject.denyAdmin);
          this.AppendSql(",");
          this.AppendNullable(validObject.defaultAttrib);
          this.AppendSql(",");
          this.AppendNullable(validObject.helpText);
          this.AppendSql(Environment.NewLine);
        }
      }
    }

    private void TraceFormsDetails(IList<InsertRuleAttributes> rules)
    {
      IVssRequestContext requestContext = this.SqlBatch.RequestContext;
      requestContext.Trace(908831, TraceLevel.Info, "DalElements", nameof (DalSaveRuleElement), "Saving rules with ids: {0}", (object) string.Join<int>(", ", rules.Select<InsertRuleAttributes, int>((Func<InsertRuleAttributes, int>) (rule => rule.Id))));
      List<InsertRuleAttributes> list1 = rules.Where<InsertRuleAttributes>((Func<InsertRuleAttributes, bool>) (r => r.thenFieldId == -14)).ToList<InsertRuleAttributes>();
      if (!list1.Any<InsertRuleAttributes>())
        return;
      requestContext.Trace(908831, TraceLevel.Info, "DalElements", nameof (DalSaveRuleElement), "Saving Form rules with Ids: {0}", (object) string.Join<InsertRuleAttributes>(", ", (IEnumerable<InsertRuleAttributes>) list1));
      List<InsertRuleAttributes> list2 = list1.Where<InsertRuleAttributes>((Func<InsertRuleAttributes, bool>) (r => !r.suggestion.GetValueOrDefault() && !r.defaultAttrib.GetValueOrDefault() && !r.helpText.GetValueOrDefault() && !r.denyAdmin.GetValueOrDefault() && !r.denyWrite.GetValueOrDefault() && !r.denyRead.GetValueOrDefault() && !r.grantRead.GetValueOrDefault())).ToList<InsertRuleAttributes>();
      if (!list2.Any<InsertRuleAttributes>())
        return;
      requestContext.Trace(908831, TraceLevel.Info, "DalElements", nameof (DalSaveRuleElement), "Going to delete Form rules with Ids: {0}", (object) string.Join<int>(", ", list2.Select<InsertRuleAttributes, int>((Func<InsertRuleAttributes, int>) (r => r.Id))));
    }

    private void AppendNullable(bool? value)
    {
      if (value.HasValue)
        this.AppendSql(DalSqlElement.InlineBit(value.Value));
      else
        this.AppendSql("default");
    }
  }
}
