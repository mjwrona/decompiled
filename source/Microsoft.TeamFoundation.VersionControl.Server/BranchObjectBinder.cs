// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchObjectBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class BranchObjectBinder : VersionControlObjectBinder<BranchObject>
  {
    protected SqlColumnBinder fullPath = new SqlColumnBinder("FullPath");
    protected SqlColumnBinder version = new SqlColumnBinder("Version");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder command = new SqlColumnBinder("Command");
    protected SqlColumnBinder description = new SqlColumnBinder("Description");
    protected SqlColumnBinder dateCreated = new SqlColumnBinder("DateCreated");
    protected SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    protected SqlColumnBinder relationships = new SqlColumnBinder("Relationships");
    protected SqlColumnBinder mappings = new SqlColumnBinder("Mappings");

    public BranchObjectBinder()
    {
    }

    public BranchObjectBinder(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected string GetNodeAttributeServerPath(string value) => DBPath.DatabaseToServerPath(value);

    protected override BranchObject Bind()
    {
      BranchObject branchObject = new BranchObject()
      {
        Properties = new BranchProperties(),
        DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader)
      };
      branchObject.Properties.Description = this.description.GetString((IDataReader) this.Reader, true);
      branchObject.Properties.OwnerId = this.ownerId.GetGuid((IDataReader) this.Reader);
      branchObject.Properties.RootItem = new ItemIdentifier();
      branchObject.Properties.RootItem.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader);
      branchObject.Properties.RootItem.ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader));
      branchObject.Properties.RootItem.ItemPathPair = this.GetPreDataspaceItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
      branchObject.Properties.RootItem.Version = (VersionSpec) new ChangesetVersionSpec(this.version.GetInt32((IDataReader) this.Reader));
      branchObject.ChildBranches = new List<ItemIdentifier>();
      branchObject.RelatedBranches = new List<ItemIdentifier>();
      string s1 = this.relationships.GetString((IDataReader) this.Reader, true);
      if (s1 != null)
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        settings.CheckCharacters = false;
        using (StringReader input = new StringReader(s1))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          {
            xmlDocument.Load(reader);
            foreach (XmlNode selectNode in xmlDocument.SelectNodes("/items/i"))
            {
              ItemIdentifier itemIdentifier = new ItemIdentifier();
              itemIdentifier.ItemPathPair = ItemPathPair.FromServerItem(this.GetNodeAttributeServerPath(selectNode.Attributes["f"].Value));
              itemIdentifier.Version = (VersionSpec) new ChangesetVersionSpec(int.Parse(selectNode.Attributes["v"].Value, (IFormatProvider) CultureInfo.InvariantCulture));
              itemIdentifier.DeletionId = int.Parse(selectNode.Attributes["d"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
              itemIdentifier.ChangeType = VersionControlSqlResourceComponent.GetChangeType(int.Parse(selectNode.Attributes["ct"].Value, (IFormatProvider) CultureInfo.InvariantCulture));
              switch (int.Parse(selectNode.Attributes["t"].Value, (IFormatProvider) CultureInfo.InvariantCulture))
              {
                case 1:
                  branchObject.Properties.ParentBranch = itemIdentifier;
                  continue;
                case 2:
                  branchObject.ChildBranches.Add(itemIdentifier);
                  continue;
                case 3:
                  branchObject.RelatedBranches.Add(itemIdentifier);
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      string s2 = this.mappings.GetString((IDataReader) this.Reader, true);
      branchObject.Properties.BranchMappings = new List<Mapping>();
      if (!string.IsNullOrEmpty(s2))
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(s2))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          {
            xmlDocument.Load(reader);
            foreach (XmlNode selectNode in xmlDocument.SelectNodes("/folders/f"))
              branchObject.Properties.BranchMappings.Add(new Mapping()
              {
                ItemPathPair = ItemPathPair.FromServerItem(this.GetNodeAttributeServerPath(selectNode.Attributes["s"].Value)),
                Type = int.Parse(selectNode.Attributes["m"].Value, (IFormatProvider) CultureInfo.InvariantCulture) != 0 ? WorkingFolderType.Map : WorkingFolderType.Cloak,
                Depth = int.Parse(selectNode.Attributes["d"].Value, (IFormatProvider) CultureInfo.InvariantCulture)
              });
          }
        }
      }
      return branchObject;
    }
  }
}
