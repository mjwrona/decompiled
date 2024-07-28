// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.BuildServiceClientSecurityTokenManager
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.IdentityModel.Selectors;
using System.ServiceModel.Security;

namespace Microsoft.TeamFoundation.Build.Machine
{
  internal class BuildServiceClientSecurityTokenManager : SecurityTokenManager
  {
    private BuildServiceClientCredentials ClientCredentials;

    public BuildServiceClientSecurityTokenManager(BuildServiceClientCredentials credentials) => this.ClientCredentials = credentials;

    public override SecurityTokenProvider CreateSecurityTokenProvider(
      SecurityTokenRequirement tokenRequirement)
    {
      return this.ClientCredentials.CurrentCertificate == null ? (SecurityTokenProvider) null : (SecurityTokenProvider) new X509SecurityTokenProvider(this.ClientCredentials.CurrentCertificate);
    }

    public override SecurityTokenSerializer CreateSecurityTokenSerializer(
      SecurityTokenVersion version)
    {
      return (SecurityTokenSerializer) new WSSecurityTokenSerializer();
    }

    public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(
      SecurityTokenRequirement tokenRequirement,
      out SecurityTokenResolver outOfBandTokenResolver)
    {
      outOfBandTokenResolver = (SecurityTokenResolver) null;
      return (SecurityTokenAuthenticator) new X509SecurityTokenAuthenticator(X509CertificateValidator.ChainTrust);
    }
  }
}
