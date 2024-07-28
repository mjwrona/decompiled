// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.DiagnosticTraceSource
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal class DiagnosticTraceSource : TraceSource
  {
    private const string PropagateActivityValue = "propagateActivity";

    internal DiagnosticTraceSource(string name)
      : base(name)
    {
    }

    protected override string[] GetSupportedAttributes() => new string[1]
    {
      "propagateActivity"
    };

    internal bool PropagateActivity
    {
      get
      {
        bool result = false;
        string attribute = this.Attributes["propagateActivity"];
        if (!string.IsNullOrEmpty(attribute) && !bool.TryParse(attribute, out result))
          result = false;
        return result;
      }
      set => this.Attributes["propagateActivity"] = value.ToString();
    }
  }
}
