// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RuleTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class RuleTable : TempIdReferencingTable<InsertRuleAttributes>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[53]
    {
      new SqlMetaData("ID", SqlDbType.Int),
      new SqlMetaData("Cachestamp", SqlDbType.Binary, 8L),
      new SqlMetaData("RootTreeID", SqlDbType.Int),
      new SqlMetaData("TreeID", SqlDbType.Int),
      new SqlMetaData("ProjectID", SqlDbType.Int),
      new SqlMetaData("fFlowDownTree", SqlDbType.Bit),
      new SqlMetaData("fFlowAroundTree", SqlDbType.Bit),
      new SqlMetaData("PersonID", SqlDbType.Int),
      new SqlMetaData("fInversePersonID", SqlDbType.Bit),
      new SqlMetaData("Field1ID", SqlDbType.Int),
      new SqlMetaData("Fld1IsConstID", SqlDbType.Int),
      new SqlMetaData("Fld1WasConstID", SqlDbType.Int),
      new SqlMetaData("Fld2ID", SqlDbType.Int),
      new SqlMetaData("Fld2IsConstID", SqlDbType.Int),
      new SqlMetaData("Fld2WasConstID", SqlDbType.Int),
      new SqlMetaData("Fld3ID", SqlDbType.Int),
      new SqlMetaData("Fld3IsConstID", SqlDbType.Int),
      new SqlMetaData("Fld3WasConstID", SqlDbType.Int),
      new SqlMetaData("Fld4ID", SqlDbType.Int),
      new SqlMetaData("Fld4IsConstID", SqlDbType.Int),
      new SqlMetaData("Fld4WasConstID", SqlDbType.Int),
      new SqlMetaData("ObjectTypeScopeID", SqlDbType.Int),
      new SqlMetaData("fUnless", SqlDbType.Bit),
      new SqlMetaData("fReverse", SqlDbType.Bit),
      new SqlMetaData("IfFieldID", SqlDbType.Int),
      new SqlMetaData("fIfNot", SqlDbType.Bit),
      new SqlMetaData("IfConstID", SqlDbType.Int),
      new SqlMetaData("Fld1IsConstID", SqlDbType.Int),
      new SqlMetaData("fIf2Not", SqlDbType.Bit),
      new SqlMetaData("Fld1IsConstID", SqlDbType.Int),
      new SqlMetaData("ThenFieldID", SqlDbType.Int),
      new SqlMetaData("fThenNot", SqlDbType.Bit),
      new SqlMetaData("fThenImplicitEmpty", SqlDbType.Bit),
      new SqlMetaData("fThenImplicitUnchanged", SqlDbType.Bit),
      new SqlMetaData("fThenLike", SqlDbType.Bit),
      new SqlMetaData("fThenInterior", SqlDbType.Bit),
      new SqlMetaData("fThenLeaf", SqlDbType.Bit),
      new SqlMetaData("fThenOneLevel", SqlDbType.Bit),
      new SqlMetaData("fThenTwoPlus", SqlDbType.Bit),
      new SqlMetaData("ThenConstID", SqlDbType.Int),
      new SqlMetaData("fThenConstLargetext", SqlDbType.Bit),
      new SqlMetaData("fAcl", SqlDbType.Bit),
      new SqlMetaData("VersionFrom", SqlDbType.SmallInt),
      new SqlMetaData("VersionTo", SqlDbType.SmallInt),
      new SqlMetaData("fGrantRead", SqlDbType.Bit),
      new SqlMetaData("fDenyRead", SqlDbType.Bit),
      new SqlMetaData("fGrantWrite", SqlDbType.Bit),
      new SqlMetaData("fDenyWrite", SqlDbType.Bit),
      new SqlMetaData("fGrantAdmin", SqlDbType.Bit),
      new SqlMetaData("fDenyAdmin", SqlDbType.Bit),
      new SqlMetaData("fSuggestion", SqlDbType.Bit),
      new SqlMetaData("fDefault", SqlDbType.Bit),
      new SqlMetaData("fHelptext", SqlDbType.Bit)
    };

    public RuleTable(IEnumerable<InsertRuleAttributes> entries)
      : base(entries, "typ_WitRuleTable", RuleTable.s_metadata)
    {
    }

    public override void SetRecord(InsertRuleAttributes entry, SqlDataRecord record)
    {
      int num1 = 0;
      SqlDataRecord sqlDataRecord1 = record;
      int ordinal1 = num1;
      int num2 = ordinal1 + 1;
      int id = entry.Id;
      sqlDataRecord1.SetInt32(ordinal1, id);
      SqlDataRecord record1 = record;
      int ordinal2 = num2;
      int num3 = ordinal2 + 1;
      byte[] cacheStamp = entry.cacheStamp;
      this.SetNullableBinary(record1, ordinal2, cacheStamp);
      SqlDataRecord sqlDataRecord2 = record;
      int ordinal3 = num3;
      int num4 = ordinal3 + 1;
      int rootTreeId = entry.rootTreeId;
      sqlDataRecord2.SetInt32(ordinal3, rootTreeId);
      SqlDataRecord sqlDataRecord3 = record;
      int ordinal4 = num4;
      int num5 = ordinal4 + 1;
      int areaId = entry.areaId;
      sqlDataRecord3.SetInt32(ordinal4, areaId);
      SqlDataRecord sqlDataRecord4 = record;
      int ordinal5 = num5;
      int num6 = ordinal5 + 1;
      sqlDataRecord4.SetInt32(ordinal5, 0);
      SqlDataRecord sqlDataRecord5 = record;
      int ordinal6 = num6;
      int num7 = ordinal6 + 1;
      int num8 = entry.flowDownTree ? 1 : 0;
      sqlDataRecord5.SetBoolean(ordinal6, num8 != 0);
      SqlDataRecord sqlDataRecord6 = record;
      int ordinal7 = num7;
      int num9 = ordinal7 + 1;
      int num10 = entry.flowAroundTree ? 1 : 0;
      sqlDataRecord6.SetBoolean(ordinal7, num10 != 0);
      SqlDataRecord sqlDataRecord7 = record;
      int ordinal8 = num9;
      int num11 = ordinal8 + 1;
      int personId = entry.personId;
      sqlDataRecord7.SetInt32(ordinal8, personId);
      SqlDataRecord sqlDataRecord8 = record;
      int ordinal9 = num11;
      int num12 = ordinal9 + 1;
      int num13 = entry.inversePersonId ? 1 : 0;
      sqlDataRecord8.SetBoolean(ordinal9, num13 != 0);
      SqlDataRecord sqlDataRecord9 = record;
      int ordinal10 = num12;
      int num14 = ordinal10 + 1;
      int num15 = this.TempIdOrRealId(entry.tempField1Id, entry.field1Id);
      sqlDataRecord9.SetInt32(ordinal10, num15);
      SqlDataRecord sqlDataRecord10 = record;
      int ordinal11 = num14;
      int num16 = ordinal11 + 1;
      int num17 = this.TempIdOrRealId(entry.tempField1IsConstId, entry.field1IsConstId);
      sqlDataRecord10.SetInt32(ordinal11, num17);
      SqlDataRecord sqlDataRecord11 = record;
      int ordinal12 = num16;
      int num18 = ordinal12 + 1;
      int num19 = this.TempIdOrRealId(entry.tempField1WasConstId, entry.field1WasConstId);
      sqlDataRecord11.SetInt32(ordinal12, num19);
      SqlDataRecord sqlDataRecord12 = record;
      int ordinal13 = num18;
      int num20 = ordinal13 + 1;
      int num21 = this.TempIdOrRealId(entry.tempField2Id, entry.field2Id);
      sqlDataRecord12.SetInt32(ordinal13, num21);
      SqlDataRecord sqlDataRecord13 = record;
      int ordinal14 = num20;
      int num22 = ordinal14 + 1;
      int num23 = this.TempIdOrRealId(entry.tempField2IsConstId, entry.field2IsConstId);
      sqlDataRecord13.SetInt32(ordinal14, num23);
      SqlDataRecord sqlDataRecord14 = record;
      int ordinal15 = num22;
      int num24 = ordinal15 + 1;
      int num25 = this.TempIdOrRealId(entry.tempField2WasConstId, entry.field2WasConstId);
      sqlDataRecord14.SetInt32(ordinal15, num25);
      SqlDataRecord sqlDataRecord15 = record;
      int ordinal16 = num24;
      int num26 = ordinal16 + 1;
      int num27 = this.TempIdOrRealId(entry.tempField3Id, entry.field3Id);
      sqlDataRecord15.SetInt32(ordinal16, num27);
      SqlDataRecord sqlDataRecord16 = record;
      int ordinal17 = num26;
      int num28 = ordinal17 + 1;
      int num29 = this.TempIdOrRealId(entry.tempField3IsConstId, entry.field3IsConstId);
      sqlDataRecord16.SetInt32(ordinal17, num29);
      SqlDataRecord sqlDataRecord17 = record;
      int ordinal18 = num28;
      int num30 = ordinal18 + 1;
      int num31 = this.TempIdOrRealId(entry.tempField3WasConstId, entry.field3WasConstId);
      sqlDataRecord17.SetInt32(ordinal18, num31);
      SqlDataRecord sqlDataRecord18 = record;
      int ordinal19 = num30;
      int num32 = ordinal19 + 1;
      int num33 = this.TempIdOrRealId(entry.tempField4Id, entry.field4Id);
      sqlDataRecord18.SetInt32(ordinal19, num33);
      SqlDataRecord sqlDataRecord19 = record;
      int ordinal20 = num32;
      int num34 = ordinal20 + 1;
      int num35 = this.TempIdOrRealId(entry.tempField4IsConstId, entry.field4IsConstId);
      sqlDataRecord19.SetInt32(ordinal20, num35);
      SqlDataRecord sqlDataRecord20 = record;
      int ordinal21 = num34;
      int num36 = ordinal21 + 1;
      int num37 = this.TempIdOrRealId(entry.tempField4WasConstId, entry.field4WasConstId);
      sqlDataRecord20.SetInt32(ordinal21, num37);
      SqlDataRecord sqlDataRecord21 = record;
      int ordinal22 = num36;
      int num38 = ordinal22 + 1;
      int objectTypeScopeId = entry.objectTypeScopeId;
      sqlDataRecord21.SetInt32(ordinal22, objectTypeScopeId);
      SqlDataRecord sqlDataRecord22 = record;
      int ordinal23 = num38;
      int num39 = ordinal23 + 1;
      int num40 = entry.unless ? 1 : 0;
      sqlDataRecord22.SetBoolean(ordinal23, num40 != 0);
      SqlDataRecord sqlDataRecord23 = record;
      int ordinal24 = num39;
      int num41 = ordinal24 + 1;
      int num42 = entry.reverse ? 1 : 0;
      sqlDataRecord23.SetBoolean(ordinal24, num42 != 0);
      SqlDataRecord sqlDataRecord24 = record;
      int ordinal25 = num41;
      int num43 = ordinal25 + 1;
      int num44 = this.TempIdOrRealId(entry.tempIfFieldId, entry.ifFieldId);
      sqlDataRecord24.SetInt32(ordinal25, num44);
      SqlDataRecord sqlDataRecord25 = record;
      int ordinal26 = num43;
      int num45 = ordinal26 + 1;
      int num46 = entry.ifNot ? 1 : 0;
      sqlDataRecord25.SetBoolean(ordinal26, num46 != 0);
      SqlDataRecord sqlDataRecord26 = record;
      int ordinal27 = num45;
      int num47 = ordinal27 + 1;
      int num48 = this.TempIdOrRealId(entry.tempIfConstId, entry.ifConstId);
      sqlDataRecord26.SetInt32(ordinal27, num48);
      SqlDataRecord sqlDataRecord27 = record;
      int ordinal28 = num47;
      int num49 = ordinal28 + 1;
      int num50 = this.TempIdOrRealId(entry.tempIf2FieldId, entry.if2FieldId);
      sqlDataRecord27.SetInt32(ordinal28, num50);
      SqlDataRecord sqlDataRecord28 = record;
      int ordinal29 = num49;
      int num51 = ordinal29 + 1;
      int num52 = entry.if2Not ? 1 : 0;
      sqlDataRecord28.SetBoolean(ordinal29, num52 != 0);
      SqlDataRecord sqlDataRecord29 = record;
      int ordinal30 = num51;
      int num53 = ordinal30 + 1;
      int num54 = this.TempIdOrRealId(entry.tempIf2ConstId, entry.if2ConstId);
      sqlDataRecord29.SetInt32(ordinal30, num54);
      SqlDataRecord sqlDataRecord30 = record;
      int ordinal31 = num53;
      int num55 = ordinal31 + 1;
      int num56 = this.TempIdOrRealId(entry.tempThenFieldId, entry.thenFieldId);
      sqlDataRecord30.SetInt32(ordinal31, num56);
      SqlDataRecord sqlDataRecord31 = record;
      int ordinal32 = num55;
      int num57 = ordinal32 + 1;
      int num58 = entry.thenNot ? 1 : 0;
      sqlDataRecord31.SetBoolean(ordinal32, num58 != 0);
      SqlDataRecord sqlDataRecord32 = record;
      int ordinal33 = num57;
      int num59 = ordinal33 + 1;
      int num60 = entry.thenImplicitEmpty ? 1 : 0;
      sqlDataRecord32.SetBoolean(ordinal33, num60 != 0);
      SqlDataRecord sqlDataRecord33 = record;
      int ordinal34 = num59;
      int num61 = ordinal34 + 1;
      int num62 = entry.thenImplicitUnchanged ? 1 : 0;
      sqlDataRecord33.SetBoolean(ordinal34, num62 != 0);
      SqlDataRecord sqlDataRecord34 = record;
      int ordinal35 = num61;
      int num63 = ordinal35 + 1;
      int num64 = entry.thenLike ? 1 : 0;
      sqlDataRecord34.SetBoolean(ordinal35, num64 != 0);
      SqlDataRecord sqlDataRecord35 = record;
      int ordinal36 = num63;
      int num65 = ordinal36 + 1;
      int num66 = entry.thenInterior ? 1 : 0;
      sqlDataRecord35.SetBoolean(ordinal36, num66 != 0);
      SqlDataRecord sqlDataRecord36 = record;
      int ordinal37 = num65;
      int num67 = ordinal37 + 1;
      int num68 = entry.thenLeaf ? 1 : 0;
      sqlDataRecord36.SetBoolean(ordinal37, num68 != 0);
      SqlDataRecord sqlDataRecord37 = record;
      int ordinal38 = num67;
      int num69 = ordinal38 + 1;
      int num70 = entry.thenOneLevel ? 1 : 0;
      sqlDataRecord37.SetBoolean(ordinal38, num70 != 0);
      SqlDataRecord sqlDataRecord38 = record;
      int ordinal39 = num69;
      int num71 = ordinal39 + 1;
      int num72 = entry.thenTwoPlusLevels ? 1 : 0;
      sqlDataRecord38.SetBoolean(ordinal39, num72 != 0);
      SqlDataRecord sqlDataRecord39 = record;
      int ordinal40 = num71;
      int num73 = ordinal40 + 1;
      int num74 = this.TempIdOrRealId(entry.tempThenConstId, entry.thenConstId);
      sqlDataRecord39.SetInt32(ordinal40, num74);
      SqlDataRecord sqlDataRecord40 = record;
      int ordinal41 = num73;
      int num75 = ordinal41 + 1;
      int num76 = entry.thenConstLargeText ? 1 : 0;
      sqlDataRecord40.SetBoolean(ordinal41, num76 != 0);
      SqlDataRecord sqlDataRecord41 = record;
      int ordinal42 = num75;
      int num77 = ordinal42 + 1;
      int num78 = entry.fAcl ? 1 : 0;
      sqlDataRecord41.SetBoolean(ordinal42, num78 != 0);
      SqlDataRecord sqlDataRecord42 = record;
      int ordinal43 = num77;
      int num79 = ordinal43 + 1;
      sqlDataRecord42.SetInt16(ordinal43, (short) 0);
      SqlDataRecord sqlDataRecord43 = record;
      int ordinal44 = num79;
      int num80 = ordinal44 + 1;
      sqlDataRecord43.SetInt16(ordinal44, (short) byte.MaxValue);
      SqlDataRecord record2 = record;
      int ordinal45 = num80;
      int num81 = ordinal45 + 1;
      bool? grantRead = entry.grantRead;
      this.SetNullable(record2, ordinal45, grantRead);
      SqlDataRecord record3 = record;
      int ordinal46 = num81;
      int num82 = ordinal46 + 1;
      bool? denyRead = entry.denyRead;
      this.SetNullable(record3, ordinal46, denyRead);
      SqlDataRecord record4 = record;
      int ordinal47 = num82;
      int num83 = ordinal47 + 1;
      bool? grantWrite = entry.grantWrite;
      this.SetNullable(record4, ordinal47, grantWrite);
      SqlDataRecord record5 = record;
      int ordinal48 = num83;
      int num84 = ordinal48 + 1;
      bool? denyWrite = entry.denyWrite;
      this.SetNullable(record5, ordinal48, denyWrite);
      SqlDataRecord record6 = record;
      int ordinal49 = num84;
      int num85 = ordinal49 + 1;
      bool? grantAdmin = entry.grantAdmin;
      this.SetNullable(record6, ordinal49, grantAdmin);
      SqlDataRecord record7 = record;
      int ordinal50 = num85;
      int num86 = ordinal50 + 1;
      bool? denyAdmin = entry.denyAdmin;
      this.SetNullable(record7, ordinal50, denyAdmin);
      SqlDataRecord record8 = record;
      int ordinal51 = num86;
      int num87 = ordinal51 + 1;
      bool? suggestion = entry.suggestion;
      this.SetNullable(record8, ordinal51, suggestion);
      SqlDataRecord record9 = record;
      int ordinal52 = num87;
      int num88 = ordinal52 + 1;
      bool? defaultAttrib = entry.defaultAttrib;
      this.SetNullable(record9, ordinal52, defaultAttrib);
      SqlDataRecord record10 = record;
      int ordinal53 = num88;
      int num89 = ordinal53 + 1;
      bool? helpText = entry.helpText;
      this.SetNullable(record10, ordinal53, helpText);
    }

    private void SetNullable(SqlDataRecord record, int ordinal, bool? value)
    {
      if (value.HasValue)
        record.SetBoolean(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }
  }
}
