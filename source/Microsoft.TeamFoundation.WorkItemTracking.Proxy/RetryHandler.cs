// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RetryHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Globalization;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class RetryHandler
  {
    private RetryTypes m_retryTypes;
    private int m_retriesDeadlock;
    private int m_retriesStaleView;

    internal RetryHandler(RetryTypes types) => this.m_retryTypes = RetryTypes.All & types;

    internal void HandleSoapException(SoapException se)
    {
      int errorId = this.GetErrorId(se);
      int num = errorId == 1205 ? 1 : 0;
      bool flag = this.IsStaleViewError(errorId);
      if (num != 0)
      {
        ++this.m_retriesDeadlock;
        if (2 - this.m_retriesDeadlock < 0)
          throw se;
        TeamFoundationTrace.Info(TraceKeywordSets.Database, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Soap Exception Occurred: Deadlock: {0} Attempts - {1}", (object) se.Message, (object) this.Attempts));
      }
      else
      {
        if (!flag)
          throw se;
        ++this.m_retriesStaleView;
        if (1 - this.m_retriesStaleView < 0)
          throw se;
        TeamFoundationTrace.Info(TraceKeywordSets.Database, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Soap Exception Occurred: Stale view: {0} Attempts - {1}", (object) se.Message, (object) this.Attempts));
      }
      this.RandomSleep();
    }

    internal bool HasRemainingRetries => (1 - this.m_retriesStaleView < 0 ? 1 : (2 - this.m_retriesDeadlock < 0 ? 1 : 0)) == 0;

    private void RandomSleep() => Thread.Sleep(new Random().Next(3000, 10000));

    private int Attempts => this.m_retriesDeadlock + this.m_retriesStaleView;

    private bool IsStaleViewError(int errorId)
    {
      switch (errorId)
      {
        case 4413:
        case 4501:
        case 4502:
          return true;
        default:
          return false;
      }
    }

    private int GetErrorId(SoapException se)
    {
      int errorId = 0;
      try
      {
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(se.Detail.OwnerDocument.NameTable);
        nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultdetail/03");
        if (se.Detail != null)
        {
          XmlNode xmlNode = se.Detail.SelectSingleNode("ns:details", nsmgr);
          if (xmlNode != null)
          {
            if (xmlNode.Attributes != null)
              errorId = Convert.ToInt32(xmlNode.Attributes.GetNamedItem("id").Value, (IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Info(TraceKeywordSets.Database, " Exception Occurred: " + ex.Message);
      }
      return errorId;
    }
  }
}
