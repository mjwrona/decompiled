// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssUnauthorizedException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  [ExceptionMapping("0.0", "3.0", "VssUnauthorizedException", "Microsoft.VisualStudio.Services.Common.VssUnauthorizedException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class VssUnauthorizedException : VssException
  {
    public VssUnauthorizedException()
      : this(CommonResources.VssUnauthorizedUnknownServer())
    {
    }

    public VssUnauthorizedException(string message)
      : base(message)
    {
    }

    public VssUnauthorizedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected VssUnauthorizedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
