// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.BintrayErrorSerializer
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class BintrayErrorSerializer : IUnknownUpstreamResponseSerializer
  {
    private readonly ITracerService tracerService;

    public BintrayErrorSerializer(ITracerService tracerService) => this.tracerService = tracerService;

    public bool TryDeserialize(string responseString, out IUnknownUpstreamResponse response)
    {
      using (this.tracerService.Enter((object) this, nameof (TryDeserialize)))
      {
        ArgumentUtility.CheckForNull<string>(responseString, nameof (responseString));
        string str = responseString.Replace("\\\"", "\"").Trim('"');
        response = (IUnknownUpstreamResponse) null;
        BintrayError bintrayError;
        try
        {
          bintrayError = JsonConvert.DeserializeObject<BintrayError>(str);
        }
        catch
        {
          return false;
        }
        if (bintrayError == null || bintrayError.Error == null || bintrayError.Reason == null)
          return false;
        response = (IUnknownUpstreamResponse) bintrayError;
        return true;
      }
    }
  }
}
