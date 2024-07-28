// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ResolvedDataProvider
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ResolvedDataProvider : DataProviderMetadata
  {
    public Exception Exception;

    public ResolvedDataProvider(string dataspaceId)
      : base(dataspaceId)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public float Duration { get; set; }
  }
}
