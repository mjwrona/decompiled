// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.InvalidAttributeValueException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [Serializable]
  public class InvalidAttributeValueException : UserException
  {
    public InvalidAttributeValueException()
    {
    }

    public InvalidAttributeValueException(string message)
      : base(message)
    {
    }

    public InvalidAttributeValueException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidAttributeValueException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
