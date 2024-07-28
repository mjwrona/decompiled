// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubRequest
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubRequest
  {
    public string Hub { get; set; }

    public string Method { get; set; }

    public IJsonValue[] ParameterValues { get; set; }

    public IDictionary<string, object> State { get; set; }

    public string Id { get; set; }
  }
}
