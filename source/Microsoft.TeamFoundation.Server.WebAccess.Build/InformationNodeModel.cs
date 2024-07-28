// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.InformationNodeModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class InformationNodeModel
  {
    public InformationNodeModel(BuildInformationNode node)
    {
      this.Node = node;
      this.Fields = this.Node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (n => n.Name), (Func<InformationField, string>) (n => n.Value));
      this.Children = new List<InformationNodeModel>();
      this.InProgress = string.Equals("Executing", this.GetFieldValue(InformationFields.State), StringComparison.OrdinalIgnoreCase);
      this.StartTime = CommonInformationHelper.GetDateTime((IDictionary<string, string>) this.Fields, InformationFields.StartTime);
      this.TimeStamp = CommonInformationHelper.GetDateTime((IDictionary<string, string>) this.Fields, InformationFields.Timestamp);
    }

    private Dictionary<string, string> Fields { get; set; }

    public List<InformationNodeModel> Children { get; private set; }

    public BuildInformationNode Node { get; private set; }

    public virtual string NodeType => string.Empty;

    public bool InProgress { get; private set; }

    public string Text { get; protected set; }

    public DateTime StartTime { get; private set; }

    public DateTime TimeStamp { get; private set; }

    public JsObject ToJson()
    {
      JsObject result = new JsObject();
      result["type"] = (object) this.Node.Type;
      result["text"] = (object) this.Text;
      if (!string.IsNullOrWhiteSpace(this.NodeType))
        result["nodeType"] = (object) this.NodeType;
      if (this.InProgress)
        result["status"] = (object) "inprogress";
      if (this.Children.Any<InformationNodeModel>())
        result["nodes"] = (object) this.Children.Select<InformationNodeModel, JsObject>((Func<InformationNodeModel, JsObject>) (node => node.ToJson()));
      this.ContributeToJson(result);
      return result;
    }

    protected virtual void ContributeToJson(JsObject result)
    {
    }

    protected virtual string GetFieldValue(string fieldName) => CommonInformationHelper.GetString((IDictionary<string, string>) this.Fields, fieldName);

    protected virtual int GetFieldValueInt(string fieldName) => CommonInformationHelper.GetInt((IDictionary<string, string>) this.Fields, fieldName);
  }
}
