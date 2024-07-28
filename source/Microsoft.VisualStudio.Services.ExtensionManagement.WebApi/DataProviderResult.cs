// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.DataProviderResult
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class DataProviderResult : DataProviderMetadata
  {
    public DataProviderResult(string dataspaceId = null)
      : base(dataspaceId)
    {
    }

    [DataMember]
    public Dictionary<string, object> Data { get; set; }

    [DataMember]
    public Dictionary<string, object> SharedData { get; set; }

    [DataMember]
    public List<ResolvedDataProvider> ResolvedProviders { get; set; }

    [DataMember]
    public Dictionary<string, DataProviderExceptionDetails> Exceptions { get; set; }

    [DataMember]
    public Dictionary<string, ClientDataProviderQuery> ClientProviders { get; set; }

    [DataMember]
    public string ScopeName { get; set; }

    [DataMember]
    public string ScopeValue { get; set; }
  }
}
