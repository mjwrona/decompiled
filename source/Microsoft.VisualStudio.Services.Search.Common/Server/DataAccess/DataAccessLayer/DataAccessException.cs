// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessException
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  [Serializable]
  public class DataAccessException : Exception
  {
    public DataAccessErrorCodeEnum ErrorCode { get; set; }

    public DataAccessException()
    {
    }

    public DataAccessException(string message)
      : base(message)
    {
    }

    public DataAccessException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public DataAccessException(
      DataAccessErrorCodeEnum code,
      Exception innerException = null,
      string message = null)
      : base(message, innerException)
    {
      this.ErrorCode = code;
    }

    protected DataAccessException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ErrorCode = (DataAccessErrorCodeEnum) info.GetValue(nameof (ErrorCode), typeof (DataAccessErrorCodeEnum));
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error code -----> {0}", (object) this.ErrorCode));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) ExceptionHelper.ExceptionToString((Exception) this)));
      return stringBuilder.ToString();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ErrorCode", (object) this.ErrorCode);
    }
  }
}
