// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_AuthorizationId
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_AuthorizationId : RURequestProperty_Guid
  {
    private const string c_area = "ResourceUtilizationService";
    private const string c_layer = "AuthorizationId";

    public override bool ShouldOutputEntityToTelemetry { get; protected set; } = true;

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      object obj;
      if (!requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.AuthorizationId, out obj))
      {
        if (requestContext.GetAuthenticationMechanism() != "S2S_ServicePrincipal")
          requestContext.Trace(522304017, TraceLevel.Error, "ResourceUtilizationService", "AuthorizationId", string.Format("Pipeline request submitted without authorization Id; this will result in improper handling by Resource Utilization Service; descriptor {0}", (object) requestContext.RootContext.UserContext));
        return (object) Guid.Empty;
      }
      string input = obj.ToString();
      Guid result;
      if (Guid.TryParse(input, out result))
        return (object) result;
      requestContext.Trace(522304018, TraceLevel.Warning, "ResourceUtilizationService", "AuthorizationId", "Request authorization id is not a Guid; actual value: " + input);
      return (object) Guid.Empty;
    }

    public override object GetXEventValue(XEventObjectBase xeventObject) => (object) xeventObject.UniqueIdentifier;
  }
}
