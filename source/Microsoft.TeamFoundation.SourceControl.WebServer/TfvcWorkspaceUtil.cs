// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcWorkspaceUtil
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class TfvcWorkspaceUtil
  {
    private const int c_codePageUtf8 = 65001;
    private const string c_affectedBuildDefinitionUriKey = "AffectedBuildDefinitionUris";

    public static void CreateShelveset(IVssRequestContext requestContext, TfvcShelveset shelveset)
    {
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      Workspace workspace = (Workspace) null;
      try
      {
        if (shelveset == null || shelveset.Changes == null || !shelveset.Changes.Any<TfvcChange>())
          throw new ArgumentException(Resources.Get("InvalidPathActionsArgument"), "changes").Expected(requestContext.ServiceName);
        List<TfvcWorkspaceUtil.PathActionContext> actionContexts;
        workspace = TfvcWorkspaceUtil.PendChanges(requestContext, service, shelveset.Changes, out actionContexts);
        Microsoft.TeamFoundation.VersionControl.Server.CheckinNote checkinNote = (Microsoft.TeamFoundation.VersionControl.Server.CheckinNote) null;
        if (shelveset.Notes != null && shelveset.Notes.Length != 0)
        {
          checkinNote = new Microsoft.TeamFoundation.VersionControl.Server.CheckinNote()
          {
            Values = new CheckinNoteFieldValue[shelveset.Notes.Length]
          };
          for (int index = 0; index < shelveset.Notes.Length; ++index)
            checkinNote.Values[index] = new CheckinNoteFieldValue()
            {
              Name = shelveset.Notes[index].Name,
              Value = shelveset.Notes[index].Value
            };
        }
        VersionControlLink[] versionControlLinkArray = (VersionControlLink[]) null;
        if (shelveset.WorkItems != null)
        {
          List<VersionControlLink> versionControlLinkList = new List<VersionControlLink>();
          foreach (AssociatedWorkItem workItem in shelveset.WorkItems)
          {
            string str = LinkingUtilities.EncodeUri(new ArtifactId()
            {
              Tool = "WorkItemTracking",
              ArtifactType = "WorkItem",
              ToolSpecificId = workItem.Id.ToString()
            });
            versionControlLinkList.Add(new VersionControlLink()
            {
              LinkType = 1026,
              Url = str
            });
          }
          versionControlLinkArray = versionControlLinkList.ToArray();
        }
        Shelveset shelveset1 = new Shelveset()
        {
          CheckinNote = checkinNote,
          Comment = shelveset.Comment,
          Links = versionControlLinkArray,
          Name = shelveset.Name,
          Owner = shelveset.Owner.Id,
          PolicyOverrideComment = shelveset.PolicyOverride.Comment
        };
        List<Failure> failureList = service.Shelve(requestContext, workspace.Name, workspace.OwnerName, actionContexts.Select<TfvcWorkspaceUtil.PathActionContext, string>((Func<TfvcWorkspaceUtil.PathActionContext, string>) (a => a.TargetServerPath)).ToArray<string>(), shelveset1, false);
        TfvcWorkspaceUtil.HandleFailures(requestContext, (IEnumerable<Failure>) failureList);
      }
      finally
      {
        if (workspace != null)
          service.DeleteWorkspace(requestContext, workspace.Name, workspace.OwnerName);
      }
    }

    public static int Checkin(IVssRequestContext requestContext, TfvcChangeset changeset)
    {
      requestContext.TraceEnter(700002, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (Checkin));
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      Workspace workspace = (Workspace) null;
      try
      {
        if (changeset == null || changeset.Changes == null || !changeset.Changes.Any<TfvcChange>())
          throw new ArgumentException(Resources.Get("InvalidPathActionsArgument"), "changes").Expected(requestContext.ServiceName);
        List<TfvcWorkspaceUtil.PathActionContext> actionContexts;
        workspace = TfvcWorkspaceUtil.PendChanges(requestContext, service, changeset.Changes, out actionContexts);
        requestContext.Trace(700007, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Checking in changes in temporary web checkin workspace: {0}", (object) workspace.Name);
        Changeset info = new Changeset()
        {
          Comment = changeset.Comment
        };
        using (TeamFoundationDataReader foundationDataReader = service.CheckIn(requestContext, workspace.Name, workspace.OwnerName, actionContexts.Select<TfvcWorkspaceUtil.PathActionContext, string>((Func<TfvcWorkspaceUtil.PathActionContext, string>) (a => a.TargetServerPath)).ToArray<string>(), info, (CheckinNotificationInfo) null, 17, false, 0, true, PathLength.Unspecified))
        {
          CheckinResult checkinResult = foundationDataReader.Current<CheckinResult>();
          foundationDataReader.MoveNext();
          IEnumerable<Failure> failures1 = foundationDataReader.CurrentEnumerable<Failure>();
          TfvcWorkspaceUtil.HandleFailures(requestContext, failures1);
          foundationDataReader.MoveNext();
          IEnumerable<Failure> failures2 = foundationDataReader.CurrentEnumerable<Failure>();
          TfvcWorkspaceUtil.HandleFailures(requestContext, failures2);
          foreach (string undoneServerItem in checkinResult.UndoneServerItems)
            ;
          return checkinResult.ChangesetId;
        }
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        if (ex.PropertyCollection.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => kvp.Key == "AffectedBuildDefinitionUris")))
          throw new CheckInGatedBuildException(requestContext, ex);
        throw;
      }
      finally
      {
        if (workspace != null)
          service.DeleteWorkspace(requestContext, workspace.Name, workspace.OwnerName);
        requestContext.TraceLeave(700008, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (Checkin));
      }
    }

    private static Workspace PendChanges(
      IVssRequestContext requestContext,
      TeamFoundationVersionControlService vc,
      IEnumerable<TfvcChange> changes,
      out List<TfvcWorkspaceUtil.PathActionContext> actionContexts)
    {
      string str1 = Guid.NewGuid().ToString();
      string str2 = "Z:\\item" + str1;
      actionContexts = new List<TfvcWorkspaceUtil.PathActionContext>();
      foreach (TfvcChange change in changes)
      {
        if (change == null || change.Item == null || string.IsNullOrWhiteSpace(change.Item.Path))
          throw new ArgumentException(Resources.Get("InvalidPathActionsArgument"), "change.item.path").Expected(requestContext.ServiceName);
        if (change.Item.IsFolder)
        {
          if (change.NewContent != null && !string.IsNullOrWhiteSpace(change.NewContent.Content))
            throw new ArgumentException(Resources.Get("NonEmptyFoldersNotSupported")).Expected(requestContext.ServiceName);
          if (change.ChangeType == VersionControlChangeType.Edit)
            throw new ArgumentException(Resources.Get("CannotEditFolder")).Expected(requestContext.ServiceName);
        }
        VersionControlPath.ValidatePath(change.Item.Path);
        string str3 = change.Item.Path;
        if (change.ChangeType == VersionControlChangeType.Rename)
        {
          if (string.IsNullOrWhiteSpace(change.SourceServerItem) || string.Equals(change.SourceServerItem, change.Item.Path))
            throw new ArgumentException(Resources.Get("InvalidPathActionsArgument"), "change.SourceServerItem").Expected(requestContext.ServiceName);
          VersionControlPath.ValidatePath(change.SourceServerItem);
          str3 = change.SourceServerItem;
        }
        else if (!string.IsNullOrEmpty(change.SourceServerItem))
          throw new ArgumentException(Resources.Get("IllegalSourceServerItem")).Expected(requestContext.ServiceName);
        TfvcWorkspaceUtil.PathActionContext pathActionContext = new TfvcWorkspaceUtil.PathActionContext()
        {
          Change = change,
          ChangeRequest = new ChangeRequest()
          {
            ItemSpec = new ItemSpec(str3, RecursionType.None),
            ItemType = change.Item.IsFolder ? ItemType.Folder : ItemType.File
          },
          LocalPath = Path.Combine(str2, VersionControlPath.MakeRelative(str3, "$/").Replace('/', Path.DirectorySeparatorChar)),
          TargetServerPath = change.Item.Path
        };
        actionContexts.Add(pathActionContext);
        bool allowBinary = change.NewContent != null && change.NewContent.ContentType != 0;
        switch (change.ChangeType)
        {
          case VersionControlChangeType.Add:
            pathActionContext.ChangeRequest.RequestType = RequestType.Add;
            if (pathActionContext.ChangeRequest.ItemType == ItemType.File)
            {
              pathActionContext.ChangeRequest.Encoding = TfvcWorkspaceUtil.GetSafeEncoding(change.Item, false, allowBinary);
              pathActionContext.RequiresUpload = true;
              break;
            }
            if (pathActionContext.ChangeRequest.ItemType == ItemType.Folder)
            {
              pathActionContext.ChangeRequest.Encoding = -3;
              break;
            }
            break;
          case VersionControlChangeType.Edit:
            bool allowUnchanged = change.NewContent != null && change.NewContent.ContentType == ItemContentType.Base64Encoded;
            pathActionContext.ChangeRequest.RequestType = RequestType.Edit;
            pathActionContext.ChangeRequest.Encoding = TfvcWorkspaceUtil.GetSafeEncoding(change.Item, allowUnchanged, allowBinary);
            if (change.NewContent != null && change.NewContent.ContentType == ItemContentType.RawText)
            {
              if (change.Item.ContentMetadata == null)
                change.Item.ContentMetadata = new FileContentMetadata();
              change.Item.ContentMetadata.Encoding = pathActionContext.ChangeRequest.Encoding;
              TfvcItem file = TfvcItemUtility.GetItem(requestContext, (UrlHelper) null, change.Item.Path, (VersionSpec) new LatestVersionSpec(), DeletedState.NonDeleted);
              if (pathActionContext.ChangeRequest.Encoding == -2)
                change.Item.ContentMetadata.Encoding = file.Encoding;
              else if (file.Encoding == pathActionContext.ChangeRequest.Encoding)
                pathActionContext.ChangeRequest.Encoding = -2;
              if (change.Item.ContentMetadata.Encoding == 65001)
              {
                bool containsByteOrderMark = false;
                TfvcFileUtility.TryDetectFileEncoding(requestContext, (ItemModel) file, 65001, 0L, out containsByteOrderMark);
                change.Item.ContentMetadata.EncodingWithBom = containsByteOrderMark;
              }
            }
            pathActionContext.RequiresGet = true;
            pathActionContext.RequiresUpload = true;
            break;
          case VersionControlChangeType.Rename:
            pathActionContext.ChangeRequest.RequestType = RequestType.Rename;
            pathActionContext.ChangeRequest.TargetItem = change.Item.Path;
            pathActionContext.RequiresGet = true;
            break;
          case VersionControlChangeType.Delete:
            pathActionContext.ChangeRequest.RequestType = RequestType.Delete;
            pathActionContext.RequiresGet = true;
            break;
          default:
            throw new ArgumentException(Resources.Format("UnsupportedChangeType", (object) change.ChangeType.ToString())).Expected(requestContext.ServiceName);
        }
        if (pathActionContext.RequiresGet && change.Item.ChangesetVersion <= 0)
          throw new ArgumentException(Resources.Format("IllegalChangeOnItemThatDoesNotExist", (object) change.ChangeType.ToString())).Expected(requestContext.ServiceName);
      }
      Workspace workspace1 = new Workspace()
      {
        Comment = Resources.Get("TempWorkspaceComment"),
        Computer = ("TEMP-" + str1).Remove(31),
        Folders = new List<WorkingFolder>()
        {
          new WorkingFolder("$/", str2, WorkingFolderType.Map, 120)
        },
        IsLocal = false,
        Name = "TMP-TfsWebCheckin-" + str1,
        Options = 0,
        OwnerName = requestContext.AuthenticatedUserName
      };
      requestContext.Trace(700003, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Creating temporary web checkin workspace: {0}", (object) workspace1.Name);
      Workspace workspace2 = vc.CreateWorkspace(requestContext, workspace1);
      try
      {
        List<TfvcWorkspaceUtil.PathActionContext> list = actionContexts.Where<TfvcWorkspaceUtil.PathActionContext>((Func<TfvcWorkspaceUtil.PathActionContext, bool>) (a => a.RequiresGet)).ToList<TfvcWorkspaceUtil.PathActionContext>();
        if (list.Count > 0)
        {
          requestContext.Trace(700004, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Performing fake Get of {0} items in temporary web checkin workspace: {1}", (object) list.Count, (object) workspace2.Name);
          ServerItemLocalVersionUpdate[] array1 = list.Select<TfvcWorkspaceUtil.PathActionContext, ServerItemLocalVersionUpdate>((Func<TfvcWorkspaceUtil.PathActionContext, ServerItemLocalVersionUpdate>) (actionContext =>
          {
            return new ServerItemLocalVersionUpdate()
            {
              SourceItemPathPair = actionContext.ChangeRequest.ItemSpec.ItemPathPair,
              LocalVersion = actionContext.Change.Item.ChangesetVersion,
              TargetLocalItem = actionContext.LocalPath
            };
          })).ToArray<ServerItemLocalVersionUpdate>();
          vc.UpdateLocalVersion(requestContext, workspace2.Name, workspace2.OwnerName, (BaseLocalVersionUpdate[]) array1, PathLength.Length399);
          ItemSpec[] array2 = list.Select<TfvcWorkspaceUtil.PathActionContext, ItemSpec>((Func<TfvcWorkspaceUtil.PathActionContext, ItemSpec>) (actionContext => new ItemSpec(actionContext.LocalPath, RecursionType.None))).ToArray<ItemSpec>();
          using (TeamFoundationDataReader foundationDataReader = vc.QueryLocalVersions(requestContext, workspace2.Name, workspace2.OwnerName, array2))
          {
            int index = 0;
            foreach (IEnumerable<LocalVersion> current in foundationDataReader.CurrentEnumerable<StreamingCollection<LocalVersion>>())
            {
              LocalVersion localVersion = current.FirstOrDefault<LocalVersion>();
              if (localVersion == null || !string.Equals(localVersion.Item, list[index].LocalPath) || localVersion.Version != list[index].Change.Item.ChangesetVersion)
                throw new ItemNotFoundException(list[index].ChangeRequest.ItemSpec.Item);
              ++index;
            }
          }
        }
        requestContext.Trace(700005, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Pending {0} operations in temporary web checkin workspace: {1}", (object) actionContexts.Count, (object) workspace2.Name);
        foreach (IGrouping<RequestType, ChangeRequest> source in actionContexts.Select<TfvcWorkspaceUtil.PathActionContext, ChangeRequest>((Func<TfvcWorkspaceUtil.PathActionContext, ChangeRequest>) (a => a.ChangeRequest)).GroupBy<ChangeRequest, RequestType>((Func<ChangeRequest, RequestType>) (c => c.RequestType)))
        {
          using (TeamFoundationDataReader foundationDataReader = vc.PendChanges(requestContext, workspace2.Name, workspace2.OwnerName, source.ToArray<ChangeRequest>(), 0, 3967, (string[]) null, (string[]) null, false, PathLength.Unspecified))
          {
            foundationDataReader.MoveNext();
            IEnumerable<Failure> failures = foundationDataReader.CurrentEnumerable<Failure>();
            TfvcWorkspaceUtil.HandleFailures(requestContext, failures);
          }
        }
        foreach (TfvcWorkspaceUtil.PathActionContext pathActionContext in actionContexts.Where<TfvcWorkspaceUtil.PathActionContext>((Func<TfvcWorkspaceUtil.PathActionContext, bool>) (a => a.RequiresUpload)))
        {
          MemoryStream fileStream = (MemoryStream) null;
          try
          {
            if (pathActionContext.Change.NewContent != null && pathActionContext.Change.NewContent.Content != null)
            {
              if (pathActionContext.Change.NewContent.ContentType == ItemContentType.Base64Encoded)
              {
                try
                {
                  fileStream = new MemoryStream(Convert.FromBase64String(pathActionContext.Change.NewContent.Content), false);
                  goto label_85;
                }
                catch (FormatException ex)
                {
                  throw new ArgumentException(ex.Message, "base64Content").Expected(requestContext.ServiceName);
                }
              }
            }
            if (pathActionContext.Change.NewContent == null || pathActionContext.Change.NewContent.Content == null || pathActionContext.Change.NewContent.ContentType != ItemContentType.RawText)
              throw new ArgumentException(Resources.Get("InvalidParameters")).Expected(requestContext.ServiceName);
            int encoding1 = pathActionContext.ChangeRequest.Encoding;
            if (encoding1 == -2)
              encoding1 = pathActionContext.Change.Item.ContentMetadata.Encoding;
            Encoding encoding2;
            if (encoding1 == 65001)
            {
              FileContentMetadata contentMetadata = pathActionContext.Change.Item.ContentMetadata;
              encoding2 = (Encoding) new UTF8Encoding(contentMetadata != null && contentMetadata.EncodingWithBom, true);
            }
            else
              encoding2 = Encoding.GetEncoding(encoding1, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            fileStream = new MemoryStream(pathActionContext.Change.NewContent.Content.Length);
            using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, encoding2, 1024, true))
              streamWriter.Write(pathActionContext.Change.NewContent.Content);
            fileStream.Position = 0L;
label_85:
            byte[] md5 = MD5Util.CalculateMD5((Stream) fileStream);
            fileStream.Position = 0L;
            requestContext.Trace(700006, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Uploading content with hash {0} for {1} in temporary web checkin workspace: {2}", (object) md5, (object) pathActionContext.TargetServerPath, (object) workspace2.Name);
            vc.UploadFile(requestContext, workspace2.Name, workspace2.OwnerName, pathActionContext.TargetServerPath, md5, (Stream) fileStream, fileStream.Length, fileStream.Length, 0L, CompressionType.None);
          }
          finally
          {
            fileStream?.Dispose();
          }
        }
      }
      catch (Exception ex)
      {
        if (workspace2 != null)
          vc.DeleteWorkspace(requestContext, workspace2.Name, workspace2.OwnerName);
        throw;
      }
      return workspace2;
    }

    internal static int GetSafeEncoding(TfvcItem item, bool allowUnchanged, bool allowBinary)
    {
      int codepage = 0;
      if (item.ContentMetadata != null)
        codepage = item.ContentMetadata.Encoding;
      if (codepage == 0)
        codepage = -2;
      if (codepage == -2)
      {
        if (!allowUnchanged)
          throw new ArgumentException(Resources.Format("PathActionInvalidUnchangedEncoding"), "pathActions.action.encoding").Expected("tfvc");
      }
      else if (codepage == -1)
      {
        if (!allowBinary)
          throw new ArgumentException(Resources.Format("PathActionInvalidBinaryEncoding"), "pathActions.action.encoding").Expected("tfvc");
      }
      else
      {
        try
        {
          Encoding.GetEncoding(codepage);
        }
        catch (Exception ex)
        {
          throw new ArgumentException(Resources.Format("PathActionInvalidEncodingValue", (object) codepage), "pathActions.action.encoding").Expected("tfvc");
        }
      }
      return codepage;
    }

    private static void HandleFailures(
      IVssRequestContext requestContext,
      IEnumerable<Failure> failures)
    {
      if (failures == null)
        return;
      foreach (Failure failure in failures)
      {
        if (failure.Severity == SeverityType.Warning)
        {
          requestContext.Trace(700009, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Ignored warning from VC command: {0}", (object) failure.Message);
        }
        else
        {
          if (failure.Exception != null)
            throw failure.Exception;
          if (failure.Code == "ItemNotFoundException")
            throw new ItemNotFoundException(failure.ServerItem);
          throw new Exception(failure.Message);
        }
      }
    }

    private class PathActionContext
    {
      public TfvcChange Change { get; set; }

      public ChangeRequest ChangeRequest { get; set; }

      public string LocalPath { get; set; }

      public string TargetServerPath { get; set; }

      public bool RequiresGet { get; set; }

      public bool RequiresUpload { get; set; }
    }
  }
}
