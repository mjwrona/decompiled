// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WebServiceCallList
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  public class WebServiceCallList : BindingList<WebServiceCall>
  {
    public void AddTime(string webServiceName, int runTime)
    {
      WebServiceCall webServiceCall1 = new WebServiceCall(webServiceName, runTime, false);
      List<WebServiceCall> items = this.Items as List<WebServiceCall>;
      int index1 = -1;
      for (int index2 = 0; index2 < items.Count; ++index2)
      {
        WebServiceCall webServiceCall2 = items[index2];
        if (webServiceCall2.WebServiceCalled == webServiceName && webServiceCall2.RunTime == 0 && webServiceCall2.IsRunning)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 == -1)
      {
        this.Add(webServiceCall1);
      }
      else
      {
        WebServiceCall webServiceCall3 = items[index1];
        webServiceCall1.StartTime = webServiceCall3.StartTime;
        webServiceCall1.ThreadId = webServiceCall3.ThreadId;
        webServiceCall1.ThreadPriority = webServiceCall3.ThreadPriority;
        this.SetItem(index1, webServiceCall1);
      }
    }

    internal void AddRunning(string webServiceName, int threadId, ThreadPriority threadPriority) => this.Add(new WebServiceCall(webServiceName, 0, true)
    {
      ThreadId = threadId,
      ThreadPriority = threadPriority.ToString()
    });
  }
}
