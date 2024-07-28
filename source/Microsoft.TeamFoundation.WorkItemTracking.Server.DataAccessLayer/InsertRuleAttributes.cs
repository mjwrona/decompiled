// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.InsertRuleAttributes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal sealed class InsertRuleAttributes
  {
    public int Id;
    public byte[] cacheStamp;
    public int areaId;
    public int tempAreaId;
    public int rootTreeId;
    public int tempRootTreeId;
    public int personId;
    public int tempPersonId;
    public bool inversePersonId;
    public int objectTypeScopeId;
    public int tempObjectTypeScopeId;
    public bool unless;
    public int ifFieldId;
    public int tempIfFieldId;
    public bool ifNot;
    public int ifConstId;
    public int tempIfConstId;
    public int if2FieldId;
    public int tempIf2FieldId;
    public bool if2Not;
    public int if2ConstId;
    public int tempIf2ConstId;
    public int thenFieldId;
    public int tempThenFieldId;
    public bool thenNot;
    public bool thenLike;
    public bool thenLeaf;
    public bool thenInterior;
    public bool thenOneLevel;
    public bool thenTwoPlusLevels;
    public bool thenConstLargeText;
    public bool thenImplicitEmpty;
    public bool thenImplicitUnchanged;
    public int thenConstId;
    public int tempThenConstId;
    public int field1Id;
    public int tempField1Id;
    public int field1IsConstId;
    public int tempField1IsConstId;
    public int field1WasConstId;
    public int tempField1WasConstId;
    public int field2Id;
    public int tempField2Id;
    public int field2IsConstId;
    public int tempField2IsConstId;
    public int field2WasConstId;
    public int tempField2WasConstId;
    public int field3Id;
    public int tempField3Id;
    public int field3IsConstId;
    public int tempField3IsConstId;
    public int field3WasConstId;
    public int tempField3WasConstId;
    public int field4Id;
    public int tempField4Id;
    public int field4IsConstId;
    public int tempField4IsConstId;
    public int field4WasConstId;
    public int tempField4WasConstId;
    public bool flowDownTree;
    public bool flowAroundTree;
    public bool reverse;
    public bool fAcl;
    public bool? grantRead;
    public bool? denyRead;
    public bool? grantWrite;
    public bool? denyWrite;
    public bool? suggestion;
    public bool? grantAdmin;
    public bool? denyAdmin;
    public bool? defaultAttrib;
    public bool? helpText;
  }
}
