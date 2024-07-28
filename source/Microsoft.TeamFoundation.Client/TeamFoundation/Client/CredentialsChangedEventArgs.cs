// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CredentialsChangedEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  public class CredentialsChangedEventArgs : EventArgs
  {
    private ICredentials m_credentials;

    public CredentialsChangedEventArgs(ICredentials credentials) => this.m_credentials = credentials;

    public ICredentials Credentials => this.m_credentials;
  }
}
