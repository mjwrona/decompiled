// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPointGridDisplayColumn
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestPointGridDisplayColumn : Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryDisplayColumn
  {
    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "type")]
    public string Type { get; set; }
  }
}
