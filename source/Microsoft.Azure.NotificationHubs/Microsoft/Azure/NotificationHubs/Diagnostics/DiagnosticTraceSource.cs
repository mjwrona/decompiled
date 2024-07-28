// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.DiagnosticTraceSource
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class DiagnosticTraceSource : PiiTraceSource
  {
    private const string PropagateActivityValue = "propagateActivity";

    internal DiagnosticTraceSource(string name, string eventSourceName)
      : base(name, eventSourceName)
    {
    }

    internal DiagnosticTraceSource(string name, string eventSourceName, SourceLevels level)
      : base(name, eventSourceName, level)
    {
    }

    protected override string[] GetSupportedAttributes()
    {
      string[] supportedAttributes1 = base.GetSupportedAttributes();
      string[] supportedAttributes2 = new string[supportedAttributes1.Length + 1];
      for (int index = 0; index < supportedAttributes1.Length; ++index)
        supportedAttributes2[index] = supportedAttributes1[index];
      supportedAttributes2[supportedAttributes1.Length] = "propagateActivity";
      return supportedAttributes2;
    }

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
