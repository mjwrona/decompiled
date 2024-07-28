// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.Client.Error
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MSGraph.Client
{
  [DataContract]
  public class Error
  {
    [DataMember(EmitDefaultValue = false)]
    public string Code { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public string Message { get; private set; }

    [DataMember(EmitDefaultValue = false, Name = "request-id")]
    public string RequestId { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime Date { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public Error InnerError { get; private set; }
  }
}
