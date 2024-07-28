// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.HttpNotFoundException
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class HttpNotFoundException : IcmException
  {
    public HttpNotFoundException()
    {
    }

    public HttpNotFoundException(string message)
      : base(message)
    {
    }

    public HttpNotFoundException(string message, Exception exception)
      : base(message, exception)
    {
    }

    public HttpNotFoundException(
      string enumName,
      string className,
      string propertyName,
      string value)
      : base(TypeUtility.Format("Unable to convert {0}.{1} ({2}) to enum {3}", (object) className, (object) propertyName, (object) value, (object) enumName))
    {
    }

    public HttpNotFoundException(
      string enumName,
      string className,
      string propertyName,
      long value)
      : base(TypeUtility.Format("Unable to convert {0} value ({2:d}) to string for {0}.{1}", (object) enumName, (object) value, (object) className, (object) propertyName))
    {
    }

    protected HttpNotFoundException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
