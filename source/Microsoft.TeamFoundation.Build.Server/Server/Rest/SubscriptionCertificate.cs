// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.SubscriptionCertificate
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
  internal class SubscriptionCertificate
  {
    [DataMember(EmitDefaultValue = false, Order = 0)]
    public string SubscriptionCertificatePublicKey { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 1)]
    public string SubscriptionCertificateThumbprint { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 2)]
    public string SubscriptionCertificateData { get; set; }
  }
}
