// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryEntityMerger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class DirectoryEntityMerger
  {
    internal static IDirectoryEntity MergeEntities(IDirectoryEntity x, IDirectoryEntity y)
    {
      if (x == null)
        return y;
      if (y == null)
        return x;
      IDirectoryEntity preferred = x;
      IDirectoryEntity secondary = y;
      if ("vsd".Equals(y.LocalDirectory) && string.IsNullOrWhiteSpace(x.LocalDirectory))
      {
        preferred = y;
        secondary = x;
      }
      string entityId = preferred.EntityId ?? secondary.EntityId;
      string str = preferred.EntityType ?? secondary.EntityType;
      string originDirectory = preferred.OriginDirectory ?? secondary.OriginDirectory;
      string originId = preferred.OriginId ?? secondary.OriginId;
      string localDirectory = preferred.LocalDirectory ?? secondary.LocalDirectory;
      string localId = preferred.LocalId ?? secondary.LocalId;
      IDictionary<string, object> properties = DirectoryEntityMerger.MergeProperties(preferred, secondary);
      if ("User".Equals(str))
      {
        DirectoryUser directoryUser1 = DirectoryEntityBuilder.BuildEntity<DirectoryUser>(originDirectory, originId, entityId, localDirectory, localId, properties: properties);
        IDirectoryUser x1 = preferred as IDirectoryUser;
        IDirectoryUser y1 = secondary as IDirectoryUser;
        if (x1 == null && y1 == null)
          return (IDirectoryEntity) directoryUser1;
        if (y1 == null)
          y1 = (IDirectoryUser) directoryUser1;
        if (x1 == null)
          x1 = (IDirectoryUser) directoryUser1;
        directoryUser1.Active = x1.Active.HasValue ? x1.Active : y1.Active;
        directoryUser1.DisplayName = DirectoryEntityMerger.MergeDisplayName((IDirectoryEntity) x1, (IDirectoryEntity) y1);
        DirectoryUser directoryUser2 = directoryUser1;
        SubjectDescriptor? subjectDescriptor1;
        if (x1.SubjectDescriptor.HasValue)
        {
          SubjectDescriptor? subjectDescriptor2 = x1.SubjectDescriptor;
          SubjectDescriptor subjectDescriptor3 = new SubjectDescriptor();
          if ((subjectDescriptor2.HasValue ? (subjectDescriptor2.HasValue ? (subjectDescriptor2.GetValueOrDefault() != subjectDescriptor3 ? 1 : 0) : 0) : 1) != 0)
          {
            subjectDescriptor2 = x1.SubjectDescriptor;
            if (!subjectDescriptor2.Value.IsUnknownSubjectType())
            {
              subjectDescriptor1 = x1.SubjectDescriptor;
              goto label_18;
            }
          }
        }
        subjectDescriptor1 = y1.SubjectDescriptor;
label_18:
        directoryUser2.SubjectDescriptor = subjectDescriptor1;
        directoryUser1.Department = x1.Department ?? y1.Department;
        directoryUser1.Guest = x1.Guest.HasValue ? x1.Guest : y1.Guest;
        directoryUser1.JobTitle = x1.JobTitle ?? y1.JobTitle;
        directoryUser1.Mail = x1.Mail ?? y1.Mail;
        directoryUser1.MailNickname = x1.MailNickname ?? y1.MailNickname;
        directoryUser1.PhysicalDeliveryOfficeName = x1.PhysicalDeliveryOfficeName ?? y1.PhysicalDeliveryOfficeName;
        directoryUser1.ScopeName = DirectoryEntityMerger.MergeScopeName((IDirectoryEntity) x1, (IDirectoryEntity) y1);
        directoryUser1.SignInAddress = x1.SignInAddress ?? y1.SignInAddress;
        directoryUser1.Surname = x1.Surname ?? y1.Surname;
        return (IDirectoryEntity) directoryUser1;
      }
      if ("Group".Equals(str))
      {
        DirectoryGroup directoryGroup1 = DirectoryEntityBuilder.BuildEntity<DirectoryGroup>(originDirectory, originId, entityId, localDirectory, localId, properties: properties);
        IDirectoryGroup x2 = (IDirectoryGroup) preferred;
        IDirectoryGroup y2 = (IDirectoryGroup) secondary;
        if (x2 == null && y2 == null)
          return (IDirectoryEntity) directoryGroup1;
        if (y2 == null)
          y2 = (IDirectoryGroup) directoryGroup1;
        if (x2 == null)
          x2 = (IDirectoryGroup) directoryGroup1;
        directoryGroup1.Active = x2.Active.HasValue ? x2.Active : y2.Active;
        directoryGroup1.DisplayName = DirectoryEntityMerger.MergeDisplayName((IDirectoryEntity) x2, (IDirectoryEntity) y2);
        DirectoryGroup directoryGroup2 = directoryGroup1;
        SubjectDescriptor? subjectDescriptor4;
        if (x2.SubjectDescriptor.HasValue)
        {
          SubjectDescriptor? subjectDescriptor5 = x2.SubjectDescriptor;
          SubjectDescriptor subjectDescriptor6 = new SubjectDescriptor();
          if ((subjectDescriptor5.HasValue ? (subjectDescriptor5.HasValue ? (subjectDescriptor5.GetValueOrDefault() != subjectDescriptor6 ? 1 : 0) : 0) : 1) != 0)
          {
            subjectDescriptor5 = x2.SubjectDescriptor;
            if (!subjectDescriptor5.Value.IsUnknownSubjectType())
            {
              subjectDescriptor4 = x2.SubjectDescriptor;
              goto label_31;
            }
          }
        }
        subjectDescriptor4 = y2.SubjectDescriptor;
label_31:
        directoryGroup2.SubjectDescriptor = subjectDescriptor4;
        directoryGroup1.Description = x2.Description ?? y2.Description;
        directoryGroup1.Mail = x2.Mail ?? y2.Mail;
        directoryGroup1.MailNickname = x2.MailNickname ?? y2.MailNickname;
        directoryGroup1.ScopeName = DirectoryEntityMerger.MergeScopeName((IDirectoryEntity) x2, (IDirectoryEntity) y2);
        return (IDirectoryEntity) directoryGroup1;
      }
      if (!"ServicePrincipal".Equals(str))
        return preferred;
      DirectoryServicePrincipal servicePrincipal = DirectoryEntityBuilder.BuildEntity<DirectoryServicePrincipal>(originDirectory, originId, entityId, localDirectory, localId, properties: properties);
      IDirectoryServicePrincipal x3 = preferred as IDirectoryServicePrincipal;
      IDirectoryServicePrincipal y3 = secondary as IDirectoryServicePrincipal;
      if (x3 == null && y3 == null)
        return (IDirectoryEntity) servicePrincipal;
      if (y3 == null)
        y3 = (IDirectoryServicePrincipal) servicePrincipal;
      if (x3 == null)
        x3 = (IDirectoryServicePrincipal) servicePrincipal;
      servicePrincipal.Active = x3.Active ?? y3.Active;
      servicePrincipal.DisplayName = DirectoryEntityMerger.MergeDisplayName((IDirectoryEntity) x3, (IDirectoryEntity) y3);
      servicePrincipal.AppId = x3.AppId ?? y3.AppId;
      return (IDirectoryEntity) servicePrincipal;
    }

    private static IDictionary<string, object> MergeProperties(
      IDirectoryEntity preferred,
      IDirectoryEntity secondary)
    {
      DirectoryEntity directoryEntity1 = preferred as DirectoryEntity;
      DirectoryEntity directoryEntity2 = secondary as DirectoryEntity;
      return directoryEntity1?.Properties == null && directoryEntity2?.Properties == null ? (IDictionary<string, object>) new Dictionary<string, object>() : DirectoryEntityMerger.MergeProperties(directoryEntity1.Properties, directoryEntity2.Properties);
    }

    private static IDictionary<string, object> MergeProperties(
      IDictionary<string, object> preferred,
      IDictionary<string, object> secondary)
    {
      if (preferred.IsNullOrEmpty<KeyValuePair<string, object>>() && secondary.IsNullOrEmpty<KeyValuePair<string, object>>())
        return (IDictionary<string, object>) new Dictionary<string, object>();
      if (preferred.IsNullOrEmpty<KeyValuePair<string, object>>())
        return (IDictionary<string, object>) new Dictionary<string, object>(secondary);
      if (secondary.IsNullOrEmpty<KeyValuePair<string, object>>())
        return (IDictionary<string, object>) new Dictionary<string, object>(preferred);
      Dictionary<string, object> dictionary = new Dictionary<string, object>(secondary);
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) preferred)
      {
        if (keyValuePair.Value != null)
          dictionary[keyValuePair.Key] = keyValuePair.Value;
      }
      return (IDictionary<string, object>) dictionary;
    }

    private static string MergeDisplayName(IDirectoryEntity x, IDirectoryEntity y) => x.DisplayName == null || y.DisplayName != null && x.LocalId == null ? y.DisplayName : x.DisplayName;

    private static string MergeScopeName(IDirectoryEntity x, IDirectoryEntity y) => x.ScopeName == null || y.ScopeName != null && x.LocalId != null ? y.ScopeName : x.ScopeName;
  }
}
