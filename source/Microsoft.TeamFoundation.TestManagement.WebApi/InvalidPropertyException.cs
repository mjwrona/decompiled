// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.InvalidPropertyException
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [ExceptionMapping("0.0", "3.0", "InvalidPropertyException", "Microsoft.TeamFoundation.TestManagement.WebApi.InvalidPropertyException, Microsoft.TeamFoundation.TestManagement.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class InvalidPropertyException : VssServiceException
  {
    public InvalidPropertyException(string property, string message)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, message, (object) property))
    {
    }

    public InvalidPropertyException(string message)
      : base(message)
    {
    }

    public InvalidPropertyException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public InvalidPropertyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
