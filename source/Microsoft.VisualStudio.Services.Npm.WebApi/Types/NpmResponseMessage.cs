// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.NpmResponseMessage
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class NpmResponseMessage
  {
    [DataMember(Name = "success", EmitDefaultValue = false)]
    public string Success { get; set; }

    [DataMember(Name = "error", EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(Name = "reason", EmitDefaultValue = false)]
    public string Reason { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Dictionary<string, object> CustomProperties { get; set; }

    [DataMember]
    public WrappedException InnerException { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string TypeName { get; set; }

    [DataMember]
    public string TypeKey { get; set; }

    [DataMember]
    public int ErrorCode { get; set; }

    [DataMember]
    public int EventId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string StackTrace { get; set; }

    public static NpmResponseMessage FromWrappedException(
      WrappedException wrappedException,
      HttpStatusCode statusCode)
    {
      return new NpmResponseMessage()
      {
        Success = "false",
        Error = wrappedException.Message,
        Reason = wrappedException.Message,
        CustomProperties = wrappedException.CustomProperties,
        InnerException = wrappedException.InnerException,
        Message = wrappedException.Message,
        TypeName = wrappedException.TypeName,
        TypeKey = wrappedException.TypeKey,
        ErrorCode = wrappedException.ErrorCode,
        EventId = wrappedException.EventId,
        StackTrace = wrappedException.StackTrace
      };
    }
  }
}
