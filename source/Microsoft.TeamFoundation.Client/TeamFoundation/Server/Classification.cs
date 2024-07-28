// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Classification
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  internal class Classification : TfsHttpClient
  {
    internal Classification(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    protected override string ComponentName => "CommonStructure";

    public Classification(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("d9c3f8ff-8938-4193-919b-7588e81cb730");

    protected override string ServiceType => "CommonStructure";

    public string CreateNode(string nodeName, string parentNodeUri) => (string) this.Invoke((TfsClientOperation) new Classification.CreateNodeClientOperation(), new object[2]
    {
      (object) nodeName,
      (object) parentNodeUri
    });

    public ProjectInfo CreateProject(string projectName, XmlNode structure) => (ProjectInfo) this.Invoke((TfsClientOperation) new Classification.CreateProjectClientOperation(), new object[2]
    {
      (object) projectName,
      (object) structure
    });

    public void DeleteBranches(string[] nodeUris, string reclassifyUri) => this.Invoke((TfsClientOperation) new Classification.DeleteBranchesClientOperation(), new object[2]
    {
      (object) nodeUris,
      (object) reclassifyUri
    });

    public void DeleteProject(string projectUri) => this.Invoke((TfsClientOperation) new Classification.DeleteProjectClientOperation(), new object[1]
    {
      (object) projectUri
    });

    public string GetChangedNodes(int firstSequenceId) => (string) this.Invoke((TfsClientOperation) new Classification.GetChangedNodesClientOperation(), new object[1]
    {
      (object) firstSequenceId
    });

    public XmlNode GetDeletedNodesXml(string projectUri, DateTime since) => (XmlNode) this.Invoke((TfsClientOperation) new Classification.GetDeletedNodesXmlClientOperation(), new object[2]
    {
      (object) projectUri,
      (object) since
    });

    public NodeInfo GetNode(string nodeUri) => (NodeInfo) this.Invoke((TfsClientOperation) new Classification.GetNodeClientOperation(), new object[1]
    {
      (object) nodeUri
    });

    public NodeInfo GetNodeFromPath(string nodePath) => (NodeInfo) this.Invoke((TfsClientOperation) new Classification.GetNodeFromPathClientOperation(), new object[1]
    {
      (object) nodePath
    });

    public XmlNode GetNodesXml(string[] nodeUris, bool childNodes) => (XmlNode) this.Invoke((TfsClientOperation) new Classification.GetNodesXmlClientOperation(), new object[2]
    {
      (object) nodeUris,
      (object) childNodes
    });

    public ProjectInfo GetProject(string projectUri) => (ProjectInfo) this.Invoke((TfsClientOperation) new Classification.GetProjectClientOperation(), new object[1]
    {
      (object) projectUri
    });

    public ProjectInfo GetProjectFromName(string projectName) => (ProjectInfo) this.Invoke((TfsClientOperation) new Classification.GetProjectFromNameClientOperation(), new object[1]
    {
      (object) projectName
    });

    public void GetProjectProperties(
      string projectUri,
      out string name,
      out string state,
      out int templateId,
      out ProjectProperty[] properties)
    {
      object[] outputs;
      this.Invoke((TfsClientOperation) new Classification.GetProjectPropertiesClientOperation(), new object[1]
      {
        (object) projectUri
      }, out outputs);
      name = (string) outputs[0];
      state = (string) outputs[1];
      templateId = (int) outputs[2];
      properties = (ProjectProperty[]) outputs[3];
    }

    public ProjectInfo[] ListAllProjects() => (ProjectInfo[]) this.Invoke((TfsClientOperation) new Classification.ListAllProjectsClientOperation(), Array.Empty<object>());

    public ProjectInfo[] ListProjects() => (ProjectInfo[]) this.Invoke((TfsClientOperation) new Classification.ListProjectsClientOperation(), Array.Empty<object>());

    public NodeInfo[] ListStructures(string projectUri) => (NodeInfo[]) this.Invoke((TfsClientOperation) new Classification.ListStructuresClientOperation(), new object[1]
    {
      (object) projectUri
    });

    public void MoveBranch(string nodeUri, string newParentNodeUri) => this.Invoke((TfsClientOperation) new Classification.MoveBranchClientOperation(), new object[2]
    {
      (object) nodeUri,
      (object) newParentNodeUri
    });

    public void RenameNode(string nodeUri, string newNodeName) => this.Invoke((TfsClientOperation) new Classification.RenameNodeClientOperation(), new object[2]
    {
      (object) nodeUri,
      (object) newNodeName
    });

    public void ReorderNode(string nodeUri, int moveBy) => this.Invoke((TfsClientOperation) new Classification.ReorderNodeClientOperation(), new object[2]
    {
      (object) nodeUri,
      (object) moveBy
    });

    public void UpdateProjectProperties(
      string projectUri,
      string state,
      ProjectProperty[] properties)
    {
      this.Invoke((TfsClientOperation) new Classification.UpdateProjectPropertiesClientOperation(), new object[3]
      {
        (object) projectUri,
        (object) state,
        (object) properties
      });
    }

    internal sealed class CreateNodeClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateNode";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateNodeResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/CreateNode";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeName", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "parentNodeUri", parameter2);
      }
    }

    internal sealed class CreateProjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateProject";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateProjectResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/CreateProject";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ProjectInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter1);
        XmlNode parameter2 = (XmlNode) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "structure", parameter2);
      }
    }

    internal sealed class DeleteBranchesClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteBranches";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/DeleteBranches";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "nodeUris", parameter1, false, false);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "reclassifyUri", parameter2);
      }
    }

    internal sealed class DeleteProjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteProject";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/DeleteProject";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class GetChangedNodesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetChangedNodes";

      public override bool HasOutputs => true;

      public override string ResultName => "GetChangedNodesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetChangedNodes";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "firstSequenceId", parameter);
      }
    }

    internal sealed class GetDeletedNodesXmlClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetDeletedNodesXml";

      public override bool HasOutputs => true;

      public override string ResultName => "GetDeletedNodesXmlResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetDeletedNodesXml";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.XmlNodeFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        DateTime parameter2 = (DateTime) parameters[1];
        if (!(parameter2 != DateTime.MinValue))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "since", parameter2);
      }
    }

    internal sealed class GetNodeClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetNode";

      public override bool HasOutputs => true;

      public override string ResultName => "GetNodeResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetNode";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) NodeInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "nodeUri", parameter);
      }
    }

    internal sealed class GetNodeFromPathClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetNodeFromPath";

      public override bool HasOutputs => true;

      public override string ResultName => "GetNodeFromPathResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetNodeFromPath";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) NodeInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "nodePath", parameter);
      }
    }

    internal sealed class GetNodesXmlClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetNodesXml";

      public override bool HasOutputs => true;

      public override string ResultName => "GetNodesXmlResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetNodesXml";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.XmlNodeFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "nodeUris", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "childNodes", parameter2);
      }
    }

    internal sealed class GetProjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetProject";

      public override bool HasOutputs => true;

      public override string ResultName => "GetProjectResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetProject";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ProjectInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class GetProjectFromNameClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetProjectFromName";

      public override bool HasOutputs => true;

      public override string ResultName => "GetProjectFromNameResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetProjectFromName";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) ProjectInfo.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter);
      }
    }

    internal sealed class GetProjectPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetProjectProperties";

      public override bool HasOutputs => true;

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/GetProjectProperties";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = new object[4];
        outputs[0] = (object) null;
        outputs[1] = (object) null;
        outputs[2] = (object) 0;
        outputs[3] = (object) Helper.ZeroLengthArrayOfProjectProperty;
        return (object) null;
      }

      public override void ReadOutput(
        IServiceProvider serviceProvider,
        XmlReader reader,
        object[] outputs)
      {
        switch (reader.Name)
        {
          case "name":
            outputs[0] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "state":
            outputs[1] = (object) XmlUtility.StringFromXmlElement(reader);
            break;
          case "templateId":
            outputs[2] = (object) XmlUtility.Int32FromXmlElement(reader);
            break;
          case "properties":
            outputs[3] = (object) Helper.ArrayOfProjectPropertyFromXml(serviceProvider, reader, false);
            break;
          default:
            base.ReadOutput(serviceProvider, reader, outputs);
            break;
        }
      }

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class ListAllProjectsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListAllProjects";

      public override bool HasOutputs => true;

      public override string ResultName => "ListAllProjectsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/ListAllProjects";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfProjectInfo;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfProjectInfoFromXml(serviceProvider, reader, false);
    }

    internal sealed class ListProjectsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListProjects";

      public override bool HasOutputs => true;

      public override string ResultName => "ListProjectsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/ListProjects";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfProjectInfo;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfProjectInfoFromXml(serviceProvider, reader, false);
    }

    internal sealed class ListStructuresClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListStructures";

      public override bool HasOutputs => true;

      public override string ResultName => "ListStructuresResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/ListStructures";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfNodeInfo;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfNodeInfoFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class MoveBranchClientOperation : TfsClientOperation
    {
      public override string BodyName => "MoveBranch";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/MoveBranch";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "newParentNodeUri", parameter2);
      }
    }

    internal sealed class RenameNodeClientOperation : TfsClientOperation
    {
      public override string BodyName => "RenameNode";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/RenameNode";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "newNodeName", parameter2);
      }
    }

    internal sealed class ReorderNodeClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReorderNode";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/ReorderNode";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "nodeUri", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "moveBy", parameter2);
      }
    }

    internal sealed class UpdateProjectPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateProjectProperties";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03/UpdateProjectProperties";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "state", parameter2);
        ProjectProperty[] parameter3 = (ProjectProperty[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "properties", parameter3, false, false);
      }
    }
  }
}
