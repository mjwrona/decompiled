// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProxyStatisticsWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Globalization;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  internal sealed class ProxyStatisticsWebService : TfsHttpClientBase
  {
    public ProxyStatisticsWebService(VssCredentials credentials, Uri url)
      : base(url, new Guid?(), (CultureInfo) null, credentials)
    {
    }

    protected override string ComponentName => "Framework";

    public int? Timeout { get; set; }

    protected override TfsRequestSettings ApplyCustomSettings(TfsRequestSettings settings)
    {
      if (!this.Timeout.HasValue)
        return base.ApplyCustomSettings(settings);
      settings = settings.Clone();
      settings.SendTimeout = TimeSpan.FromMilliseconds((double) this.Timeout.Value);
      return settings;
    }

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public ProxyStatisticsInfo[] QueryProxyStatistics() => (ProxyStatisticsInfo[]) this.Invoke((TfsClientOperation) new ProxyStatisticsWebService.QueryProxyStatisticsClientOperation(), Array.Empty<object>());

    public IAsyncResult BeginQueryProxyStatistics(AsyncCallback callback, object state) => this.BeginInvoke((TfsClientOperation) new ProxyStatisticsWebService.QueryProxyStatisticsClientOperation(), Array.Empty<object>(), callback, state);

    public ProxyStatisticsInfo[] EndQueryProxyStatistics(IAsyncResult result) => (ProxyStatisticsInfo[]) this.EndInvoke(result);

    internal sealed class QueryProxyStatisticsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryProxyStatistics";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryProxyStatisticsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Statistics/03/QueryProxyStatistics";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Statistics/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) ProxyStatisticsWebService.Helper.ZeroLengthArrayOfProxyStatisticsInfo;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ProxyStatisticsWebService.Helper.ArrayOfProxyStatisticsInfoFromXml(serviceProvider, reader, false);
    }

    private static class Helper
    {
      private static ProxyStatisticsInfo[] m_zeroLengthArrayOfProxyStatisticsInfo;

      internal static ProxyStatisticsInfo[] ZeroLengthArrayOfProxyStatisticsInfo
      {
        get
        {
          if (ProxyStatisticsWebService.Helper.m_zeroLengthArrayOfProxyStatisticsInfo == null)
            ProxyStatisticsWebService.Helper.m_zeroLengthArrayOfProxyStatisticsInfo = Array.Empty<ProxyStatisticsInfo>();
          return ProxyStatisticsWebService.Helper.m_zeroLengthArrayOfProxyStatisticsInfo;
        }
      }

      internal static ProxyStatisticsInfo[] ArrayOfProxyStatisticsInfoFromXml(
        IServiceProvider serviceProvider,
        XmlReader reader,
        bool inline)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return XmlUtility.ArrayOfObjectFromXml<ProxyStatisticsInfo>(serviceProvider, reader, "ProxyStatisticsInfo", inline, ProxyStatisticsWebService.Helper.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ProxyStatisticsWebService.Helper.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, ProxyStatisticsInfo>(ProxyStatisticsInfo.FromXml)));
      }
    }
  }
}
