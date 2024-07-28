// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitAsyncRefOperationParameters
  {
    [DataMember]
    public GitRepository Repository { get; set; }

    [DataMember]
    public GitAsyncRefOperationSource Source { get; set; }

    [DataMember]
    public string OntoRefName { get; set; }

    [DataMember]
    public string GeneratedRefName { get; set; }
  }
}
