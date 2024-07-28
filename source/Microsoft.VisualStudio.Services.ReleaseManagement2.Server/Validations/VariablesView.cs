// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.VariablesView
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class VariablesView
  {
    private IDictionary<string, IDictionary<string, string>> mergedEnvironmentVariables;

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nested generics appropriate here")]
    public IDictionary<string, IDictionary<string, string>> MergedEnvironmentVariables
    {
      get
      {
        if (this.mergedEnvironmentVariables == null)
          this.mergedEnvironmentVariables = (IDictionary<string, IDictionary<string, string>>) new Dictionary<string, IDictionary<string, string>>();
        return this.mergedEnvironmentVariables;
      }
      internal set => this.mergedEnvironmentVariables = value;
    }
  }
}
