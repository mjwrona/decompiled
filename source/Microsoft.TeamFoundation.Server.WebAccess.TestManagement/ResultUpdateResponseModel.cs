// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ResultUpdateResponseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ResultUpdateResponseModel
  {
    public ResultUpdateResponseModel(ResultUpdateResponse response) => this.Revision = response.Revision;

    [DataMember(Name = "revision")]
    public int Revision { get; set; }
  }
}
