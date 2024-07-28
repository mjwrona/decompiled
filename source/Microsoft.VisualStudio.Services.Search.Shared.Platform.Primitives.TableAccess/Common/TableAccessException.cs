// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common.TableAccessException
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common
{
  [Serializable]
  public class TableAccessException : Exception, ISerializable
  {
    public TableAccessException(
      TableAcessErrorCodeEnum code,
      Exception innerException = null,
      string message = null)
      : base(message, innerException)
    {
      this.ErrorCode = code;
    }

    public TableAccessException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ErrorCode = (TableAcessErrorCodeEnum) info.GetValue(nameof (ErrorCode), typeof (TableAcessErrorCodeEnum));
    }

    public TableAcessErrorCodeEnum ErrorCode { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error code -----> {0}", (object) this.ErrorCode));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) ExceptionHelper.ExceptionToString((Exception) this)));
      return stringBuilder.ToString();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
      base.GetObjectData(info, ctxt);
      info.AddValue("ErrorCode", (object) this.ErrorCode);
    }
  }
}
