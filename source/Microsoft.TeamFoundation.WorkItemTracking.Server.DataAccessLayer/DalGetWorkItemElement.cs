// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetWorkItemElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetWorkItemElement : DalGetWorkItemElementBase
  {
    public static PayloadTableSchema c_zeroTableSchema => new PayloadTableSchema(new PayloadTableSchema.Column[1]
    {
      new PayloadTableSchema.Column("zero", typeof (int))
    });

    public static PayloadTableSchema c_longTextTableSchema => new PayloadTableSchema(new PayloadTableSchema.Column[5]
    {
      new PayloadTableSchema.Column("FldID", typeof (int)),
      new PayloadTableSchema.Column("AddedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("Words", typeof (string)),
      new PayloadTableSchema.Column("ExtID", typeof (int)),
      new PayloadTableSchema.Column("fHtml", typeof (bool))
    });

    public static PayloadTableSchema c_attachmentsTableSchema => new PayloadTableSchema(new PayloadTableSchema.Column[12]
    {
      new PayloadTableSchema.Column("FldID", typeof (int)),
      new PayloadTableSchema.Column("RemovedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("AddedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("FilePath", typeof (string)),
      new PayloadTableSchema.Column("OriginalName", typeof (string)),
      new PayloadTableSchema.Column("ExtID", typeof (int)),
      new PayloadTableSchema.Column("Comment", typeof (string)),
      new PayloadTableSchema.Column("CreationDate", typeof (DateTime)),
      new PayloadTableSchema.Column("LastWriteDate", typeof (DateTime)),
      new PayloadTableSchema.Column("Length", typeof (int)),
      new PayloadTableSchema.Column("AuthorizedAddedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("AuthorizedRemovedDate", typeof (DateTime))
    });

    public static PayloadTableSchema c_workItemLinksAreTableSchema => new PayloadTableSchema(new PayloadTableSchema.Column[10]
    {
      new PayloadTableSchema.Column("ID", typeof (int)),
      new PayloadTableSchema.Column("Comment", typeof (string)),
      new PayloadTableSchema.Column("Changed By", typeof (int)),
      new PayloadTableSchema.Column("Changed Date", typeof (DateTime)),
      new PayloadTableSchema.Column("Revised By", typeof (int)),
      new PayloadTableSchema.Column("Revised Date", typeof (DateTime)),
      new PayloadTableSchema.Column("LinkType", typeof (short)),
      new PayloadTableSchema.Column("Lock", typeof (bool)),
      new PayloadTableSchema.Column("AuthorizedAddedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("AuthorizedRemovedDate", typeof (DateTime))
    });

    public static PayloadTableSchema c_workItemLinksWereTableSchema => new PayloadTableSchema(new PayloadTableSchema.Column[10]
    {
      new PayloadTableSchema.Column("ID", typeof (int)),
      new PayloadTableSchema.Column("Comment", typeof (string)),
      new PayloadTableSchema.Column("Changed By", typeof (int)),
      new PayloadTableSchema.Column("Changed Date", typeof (DateTime)),
      new PayloadTableSchema.Column("Revised By", typeof (int)),
      new PayloadTableSchema.Column("Revised Date", typeof (DateTime)),
      new PayloadTableSchema.Column("LinkType", typeof (short)),
      new PayloadTableSchema.Column("Lock", typeof (bool)),
      new PayloadTableSchema.Column("AuthorizedAddedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("AuthorizedRemovedDate", typeof (DateTime))
    });

    public static PayloadTableSchema c_workItemLongSchema => new PayloadTableSchema(new PayloadTableSchema.Column[8]
    {
      new PayloadTableSchema.Column("FieldId", typeof (int)),
      new PayloadTableSchema.Column("AuthorizedDate", typeof (DateTime)),
      new PayloadTableSchema.Column("IntValue", typeof (int)),
      new PayloadTableSchema.Column("FloatValue", typeof (double)),
      new PayloadTableSchema.Column("DateTimeValue", typeof (DateTime)),
      new PayloadTableSchema.Column("GuidValue", typeof (Guid)),
      new PayloadTableSchema.Column("BitValue", typeof (bool)),
      new PayloadTableSchema.Column("StringValue", typeof (string))
    });
  }
}
