// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_IPAddress
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_IPAddress : RURequestProperty_String
  {
    public override RUStage PropertyKnownAt { get; protected set; } = RUStage.BeginRequest;

    public override bool ShouldOutputEntityToTelemetry { get; protected set; } = true;

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      string str = requestContext.RemoteIPAddress();
      if (str == null)
        return (object) null;
      return (object) ((IEnumerable<string>) str.Split(',')).FirstOrDefault<string>();
    }

    public override object GetXEventValue(XEventObjectBase xeventObject) => (object) ContextInfo.DecodeIPAddress(xeventObject.UniqueIdentifier.ToByteArray());

    public override bool EvaluateSpecialProperty(object requestValue, string specialProperty)
    {
      string ipString = (string) requestValue;
      IPAddress address;
      return StringComparer.OrdinalIgnoreCase.Equals(specialProperty, "loopback") ? IPAddress.TryParse(ipString, out address) && IPAddress.IsLoopback(address) : StringComparer.OrdinalIgnoreCase.Equals(specialProperty, "parseable") && !string.IsNullOrEmpty(ipString) && IPAddress.TryParse(ipString, out IPAddress _);
    }
  }
}
