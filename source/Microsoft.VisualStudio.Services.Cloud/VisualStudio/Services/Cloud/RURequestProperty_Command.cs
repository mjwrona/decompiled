// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_Command
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_Command : RURequestProperty_String
  {
    public override RUStage PropertyKnownAt { get; protected set; } = RUStage.EnterMethod;

    [Obsolete("PropertyKnownAtRequestReady is deprecated, please use PropertyKnowAt instead.")]
    public override bool PropertyKnownAtRequestReady { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      MethodInformation method = requestContext.Method;
      return method == null ? (object) null : (object) method.Name;
    }

    public override object GetXEventValue(XEventObjectBase xeventObject) => (object) string.Empty;
  }
}
