// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.VariableSecret
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class VariableSecret
  {
    public string Name;
    public string Value;
    private const string c_secretVariableStrongBoxLookupKeyFormat = "variables/{0}";

    public virtual string GetLookupKey() => VariableSecret.GetLookupKey(this.Name);

    public static string GetLookupKey(string name) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "variables/{0}", (object) name);
  }
}
