// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenRequestException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.OAuth
{
  [Serializable]
  public class VssOAuthTokenRequestException : VssOAuthException
  {
    public VssOAuthTokenRequestException(string message)
      : base(message)
    {
    }

    public VssOAuthTokenRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected VssOAuthTokenRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Error = info.GetString("m_error");
    }

    public string Error { get; set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("m_error", (object) this.Error);
    }
  }
}
