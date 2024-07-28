// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class DataCollectorInformation
  {
    private List<NameValuePair> m_properties;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string TypeUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public XmlNode DefaultConfiguration { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public XmlNode ConfigurationEditorConfiguration { get; set; }

    [XmlArray]
    [XmlArrayItem(Type = typeof (NameValuePair))]
    [ClientProperty(ClientVisibility.Private)]
    public List<NameValuePair> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new List<NameValuePair>();
        return this.m_properties;
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataCollectorInformation Name={0}", (object) this.TypeUri);

    internal static void Register(
      TestManagementRequestContext context,
      List<DataCollectorInformation> collectors)
    {
      ArgumentUtility.CheckForNull<List<DataCollectorInformation>>(collectors, nameof (collectors), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (DataCollectorInformation collector in collectors)
        {
          string str = collector.TypeUri ?? string.Empty;
          Validator.CheckAndTrimString(ref str, "TypeUri", 256);
          collector.Id = Guid.NewGuid();
          ArgumentUtility.CheckForNull<DataCollectorInformation>(collector, "collector", context.RequestContext.ServiceName);
          DataCollectorInformation.ValidateXml(context, collector);
          planningDatabase.RegisterDataCollector(collector);
        }
      }
    }

    internal static void Unregister(
      TestManagementRequestContext context,
      List<DataCollectorInformation> collectors)
    {
      ArgumentUtility.CheckForNull<List<DataCollectorInformation>>(collectors, nameof (collectors), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (DataCollectorInformation collector in collectors)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(collector.TypeUri, "typeUri", context.RequestContext.ServiceName);
          planningDatabase.UnregisterDataCollector(collector.TypeUri);
        }
      }
    }

    internal static void Update(
      TestManagementRequestContext context,
      List<DataCollectorInformation> collectors)
    {
      ArgumentUtility.CheckForNull<List<DataCollectorInformation>>(collectors, nameof (collectors), context.RequestContext.ServiceName);
      context.SecurityManager.CheckManageTestControllersPermission(context);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        foreach (DataCollectorInformation collector in collectors)
        {
          try
          {
            ArgumentUtility.CheckForNull<DataCollectorInformation>(collector, "collector", context.RequestContext.ServiceName);
            planningDatabase.UpdateDataCollector(collector);
          }
          catch (TestObjectNotFoundException ex)
          {
            throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DataCollectorNotFound, (object) collector.TypeUri));
          }
        }
      }
    }

    internal static List<DataCollectorInformation> Query(TfsTestManagementRequestContext context)
    {
      List<DataCollectorInformation> collectorInformationList = new List<DataCollectorInformation>();
      if (!context.SecurityManager.HasGenericReadPermission((TestManagementRequestContext) context))
        return (List<DataCollectorInformation>) null;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        return planningDatabase.QueryDataCollectors((string) null);
    }

    internal static DataCollectorInformation Find(
      TestManagementRequestContext context,
      string typeUri)
    {
      if (context.SecurityManager.HasGenericReadPermission(context))
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          List<DataCollectorInformation> collectorInformationList = planningDatabase.QueryDataCollectors(typeUri);
          if (collectorInformationList != null && collectorInformationList.Count > 0)
            return collectorInformationList[0];
          context.TraceVerbose("BusinessLayer", "DataCollectorInformation.Find No collector found.");
        }
      }
      else
        context.TraceVerbose("BusinessLayer", "DataCollectorInformation.Find Permission check failed.");
      return (DataCollectorInformation) null;
    }

    internal static void ValidateXml(
      TestManagementRequestContext context,
      DataCollectorInformation collector)
    {
      string serviceName = context?.RequestContext.ServiceName;
      if (!TcmArgumentValidator.ValidateXml(collector.DefaultConfiguration, true))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "DefaultConfiguration")).Expected(serviceName);
      if (!TcmArgumentValidator.ValidateXml(collector.ConfigurationEditorConfiguration, true))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "ConfigurationEditorConfiguration")).Expected(serviceName);
    }
  }
}
