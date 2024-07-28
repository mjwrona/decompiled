// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_IsPublicUser
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_IsPublicUser : RURequestProperty_Bool
  {
    public override bool ShouldOutputEntityToTelemetry { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext) => (object) requestContext.IsRootContextPublicUser();

    public override object GetXEventValue(XEventObjectBase xeventObject) => (object) ((xeventObject.Type & ContextType.Public) != 0);
  }
}
