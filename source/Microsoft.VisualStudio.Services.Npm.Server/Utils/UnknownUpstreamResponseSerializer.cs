// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.UnknownUpstreamResponseSerializer
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class UnknownUpstreamResponseSerializer : IUnknownUpstreamResponseSerializer
  {
    private IEnumerable<IUnknownUpstreamResponseSerializer> serializers;
    private readonly ITracerService tracerService;

    public UnknownUpstreamResponseSerializer(ITracerService tracerService)
    {
      this.serializers = (IEnumerable<IUnknownUpstreamResponseSerializer>) new IUnknownUpstreamResponseSerializer[1]
      {
        (IUnknownUpstreamResponseSerializer) new BintrayErrorSerializer(tracerService)
      };
      this.tracerService = tracerService;
    }

    public bool TryDeserialize(string responseString, out IUnknownUpstreamResponse response)
    {
      using (this.tracerService.Enter((object) this, nameof (TryDeserialize)))
      {
        foreach (IUnknownUpstreamResponseSerializer serializer in this.serializers)
        {
          IUnknownUpstreamResponse response1;
          if (serializer.TryDeserialize(responseString, out response1))
          {
            response = response1;
            return true;
          }
        }
        response = (IUnknownUpstreamResponse) null;
        return false;
      }
    }
  }
}
