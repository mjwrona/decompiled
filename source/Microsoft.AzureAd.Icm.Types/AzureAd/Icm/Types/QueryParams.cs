// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.QueryParams
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class QueryParams
  {
    [DataMember(Name = "domainContext")]
    public CustomSearchDomainContext DomainContext { get; set; }

    [DataMember(Name = "execute")]
    public bool ExecuteQuery { get; set; }

    [DataMember(Name = "query")]
    public ClauseSet QueryItem { get; set; }
  }
}
