// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsPermissionComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsPermissionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsPermissionComponent>(1, true)
    }, "AnalyticsPermission");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public AnalyticsPermissionComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) AnalyticsPermissionComponent.s_sqlExceptionFactories;

    public virtual IEnumerable<AnalyticsPermissionAcl> GetViewAnalyticsPermissionAcl(
      int allowPermission,
      int denyPermission)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetPermissionAcl", false);
      this.BindGuid("@namespaceGuid", AnalyticsSecurityNamespace.Id);
      this.BindInt("@allowPermission", allowPermission);
      this.BindInt("@denyPermission", denyPermission);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AnalyticsPermissionAcl>((ObjectBinder<AnalyticsPermissionAcl>) new AnalyticsPermissionACLColumns());
        return (IEnumerable<AnalyticsPermissionAcl>) resultCollection.GetCurrent<AnalyticsPermissionAcl>().Items;
      }
    }
  }
}
