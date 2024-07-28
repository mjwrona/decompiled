// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.ArtifactLinkTypesService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Linking
{
  public class ArtifactLinkTypesService : IArtifactLinkTypesService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Dictionary<string, List<RegistrationArtifactType>> GetArtifactLinkTypes(
      IVssRequestContext requestContext)
    {
      Dictionary<string, List<RegistrationArtifactType>> artifactLinkTypes = new Dictionary<string, List<RegistrationArtifactType>>((IEqualityComparer<string>) VssStringComparer.ToolId);
      List<OutboundLinkType> hardcodedLinkTypes = this.GetHardcodedLinkTypes();
      Dictionary<string, Dictionary<string, List<OutboundLinkType>>> dictionary1 = new Dictionary<string, Dictionary<string, List<OutboundLinkType>>>((IEqualityComparer<string>) VssStringComparer.ArtifactType);
      foreach (OutboundLinkType outboundLinkType in hardcodedLinkTypes)
      {
        Dictionary<string, List<OutboundLinkType>> dictionary2;
        if (!dictionary1.TryGetValue(outboundLinkType.ToolType, out dictionary2))
        {
          dictionary2 = new Dictionary<string, List<OutboundLinkType>>((IEqualityComparer<string>) VssStringComparer.ArtifactTool);
          dictionary1[outboundLinkType.ToolType] = dictionary2;
        }
        List<OutboundLinkType> outboundLinkTypeList;
        if (!dictionary2.TryGetValue(outboundLinkType.ArtifactName, out outboundLinkTypeList))
        {
          outboundLinkTypeList = new List<OutboundLinkType>();
          dictionary2[outboundLinkType.ArtifactName] = outboundLinkTypeList;
        }
        outboundLinkTypeList.Add(outboundLinkType);
      }
      foreach (KeyValuePair<string, Dictionary<string, List<OutboundLinkType>>> keyValuePair1 in dictionary1)
      {
        List<RegistrationArtifactType> registrationArtifactTypeList;
        if (!artifactLinkTypes.TryGetValue(keyValuePair1.Key, out registrationArtifactTypeList))
        {
          registrationArtifactTypeList = new List<RegistrationArtifactType>();
          artifactLinkTypes[keyValuePair1.Key] = registrationArtifactTypeList;
        }
        foreach (KeyValuePair<string, List<OutboundLinkType>> keyValuePair2 in keyValuePair1.Value)
          registrationArtifactTypeList.Add(new RegistrationArtifactType()
          {
            Name = keyValuePair2.Key,
            OutboundLinkTypes = keyValuePair2.Value.Count != 1 || !VssStringComparer.LinkName.Equals(keyValuePair2.Value[0].Name, ArtifactLinkIds.NoOutboundLink) ? keyValuePair2.Value.ToArray() : Array.Empty<OutboundLinkType>()
          });
      }
      return artifactLinkTypes;
    }

    public IReadOnlyCollection<RegistrationArtifactType> GetArtifactLinkTypes(
      IVssRequestContext requestContext,
      string toolId)
    {
      List<RegistrationArtifactType> artifactLinkTypes;
      if (this.GetArtifactLinkTypes(requestContext).TryGetValue(toolId, out artifactLinkTypes))
        return (IReadOnlyCollection<RegistrationArtifactType>) artifactLinkTypes;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ToolId", toolId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ArtifactLinkTypesService), "GetArtifactLinkTypes_NoLinkTypesFound", properties);
      return (IReadOnlyCollection<RegistrationArtifactType>) new List<RegistrationArtifactType>(0);
    }

    private List<OutboundLinkType> GetHardcodedLinkTypes() => new List<OutboundLinkType>()
    {
      new OutboundLinkType("ArchitectureTools", "ModelLink")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("WorkItemTracking", "HyperLink")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Branch",
        TargetArtifactTypeTool = "Git",
        TargetArtifactTypeName = "Branch"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Build",
        TargetArtifactTypeTool = "Build",
        TargetArtifactTypeName = "Build"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Fixed in Changeset",
        TargetArtifactTypeTool = "VersionControl",
        TargetArtifactTypeName = "Changeset"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Fixed in Commit",
        TargetArtifactTypeTool = "Git",
        TargetArtifactTypeName = "Commit"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Found in build",
        TargetArtifactTypeTool = "Build",
        TargetArtifactTypeName = "Build"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "GitHub Commit",
        TargetArtifactTypeTool = "GitHub",
        TargetArtifactTypeName = "Commit"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "GitHub Issue",
        TargetArtifactTypeTool = "GitHub",
        TargetArtifactTypeName = "Issue"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "GitHub Pull Request",
        TargetArtifactTypeTool = "GitHub",
        TargetArtifactTypeName = "PullRequest"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Integrated in build",
        TargetArtifactTypeTool = "Build",
        TargetArtifactTypeName = "Build"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Integrated in release environment",
        TargetArtifactTypeTool = "ReleaseManagement",
        TargetArtifactTypeName = "ReleaseEnvironment"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Model Link",
        TargetArtifactTypeTool = "ArchitectureTools",
        TargetArtifactTypeName = "ModelLink"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Pull Request",
        TargetArtifactTypeTool = "Git",
        TargetArtifactTypeName = "PullRequestId"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Related Workitem",
        TargetArtifactTypeTool = "WorkItemTracking",
        TargetArtifactTypeName = "Workitem"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Result Attachment",
        TargetArtifactTypeTool = "TestManagement",
        TargetArtifactTypeName = "TcmResultAttachment"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Source Code File",
        TargetArtifactTypeTool = "VersionControl",
        TargetArtifactTypeName = "LatestItemVersion"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Storyboard",
        TargetArtifactTypeTool = "Requirements",
        TargetArtifactTypeName = "Storyboard"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Tag",
        TargetArtifactTypeTool = "Git",
        TargetArtifactTypeName = "Tag"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Test",
        TargetArtifactTypeTool = "TestManagement",
        TargetArtifactTypeName = "TcmTest"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Test Result",
        TargetArtifactTypeTool = "TestManagement",
        TargetArtifactTypeName = "TcmResult"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Wiki Page",
        TargetArtifactTypeTool = "Wiki",
        TargetArtifactTypeName = "WikiPage"
      },
      new OutboundLinkType("WorkItemTracking", "WorkItem")
      {
        Name = "Workitem Hyperlink",
        TargetArtifactTypeTool = "WorkItemTracking",
        TargetArtifactTypeName = "Hyperlink"
      },
      new OutboundLinkType("VersionControl", "Changeset")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("VersionControl", "Label")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("VersionControl", "LatestItemVersion")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("VersionControl", "ShelvedItem")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("VersionControl", "Shelveset")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("VersionControl", "VersionedItem")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Git", "Branch")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Git", "Commit")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Git", "PullRequestId")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Git", "Tag")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Build", "Build")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("GitHub", "Commit")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("GitHub", "PullRequest")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("TestManagement", "TcmResult")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("TestManagement", "TcmResultAttachment")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("TestManagement", "TcmTest")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Requirements", "Storyboard")
      {
        Name = "No Outbound Link"
      },
      new OutboundLinkType("Wiki", "WikiPage")
      {
        Name = "No Outbound Link"
      }
    };
  }
}
