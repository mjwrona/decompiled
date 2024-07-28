// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalAzureContainerRegistryPushEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  [DataContract]
  [ClientInternalUseOnly(true)]
  public class ExternalAzureContainerRegistryPushEvent
  {
    [IgnoreDataMember]
    public static ApiResourceVersion CurrentVersion = new ApiResourceVersion(new Version(2, 0), 1);
    [DataMember]
    public AzureContainerRegistryPushData PushData;
    [DataMember]
    public AzureContainerRegistryRepository RepositoryData;
    [DataMember]
    public IDictionary<string, string> Properties;
  }
}
