// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildInformationModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildInformationModel
  {
    private IDictionary<string, BuildConfigurationModel> m_configurationDict;

    private IEnumerable<InformationNodeModel> Nodes { get; set; }

    public IDictionary<string, BuildConfigurationModel> Configurations
    {
      get
      {
        if (this.m_configurationDict == null)
          this.m_configurationDict = (IDictionary<string, BuildConfigurationModel>) this.GetConfigurationModels(this.Nodes).ToDictionary<BuildConfigurationModel, string>((Func<BuildConfigurationModel, string>) (model => model.Platform + "|" + model.Flavor));
        return this.m_configurationDict;
      }
    }

    public BuildInformationModel(IEnumerable<BuildInformationNode> nodes) => this.BuildInformationHierarchy(nodes);

    private void BuildInformationHierarchy(IEnumerable<BuildInformationNode> nodes)
    {
      if (nodes.Any<BuildInformationNode>())
      {
        Dictionary<int, InformationNodeModel> dictionary1 = new Dictionary<int, InformationNodeModel>();
        Dictionary<int, List<InformationNodeModel>> dictionary2 = new Dictionary<int, List<InformationNodeModel>>();
        List<InformationNodeModel> informationNodeModelList;
        foreach (BuildInformationNode node in nodes)
        {
          InformationNodeModel informationNodeModel = InformationNodeModelFactory.Create(node);
          dictionary1[node.NodeId] = informationNodeModel;
          if (!dictionary2.TryGetValue(node.ParentId, out informationNodeModelList))
          {
            informationNodeModelList = new List<InformationNodeModel>();
            dictionary2.Add(node.ParentId, informationNodeModelList);
          }
          informationNodeModelList.Add(informationNodeModel);
        }
        foreach (KeyValuePair<int, List<InformationNodeModel>> keyValuePair in dictionary2)
        {
          InformationNodeModel informationNodeModel;
          if (dictionary1.TryGetValue(keyValuePair.Key, out informationNodeModel))
            informationNodeModel.Children.AddRange((IEnumerable<InformationNodeModel>) keyValuePair.Value);
        }
        foreach (KeyValuePair<int, List<InformationNodeModel>> keyValuePair in dictionary2)
        {
          InformationNodeModel informationNodeModel;
          if (dictionary1.TryGetValue(keyValuePair.Key, out informationNodeModel))
          {
            informationNodeModel.Children.Sort((IComparer<InformationNodeModel>) new InformationNodeModelComparer());
            InformationNodeHelpers.FixSorting((IEnumerable<InformationNodeModel>) informationNodeModel.Children);
          }
        }
        if (!dictionary2.TryGetValue(0, out informationNodeModelList))
          return;
        informationNodeModelList.Sort((IComparer<InformationNodeModel>) new InformationNodeModelComparer());
        this.Nodes = (IEnumerable<InformationNodeModel>) informationNodeModelList;
      }
      else
        this.Nodes = (IEnumerable<InformationNodeModel>) Array.Empty<InformationNodeModel>();
    }

    private List<BuildConfigurationModel> GetConfigurationModels(
      IEnumerable<InformationNodeModel> nodeModels)
    {
      List<BuildConfigurationModel> configurationModels = new List<BuildConfigurationModel>();
      foreach (InformationNodeModel nodeModel in nodeModels)
      {
        if (nodeModel is BuildConfigurationModel configurationModel)
          configurationModels.Add(configurationModel);
        if (nodeModel.Children.Any<InformationNodeModel>())
          configurationModels.AddRange((IEnumerable<BuildConfigurationModel>) this.GetConfigurationModels((IEnumerable<InformationNodeModel>) nodeModel.Children));
      }
      return configurationModels;
    }

    public List<IntermediateLogNodeModel> GetIntermediateLogNodeModels() => this.GetIntermediateLogNodeModels(this.Nodes);

    private List<IntermediateLogNodeModel> GetIntermediateLogNodeModels(
      IEnumerable<InformationNodeModel> nodeModels)
    {
      List<IntermediateLogNodeModel> intermediateLogNodeModels = new List<IntermediateLogNodeModel>();
      foreach (InformationNodeModel nodeModel in nodeModels)
      {
        if (nodeModel is IntermediateLogNodeModel intermediateLogNodeModel)
          intermediateLogNodeModels.Add(intermediateLogNodeModel);
        if (nodeModel.Children.Any<InformationNodeModel>())
          intermediateLogNodeModels.AddRange((IEnumerable<IntermediateLogNodeModel>) this.GetIntermediateLogNodeModels((IEnumerable<InformationNodeModel>) nodeModel.Children));
      }
      return intermediateLogNodeModels;
    }

    public object ToJson() => (object) this.Nodes.Select<InformationNodeModel, JsObject>((Func<InformationNodeModel, JsObject>) (node => node.ToJson()));
  }
}
