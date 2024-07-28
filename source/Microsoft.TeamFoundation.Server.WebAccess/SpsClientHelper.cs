// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.SpsClientHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class SpsClientHelper
  {
    public virtual OrganizationHttpClient CreateSpsClient(
      IVssRequestContext deploymentContext,
      Guid collectionId)
    {
      return HttpClientHelper.CreateSpsClient<OrganizationHttpClient>(deploymentContext, collectionId, InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.SPS));
    }
  }
}
