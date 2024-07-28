// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExceptionMapping("0.0", "3.0", "CommerceException", "Microsoft.VisualStudio.Services.Commerce.CommerceException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class CommerceException : VssServiceException
  {
    public CommerceException()
    {
    }

    public CommerceException(string message)
      : base(message)
    {
    }

    public CommerceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected CommerceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
