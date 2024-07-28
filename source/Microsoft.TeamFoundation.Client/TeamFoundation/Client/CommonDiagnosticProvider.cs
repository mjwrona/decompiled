// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommonDiagnosticProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  internal class CommonDiagnosticProvider : ITfsDiagnosticProvider
  {
    private static readonly string s_featureTeamCommon = "Common";
    private static readonly string s_featureAreaEnvironment = "Environment";
    private readonly string s_areaPathEnvironment;

    internal CommonDiagnosticProvider() => this.s_areaPathEnvironment = TfsDiagnosticService.BuildAreaPath(CommonDiagnosticProvider.s_featureTeamCommon, CommonDiagnosticProvider.s_featureAreaEnvironment);

    public string Name => "TFS common diagnostic provider";

    public string[] AreaPaths => new string[1]
    {
      this.s_areaPathEnvironment
    };

    public void WriteState(string areaPath, XmlWriter writer)
    {
      if (!string.Equals(areaPath, this.s_areaPathEnvironment, StringComparison.OrdinalIgnoreCase))
        return;
      using (new XmlElementWriterUtility("OSInfo", writer))
        writer.WriteAttributeString("Version", Environment.OSVersion.VersionString);
      using (new XmlElementWriterUtility("DisplayInfo", writer))
      {
        writer.WriteAttributeString("HighContrast", SystemInformation.HighContrast.ToString());
        writer.WriteAttributeString("VirtualScreenSize", SystemInformation.VirtualScreen.ToString());
        using (new XmlElementWriterUtility("DisplayList", writer))
        {
          foreach (Screen allScreen in Screen.AllScreens)
          {
            using (new XmlElementWriterUtility("Display", writer))
            {
              XmlWriter xmlWriter1 = writer;
              Rectangle rectangle = allScreen.Bounds;
              string str1 = rectangle.ToString();
              xmlWriter1.WriteAttributeString("Bounds", str1);
              XmlWriter xmlWriter2 = writer;
              rectangle = allScreen.WorkingArea;
              string str2 = rectangle.ToString();
              xmlWriter2.WriteAttributeString("WorkingArea", str2);
              writer.WriteAttributeString("IsPrimary", allScreen.Primary.ToString());
            }
          }
        }
      }
    }
  }
}
