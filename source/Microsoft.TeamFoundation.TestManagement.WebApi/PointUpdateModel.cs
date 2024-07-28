// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PointUpdateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class PointUpdateModel
  {
    public PointUpdateModel(bool? resetToActive = null, string outcome = "", IdentityRef tester = null)
    {
      this.ResetToActive = resetToActive;
      if (!string.IsNullOrEmpty(outcome))
        this.Outcome = outcome;
      if (tester == null)
        return;
      this.Tester = tester;
    }

    [DataMember]
    public IdentityRef Tester { get; private set; }

    [DataMember]
    public string Outcome { get; private set; }

    [DataMember]
    public bool? ResetToActive { get; private set; }

    public override string ToString() => string.Format("Tester : {0}, Outcome : {1}", (object) this.Tester, (object) this.Outcome);
  }
}
