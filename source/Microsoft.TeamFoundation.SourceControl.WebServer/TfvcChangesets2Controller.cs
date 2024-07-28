// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesets2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "tfvc", ResourceName = "Changesets", ResourceVersion = 2)]
  public class TfvcChangesets2Controller : TfvcChangesetsController
  {
    private const int MaxPostRequestSize = 26214400;

    [ClientExample("POST__tfvc_changesets_CreateChange.json", "POST an add text file changeset.", null, null)]
    [ClientExample("POST__tfvc_changesets_CreateChange_Binary.json", "POST an add binary file changeset.", null, null)]
    [ClientExample("POST__tfvc_changesets_CreateChangeMultiFile.json", "POST an add multiple file changeset.", null, null)]
    [ClientExample("POST__tfvc_changesets_EditExistingFile.json", "POST a file edit changeset.", null, null)]
    [HttpPost]
    [ClientResponseType(typeof (TfvcChangesetRef), null, null)]
    [ClientRequestBodyType(typeof (TfvcChangeset), "changeset")]
    public HttpResponseMessage CreateChangeset()
    {
      if (this.Request != null)
      {
        if (this.Request.Content != null)
        {
          TfvcChangeset changesetToCreate;
          try
          {
            using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 26214401L, true))
            {
              try
              {
                changesetToCreate = JsonConvert.DeserializeObject<TfvcChangeset>(new StreamReader((Stream) restrictedStream).ReadToEnd());
                if (this.Request.Headers.Referrer != (Uri) null)
                {
                  if (!GetWebEditingEnabled())
                    throw new WebEditingDisabledException(Resources.Get("WebEditDisabled"));
                }
              }
              catch (JsonException ex)
              {
                this.TfsRequestContext.TraceException(700001, TraceLevel.Warning, this.TraceArea, WebApiTraceLayers.Controller, (Exception) ex);
                if (restrictedStream.Position >= 26214400L)
                  throw new ArgumentException(Resources.Format("RequestMaxSizeExceeded", (object) 26214400)).Expected(this.TfsRequestContext.ServiceName);
                throw new ArgumentException(Resources.Get("InvalidJsonBody")).Expected(this.TfsRequestContext.ServiceName);
              }
            }
          }
          catch (InvalidOperationException ex)
          {
            this.TfsRequestContext.TraceException(700302, TraceLevel.Warning, this.TraceArea, WebApiTraceLayers.Controller, (Exception) ex);
            throw new InvalidArgumentValueException(Resources.Get("InvalidJsonBody"));
          }
          if (changesetToCreate != null && changesetToCreate.ChangesetId == 0 && changesetToCreate.Author == null)
          {
            if (changesetToCreate.CheckedInBy == null)
            {
              try
              {
                int id = TfvcWorkspaceUtil.Checkin(this.TfsRequestContext, changesetToCreate);
                if (id <= 0)
                  throw new InvalidArgumentValueException(Resources.Get("CheckinFailedError"));
                return this.Request.CreateResponse<TfvcChangesetRef>(HttpStatusCode.Created, (TfvcChangesetRef) TfvcChangesetUtility.GetChangesetById(this.TfsRequestContext, this.Url, id, 0, false, true, false, 2000));
              }
              catch (CheckInGatedBuildException ex)
              {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                  IsGatedCheckin = true,
                  AffectedBuildDefinitionUris = ex.AffectedBuildDefinitionUris,
                  AffectedBuildDefinitionNames = ex.AffectedBuildDefinitionNames,
                  ShelvesetName = ex.ShelvesetName,
                  BuildId = ex.BuildId,
                  BuildUri = ex.BuildUri
                });
              }
              catch (ItemExistsException ex)
              {
                throw new InvalidArgumentValueException(Resources.Format("ItemExistsException", (object) ex.ServerItem));
              }
              catch (TeamProjectNotFoundException ex)
              {
                throw new InvalidArgumentValueException(Resources.Format("TeamProjectDoesNotExist", (object) ex.Project));
              }
            }
          }
          throw new ArgumentException(Resources.Get("InvalidParameters")).Expected(this.TfsRequestContext.ServiceName);

          bool GetWebEditingEnabled()
          {
            bool webEditingEnabled = true;
            try
            {
              string[] strArray = changesetToCreate.Changes.FirstOrDefault<TfvcChange>().Item.Path.Split('/');
              if (strArray.Length < 2)
                return true;
              Guid id = TfsProjectHelpers.GetProjectFromName(this.TfsRequestContext, strArray[1]).Id;
              webEditingEnabled = this.TfsRequestContext.GetService<ISettingsService>().GetValue<bool>(this.TfsRequestContext, SettingsUserScope.Parse("host"), "project", id.ToString(), Resources.Get("WebEditEnabledSettingPath"), true);
            }
            catch (Exception ex)
            {
              this.TfsRequestContext.TraceException(700303, TraceLevel.Warning, this.TraceArea, WebApiTraceLayers.Controller, ex);
            }
            return webEditingEnabled;
          }
        }
      }
      throw new ArgumentException(Resources.Format("ChangesetToBeCreatedNotSpecified"));
    }
  }
}
