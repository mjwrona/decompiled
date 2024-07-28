// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.ProvenanceInfoProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class ProvenanceInfoProvider : IProvenanceInfoProvider
  {
    private readonly string userAgent;
    private readonly IReadOnlyList<ISessionRequestProvider> sessionRequestProviders;

    public ProvenanceInfoProvider(
      string userAgent,
      IReadOnlyList<ISessionRequestProvider> sessionRequestProviders)
    {
      this.userAgent = userAgent;
      this.sessionRequestProviders = sessionRequestProviders;
    }

    public async Task<ProvenanceInfo> GetProvenanceInfoAsync()
    {
      ProvenanceInfo provenance = new ProvenanceInfo()
      {
        UserAgent = this.userAgent
      };
      foreach (ISessionRequestProvider sessionRequestProvider in (IEnumerable<ISessionRequestProvider>) this.sessionRequestProviders)
      {
        (bool, SessionRequest) sessionRequest = await sessionRequestProvider.TryGetSessionRequest();
        if (sessionRequest.Item1 && sessionRequest.Item2 != null)
        {
          provenance.ProvenanceSource = sessionRequest.Item2.Source;
          provenance.Data = sessionRequest.Item2.Data;
          break;
        }
      }
      ProvenanceInfo provenanceInfoAsync = provenance;
      provenance = (ProvenanceInfo) null;
      return provenanceInfoAsync;
    }
  }
}
