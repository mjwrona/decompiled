// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ServicePointHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.Net;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ServicePointHelper
  {
    public static string GetServicePointManagerSummary()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("{0}={1},", (object) "DefaultConnectionLimit", (object) ServicePointManager.DefaultConnectionLimit));
      stringBuilder.Append(string.Format("{0}={1}", (object) "SecurityProtocol", (object) ServicePointManager.SecurityProtocol));
      return stringBuilder.ToString();
    }
  }
}
