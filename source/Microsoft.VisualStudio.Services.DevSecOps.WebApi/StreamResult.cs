// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.StreamResult
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class StreamResult
  {
    public StreamResult(string streamId, bool hasAnyResults)
    {
      this.StreamId = streamId;
      this.HasAnyResults = hasAnyResults;
      this.Results = (IList<Violation>) new List<Violation>();
    }

    [DataMember]
    public string StreamId { get; }

    [DataMember]
    public bool HasAnyResults { get; private set; }

    [DataMember]
    public IList<Violation> Results { get; }

    public void AddResult(Violation violation, bool includeInResults)
    {
      this.HasAnyResults = true;
      if (!includeInResults)
        return;
      this.Results.Add(violation);
    }
  }
}
