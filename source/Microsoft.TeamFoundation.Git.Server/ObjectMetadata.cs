// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ObjectMetadata
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class ObjectMetadata : IObjectMetadata
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly OdbId m_odbId;
    private readonly IGitObjectSet m_objectSet;
    private readonly ITeamFoundationEventService m_eventService;

    public ObjectMetadata(
      IVssRequestContext requestContext,
      OdbId odbId,
      IGitObjectSet objectSet,
      ITeamFoundationEventService eventService)
    {
      this.m_requestContext = requestContext;
      this.m_odbId = odbId;
      this.m_objectSet = objectSet;
      this.m_eventService = eventService;
    }

    public List<ObjectIdAndSize> GetObjectSizes(
      IEnumerable<Sha1Id> objectIds,
      string eventFeatureSpace,
      bool continueOnError)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(objectIds, nameof (objectIds));
      HashSet<Sha1Id> source = new HashSet<Sha1Id>(objectIds);
      List<ObjectIdAndSize> objectSizes = new List<ObjectIdAndSize>();
      using (GitOdbComponent gitOdbComponent = this.m_requestContext.CreateGitOdbComponent(this.m_odbId))
      {
        foreach (ObjectIdAndSize readObjectLength in (IEnumerable<ObjectIdAndSize>) gitOdbComponent.ReadObjectLengths(objectIds))
        {
          objectSizes.Add(readObjectLength);
          source.Remove(readObjectLength.Id);
        }
      }
      if (source.Count == 0)
        return objectSizes;
      List<ObjectIdAndSize> objectIdAndSizeList = new List<ObjectIdAndSize>();
      foreach (Sha1Id sha1Id in source)
      {
        try
        {
          using (Stream content = this.m_objectSet.GetContent(sha1Id, out GitObjectType _))
          {
            ObjectIdAndSize objectIdAndSize = new ObjectIdAndSize(sha1Id, content.Length);
            objectIdAndSizeList.Add(objectIdAndSize);
          }
        }
        catch (GitObjectDoesNotExistException ex)
        {
          if (!continueOnError)
            throw;
        }
      }
      this.m_eventService.PublishNotification(this.m_requestContext, (object) new KnownObjectLengthsNotification(this.m_odbId, objectIdAndSizeList.Take<ObjectIdAndSize>(100000)));
      if (eventFeatureSpace != null)
        new ClientTraceData().PublishObjectMetadataGetSizesCtData(this.m_requestContext, this.m_odbId, source.Count<Sha1Id>(), eventFeatureSpace);
      objectSizes.AddRange((IEnumerable<ObjectIdAndSize>) objectIdAndSizeList);
      return objectSizes;
    }
  }
}
