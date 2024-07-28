// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolSecurity
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePoolSecurity
  {
    [DataMember(Name = "PublicKeyCertificates", IsRequired = false, EmitDefaultValue = false, Order = 2)]
    private List<CertificateInfo> m_serializedCertificates;
    [IgnoreDataMember]
    private Collection<X509Certificate2> m_publicKeyCertificates;

    internal MachinePoolSecurity()
    {
    }

    [DataMember(IsRequired = true, Order = 0)]
    public string Issuer { get; set; }

    [IgnoreDataMember]
    public Collection<X509Certificate2> PublicKeyCertificates
    {
      get
      {
        if (this.m_publicKeyCertificates == null)
          this.m_publicKeyCertificates = new Collection<X509Certificate2>();
        return this.m_publicKeyCertificates;
      }
    }

    [DataMember(IsRequired = true, Order = 2)]
    public string Realm { get; set; }

    [DataMember(IsRequired = true, Order = 3)]
    public string RemoteServicePrincipalId { get; set; }

    [DataMember(IsRequired = true, Order = 4)]
    public Guid ServiceIdentityId { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.PublicKeyCertificates.Count <= 0)
        return;
      this.m_serializedCertificates = this.PublicKeyCertificates.Select<X509Certificate2, CertificateInfo>((Func<X509Certificate2, CertificateInfo>) (x => new CertificateInfo()
      {
        Data = x.Export(X509ContentType.Cert),
        Thumbprint = x.Thumbprint
      })).ToList<CertificateInfo>();
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_serializedCertificates == null || this.m_serializedCertificates.Count == 0)
        return;
      foreach (CertificateInfo serializedCertificate in this.m_serializedCertificates)
        this.PublicKeyCertificates.Add(new X509Certificate2(serializedCertificate.Data));
    }
  }
}
