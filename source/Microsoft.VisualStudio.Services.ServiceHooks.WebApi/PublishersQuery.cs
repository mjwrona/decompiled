// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.PublishersQuery
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class PublishersQuery
  {
    [DataMember(EmitDefaultValue = false)]
    public IList<string> PublisherIds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> PublisherInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Publisher> Results { get; set; }
  }
}
