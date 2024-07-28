// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SessionToken
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class SessionToken
  {
    [DataMember(EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Token { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ValidTo { get; set; }
  }
}
