// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public abstract class TestManagementAreaController : TfsAreaController
  {
    private TestManagerRequestContext m_testManagerContext;

    public override string AreaName => "TestManagement";

    public override string TraceArea => "WebAccess.TestManagement";

    public TestManagerRequestContext TestContext
    {
      get
      {
        if (this.m_testManagerContext == null)
          this.m_testManagerContext = new TestManagerRequestContext((TfsController) this);
        return this.m_testManagerContext;
      }
    }

    protected QueryModel DeserializeQueryJson(IVssRequestContext requestContext, string queryJson)
    {
      QueryModel var = !requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<QueryModel>(queryJson) : JsonConvert.DeserializeObject<QueryModel>(queryJson);
      ArgumentUtility.CheckForNull<QueryModel>(var, "deserializedQuery", "Test Results");
      ArgumentUtility.CheckForNull<QueryColumnInfoModel>(var.Columns, "deserializedQuery.Columns", "Test Results");
      ArgumentUtility.CheckForNull<FilterModel>(var.Filter, "deserializedQuery.Filter", "Test Results");
      return var;
    }
  }
}
