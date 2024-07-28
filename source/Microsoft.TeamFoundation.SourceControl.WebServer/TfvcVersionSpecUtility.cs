// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcVersionSpecUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcVersionSpecUtility
  {
    public static VersionSpec GetVersionSpec(
      IVssRequestContext requestContext,
      TfvcVersionDescriptor versionDescriptor)
    {
      VersionSpec versionSpec = (VersionSpec) null;
      if (versionDescriptor == null)
        return versionSpec;
      if (versionDescriptor.VersionOption == TfvcVersionOption.UseRename)
      {
        if (versionDescriptor.VersionType != TfvcVersionType.MergeSource)
          throw new InvalidVersionException(Resources.Get("InvalidUseRenameError"));
        int result;
        if (int.TryParse(versionDescriptor.Version, out result))
          return (VersionSpec) new MergeSourceVersionSpec(result, true);
        throw new InvalidVersionException(Resources.Get("InvalidMergeSourceError"));
      }
      VersionSpec version;
      if (string.IsNullOrEmpty(versionDescriptor.Version))
      {
        if (versionDescriptor.VersionType != TfvcVersionType.Latest && versionDescriptor.VersionType != TfvcVersionType.Tip)
          throw new InvalidVersionException(Resources.Get("InvalidLatestVersion"));
        version = (VersionSpec) new LatestVersionSpec();
      }
      else
      {
        switch (versionDescriptor.VersionType)
        {
          case TfvcVersionType.None:
          case TfvcVersionType.Changeset:
            int result1;
            if (!int.TryParse(versionDescriptor.Version, out result1))
              throw new InvalidVersionException(Resources.Get("InvalidChangesetVersion"));
            version = (VersionSpec) new ChangesetVersionSpec(result1);
            break;
          case TfvcVersionType.Shelveset:
            string ownerIdentifier = (string) null;
            string[] strArray = versionDescriptor.Version.Split(new char[1]
            {
              ';'
            }, StringSplitOptions.None);
            string shelvesetName = strArray.Length != 0 && !string.IsNullOrEmpty(strArray[0]) && !string.IsNullOrEmpty(strArray[0].Trim()) ? strArray[0] : throw new InvalidVersionException(Resources.Get("InvalidShelvesetVersion"));
            if (strArray.Length == 1)
              ownerIdentifier = requestContext.AuthenticatedUserName;
            else
              TfvcVersionSpecUtility.TryParseOwnerId(requestContext, strArray[1], out ownerIdentifier);
            string shelvesetOwner = ownerIdentifier;
            version = (VersionSpec) new ShelvesetVersionSpec(shelvesetName, shelvesetOwner);
            break;
          case TfvcVersionType.Change:
            int result2;
            if (!int.TryParse(versionDescriptor.Version, out result2))
              throw new InvalidVersionException(Resources.Get("InvalidChangeVersion"));
            version = (VersionSpec) new ChangeVersionSpec(result2);
            break;
          case TfvcVersionType.Date:
            version = VersionSpec.ParseSingleSpec("D" + versionDescriptor.Version, "*");
            break;
          case TfvcVersionType.Tip:
            version = (VersionSpec) new TipVersionSpec(versionDescriptor.Version);
            break;
          case TfvcVersionType.MergeSource:
            int result3;
            if (!int.TryParse(versionDescriptor.Version, out result3))
              throw new InvalidVersionException(Resources.Get("InvalidMergeSourceError"));
            version = (VersionSpec) new MergeSourceVersionSpec(result3, false);
            break;
          default:
            version = VersionSpec.ParseSingleSpec(versionDescriptor.Version, "*");
            break;
        }
      }
      if (version != null && versionDescriptor.VersionOption == TfvcVersionOption.Previous)
        version = (VersionSpec) new PreviousVersionSpec(version);
      return version;
    }

    public static TfvcVersionDescriptor GetVersionDescriptorFromQueryParameters(
      HttpRequestMessage request)
    {
      IEnumerable<KeyValuePair<string, string>> queryNameValuePairs = request.GetQueryNameValuePairs();
      NameValueCollection nameValueCollection = new NameValueCollection();
      foreach (KeyValuePair<string, string> keyValuePair in queryNameValuePairs)
        nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value);
      TfvcVersionDescriptor fromQueryParameters = new TfvcVersionDescriptor();
      TfvcVersionOption result1;
      if (Enum.TryParse<TfvcVersionOption>(nameValueCollection.Get("versionOption"), out result1))
        fromQueryParameters.VersionOption = result1;
      TfvcVersionType result2;
      if (Enum.TryParse<TfvcVersionType>(nameValueCollection.Get("versionType"), out result2))
        fromQueryParameters.VersionType = result2;
      fromQueryParameters.Version = nameValueCollection.Get("version");
      return fromQueryParameters;
    }

    public static bool TryParseOwnerId(
      IVssRequestContext requestContext,
      string givenIdentifier,
      out string ownerIdentifier)
    {
      Guid result;
      if (Guid.TryParse(givenIdentifier, out result))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = IdentityHelper.FindIdentity(requestContext, result);
        ownerIdentifier = IdentityHelper.GetUniqueName(identity);
        return true;
      }
      ownerIdentifier = givenIdentifier;
      return false;
    }
  }
}
