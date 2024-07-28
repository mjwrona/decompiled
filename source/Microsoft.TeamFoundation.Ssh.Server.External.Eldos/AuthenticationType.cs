// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.AuthenticationType
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  [Flags]
  public enum AuthenticationType
  {
    UNKNOWN = 0,
    SSH_AUTH_TYPE_RHOSTS = 1,
    SSH_AUTH_TYPE_PUBLICKEY = 2,
    SSH_AUTH_TYPE_PASSWORD = 4,
    SSH_AUTH_TYPE_HOSTBASED = 8,
    SSH_AUTH_TYPE_KEYBOARD = 16, // 0x00000010
    SSH_AUTH_TYPE_GSSAPI_WITH_MIC = 32, // 0x00000020
    SSH_AUTH_TYPE_GSSAPI_KEYEX = 64, // 0x00000040
    SSH_AUTH_TYPE_PUBLICKEYAGENT = 128, // 0x00000080
  }
}
