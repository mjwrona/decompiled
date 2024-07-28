// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogBlogStorageConnection
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogBlogStorageConnection : ITestLogStorageConnection
  {
    private ILogStoreConnectionEndpoint _logStoreConnectionEndpoint;
    private bool _supportsHttps;

    public TestLogBlogStorageConnection(
      IVssRequestContext requestContext,
      Dictionary<string, string> connectionSettings)
    {
      string str;
      if (connectionSettings == null || !connectionSettings.TryGetValue("TestLogStorageAccountConnectionString", out str))
        return;
      this._logStoreConnectionEndpoint = (ILogStoreConnectionEndpoint) new LogStoreConnectionEndpoint(requestContext, str);
      this._supportsHttps = !string.Equals(str, FrameworkServerConstants.DevStorageConnectionString, StringComparison.OrdinalIgnoreCase);
    }

    public ILogStoreConnectionEndpoint GetLogStoreConnectionEndpoint() => this._logStoreConnectionEndpoint;

    public bool SupportsHttps() => this._supportsHttps;
  }
}
