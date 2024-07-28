// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.TemporaryQueryModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class TemporaryQueryModel
  {
    private int _queryType = 1;

    [DataMember(EmitDefaultValue = false)]
    public string QueryText { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int QueryType
    {
      get => this._queryType;
      set => this._queryType = value;
    }
  }
}
