// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.PerfTraceListener
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public class PerfTraceListener : TraceListener
  {
    private static readonly Regex s_requestRegex = new Regex("Web method running: (\\[.*\\]) (.*)");
    private static readonly Regex s_responseRegex = new Regex("Web method response: (\\[.*\\]) (.*) (\\d+) ms");
    private DialogPerfListener m_dialog;

    public PerfTraceListener()
    {
      Thread thread = new Thread((ThreadStart) (() =>
      {
        this.m_dialog = new DialogPerfListener();
        Application.Run((Form) this.m_dialog);
      }));
      thread.SetApartmentState(ApartmentState.STA);
      thread.IsBackground = true;
      thread.Start();
    }

    public override void Write(string message)
    {
      if (this.m_dialog == null || !this.m_dialog.Visible)
        return;
      Match match1 = PerfTraceListener.s_requestRegex.Match(message);
      if (match1.Success)
      {
        string serviceAddress = match1.Groups[1].Value;
        this.m_dialog.AddRunning(match1.Groups[2].Value, serviceAddress);
      }
      else
      {
        Match match2 = PerfTraceListener.s_responseRegex.Match(message);
        if (match2.Success)
          this.m_dialog.AddTime(match2.Groups[2].Value, Convert.ToInt32(match2.Groups[3].Value, (IFormatProvider) CultureInfo.InvariantCulture));
        else if (message.StartsWith("LWT-Load "))
          this.m_dialog.AddRunning(message.Split(' ')[1], "Load");
        else if (message.StartsWith("LWT-Load-Complete"))
        {
          string[] strArray = message.Split(' ');
          this.m_dialog.AddTime(strArray[1], Convert.ToInt32(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (message.StartsWith("LWT-Save "))
          this.m_dialog.AddRunning(message.Split(' ')[1], "Save");
        else if (message.StartsWith("LWT-Save-Complete"))
        {
          string[] strArray = message.Split(' ');
          this.m_dialog.AddTime(strArray[1], Convert.ToInt32(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (message.StartsWith("LWT-Scan "))
        {
          this.m_dialog.AddRunning(message.Split(' ')[1], "Path");
        }
        else
        {
          if (!message.StartsWith("LWT-Scan-Complete"))
            return;
          string[] strArray = message.Split(' ');
          this.m_dialog.AddTime(strArray[1], Convert.ToInt32(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
    }

    public override void WriteLine(string message) => this.Write(message);
  }
}
