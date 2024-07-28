// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.PlatformNewDomainUrlMigrationRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [XmlInclude(typeof (OnboardNewDomainRequest))]
  [XmlInclude(typeof (RevertToOldDomainRequest))]
  [DataContract]
  public abstract class PlatformNewDomainUrlMigrationRequest : ServicingOrchestrationRequest
  {
    [DataMember]
    public bool WithOrganization { get; set; }
  }
}
