// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.BuildControllerServiceProxy2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Machine.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal sealed class BuildControllerServiceProxy2010 : ServiceProxy<IBuildControllerService>
  {
    public BuildControllerServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : base(requestContext, url, requireClientCertificates)
    {
    }

    public BuildControllerServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates,
      TimeSpan sendTimeout)
      : base(requestContext, url, requireClientCertificates, sendTimeout)
    {
    }

    public void DeleteBuildSymbols(string storePath, string transactionId) => this.Channel.DeleteBuildSymbols(storePath, transactionId);

    public IAsyncResult BeginDeleteBuildSymbols(
      string storePath,
      string transactionId,
      AsyncCallback callback,
      object state)
    {
      return this.Channel.BeginDeleteBuildSymbols(storePath, transactionId, callback, state);
    }

    public void EndDeleteBuildSymbols(IAsyncResult result) => this.Channel.EndDeleteBuildSymbols(result);

    public void DeleteBuildDrop(string dropLocation) => this.Channel.DeleteBuildDrop(dropLocation);

    public IAsyncResult BeginDeleteBuildDrop(
      string dropLocation,
      AsyncCallback callback,
      object state)
    {
      return this.Channel.BeginDeleteBuildDrop(dropLocation, callback, state);
    }

    public void EndDeleteBuildDrop(IAsyncResult result) => this.Channel.EndDeleteBuildDrop(result);

    public bool HasDeletePermission(string path, out string error) => this.Channel.HasDeletePermission(path, out error);

    public IAsyncResult BeginHasDeletePermission(string path, AsyncCallback callback, object state) => this.Channel.BeginHasDeletePermission(path, callback, state);

    public bool EndHasDeletePermission(out string error, IAsyncResult result) => this.Channel.EndHasDeletePermission(out error, result);

    public void StartBuild(string buildUri) => this.Do((Action<IBuildControllerService>) (channel => channel.StartBuild(buildUri)));

    public void StopBuild(string buildUri) => this.Do((Action<IBuildControllerService>) (channel => channel.StopBuild(buildUri)));

    public void TestConnection() => this.Do((Action<IBuildControllerService>) (channel => channel.TestConnection()));
  }
}
