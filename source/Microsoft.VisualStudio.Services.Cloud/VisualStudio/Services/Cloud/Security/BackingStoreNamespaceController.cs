// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Security.BackingStoreNamespaceController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud.Security
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "SBS", ResourceName = "SBSNamespace", ResourceVersion = 3)]
  public class BackingStoreNamespaceController : BackingStoreNamespaceBaseController
  {
    private const string c_area = "BackingStoreNamespace";
    private const string c_layer = "BackingStoreNamespaceController";

    [HttpGet]
    public SecurityNamespaceDataCollection QuerySecurityDataNamespace(
      Guid securityNamespaceId,
      bool useVsidSubjects = true)
    {
      if (useVsidSubjects)
        this.TfsRequestContext.Trace(60001, TraceLevel.Info, "BackingStoreNamespace", nameof (BackingStoreNamespaceController), "Use VSID subjects");
      else
        this.TfsRequestContext.Trace(60002, TraceLevel.Info, "BackingStoreNamespace", nameof (BackingStoreNamespaceController), "Do not use VSID subjects");
      Guid securityNamespaceId1 = securityNamespaceId;
      bool flag = useVsidSubjects;
      Guid aclStoreId = new Guid();
      int num = flag ? 1 : 0;
      return this.QuerySecurityData(securityNamespaceId1, aclStoreId, useVsidSubjects: num != 0);
    }
  }
}
