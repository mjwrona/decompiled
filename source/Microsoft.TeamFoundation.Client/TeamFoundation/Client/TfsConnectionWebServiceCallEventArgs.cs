// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConnectionWebServiceCallEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  public class TfsConnectionWebServiceCallEventArgs : EventArgs
  {
    private TfsConnection m_tfsConnection;
    private long m_requestId;
    private string m_methodName;
    private string m_componentName;
    private long m_length;

    public TfsConnectionWebServiceCallEventArgs(
      TfsConnection originatingTfsConnection,
      long requestId,
      string methodName,
      string componentName,
      long length)
    {
      this.m_tfsConnection = originatingTfsConnection;
      this.m_requestId = requestId;
      this.m_methodName = methodName;
      this.m_componentName = componentName;
      this.m_length = length;
    }

    public TfsConnection TfsConnection => this.m_tfsConnection;

    public long RequestId => this.m_requestId;

    public string MethodName => this.m_methodName ?? string.Empty;

    public string ComponentName => this.m_componentName ?? string.Empty;

    public long Length => this.m_length;
  }
}
