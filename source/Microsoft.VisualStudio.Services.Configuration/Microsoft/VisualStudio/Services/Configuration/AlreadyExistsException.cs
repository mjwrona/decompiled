// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.AlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Serializable]
  public class AlreadyExistsException : ConfigurationException
  {
    public AlreadyExistsException()
    {
    }

    public AlreadyExistsException(string message)
      : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AlreadyExistsException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
