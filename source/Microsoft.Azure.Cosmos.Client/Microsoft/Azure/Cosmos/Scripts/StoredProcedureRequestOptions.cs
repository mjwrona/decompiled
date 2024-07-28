// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.StoredProcedureRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.Scripts
{
  public class StoredProcedureRequestOptions : RequestOptions
  {
    public bool EnableScriptLogging { get; set; }

    public string SessionToken { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel
    {
      get => this.BaseConsistencyLevel;
      set => this.BaseConsistencyLevel = value;
    }

    internal override void PopulateRequestOptions(RequestMessage request)
    {
      if (this.EnableScriptLogging)
        request.Headers.Add("x-ms-documentdb-script-enable-logging", bool.TrueString);
      RequestOptions.SetSessionToken(request, this.SessionToken);
      base.PopulateRequestOptions(request);
    }
  }
}
