// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.Classification4
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03", Description = "DevOps Classification web service V4.0")]
  [ClientService(ServiceName = "CommonStructure4", CollectionServiceIdentifier = "edd317f7-a7c3-4c97-a039-ba933e895201")]
  [ProxyParentClass("Classification3", IgnoreInheritedMethods = true)]
  public class Classification4 : Classification3
  {
    private static readonly HashSet<string> s_backCompatPropertyNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName)
    {
      TeamConstants.DefaultTeamPropertyName
    };

    public Classification4()
      : base(4)
    {
    }

    public Classification4(int serviceVersion)
      : base(serviceVersion)
    {
    }

    [WebMethod]
    public string CreateNodeWithDates(
      string nodeName,
      string parentNodeUri,
      [XmlElement(DataType = "date")] DateTime? startDate,
      [XmlElement(DataType = "date")] DateTime? finishDate)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("CreateNode", MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (nodeName), (object) nodeName);
        methodInformation.AddParameter(nameof (parentNodeUri), (object) parentNodeUri);
        methodInformation.AddParameter(nameof (startDate), (object) startDate);
        methodInformation.AddParameter(nameof (finishDate), (object) finishDate);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().CreateNode(this.RequestContext, nodeName, parentNodeUri, startDate, finishDate);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void SetIterationDates(string nodeUri, [XmlElement(DataType = "date")] DateTime? startDate, [XmlElement(DataType = "date")] DateTime? finishDate)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetIterationDates), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (nodeUri), (object) nodeUri);
        methodInformation.AddParameter(nameof (startDate), (object) startDate);
        methodInformation.AddParameter(nameof (finishDate), (object) finishDate);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().SetIterationDates(this.RequestContext, nodeUri, startDate, finishDate);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CommonStructureProjectProperty GetProjectProperty(string projectUri, string name)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProjectProperty), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (name), (object) name);
        this.EnterMethod(methodInformation);
        Guid id;
        projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
        string str = InternalCommonStructureUtils.TranslatePropertyName(name);
        if (InternalCommonStructureUtils.BackCompatSystemPropertyNames.Contains(str))
        {
          ProjectProperty projectProperty = this.RequestContext.GetService<IProjectService>().GetProjectProperties(this.RequestContext, id, str).FirstOrDefault<ProjectProperty>();
          if (projectProperty != null)
            return new CommonStructureProjectProperty(name, (string) projectProperty.Value);
        }
        return new CommonStructureProjectProperty(name, string.Empty);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void SetProjectProperty(string projectUri, string name, string value)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetProjectProperty), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddParameter(nameof (value), (object) value);
        this.EnterMethod(methodInformation);
        this.CheckPermissionsAndSetProperties(ProjectInfo.GetProjectId(projectUri), (IEnumerable<ProjectProperty>) new ProjectProperty[1]
        {
          new ProjectProperty(InternalCommonStructureUtils.TranslatePropertyName(name), (object) value)
        }, Classification4.s_backCompatPropertyNames);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
