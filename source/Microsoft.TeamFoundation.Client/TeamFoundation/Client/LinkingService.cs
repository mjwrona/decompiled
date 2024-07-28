// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.LinkingService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  internal class LinkingService : ILinking, ILinkingProvider, ILinkingConsumer
  {
    private TfsTeamProjectCollection m_tfs;
    private IRegistration m_registrationProxy;
    private volatile Dictionary<string, LinkingProxy> m_artifactProviders;
    private volatile Dictionary<string, LinkingProxy> m_artifactConsumers;
    private static object s_initializationLock = new object();
    private volatile string m_serverUrlPublic;

    internal LinkingService(TfsTeamProjectCollection tfs)
    {
      this.m_tfs = tfs;
      this.m_registrationProxy = (IRegistration) tfs.GetService(typeof (IRegistration));
    }

    public Artifact[] GetArtifacts(string[] artifactUriList)
    {
      string[] arr = artifactUriList != null ? new string[artifactUriList.Length] : throw new ArgumentNullException(nameof (artifactUriList));
      for (int index = 0; index < artifactUriList.Length; ++index)
        arr[index] = !LinkingUtilities.IsUriWellFormed(artifactUriList[index]) ? (string) null : artifactUriList[index];
      int[] nullElementIndices;
      object[] filteredArr;
      this.GetNullElementIndexes((object[]) arr, out nullElementIndices, out filteredArr);
      string[] artifactUriList1 = new string[filteredArr.Length];
      filteredArr.CopyTo((Array) artifactUriList1, 0);
      Artifact[] artifactsProcess = this.GetArtifactsProcess(artifactUriList1);
      Artifact[] artifacts = new Artifact[artifactsProcess.Length + nullElementIndices.Length];
      this.MergeNullElements((object[]) artifactsProcess, nullElementIndices).CopyTo((Array) artifacts, 0);
      return artifacts;
    }

    public Artifact[] GetReferencingArtifacts(string[] uriList) => this.GetReferencingArtifacts(uriList, (LinkFilter[]) null);

    public Artifact[] GetReferencingArtifacts(string[] uriList, LinkFilter[] filters)
    {
      ArgumentUtility.CheckForNull<string[]>(uriList, nameof (uriList));
      ArrayList artifactList = new ArrayList();
      if (this.m_artifactConsumers == null)
      {
        lock (LinkingService.s_initializationLock)
        {
          if (this.m_artifactConsumers == null)
            this.m_artifactConsumers = this.InitializeConsumers();
        }
      }
      foreach (string uri in uriList)
      {
        if (!LinkingUtilities.IsUriWellFormed(uri))
        {
          if (uri == null)
            throw new ArgumentException(ClientResources.NullArtifactUrlInUrlList(), nameof (uriList));
          throw new TeamFoundationServerException(CommonResources.MalformedUri((object) uri));
        }
      }
      try
      {
        if (filters == null)
        {
          foreach (LinkingProxy linkingProxy in this.m_artifactConsumers.Values)
            artifactList.AddRange((ICollection) linkingProxy.GetReferencingArtifacts(uriList));
        }
        else
        {
          foreach (LinkFilter filter in filters)
          {
            if (filter.FilterType != FilterType.ToolType)
              throw new ArgumentException(TFCommonResources.UnsupportedLinkFilter((object) filter.FilterType));
            foreach (string filterValue in filter.FilterValues)
            {
              LinkingProxy linkingProxy;
              if (this.m_artifactConsumers.TryGetValue(filterValue, out linkingProxy))
                artifactList.AddRange((ICollection) linkingProxy.GetReferencingArtifacts(uriList));
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServerException(CommonResources.FailureGetArtifact(), ex);
      }
      return (Artifact[]) LinkingUtilities.RemoveDuplicateArtifacts(artifactList).ToArray(typeof (Artifact));
    }

    public string GetArtifactUrl(string uri) => this.GetArtifactUrl(LinkingUtilities.DecodeUri(uri));

    public string GetArtifactUrl(ArtifactId artId) => this.GetArtifactUrl(artId, this.m_tfs.Uri.ToString());

    public string GetArtifactUrlExternal(string uri) => this.GetArtifactUrl(LinkingUtilities.DecodeUri(uri), this.ServerUrlPublic);

    public string GetArtifactUrlExternal(ArtifactId artId) => this.GetArtifactUrl(artId, this.ServerUrlPublic);

    public string ServerUrlPublic
    {
      get
      {
        if (this.m_serverUrlPublic == null)
        {
          lock (LinkingService.s_initializationLock)
          {
            if (this.m_serverUrlPublic == null)
              this.m_serverUrlPublic = LinkingUtilities.GetServerUrl(this.m_tfs.Uri);
          }
        }
        return this.m_serverUrlPublic;
      }
    }

    private Dictionary<string, LinkingProxy> InitializeConsumers()
    {
      Dictionary<string, LinkingProxy> dictionary = new Dictionary<string, LinkingProxy>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistrationEntry registrationEntry in this.m_registrationProxy.GetRegistrationEntries(string.Empty))
      {
        string serviceInterfaceUrl = RegistrationUtilities.GetServiceInterfaceUrl(registrationEntry, "LinkingConsumerService");
        if (!string.IsNullOrEmpty(serviceInterfaceUrl))
        {
          LinkingProxy linkingProxy = new LinkingProxy(this.m_tfs, serviceInterfaceUrl);
          dictionary.Add(registrationEntry.Type, linkingProxy);
        }
      }
      return dictionary;
    }

    private Dictionary<string, LinkingProxy> InitializeProviders()
    {
      Dictionary<string, LinkingProxy> dictionary = new Dictionary<string, LinkingProxy>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistrationEntry registrationEntry in this.m_registrationProxy.GetRegistrationEntries(string.Empty))
      {
        string serviceInterfaceUrl = RegistrationUtilities.GetServiceInterfaceUrl(registrationEntry, "LinkingProviderService");
        if (!string.IsNullOrEmpty(serviceInterfaceUrl))
        {
          LinkingProxy linkingProxy = new LinkingProxy(this.m_tfs, serviceInterfaceUrl);
          dictionary.Add(registrationEntry.Type, linkingProxy);
        }
      }
      return dictionary;
    }

    private Artifact[] GetArtifactsProcess(string[] artifactUriList)
    {
      Hashtable hashtable1 = new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < artifactUriList.Length; ++index)
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUriList[index]);
        string tool = artifactId.Tool;
        if (hashtable1[(object) tool] == null)
          hashtable1[(object) tool] = (object) new ArrayList()
          {
            (object) artifactId
          };
        else
          ((ArrayList) hashtable1[(object) tool]).Add((object) artifactId);
      }
      Hashtable hashtable2 = new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      foreach (string key in (IEnumerable) hashtable1.Keys)
      {
        ArtifactId[] array = (ArtifactId[]) ((ArrayList) hashtable1[(object) key]).ToArray(typeof (ArtifactId));
        Artifact[] artifactsProcess = this.GetArtifactsProcess(key, array);
        for (int index = 0; index < array.Length; ++index)
          hashtable2[(object) LinkingUtilities.EncodeUri(array[index])] = (object) artifactsProcess[index];
      }
      Artifact[] artifactsProcess1 = new Artifact[artifactUriList.Length];
      for (int index = 0; index < artifactUriList.Length; ++index)
        artifactsProcess1[index] = (Artifact) hashtable2[(object) artifactUriList[index]];
      return artifactsProcess1;
    }

    private Artifact[] GetArtifactsProcess(string tool, ArtifactId[] artifactIds)
    {
      if (this.m_artifactProviders == null)
      {
        lock (LinkingService.s_initializationLock)
        {
          if (this.m_artifactProviders == null)
            this.m_artifactProviders = this.InitializeProviders();
        }
      }
      Artifact[] artifactsProcess = new Artifact[artifactIds.Length];
      LinkingProxy linkingProxy;
      if (this.m_artifactProviders.TryGetValue(tool, out linkingProxy))
      {
        ArrayList arrayList = new ArrayList();
        foreach (ArtifactId artifactId in artifactIds)
        {
          string str = LinkingUtilities.EncodeUri(artifactId);
          arrayList.Add((object) str);
        }
        string[] array = (string[]) arrayList.ToArray(typeof (string));
        try
        {
          artifactsProcess = linkingProxy.GetArtifacts(array);
        }
        catch (Exception ex)
        {
          throw new TeamFoundationServerException(TFCommonResources.FailureGetArtifactWithTool((object) tool), ex);
        }
        if (artifactsProcess == null || artifactsProcess.Length != artifactIds.Length)
          throw new TeamFoundationServerException(TFCommonResources.FailureGetArtifactWithTool((object) tool));
      }
      return artifactsProcess;
    }

    private string GetArtifactUrl(ArtifactId artId, string serverUrl)
    {
      ArgumentUtility.CheckForNull<ArtifactId>(artId, nameof (artId));
      if (!LinkingUtilities.IsToolTypeWellFormed(artId.Tool) || !LinkingUtilities.IsArtifactTypeWellFormed(artId.ArtifactType))
        throw new ArgumentException(CommonResources.MalformedArtifactId((object) artId.ToString()));
      RegistrationEntry[] registrationEntries = this.m_registrationProxy.GetRegistrationEntries(artId.Tool);
      bool flag = false;
      foreach (RegistrationEntry registrationEntry in registrationEntries)
      {
        foreach (ArtifactType artifactType in registrationEntry.ArtifactTypes)
        {
          if (VssStringComparer.ArtifactType.Compare(artifactType.Name, artId.ArtifactType) == 0)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        throw new ArgumentException(CommonResources.MalformedArtifactId((object) artId.ToString()));
      string empty = string.Empty;
      foreach (RegistrationEntry registrationEntry in registrationEntries)
      {
        foreach (RegistrationExtendedAttribute extendedAttribute in registrationEntry.RegistrationExtendedAttributes)
        {
          if (VssStringComparer.ArtifactType.Compare(extendedAttribute.Name, "ArtifactDisplayUrl") == 0)
          {
            empty = extendedAttribute.Value;
            break;
          }
        }
      }
      return LinkingUtilities.GetArtifactUrl(empty, artId, serverUrl);
    }

    private void GetNullElementIndexes(
      object[] arr,
      out int[] nullElementIndices,
      out object[] filteredArr)
    {
      ArrayList arrayList1 = new ArrayList();
      ArrayList arrayList2 = new ArrayList();
      for (int index = 0; index < arr.Length; ++index)
      {
        if (arr[index] == null)
          arrayList1.Add((object) index);
        else
          arrayList2.Add(arr[index]);
      }
      nullElementIndices = (int[]) arrayList1.ToArray(typeof (int));
      filteredArr = (object[]) arrayList2.ToArray(typeof (object));
    }

    private object[] MergeNullElements(object[] arr, int[] nullElementIndices)
    {
      object[] objArray = new object[arr.Length + nullElementIndices.Length];
      int num = 0;
      int index1 = 0;
      for (int index2 = 0; index2 < objArray.Length; ++index2)
      {
        if (index1 < nullElementIndices.Length && index2 == nullElementIndices[index1])
          ++index1;
        else
          objArray[index2] = arr[num++];
      }
      return objArray;
    }
  }
}
