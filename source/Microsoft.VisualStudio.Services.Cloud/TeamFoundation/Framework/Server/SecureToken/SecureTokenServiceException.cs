// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.SecureTokenServiceException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.SecureToken
{
  public class SecureTokenServiceException : VssServiceException
  {
    public SecureTokenServiceException(string msg, Exception cause)
      : base(msg, cause)
    {
    }

    public SecureTokenServiceException(string msg)
      : base(msg)
    {
    }
  }
}
