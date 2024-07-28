// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewAndQueryBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsViewAndQueryBinder : AnalyticsViewBinder
  {
    private SqlColumnBinder m_queryId = new SqlColumnBinder("QueryId");
    private SqlColumnBinder m_entitySet = new SqlColumnBinder("EntitySet");
    private SqlColumnBinder m_template = new SqlColumnBinder("Template");

    protected override AnalyticsView Bind()
    {
      AnalyticsView analyticsView = base.Bind();
      analyticsView.Query = new AnalyticsViewQuery()
      {
        Id = this.m_queryId.GetGuid((IDataReader) this.Reader),
        EntitySet = this.m_entitySet.GetString((IDataReader) this.Reader, false),
        ODataTemplate = this.m_template.GetString((IDataReader) this.Reader, false)
      };
      return analyticsView;
    }
  }
}
