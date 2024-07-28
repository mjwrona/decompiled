// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsViewBinder : ObjectBinder<AnalyticsView>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("Id");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_visibility = new SqlColumnBinder("Visibility");
    private SqlColumnBinder m_viewType = new SqlColumnBinder("ViewType");
    private SqlColumnBinder m_definition = new SqlColumnBinder("Definition");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_lastModifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder m_lastModifiedDate = new SqlColumnBinder("LastModifiedDate");

    protected override AnalyticsView Bind()
    {
      Guid? nullableGuid1 = this.m_createdBy.GetNullableGuid((IDataReader) this.Reader);
      Guid guid;
      IdentityRef identityRef1;
      if (nullableGuid1.HasValue)
      {
        IdentityRef identityRef2 = new IdentityRef();
        guid = nullableGuid1.Value;
        identityRef2.Id = guid.ToString();
        identityRef1 = identityRef2;
      }
      else
        identityRef1 = (IdentityRef) null;
      IdentityRef identityRef3 = identityRef1;
      Guid? nullableGuid2 = this.m_lastModifiedBy.GetNullableGuid((IDataReader) this.Reader);
      IdentityRef identityRef4;
      if (nullableGuid2.HasValue)
      {
        IdentityRef identityRef5 = new IdentityRef();
        guid = nullableGuid2.Value;
        identityRef5.Id = guid.ToString();
        identityRef4 = identityRef5;
      }
      else
        identityRef4 = (IdentityRef) null;
      IdentityRef identityRef6 = identityRef4;
      return new AnalyticsView()
      {
        Id = this.m_id.GetGuid((IDataReader) this.Reader),
        Name = this.m_name.GetString((IDataReader) this.Reader, false),
        Description = this.m_description.GetString((IDataReader) this.Reader, true),
        Visibility = (AnalyticsViewVisibility) this.m_visibility.GetInt32((IDataReader) this.Reader),
        ViewType = (AnalyticsViewType) this.m_viewType.GetInt32((IDataReader) this.Reader),
        Definition = this.m_definition.GetString((IDataReader) this.Reader, true),
        CreatedBy = identityRef3,
        LastModifiedBy = identityRef6,
        CreatedDate = this.m_createdDate.GetDateTime((IDataReader) this.Reader),
        LastModifiedDate = this.m_lastModifiedDate.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
