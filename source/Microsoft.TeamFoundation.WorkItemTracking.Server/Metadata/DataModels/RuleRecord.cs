// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class RuleRecord
  {
    public int RuleID { get; set; }

    public int AreaID { get; set; }

    public int Fld1ID { get; set; }

    public int Fld1IsConstID { get; set; }

    public int Fld1WasConstID { get; set; }

    public int Fld2ID { get; set; }

    public string Fld2Is { get; set; }

    public int Fld2IsConstID { get; set; }

    public string Fld2Was { get; set; }

    public int Fld2WasConstID { get; set; }

    public int Fld3ID { get; set; }

    public string Fld3Is { get; set; }

    public int Fld3IsConstID { get; set; }

    public int Fld3WasConstID { get; set; }

    public int Fld4ID { get; set; }

    public int Fld4IsConstID { get; set; }

    public int Fld4WasConstID { get; set; }

    public string If { get; set; }

    public int IfConstID { get; set; }

    public int IfFldID { get; set; }

    public int If2ConstID { get; set; }

    public int If2FldID { get; set; }

    public int ObjectTypeScopeID { get; set; }

    public string Person { get; set; }

    public int PersonID { get; set; }

    public Guid PersonVsId { get; set; }

    public int RootTreeID { get; set; }

    public string Then { get; set; }

    public int ThenConstID { get; set; }

    public int ThenFldID { get; set; }

    public RuleFlags RuleFlags { get; set; }

    public RuleFlags2 RuleFlags2 { get; set; }

    public string Form { get; set; }

    public RuleRecord Clone() => new RuleRecord()
    {
      RuleID = this.RuleID,
      AreaID = this.AreaID,
      Fld1ID = this.Fld1ID,
      Fld1IsConstID = this.Fld1IsConstID,
      Fld1WasConstID = this.Fld1WasConstID,
      Fld2ID = this.Fld2ID,
      Fld2Is = this.Fld2Is,
      Fld2IsConstID = this.Fld2IsConstID,
      Fld2Was = this.Fld2Was,
      Fld2WasConstID = this.Fld2WasConstID,
      Fld3ID = this.Fld3ID,
      Fld3Is = this.Fld3Is,
      Fld3IsConstID = this.Fld3IsConstID,
      Fld3WasConstID = this.Fld3WasConstID,
      Fld4ID = this.Fld4ID,
      Fld4IsConstID = this.Fld4IsConstID,
      Fld4WasConstID = this.Fld4WasConstID,
      If = this.If,
      IfConstID = this.IfConstID,
      IfFldID = this.IfFldID,
      If2ConstID = this.If2ConstID,
      If2FldID = this.If2FldID,
      ObjectTypeScopeID = this.ObjectTypeScopeID,
      Person = this.Person,
      PersonID = this.PersonID,
      PersonVsId = this.PersonVsId,
      RootTreeID = this.RootTreeID,
      Then = this.Then,
      ThenConstID = this.ThenConstID,
      ThenFldID = this.ThenFldID,
      RuleFlags = this.RuleFlags,
      RuleFlags2 = this.RuleFlags2,
      Form = this.Form
    };

    public bool Check(RuleFlags requiredFlags, RuleFlags prohibitedFlags) => (this.RuleFlags & requiredFlags) == requiredFlags && (this.RuleFlags & prohibitedFlags) == RuleFlags.None;

    public string ToDebugString() => string.Format("RuleRecord {{ RuleID={0}", (object) this.RuleID) + (this.AreaID != 0 ? string.Format(", AreaID={0}", (object) this.AreaID) : string.Empty) + (this.Fld1ID != 0 ? string.Format(", Fld1ID={0}", (object) this.Fld1ID) : string.Empty) + (this.Fld1IsConstID != 0 ? string.Format(", Fld1IsConstID={0}", (object) this.Fld1IsConstID) : string.Empty) + (this.Fld1WasConstID != 0 ? string.Format(", Fld1WasConstID={0}", (object) this.Fld1WasConstID) : string.Empty) + (this.Fld2ID != 0 ? string.Format(", Fld2ID={0}", (object) this.Fld2ID) : string.Empty) + (this.Fld2Is != null ? ", Fld2Is=" + this.Fld2Is : string.Empty) + (this.Fld2IsConstID != 0 ? string.Format(", Fld2IsConstID={0}", (object) this.Fld2IsConstID) : string.Empty) + (this.Fld2Was != null ? ", Fld2Was=" + this.Fld2Was : string.Empty) + (this.Fld2WasConstID != 0 ? string.Format(", Fld2WasConstID={0}", (object) this.Fld2WasConstID) : string.Empty) + (this.Fld3ID != 0 ? string.Format(", Fld3ID={0}", (object) this.Fld3ID) : string.Empty) + (this.Fld3Is != null ? ", Fld3Is=" + this.Fld3Is : string.Empty) + (this.Fld3IsConstID != 0 ? string.Format(", Fld3IsConstID={0}", (object) this.Fld3IsConstID) : string.Empty) + (this.Fld3WasConstID != 0 ? string.Format(", Fld3WasConstID={0}", (object) this.Fld3WasConstID) : string.Empty) + (this.Fld4ID != 0 ? string.Format(", Fld4ID={0}", (object) this.Fld4ID) : string.Empty) + (this.Fld4IsConstID != 0 ? string.Format(", Fld4IsConstID={0}", (object) this.Fld4IsConstID) : string.Empty) + (this.Fld4WasConstID != 0 ? string.Format(", Fld4WasConstID={0}", (object) this.Fld4WasConstID) : string.Empty) + (this.If != null ? ", If=" + this.If : string.Empty) + (this.IfConstID != 0 ? string.Format(", IfConstID={0}", (object) this.IfConstID) : string.Empty) + (this.IfFldID != 0 ? string.Format(", IfFldID={0}", (object) this.IfFldID) : string.Empty) + (this.If2ConstID != 0 ? string.Format(", If2ConstID={0}", (object) this.If2ConstID) : string.Empty) + (this.If2FldID != 0 ? string.Format(", If2FldID={0}", (object) this.If2FldID) : string.Empty) + (this.ObjectTypeScopeID != 0 ? string.Format(", ObjectTypeScopeID={0}", (object) this.ObjectTypeScopeID) : string.Empty) + (this.Person != null ? ", Person=" + this.Person : string.Empty) + (this.PersonID != 0 ? string.Format(", PersonID={0}", (object) this.PersonID) : string.Empty) + (this.PersonVsId != Guid.Empty ? string.Format(", PersonVsId={0}", (object) this.PersonVsId) : string.Empty) + (this.RootTreeID != 0 ? string.Format(", RootTreeID={0}", (object) this.RootTreeID) : string.Empty) + (this.Then != null ? ", Then=" + this.Then : string.Empty) + (this.ThenConstID != 0 ? string.Format(", ThenConstID={0}", (object) this.ThenConstID) : string.Empty) + (this.ThenFldID != 0 ? string.Format(", ThenFldID={0}", (object) this.ThenFldID) : string.Empty) + (this.RuleFlags != RuleFlags.None ? string.Format(", RuleFlags=[{0}]", (object) this.RuleFlags) : string.Empty) + (this.RuleFlags2 != RuleFlags2.None ? string.Format(", RuleFlags2=[{0}]", (object) this.RuleFlags2) : string.Empty) + (this.Form != null ? ", Form=" + this.Form : string.Empty) + " }}";
  }
}
