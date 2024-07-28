// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemCustomFieldValue
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemCustomFieldValue : IRevisedWorkItemEntity
  {
    public int Id { get; set; }

    public int FieldId { get; set; }

    public int FieldType { get; set; }

    public bool IsIdentityField { get; set; }

    public DateTime AuthorizedDate { get; set; }

    public DateTime RevisedDate { get; set; }

    public int? IntValue { get; set; }

    public double? FloatValue { get; set; }

    public DateTime? DateTimeValue { get; set; }

    public Guid? GuidValue { get; set; }

    public bool? BitValue { get; set; }

    public string StringValue { get; set; }

    public object Value
    {
      get
      {
        if (this.StringValue != null)
          return (object) this.StringValue;
        if (this.IntValue.HasValue)
          return (object) this.IntValue.Value;
        if (this.DateTimeValue.HasValue)
          return (object) this.DateTimeValue.Value;
        if (this.FloatValue.HasValue)
          return (object) this.FloatValue.Value;
        if (this.GuidValue.HasValue)
          return (object) this.GuidValue.Value;
        return this.BitValue.HasValue ? (object) this.BitValue.Value : (object) null;
      }
    }

    int IRevisedWorkItemEntity.Id => this.Id;

    DateTime IRevisedWorkItemEntity.AuthorizedDate
    {
      get => this.AuthorizedDate;
      set => this.AuthorizedDate = value;
    }

    DateTime IRevisedWorkItemEntity.RevisedDate => this.RevisedDate;

    bool IRevisedWorkItemEntity.SpansMultipleRevisions => this.FieldId != 54;

    internal class WorkItemLargeTextCustomFieldValue : WorkItemCustomFieldValue
    {
      public bool IsHtml { get; set; }
    }
  }
}
