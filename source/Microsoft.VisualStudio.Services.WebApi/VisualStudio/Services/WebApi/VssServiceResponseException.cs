// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssServiceResponseException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [ExceptionMapping("0.0", "3.0", "VssServiceResponseException", "Microsoft.VisualStudio.Services.WebApi.VssServiceResponseException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class VssServiceResponseException : VssServiceException
  {
    public VssServiceResponseException(
      HttpStatusCode code,
      string message,
      Exception innerException)
      : base(message, innerException)
    {
      this.HttpStatusCode = code;
    }

    protected VssServiceResponseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.HttpStatusCode = (HttpStatusCode) info.GetInt32(nameof (HttpStatusCode));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("HttpStatusCode", (int) this.HttpStatusCode);
    }

    public HttpStatusCode HttpStatusCode { get; private set; }
  }
}
