// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.SshNoMoreAuthMethodsAvailableException
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  [Serializable]
  internal class SshNoMoreAuthMethodsAvailableException : Exception
  {
    public SshNoMoreAuthMethodsAvailableException(string message)
      : base(message)
    {
    }

    public SshNoMoreAuthMethodsAvailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
