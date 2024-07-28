// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.SshKeyInvalidException
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  [Serializable]
  public class SshKeyInvalidException : VssServiceException
  {
    public SshKeyInvalidException()
    {
    }

    public SshKeyInvalidException(string message)
      : base(message)
    {
    }

    public SshKeyInvalidException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected SshKeyInvalidException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.LoadKeyResult = info.GetInt32(nameof (LoadKeyResult));
    }

    public SshKeyInvalidException(int loadKeyResult)
      : this("Specified key is not a valid SSH key")
    {
      this.LoadKeyResult = loadKeyResult;
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("LoadKeyResult", this.LoadKeyResult);
    }

    public int LoadKeyResult { get; set; }
  }
}
