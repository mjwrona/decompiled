// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.BrokenPolicyEvaluationRecordContext
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [DataContract]
  public class BrokenPolicyEvaluationRecordContext : TeamFoundationPolicyEvaluationRecordContext
  {
    public static readonly Guid NoError = new Guid("D06D4F24-65F0-40A8-B002-4083A185F450");
    public static readonly Guid MissingType = new Guid("9F3C4C7D-F104-4063-AEB6-35F3CA2396C7");
    public static readonly Guid Unknown = new Guid("BDDCDD34-FC89-44DE-9515-F6728D2CB31E");

    [JsonConstructor]
    public BrokenPolicyEvaluationRecordContext(Guid errorCode) => this.ErrorCode = errorCode;

    [DataMember(IsRequired = true)]
    public Guid ErrorCode { get; private set; }
  }
}
