// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.IBuildAgentNotifyService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.ServiceModel;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ServiceContract(Namespace = "http://www.tempuri.org/Microsoft/TeamFoundation/Build/07")]
  internal interface IBuildAgentNotifyService
  {
    [OperationContract(AsyncPattern = true, IsOneWay = true)]
    IAsyncResult BeginNotifyAgentAvailable(
      string buildUri,
      int reservationId,
      string agentUri,
      AsyncCallback callback,
      object state);

    void EndNotifyAgentAvailable(IAsyncResult result);
  }
}
