// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.IdentityInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class IdentityInformationProvider : IIdentityInformationProvider
  {
    public static readonly string HardwareIdEventFaultName = "VS/TelemetryApi/Identity/HardwareIdFault";
    internal const string PersistedIdentityNotFoundTemplate = "Persisted identity '{0}' was not found.";
    internal const string IdChangeCausedInvalidationTemplate = "{0} change requires invalidation of persisted ID.";
    internal const string EventParameterLastValueSuffix = ".Last";
    internal const string ConfigVersionInvalidationTemplate = "Persisted config version '{0}' is less than MinValidConfigVersion '{1}'";
    internal const string PrimaryIdValueNoLongerExistsTemplate = "PrimaryId '{0}' value no longer found on machine.";
    internal const string PrimaryIdValueNoLongerExists = "PrimaryIdValueNoLongerExists";
    internal const string HardwarePersistenceDate = "HardwareIdDate";
    internal const string HardwarePersistenceAge = "HardwareIdAge";
    internal const string IsMachineStoreAccessiblePropertyName = "IsMachineStoreAccessible";
    internal static readonly string HardwareIdNotObtained = "HardwareId not obtained";
    internal static readonly string HardwareIdNotPeristed = "HardwareId not persisted.";
    private const string ExceptionObtainingHardwareIdFaultDescription = "ExceptionObtainingHardwareId";
    private static readonly Lazy<IPGlobalProperties> IpGlobalProperties = new Lazy<IPGlobalProperties>(new Func<IPGlobalProperties>(IdentityInformationProvider.InitializeIpGlobalProperties));
    private readonly Lazy<IPersistentPropertyBag> lazyMachineStore;
    private readonly Lazy<Dictionary<string, IdentityInformationProvider.MachineIdentitifier>> lazyMachineIdentifiers;
    internal static readonly Dictionary<string, string> StoredPropertyNamesToInts = new Dictionary<string, string>()
    {
      {
        nameof (BiosSerialNumber),
        "0"
      },
      {
        nameof (BiosUUID),
        "1"
      },
      {
        nameof (ConfigVersion),
        "2"
      },
      {
        nameof (DNSDomain),
        "3"
      },
      {
        nameof (HardwareIdDate),
        "4"
      },
      {
        nameof (MachineName),
        "5"
      },
      {
        nameof (PersistedSelectedInterface),
        "6"
      },
      {
        nameof (PersistedSelectedMACAddress),
        "7"
      },
      {
        nameof (SelectedMACAddress),
        "8"
      }
    };
    private static readonly Lazy<PIIPropertyProcessor> LazyPiiPropertyProcessor = new Lazy<PIIPropertyProcessor>((Func<PIIPropertyProcessor>) (() => new PIIPropertyProcessor()));
    private static readonly object LazyPiiPropertyProcessorLockObject = new object();
    private INetworkInterfacesInformationProvider networkInterfacesInformationProvider;

    private Dictionary<string, IdentityInformationProvider.MachineIdentitifier> MachineIdentifiers => this.lazyMachineIdentifiers.Value;

    private List<Exception> ExceptionsEncounteredObtainingHardwareId { get; } = new List<Exception>();

    private bool IsMachineStoreAccessible => this.lazyMachineStore.Value != null;

    protected IdentityInformationProvider(Func<IPersistentPropertyBag> createStore)
    {
      createStore.RequiresArgumentNotNull<Func<IPersistentPropertyBag>>(nameof (createStore));
      this.lazyMachineStore = new Lazy<IPersistentPropertyBag>(createStore);
      this.lazyMachineIdentifiers = new Lazy<Dictionary<string, IdentityInformationProvider.MachineIdentitifier>>((Func<Dictionary<string, IdentityInformationProvider.MachineIdentitifier>>) (() => this.ConfigureIdentities()));
    }

    internal static string DefaultStorageFileName => System.IO.Path.Combine("Microsoft Visual Studio", "prpbg.dat");

    private static string JoinIdentifiers(
      IEnumerable<IdentityInformationProvider.MachineIdentitifier> ids,
      Func<IdentityInformationProvider.MachineIdentitifier, string> transform)
    {
      string str = string.Join("|", ids.Select<IdentityInformationProvider.MachineIdentitifier, string>(transform));
      return !(str == "||") ? str : (string) null;
    }

    internal string ConfigVersion => this.MachineIdentityConfig.ConfigVersion;

    internal string MinValidConfigVersion => this.MachineIdentityConfig.MinValidConfigVersion;

    internal bool InvalidateOnPrimaryIdChange => this.MachineIdentityConfig.InvalidateOnPrimaryIdChange;

    internal string[] HardwareIdValues => this.MachineIdentityConfig.HardwareIdComponents;

    public string PersistedSelectedMACAddress => this.MachineIdentifiers[nameof (PersistedSelectedMACAddress)].CurrentStoredValue;

    public string PersistedSelectedInterface => this.MachineIdentifiers[nameof (PersistedSelectedInterface)].CurrentStoredValue;

    public event EventHandler<EventArgs> HardwareIdCalculationCompleted;

    public bool PersistedIdWasInvalidated { get; protected set; }

    public string PersistedIdInvalidationReason { get; protected set; }

    public bool AnyValueChanged { get; protected set; }

    public string HardwareId => this.MachineIdentifiers[nameof (HardwareId)].CurrentSystemValue;

    public void GetHardwareIdWithCalculationCompletedEvent(Action<string> callback)
    {
      if (this.IsMachineStoreAccessible)
      {
        string hardwareId = this.HardwareId;
        if (!this.ExceptionsEncounteredObtainingHardwareId.Any<Exception>())
          callback(hardwareId);
      }
      EventHandler<EventArgs> calculationCompleted = this.HardwareIdCalculationCompleted;
      if (calculationCompleted == null)
        return;
      calculationCompleted((object) this, EventArgs.Empty);
    }

    public string MachineName => IdentityInformationProvider.IpGlobalProperties.Value.HostName;

    public string DNSDomain => IdentityInformationProvider.IpGlobalProperties.Value.DomainName;

    public abstract string BiosSerialNumber { get; }

    public abstract Guid BiosUUID { get; }

    public abstract BiosFirmwareTableParserError BiosInformationError { get; }

    public DateTime? HardwareIdDate
    {
      get
      {
        DateTime result;
        return !DateTime.TryParse(this.lazyMachineStore.Value.GetProperty(IdentityInformationProvider.StoredPropertyNamesToInts[nameof (HardwareIdDate)]) as string, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) ? new DateTime?() : new DateTime?(result);
      }
    }

    public string SelectedMACAddress => this.networkInterfacesInformationProvider.SelectedMACAddress;

    public List<NetworkInterfaceCardInformation> PrioritizedNetworkInterfaces => this.networkInterfacesInformationProvider.PrioritizedNetworkInterfaces;

    public TelemetryManifestMachineIdentityConfig MachineIdentityConfig { get; private set; }

    public void Initialize(
      TelemetryContext telemetryContext,
      ITelemetryScheduler contextScheduler,
      TelemetryManifestMachineIdentityConfig machineIdentityConfig)
    {
      telemetryContext.RequiresArgumentNotNull<TelemetryContext>(nameof (telemetryContext));
      this.MachineIdentityConfig = machineIdentityConfig ?? TelemetryManifestMachineIdentityConfig.DefaultConfig;
      this.networkInterfacesInformationProvider = (INetworkInterfacesInformationProvider) new NetworkInterfacesInformationProvider(this.MachineIdentityConfig.SmaRules);
      // ISSUE: explicit non-virtual call
      (contextScheduler ?? (ITelemetryScheduler) new TelemetryScheduler()).Schedule((Action) (() => this.GetHardwareIdWithCalculationCompletedEvent((Action<string>) (hardwareId => telemetryContext.SharedProperties[IdentityPropertyProvider.HardwareIdPropertyName] = (object) __nonvirtual (this.HardwareId)))));
    }

    public IEnumerable<KeyValuePair<string, object>> CollectIdentifiers(
      List<Exception> collectionExceptions)
    {
      yield return new KeyValuePair<string, object>("IsMachineStoreAccessible", (object) this.IsMachineStoreAccessible);
      if (this.IsMachineStoreAccessible)
      {
        List<string> primaryIdValuesNoLongerExist = new List<string>();
        foreach (IdentityInformationProvider.MachineIdentitifier id in this.MachineIdentifiers.Values)
        {
          yield return new KeyValuePair<string, object>(id.Name, id.Type != IdentityInformationProvider.MachineIdentitifier.IdType.Informational ? (object) new TelemetryPiiProperty((object) id.CurrentSystemValueUnhased) : (object) id.ValidValue);
          if (id.Type == IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryId && !string.IsNullOrWhiteSpace(id.LastPersistedValue) && !(id as IdentityInformationProvider.MachinePrimaryIdentitifier).PrimaryIdStillExists)
            primaryIdValuesNoLongerExist.Add(id.Name);
          if (!id.IsLastPersistedValueValid)
          {
            if (!string.IsNullOrWhiteSpace(id.LastPersistedValue))
              yield return new KeyValuePair<string, object>(id.Name + ".Last", (object) id.LastPersistedValue);
            if (id.Type != IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryCombinedId)
            {
              id.StoreValidValue();
              this.AnyValueChanged = true;
            }
          }
        }
        if (primaryIdValuesNoLongerExist.Any<string>())
          yield return new KeyValuePair<string, object>("PrimaryIdValueNoLongerExists", (object) string.Join(",", (IEnumerable<string>) primaryIdValuesNoLongerExist));
        yield return new KeyValuePair<string, object>("HardwareIdDate", (object) this.HardwareIdDate.Value);
        yield return new KeyValuePair<string, object>("HardwareIdAge", (object) (DateTime.UtcNow - this.HardwareIdDate.Value).TotalDays);
        try
        {
          this.lazyMachineStore.Value.Persist();
        }
        catch (Exception ex)
        {
          collectionExceptions.Add(ex);
        }
      }
    }

    public void SchedulePostPersistedSharedPropertyAndSendAnyFaults(
      TelemetrySession telemetrySession,
      ITelemetryScheduler scheduler)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      if (!this.IsMachineStoreAccessible)
        return;
      scheduler.Schedule((Action) (() => this.PostAnyFaultsGettingHardwareId(telemetrySession, telemetrySession.CancellationToken)), new CancellationToken?(telemetrySession.CancellationToken));
      scheduler.Schedule((Action) (() => this.PostPersistedSharedProperties(telemetrySession, telemetrySession.CancellationToken)), new CancellationToken?(telemetrySession.CancellationToken));
    }

    private void PostAnyFaultsGettingHardwareId(
      TelemetrySession telemetrySession,
      CancellationToken cancellationToken)
    {
      foreach (Exception exceptionObject in this.ExceptionsEncounteredObtainingHardwareId)
      {
        if (cancellationToken.IsCancellationRequested)
          break;
        telemetrySession.PostFault(IdentityInformationProvider.HardwareIdEventFaultName, "ExceptionObtainingHardwareId", exceptionObject);
      }
    }

    private void PostPersistedSharedProperties(
      TelemetrySession telemetrySession,
      CancellationToken cancellationToken)
    {
      if (this.ExceptionsEncounteredObtainingHardwareId.Any<Exception>())
        return;
      string persistedSharedProperty = telemetrySession.GetPersistedSharedProperty(IdentityPropertyProvider.HardwareIdPropertyName) as string;
      string hardwareId = this.HardwareId;
      if (!(this.HardwareId != IdentityInformationProvider.HardwareIdNotObtained) || string.Equals(hardwareId, persistedSharedProperty, StringComparison.Ordinal))
        return;
      telemetrySession.SetPersistedSharedProperty(IdentityPropertyProvider.HardwareIdPropertyName, hardwareId);
    }

    private string GetHardwareIdWithInvalidation()
    {
      string withInvalidation = IdentityInformationProvider.HardwareIdNotObtained;
      try
      {
        withInvalidation = this.MachineIdentifiers["HardwareId"].LastPersistedValue;
        if (!this.PersistedIdWasInvalidated && string.IsNullOrWhiteSpace(withInvalidation))
        {
          this.PersistedIdWasInvalidated = true;
          this.PersistedIdInvalidationReason = IdentityInformationProvider.HardwareIdNotPeristed;
        }
        if (!this.PersistedIdWasInvalidated)
        {
          foreach (IdentityInformationProvider.MachineIdentitifier machineIdentitifier in ((IEnumerable<string>) this.HardwareIdValues).Select<string, IdentityInformationProvider.MachineIdentitifier>((Func<string, IdentityInformationProvider.MachineIdentitifier>) (name => this.MachineIdentifiers[name])).Where<IdentityInformationProvider.MachineIdentitifier>((Func<IdentityInformationProvider.MachineIdentitifier, bool>) (id => id.Type == IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryImmutableId)))
          {
            if (!this.PersistedIdWasInvalidated && !machineIdentitifier.IsLastPersistedValueValid)
            {
              this.PersistedIdWasInvalidated = true;
              this.PersistedIdInvalidationReason = string.Format(string.IsNullOrWhiteSpace(machineIdentitifier.LastPersistedValue) ? "Persisted identity '{0}' was not found." : "{0} change requires invalidation of persisted ID.", (object) machineIdentitifier.Name);
            }
          }
        }
        IdentityInformationProvider.MachineIdentitifier machineIdentifier = this.MachineIdentifiers["ConfigVersion"];
        if (!this.PersistedIdWasInvalidated && string.Compare(machineIdentifier.LastPersistedValue, this.MinValidConfigVersion, StringComparison.OrdinalIgnoreCase) < 0)
        {
          this.PersistedIdWasInvalidated = true;
          this.PersistedIdInvalidationReason = string.Format("Persisted config version '{0}' is less than MinValidConfigVersion '{1}'", (object) machineIdentifier.LastPersistedValue, (object) this.MinValidConfigVersion);
        }
        if (!this.PersistedIdWasInvalidated && this.InvalidateOnPrimaryIdChange)
        {
          foreach (IdentityInformationProvider.MachineIdentitifier machineIdentitifier in ((IEnumerable<string>) this.HardwareIdValues).Select<string, IdentityInformationProvider.MachineIdentitifier>((Func<string, IdentityInformationProvider.MachineIdentitifier>) (name => this.MachineIdentifiers[name])).Where<IdentityInformationProvider.MachineIdentitifier>((Func<IdentityInformationProvider.MachineIdentitifier, bool>) (id => id.Type == IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryId && !this.PersistedIdWasInvalidated && !id.IsLastPersistedValueValid)))
          {
            this.PersistedIdWasInvalidated = true;
            this.PersistedIdInvalidationReason = string.Format(string.IsNullOrWhiteSpace(machineIdentitifier.LastPersistedValue) ? "Persisted identity '{0}' was not found." : "PrimaryId '{0}' value no longer found on machine.", (object) machineIdentitifier.Name);
          }
        }
        if (this.PersistedIdWasInvalidated)
        {
          withInvalidation = IdentityInformationProvider.JoinIdentifiers(((IEnumerable<string>) this.HardwareIdValues).Select<string, IdentityInformationProvider.MachineIdentitifier>((Func<string, IdentityInformationProvider.MachineIdentitifier>) (idName => this.MachineIdentifiers[idName])), (Func<IdentityInformationProvider.MachineIdentitifier, string>) (id => id.CurrentSystemValue));
          this.lazyMachineStore.Value.SetProperty(IdentityInformationProvider.StoredPropertyNamesToInts["HardwareIdDate"], DateTime.UtcNow.ToString("o"));
          this.lazyMachineStore.Value.SetProperty(IdentityInformationProvider.StoredPropertyNamesToInts["ConfigVersion"], this.ConfigVersion);
          foreach (string hardwareIdValue in this.HardwareIdValues)
            this.MachineIdentifiers[hardwareIdValue].StoreValidValue();
        }
        this.lazyMachineStore.Value.Persist();
        return withInvalidation;
      }
      catch (Exception ex)
      {
        this.ExceptionsEncounteredObtainingHardwareId.Add(ex);
      }
      return withInvalidation;
    }

    internal static string FormatPropertyValue<T>(T value)
    {
      if ((object) value == null)
        return (string) null;
      return !(typeof (T) == typeof (Guid)) ? value.ToString() : ((Guid) (object) value).ToString("D");
    }

    private bool MACAddressStillExists(string mac)
    {
      lock (IdentityInformationProvider.LazyPiiPropertyProcessorLockObject)
        return this.PrioritizedNetworkInterfaces.Any<NetworkInterfaceCardInformation>((Func<NetworkInterfaceCardInformation, bool>) (nic => string.Equals(mac, IdentityInformationProvider.LazyPiiPropertyProcessor.Value.ConvertToHashedValue((object) new TelemetryPiiProperty((object) nic.MacAddress)))));
    }

    private bool NetworkInterfaceStillExists(string interfaceDescription) => this.PrioritizedNetworkInterfaces.Any<NetworkInterfaceCardInformation>((Func<NetworkInterfaceCardInformation, bool>) (nic => string.Equals(interfaceDescription, nic.Description, StringComparison.OrdinalIgnoreCase)));

    private Dictionary<string, IdentityInformationProvider.MachineIdentitifier> ConfigureIdentities()
    {
      Dictionary<string, IdentityInformationProvider.MachineIdentitifier> identifiersDict = new Dictionary<string, IdentityInformationProvider.MachineIdentitifier>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "MachineName",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "MachineName", IdentityInformationProvider.MachineIdentitifier.IdType.Secondary, (Func<string>) (() => this.MachineName))
        },
        {
          "DNSDomain",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "DNSDomain", IdentityInformationProvider.MachineIdentitifier.IdType.Secondary, (Func<string>) (() => this.DNSDomain))
        },
        {
          "SelectedMACAddress",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "SelectedMACAddress", IdentityInformationProvider.MachineIdentitifier.IdType.Secondary, (Func<string>) (() => this.SelectedMACAddress))
        },
        {
          "BiosUUID",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "BiosUUID", IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryImmutableId, (Func<string>) (() => IdentityInformationProvider.FormatPropertyValue<Guid>(this.BiosUUID)))
        },
        {
          "BiosSerialNumber",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "BiosSerialNumber", IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryImmutableId, (Func<string>) (() => this.BiosSerialNumber))
        },
        {
          "ConfigVersion",
          new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "ConfigVersion", IdentityInformationProvider.MachineIdentitifier.IdType.Informational, (Func<string>) (() => this.ConfigVersion), (Func<string, bool>) (persistedConfigVersion => string.CompareOrdinal(persistedConfigVersion, this.ConfigVersion) >= 0))
        },
        {
          "PersistedSelectedMACAddress",
          (IdentityInformationProvider.MachineIdentitifier) new IdentityInformationProvider.MachinePrimaryIdentitifier(this.lazyMachineStore, "PersistedSelectedMACAddress", (Func<string>) (() => this.SelectedMACAddress), (Func<string, bool>) (persistedSelectedMACAddress => !this.InvalidateOnPrimaryIdChange), new Func<string, bool>(this.MACAddressStillExists))
        }
      };
      identifiersDict.Add("PersistedSelectedInterface", new IdentityInformationProvider.MachineIdentitifier(this.lazyMachineStore, "PersistedSelectedInterface", IdentityInformationProvider.MachineIdentitifier.IdType.Informational, (Func<string>) (() => this.SelectedNetworkInterfaceDescription), (Func<string, bool>) (persistedInterface => identifiersDict["PersistedSelectedMACAddress"].IsLastPersistedValueValid || object.Equals((object) persistedInterface, (object) this.SelectedNetworkInterfaceDescription)), primaryIdStillExists: new Func<string, bool>(this.NetworkInterfaceStillExists)));
      identifiersDict.Add("HardwareId", (IdentityInformationProvider.MachineIdentitifier) new IdentityInformationProvider.MachineComboIdentitifier(this.lazyMachineStore, "HardwareId", (Func<string>) (() => this.GetHardwareIdWithInvalidation()), (Func<IEnumerable<IdentityInformationProvider.MachineIdentitifier>>) (() => ((IEnumerable<string>) this.HardwareIdValues).Select<string, IdentityInformationProvider.MachineIdentitifier>((Func<string, IdentityInformationProvider.MachineIdentitifier>) (idName => identifiersDict[idName])))));
      return identifiersDict;
    }

    private string SelectedNetworkInterfaceDescription => this.PrioritizedNetworkInterfaces.FirstOrDefault<NetworkInterfaceCardInformation>((Func<NetworkInterfaceCardInformation, bool>) (n => n.SelectionRank == 0))?.Description;

    private static IPGlobalProperties InitializeIpGlobalProperties() => IPGlobalProperties.GetIPGlobalProperties();

    internal class MachineIdentitifier
    {
      protected readonly Lazy<IPersistentPropertyBag> lazyIdStore;
      protected Func<string> getCurrentStoredValue;
      protected Lazy<string> lazyCurrentSystemValueUnhashed;
      protected Lazy<bool> lazyIsValid;
      private Lazy<string> lazyGetCurrentSystemValue;

      internal MachineIdentitifier(
        Lazy<IPersistentPropertyBag> store,
        string name,
        IdentityInformationProvider.MachineIdentitifier.IdType type,
        Func<string> getCurrentSystemValue,
        Func<string, bool> validateValue = null,
        string[] comboIds = null,
        Func<string, bool> primaryIdStillExists = null)
      {
        IdentityInformationProvider.MachineIdentitifier machineIdentitifier = this;
        this.lazyIdStore = store;
        this.Name = name;
        this.Type = type;
        this.lazyCurrentSystemValueUnhashed = new Lazy<string>(getCurrentSystemValue);
        this.lazyGetCurrentSystemValue = new Lazy<string>((Func<string>) (() =>
        {
          lock (IdentityInformationProvider.LazyPiiPropertyProcessorLockObject)
            return type == IdentityInformationProvider.MachineIdentitifier.IdType.Informational ? machineIdentitifier.CurrentSystemValueUnhased : IdentityInformationProvider.LazyPiiPropertyProcessor.Value.ConvertToHashedValue((object) new TelemetryPiiProperty((object) machineIdentitifier.CurrentSystemValueUnhased));
        }));
        if (type != IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryCombinedId)
          this.LastPersistedValue = this.GetStoreProperty(this.Name);
        this.getCurrentStoredValue = (Func<string>) (() => machineIdentitifier.GetStoreProperty(machineIdentitifier.Name));
        this.lazyIsValid = new Lazy<bool>((Func<bool>) (() =>
        {
          if (string.IsNullOrWhiteSpace(machineIdentitifier.LastPersistedValue))
            return false;
          return validateValue == null ? object.Equals((object) machineIdentitifier.LastPersistedValue, (object) machineIdentitifier.CurrentSystemValue) : validateValue(machineIdentitifier.LastPersistedValue);
        }));
      }

      public string Name { get; }

      public IdentityInformationProvider.MachineIdentitifier.IdType Type { get; }

      public string CurrentSystemValueUnhased => this.lazyCurrentSystemValueUnhashed.Value;

      public string CurrentStoredValue => this.getCurrentStoredValue();

      public string ValidValue => !this.IsLastPersistedValueValid ? this.CurrentSystemValue : this.LastPersistedValue;

      public string LastPersistedValue { get; protected set; }

      public bool IsLastPersistedValueValid => this.lazyIsValid.Value;

      private IPersistentPropertyBag IdStore => this.lazyIdStore.Value;

      public void StoreValidValue()
      {
        if (!(this.CurrentStoredValue != this.ValidValue))
          return;
        this.SetStoreProperty(this.Name, this.ValidValue);
      }

      public string CurrentSystemValue => this.lazyGetCurrentSystemValue.Value;

      private string GetStoreProperty(string propertyName) => this.IdStore.GetProperty(IdentityInformationProvider.StoredPropertyNamesToInts[propertyName]) as string;

      private void SetStoreProperty(string propertyName, string val) => this.IdStore.SetProperty(IdentityInformationProvider.StoredPropertyNamesToInts[propertyName], val);

      public enum IdType
      {
        PrimaryImmutableId,
        PrimaryCombinedId,
        PrimaryId,
        Secondary,
        Informational,
      }
    }

    internal class MachinePrimaryIdentitifier : IdentityInformationProvider.MachineIdentitifier
    {
      private readonly Lazy<bool> lazyPrimaryIdStillExists;

      public MachinePrimaryIdentitifier(
        Lazy<IPersistentPropertyBag> lazyIdStore,
        string name,
        Func<string> getCurrentValue,
        Func<string, bool> validateValue,
        Func<string, bool> primaryIdStillExists)
        : base(lazyIdStore, name, IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryId, getCurrentValue, validateValue)
      {
        IdentityInformationProvider.MachinePrimaryIdentitifier primaryIdentitifier = this;
        this.lazyPrimaryIdStillExists = new Lazy<bool>((Func<bool>) (() => primaryIdStillExists(primaryIdentitifier.LastPersistedValue)));
        this.lazyIsValid = new Lazy<bool>((Func<bool>) (() =>
        {
          if (string.IsNullOrWhiteSpace(primaryIdentitifier.LastPersistedValue))
            return false;
          return validateValue(primaryIdentitifier.LastPersistedValue) || primaryIdStillExists(primaryIdentitifier.LastPersistedValue);
        }));
      }

      internal bool PrimaryIdStillExists => this.lazyPrimaryIdStillExists.Value;
    }

    internal class MachineComboIdentitifier : IdentityInformationProvider.MachineIdentitifier
    {
      public MachineComboIdentitifier(
        Lazy<IPersistentPropertyBag> lazyIdStore,
        string name,
        Func<string> getCurrentValue,
        Func<IEnumerable<IdentityInformationProvider.MachineIdentitifier>> getComboIds)
        : base(lazyIdStore, name, IdentityInformationProvider.MachineIdentitifier.IdType.PrimaryCombinedId, getCurrentValue)
      {
        this.LastPersistedValue = IdentityInformationProvider.JoinIdentifiers(getComboIds(), (Func<IdentityInformationProvider.MachineIdentitifier, string>) (id => id.LastPersistedValue));
        this.getCurrentStoredValue = (Func<string>) (() => (string) null);
        this.lazyIsValid = new Lazy<bool>((Func<bool>) (() =>
        {
          lock (IdentityInformationProvider.LazyPiiPropertyProcessorLockObject)
            return !string.IsNullOrWhiteSpace(this.LastPersistedValue) && object.Equals((object) this.CurrentSystemValue, (object) IdentityInformationProvider.LazyPiiPropertyProcessor.Value.ConvertToHashedValue((object) new TelemetryPiiProperty((object) this.LastPersistedValue)));
        }));
      }
    }
  }
}
