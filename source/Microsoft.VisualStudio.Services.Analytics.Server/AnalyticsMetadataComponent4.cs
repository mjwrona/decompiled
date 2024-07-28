// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent4
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent4 : AnalyticsMetadataComponent3
  {
    public override CollectionDateTime GetCollectionTimeZoneWithZoneId()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetCollectionTimeZone");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CollectionDateTime>((ObjectBinder<CollectionDateTime>) new AnalyticsMetadataComponent4.CollectionTimeColumns());
        return resultCollection.GetCurrent<CollectionDateTime>().Items.FirstOrDefault<CollectionDateTime>();
      }
    }

    public override List<WorkItemTypeField> GetWorkItemTypeFields(
      IList<Guid> projectIds,
      IList<string> workItemTypes)
    {
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetWorkItemTypeFields");
      this.BindGuidTable("@projectIds", (IEnumerable<Guid>) projectIds);
      this.BindStringTable("@workItemTypeIds", (IEnumerable<string>) workItemTypes);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WorkItemTypeField>((ObjectBinder<WorkItemTypeField>) new AnalyticsMetadataComponent4.WorkItemTypeFieldColumns());
        return resultCollection.GetCurrent<WorkItemTypeField>().Items;
      }
    }

    internal class CollectionTimeColumns : ObjectBinder<CollectionDateTime>
    {
      private SqlColumnBinder m_collectionTimeZone = new SqlColumnBinder("CollectionTimeZone");

      private TimeZoneInfo MapTimeZone(string timeZoneName) => TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

      protected override CollectionDateTime Bind() => new CollectionDateTime()
      {
        CollectionTimeZone = this.MapTimeZone(this.m_collectionTimeZone.GetString((IDataReader) this.Reader, false))
      };
    }

    internal class WorkItemTypeFieldColumns : ObjectBinder<WorkItemTypeField>
    {
      private SqlColumnBinder m_projectSK = new SqlColumnBinder("ProjectSK");
      private SqlColumnBinder m_fieldName = new SqlColumnBinder("FieldName");
      private SqlColumnBinder m_fieldReferenceName = new SqlColumnBinder("FieldReferenceName");
      private SqlColumnBinder m_fieldType = new SqlColumnBinder("FieldType");
      private SqlColumnBinder m_workItemTypeCategory = new SqlColumnBinder("WorkItemTypeCategory");
      private SqlColumnBinder m_workItemType = new SqlColumnBinder("WorkItemType");

      protected override WorkItemTypeField Bind() => new WorkItemTypeField()
      {
        ProjectSK = new Guid?(this.m_projectSK.GetGuid((IDataReader) this.Reader, false)),
        FieldName = this.m_fieldName.GetString((IDataReader) this.Reader, false),
        FieldReferenceName = this.m_fieldReferenceName.GetString((IDataReader) this.Reader, false),
        FieldType = this.m_fieldType.GetString((IDataReader) this.Reader, false),
        WorkItemTypeCategory = this.m_workItemTypeCategory.GetString((IDataReader) this.Reader, false),
        WorkItemType = this.m_workItemType.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
