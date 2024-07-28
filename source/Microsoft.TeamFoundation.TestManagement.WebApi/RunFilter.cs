// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class RunFilter : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string SourceFilter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestCaseFilter { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Filter: SourceFilter={0}, TestCaseFilter={1}", (object) this.SourceFilter, (object) this.TestCaseFilter);
  }
}
