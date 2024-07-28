// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TooManyRequestsException
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class TooManyRequestsException : IcmException
  {
    public TooManyRequestsException()
    {
    }

    public TooManyRequestsException(string message)
      : base(message, true)
    {
    }

    public TooManyRequestsException(string message, Exception exception)
      : base(message, exception)
    {
      this.IsClientError = true;
    }

    protected TooManyRequestsException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this.IsClientError = true;
    }
  }
}
