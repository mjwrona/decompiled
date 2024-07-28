// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.InvalidArgumentValueException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class InvalidArgumentValueException : ArgumentException
  {
    public InvalidArgumentValueException()
      : base(WACommonResources.InvalidArgumentValue)
    {
    }

    public InvalidArgumentValueException(string paramName)
      : base(WACommonResources.InvalidArgumentValue, paramName)
    {
    }

    public InvalidArgumentValueException(string paramName, string message)
      : base(message, paramName)
    {
    }

    public InvalidArgumentValueException(string paramName, Exception innerException)
      : base(WACommonResources.InvalidArgumentValue, paramName, innerException)
    {
    }

    public InvalidArgumentValueException(
      string message,
      string paramName,
      Exception innerException)
      : base(message, paramName, innerException)
    {
    }

    protected InvalidArgumentValueException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
