// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsDiagnosticNodeInfo
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsDiagnosticNodeInfo
  {
    public TfsDiagnosticNodeInfo(string nodeName, string areaPath, ITfsDiagnosticProvider provider)
    {
      this.Name = nodeName;
      this.AreaPath = areaPath;
      this.Provider = provider;
    }

    public string Name { get; private set; }

    public string AreaPath { get; private set; }

    public ITfsDiagnosticProvider Provider { get; private set; }

    public override string ToString() => this.Name;
  }
}
