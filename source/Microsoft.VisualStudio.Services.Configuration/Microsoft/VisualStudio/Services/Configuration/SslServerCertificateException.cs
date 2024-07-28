// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SslServerCertificateException
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Serializable]
  public class SslServerCertificateException : Exception
  {
    public SslServerCertificateException()
    {
    }

    public SslServerCertificateException(string message)
      : base(message)
    {
    }

    public SslServerCertificateException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected SslServerCertificateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
