// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent10
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent10 : AnalyticsMetadataComponent9
  {
    public override List<Tag> GetTags(ICollection<Guid> tagIds)
    {
      if (tagIds.Count == 0)
        return new List<Tag>();
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetTags");
      this.BindGuidTable("@tagIds", (IEnumerable<Guid>) tagIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tag>((ObjectBinder<Tag>) new AnalyticsMetadataComponent10.TagColumns());
        return resultCollection.GetCurrent<Tag>().Items;
      }
    }

    internal class TagColumns : ObjectBinder<Tag>
    {
      private SqlColumnBinder m_projectSK = new SqlColumnBinder("ProjectSK");
      private SqlColumnBinder m_tagId = new SqlColumnBinder("TagId");
      private SqlColumnBinder m_tagName = new SqlColumnBinder("TagName");

      protected override Tag Bind() => new Tag()
      {
        ProjectSK = new Guid?(this.m_projectSK.GetGuid((IDataReader) this.Reader, false)),
        TagId = new Guid?(this.m_tagId.GetGuid((IDataReader) this.Reader, false)),
        TagName = this.m_tagName.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
