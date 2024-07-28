// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlInstanceInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlInstanceInfo
  {
    public static SqlInstanceInfo Parse(string browserInfo)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(browserInfo, nameof (browserInfo));
      SqlInstanceInfo sqlInstanceInfo = new SqlInstanceInfo();
      string[] strArray = browserInfo.Split(';');
      for (int index = 0; index < strArray.Length - 1; index += 2)
      {
        switch (strArray[index])
        {
          case "ServerName":
            sqlInstanceInfo.ServerName = strArray[index + 1];
            break;
          case "InstanceName":
            sqlInstanceInfo.InstanceName = strArray[index + 1];
            break;
          case "IsClustered":
            string a = strArray[index + 1];
            sqlInstanceInfo.IsClustered = string.Equals(a, "Yes", StringComparison.Ordinal);
            break;
          case "Version":
            string version = strArray[index + 1];
            sqlInstanceInfo.Version = new Version(version);
            break;
          case "tcp":
            sqlInstanceInfo.IsTcpEnabled = true;
            string str = strArray[index + 1];
            sqlInstanceInfo.TcpPort = Convert.ToInt32(str);
            break;
          case "np":
            sqlInstanceInfo.IsNamedPipesEnabled = true;
            sqlInstanceInfo.NamedPipe = strArray[index + 1];
            break;
        }
      }
      return sqlInstanceInfo;
    }

    public string ServerName { get; private set; }

    public string InstanceName { get; private set; }

    public bool IsClustered { get; private set; }

    public Version Version { get; private set; }

    public bool IsTcpEnabled { get; private set; }

    public int TcpPort { get; private set; }

    public bool IsNamedPipesEnabled { get; private set; }

    public string NamedPipe { get; private set; }
  }
}
