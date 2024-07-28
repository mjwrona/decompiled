// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RegistrationEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  public class RegistrationEntry
  {
    private string m_Type;
    private ServiceInterface[] m_ServiceInterfaces;
    private Database[] m_Databases;
    private ArtifactType[] m_ArtifactTypes;
    private EventType[] m_EventTypes;
    private RegistrationExtendedAttribute[] m_RegistrationExtendedAttributes;
    private ChangeType m_ChangeType = ChangeType.NoChange;

    public string Type
    {
      get => this.m_Type;
      set => this.m_Type = value;
    }

    public ServiceInterface[] ServiceInterfaces
    {
      get => this.m_ServiceInterfaces;
      set => this.m_ServiceInterfaces = value;
    }

    public Database[] Databases
    {
      get => this.m_Databases;
      set => this.m_Databases = value;
    }

    public EventType[] EventTypes
    {
      get => this.m_EventTypes;
      set => this.m_EventTypes = value;
    }

    public ArtifactType[] ArtifactTypes
    {
      get => this.m_ArtifactTypes;
      set => this.m_ArtifactTypes = value;
    }

    public RegistrationExtendedAttribute[] RegistrationExtendedAttributes
    {
      get => this.m_RegistrationExtendedAttributes;
      set => this.m_RegistrationExtendedAttributes = value;
    }

    [XmlIgnore]
    public ChangeType ChangeType
    {
      get => this.m_ChangeType;
      set => this.m_ChangeType = value;
    }

    internal void AddMissingElements(RegistrationEntry from)
    {
      this.ServiceInterfaces = RegistrationEntry.MergeArrays<ServiceInterface>(this.ServiceInterfaces, from.ServiceInterfaces, (IComparer<ServiceInterface>) new ServiceInterfaceComparer(), (ICombiner<ServiceInterface>) new SimpleCombiner<ServiceInterface>());
      this.Databases = RegistrationEntry.MergeArrays<Database>(this.Databases, from.Databases, (IComparer<Database>) new DatabaseComparer(), (ICombiner<Database>) new SimpleCombiner<Database>());
      this.EventTypes = RegistrationEntry.MergeArrays<EventType>(this.EventTypes, from.EventTypes, (IComparer<EventType>) new EventTypeComparer(), (ICombiner<EventType>) new SimpleCombiner<EventType>());
      this.RegistrationExtendedAttributes = RegistrationEntry.MergeArrays<RegistrationExtendedAttribute>(this.RegistrationExtendedAttributes, from.RegistrationExtendedAttributes, (IComparer<RegistrationExtendedAttribute>) new RegistrationExtendedAttributeComparer(), (ICombiner<RegistrationExtendedAttribute>) new SimpleCombiner<RegistrationExtendedAttribute>());
      this.ArtifactTypes = RegistrationEntry.MergeArrays<ArtifactType>(this.ArtifactTypes, from.ArtifactTypes, (IComparer<ArtifactType>) new ArtifactTypeComparer(), (ICombiner<ArtifactType>) new ArtifactTypeCombiner());
    }

    internal static T[] MergeArrays<T>(
      T[] firstOne,
      T[] secondOne,
      IComparer<T> comparer,
      ICombiner<T> combiner)
    {
      if (firstOne == null || secondOne == null)
        return firstOne;
      T[] array1 = (T[]) firstOne.Clone();
      T[] array2 = (T[]) secondOne.Clone();
      List<T> objList = new List<T>();
      Array.Sort<T>(array1, comparer);
      Array.Sort<T>(array2, comparer);
      int index1 = 0;
      int index2 = 0;
      if (array1.Length == 0)
        return array2;
      if (array2.Length == 0)
        return array1;
      while (index1 < array1.Length && index2 < array2.Length)
      {
        if (comparer.Compare(array1[index1], array2[index2]) < 0)
          objList.Add(array1[index1++]);
        else if (comparer.Compare(array1[index1], array2[index2]) == 0)
        {
          objList.Add(combiner.Combine(array1[index1], array2[index2]));
          ++index1;
          ++index2;
        }
        else
          objList.Add(array2[index2++]);
      }
      while (index1 < array1.Length)
        objList.Add(array1[index1++]);
      while (index2 < array2.Length)
        objList.Add(array2[index2++]);
      return objList.ToArray();
    }

    internal RegistrationExtendedAttribute GetExtendedAttribute(string name) => this.RegistrationExtendedAttributes != null ? Array.Find<RegistrationExtendedAttribute>(this.RegistrationExtendedAttributes, new RegistrationExtendedAttribute(name, string.Empty).EqualsByName()) : (RegistrationExtendedAttribute) null;

    internal Predicate<RegistrationEntry> EqualsByName() => (Predicate<RegistrationEntry>) (that => VssStringComparer.RegistrationEntryName.Equals(this.Type, that.Type));
  }
}
