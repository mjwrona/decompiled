// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.TestCaseParameterDataRow
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TestCaseParameterDataRow
  {
    [DataMember(Name = "parameterValues")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IDictionary<string, TestCaseParameterValue> ParameterValues;
  }
}
