// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.IBuildControllerService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.ServiceModel;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ServiceContract(Namespace = "http://www.tempuri.org/Microsoft/TeamFoundation/Build/07")]
  internal interface IBuildControllerService
  {
    [OperationContract]
    void DeleteBuildDrop(string dropLocation);

    [OperationContract]
    void DeleteBuildSymbols(string storePath, string transactionId);

    [OperationContract]
    bool HasDeletePermission(string path, out string error);

    [OperationContract]
    void StartBuild(string buildUri);

    [OperationContract]
    [FaultContract(typeof (int))]
    void StopBuild(string buildUri);

    [OperationContract]
    void TestConnection();

    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginDeleteBuildDrop(string dropLocation, AsyncCallback callback, object state);

    void EndDeleteBuildDrop(IAsyncResult result);

    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginDeleteBuildSymbols(
      string storePath,
      string transactionId,
      AsyncCallback callback,
      object state);

    void EndDeleteBuildSymbols(IAsyncResult result);

    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginHasDeletePermission(string path, AsyncCallback callback, object state);

    bool EndHasDeletePermission(out string error, IAsyncResult result);
  }
}
