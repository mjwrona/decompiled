// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.AfnStrip
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class AfnStrip : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Stream { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long UnCompressedStreamLength { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FileName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StoredIn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AuxiliaryUrl => this.Url + "&" + string.Join("&", new Dictionary<string, string>()
    {
      {
        "project",
        this.Project
      },
      {
        "testRunId",
        this.TestRunId.ToString()
      },
      {
        "testResultId",
        this.TestResultId.ToString()
      },
      {
        "attachmentId",
        this.Id.ToString()
      },
      {
        "storedIn",
        this.StoredIn
      }
    }.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key + "=" + kvp.Value)));
  }
}
