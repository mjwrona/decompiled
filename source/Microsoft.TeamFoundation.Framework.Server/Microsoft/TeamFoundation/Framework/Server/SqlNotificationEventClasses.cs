// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotificationEventClasses
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlNotificationEventClasses
  {
    public static readonly Guid QuotaDefinitionChanged = new Guid("28760041-C4EC-47F7-B164-FA338944BD4E");
    public static readonly Guid AccountQuotaValueChanged = new Guid("56F150AF-2545-4CB2-876F-DD266366F0EB");
    public static readonly Guid AadUserMembershipChanged = new Guid("B11B5215-4F1E-4C07-94A4-14BB46F6F7E6");
    public static readonly Guid RegistrySettingsChanged = new Guid("0E9C44D7-52EB-4DCA-B467-F1A4FE3C469A");
    internal static readonly Guid SecurityNamespaceTemplatesChanged = new Guid("72B87A7E-C51C-4524-A3B8-9E59441C7A3E");
    internal static readonly Guid SecurityTemplateEntriesChanged = new Guid("0198DD67-FEA5-4A4C-B708-0825BAC2CAE3");
    internal static readonly Guid SecuritySubjectEntriesChanged = new Guid("8581fea7-8025-492c-a8f6-5d1bd23d5a4f");
    internal static readonly Guid SecuritySystemStoreSequenceIdChanged = new Guid("3DB4C7F8-52A1-469B-BFF4-D7453D1B7869");
    internal static readonly Guid ServiceHostCreated = new Guid("843CAD1E-1579-43CA-8BCB-B76B27B00FC5");
    internal static readonly Guid ServiceHostDeleted = new Guid("3108B3AA-0ADF-4B50-AE01-B3501F2C8863");
    internal static readonly Guid ServiceHostModified = new Guid("ACA54753-9892-452C-977C-ED6082FAECCC");
    internal static readonly Guid ServiceHostLevelModified = new Guid("DD790E71-961B-4B52-81CE-77924DC45A12");
    internal static readonly Guid ServiceHostStatusChanged = new Guid("E51CDFC7-2F3A-425D-B0F0-D328C673DC52");
    internal static readonly Guid ServiceHostStatusChangedResponse = new Guid("B7FC502E-A132-4492-8A2E-ED9D5DB90BEA");
    internal static readonly Guid DatabasePropertiesChanged = new Guid("169F316A-B854-44B1-820E-E2E8BE00228E");
    internal static readonly Guid DatabaseRegistered = new Guid("DE867B0C-B2EE-45C6-9946-DCC5327C0EAF");
    internal static readonly Guid DatabaseRemoved = new Guid("D1F30672-63B7-4474-A79F-D953A6201C23");
    internal static readonly Guid FlushDatabaseCache = new Guid("DA996CD3-34A4-4FE1-A3E2-D5AD3A8D86AC");
    internal static readonly Guid ServiceHostInstanceFaultIn = new Guid("31B09FE7-633F-44B5-B66E-AC849F659BDC");
    public static readonly Guid CatalogDataChanged = new Guid("6578DEB8-C964-4690-8402-BEB9F1D07E72");
    internal static readonly Guid CatalogResourceChanged = new Guid("F0AE2209-D023-4882-A748-72ED7BA0CF3B");
    internal static readonly Guid WorkspaceChanged = new Guid("0228F77B-FF25-4452-A781-2CE5DC7816F8");
    internal static readonly Guid FileTypeChanged = new Guid("D95E4E91-BBBF-4a73-8F9B-1290970C4B4F");
    internal static readonly Guid TeamProjectChanged = new Guid("90483B76-965C-40d2-8C93-AC7F1D181835");
    public static readonly Guid LegacyProcessFieldDefinitionChanged = new Guid("599CB1CD-C3F8-4447-85AF-7FA028FC2747");
    public static readonly Guid ProcessMetadataFileChanged = new Guid("AF032F5E-7868-4710-AD45-73DCCDA8BB30");
    internal static readonly Guid FlushNotificationQueueRequest = new Guid("857EA54F-2CAD-493B-9092-FE74F36971DE");
    internal static readonly Guid IsHostLoaded = new Guid("388C344D-CE68-418A-90FB-39F012ED7CE6");
    internal static readonly Guid ClearAllSqlConnectionPools = new Guid("b51a1ee0-6360-446b-a754-294712f08bd0");
    internal static readonly Guid ForceGCRequested = new Guid("16A09090-8E8E-4D42-B864-46CC1219349C");
    internal static readonly Guid PublishTestAlertRequest = new Guid("cd2b292f-ce94-4d0b-aec0-338bbe38a2f7");
    internal static readonly Guid Recycle = new Guid("8422F546-6ACD-47E7-A941-E40BA084CA8E");
    internal static readonly Guid ProcessLeaseExpired = new Guid("E5F63BF4-940B-4E30-A23C-4CA248B823FD");
    public static readonly Guid IdentityChanged = new Guid("3043ED32-1AD3-4317-8F7E-67DCD6633CD2");
    internal static readonly Guid IMS2PlatformIdentityChanged = new Guid("03B1D4AF-84C1-4B06-9EB6-B442EAA250A2");
    internal static readonly Guid IMS2FrameworkIdentityMembershipChanged = new Guid("4ba7e8cd-8d78-4568-b178-56e27ba7c965");
    internal static readonly Guid IMS2FrameworkIdentityChanged = new Guid("AF00E432-6355-4EE4-AB9B-939A64601657");
    internal static readonly Guid IMS2ClearCache = new Guid("F4FB8E4D-7087-46C6-922B-D3114E8022B2");
    internal static readonly Guid IMS2PlatformIdentityIdTranslationChanged = new Guid("B778E0A2-81FA-4D32-B546-DA068DFFBA1D");
    internal static readonly Guid IMS2PlatformIdentityStorageKeyTranslationChanged = new Guid("024E5FCE-1B52-4FAB-8D87-8DF4A599321F");
    internal static readonly Guid IdentityKeyMapChanged = new Guid("A11DB455-6718-443E-BC1E-624D9EBEB9C6");
    internal static readonly Guid GroupChanged = new Guid("CE36891D-7475-44C8-8EC5-4990943988FD");
    internal static readonly Guid GroupMembershipChanged = new Guid("5DFAEBE8-E4FA-46B3-8408-383A40604DDF");
    internal static readonly Guid JobQueueChanged = new Guid("FBB8E1AF-7A27-4B8D-A6AC-693A2D0CD6C2");
    internal static readonly Guid JobTemplatesChanged = new Guid("3DB2CC86-FEE2-40E8-8C6A-7856983A1069");
    internal static readonly Guid LocationDataChanged = new Guid("4F50280D-C5B4-464a-824E-C9A2DD0FDF93");
    internal static readonly Guid PropertyKindChanged = new Guid("861BABE3-D0BA-444F-A28C-545F9509C661");
    internal static readonly Guid CounterDataChanged = new Guid("6b40a562-5598-4607-8666-14ee7e57027b");
    internal static readonly Guid DataspaceDataDeleted = new Guid("d91d168c-53d3-4ef1-aeb1-5a0313e80cd5");
    internal static readonly Guid DataspaceDataChanged = new Guid("57ed3650-5056-4bc1-a438-c958a8a4a4d8");
    internal static readonly Guid DataspacePartitionMapChanged = new Guid("8CC081B9-2D98-4E69-BB39-633AF5BEAEEF");
    public static readonly Guid SecurityDataChanged = new Guid("716A653A-F65A-4b89-93EF-D6BBC0622EC9");
    internal static readonly Guid SecurityDataChanged2 = new Guid("159D806F-DD4E-4F2A-B269-6AA40D90F01A");
    internal static readonly Guid SecurityNamespaceChanged = new Guid("CA073888-062C-47fb-938B-CD14CE8EB42B");
    internal static readonly Guid RemoteSecurityDataChanged = new Guid("CB71B79D-4C07-4392-89B1-C985BC4954CD");
    internal static readonly Guid RemoteSecurityDataChanged2 = new Guid("249D2DB8-55E7-463E-BA2E-3CDC88F49D76");
    internal static readonly Guid RemoteSecurityNamespacesChanged = new Guid("BE8D3C33-D4E9-4C2B-A146-A10566BCE35C");
    internal static readonly Guid SigningKeyChanged = new Guid("2F4FF11B-AC98-42F1-9F31-FAE7EDED2A69");
    internal static readonly Guid MessageQueueConnection = new Guid("B17F70A8-9F3C-4E73-87CD-185E139C0142");
    internal static readonly Guid MessageQueueDataAvailable = new Guid("F343C60D-B0DE-4553-B26D-11BDF46B8D84");
    internal static readonly Guid MessageQueueRegistrationChanged = new Guid("6A117D73-E987-4E28-BD55-5B924246EF54");
    internal static readonly Guid MessageQueueRegistrationReload = new Guid("5777751C-099D-4B86-BA92-98902A531772");
    internal static readonly Guid LicensingDataChanged = new Guid("9FBC02AF-F0D4-4E4D-9047-8ACA393336A0");
    internal static readonly Guid CollectionPreferenceChanged = new Guid("61B78E26-106E-4A5F-9536-877BB4341D84");
    internal static readonly Guid UserPreferenceChanged = new Guid("33F6F5CA-686E-4144-8F5D-0EF3EE5DE499");
    internal static readonly Guid UserInstalledLanguageChanged = new Guid("56D3C336-0D22-4A15-8915-318ECEAD997B");
    internal static readonly Guid WorkItemTrackingTreeChanged = new Guid("34B8C348-F677-4A6B-AAE3-2C4EDFC9E959");
    internal static readonly Guid ServiceVersionChanged = new Guid("8CBE5B20-9A74-42F2-A454-972D7996A62D");
    internal static readonly Guid ProjectSettingsChanged = new Guid("387D3273-A59F-48C8-A444-25F26116FBAE");
    public static readonly Guid ProjectsProcessMigrated = new Guid("5E25507C-621C-4419-B1C7-E60EF971DF7C");
    internal static readonly Guid ProjectDeleted = new Guid("83d68fbc-b4a5-4a8d-8699-ce237e5ec38b");
    internal static readonly Guid ProjectAdded = new Guid("ba7c1387-01d2-4118-9f18-c84adbebc264");
    internal static readonly Guid ProjectUpdated = new Guid("3f1179d1-b9d4-45ce-9e21-df30214d1794");
    internal static readonly Guid TeamSettingsChanged = new Guid("5bc78524-ea1a-411b-a30f-dd77b3c44a04");
    internal static readonly Guid TeamBoardCardSettingsChanged = new Guid("84267EF1-DFBD-4C5B-82E4-9A24E00292E7");
    internal static readonly Guid TeamBoardSettingsChanged = new Guid("3719C577-7D66-4516-9C10-A250A96BE289");
    internal static readonly Guid TeamIterationCapacityChanged = new Guid("CC2A19D1-9AB5-4B08-BF0D-75C26CA7C9E4");
    internal static readonly Guid ProjectBoardRowsChanged = new Guid("7D05454D-9A49-4EE5-B685-2CD4339C882D");
    internal static readonly Guid ProjectBoardColumnsChanged = new Guid("723EBCEC-25F5-48A0-80F5-7996B85797FD");
    internal static readonly Guid ProjectBoardOptionsChanged = new Guid("66C86DC6-E8C9-4EB4-8295-1386DE39D6C5");
    internal static readonly Guid BoardsDeleted = new Guid("7B8BC7CC-AEAE-4609-AD45-AB0FE4CBEB5A");
    internal static readonly Guid ResourceManagementSettingsChanged = new Guid("26F69FED-0A1F-48E5-A9AD-A912CA4EE320");
    public static readonly Guid UserAddedToAccount = new Guid("9992B8FC-39CB-4693-AF8E-E6F24BA43CD3");
    public static readonly Guid UserRemovedFromAccount = new Guid("3EEC27B9-2B96-4454-B019-42953ED61C0B");
    internal static readonly Guid BasicAuthChanged = new Guid("0E64F4A2-4B25-441E-A7F3-605BD3CC902A");
    public static readonly Guid OrganizationAccountChanged = new Guid("51A0A0E5-7AE5-46DA-92E7-E73C7F32FF35");
    public static readonly Guid OrganizationChanged = new Guid("252F9C02-123F-E30A-5741-37F46F9A2A3C");
    public static readonly Guid UserAccountMappingChanged = new Guid("D8078805-185E-4530-A4C5-38F790DDF2F0");
    public static readonly Guid TaggingDefinitionsChanged = new Guid("0944C602-3C46-41C5-AE10-B78E50A2C0BE");
    internal static readonly Guid KpiChanged = new Guid("43C12D23-0E71-4DF0-A1DA-69EA1C989131");
    public static readonly Guid ProfileChanged = new Guid("87795B18-3AC4-434A-9F75-DE98D1F71D6C");
    internal static readonly Guid StrongBoxItemChanged = new Guid("C22E50EE-EFDA-4D10-A50E-B4A73E14CBCC");
    internal static readonly Guid StrongBoxDrawerChanged = new Guid("4A694B5E-D5C1-4F0B-B829-99C57F87AE3E");
    internal static readonly Guid AppChanged = new Guid("95873B06-542E-415B-8836-3D91BF3D3E74");
    internal static readonly Guid AppDeleted = new Guid("ED35C7DE-207B-4382-937C-87948F6E854E");
    internal static readonly Guid AppStateChanged = new Guid("88D48513-65B7-443C-ABB5-6C1C663B2BC5");
    internal static readonly Guid AppStateDeleted = new Guid("88C8B87F-E5A7-4E66-822E-F6838865D82B");
    internal static readonly Guid DelegatedAuthorizationChanged = new Guid("6EC62F4D-4B39-4268-BFCF-076BBDC4A3D2");
    internal static readonly Guid TokenChanged = new Guid("96bb76e3-72f1-4822-acd7-04b38920c788");
    internal static readonly Guid ResyncResourceAreas = new Guid("9E96DFE4-4FAF-4121-9FCF-3D80E811AB92");
    internal static readonly Guid WhiteListedCommandChanged = new Guid("347a53f1-144a-4da4-9779-1fb9b76004e9");
    internal static readonly Guid WhiteListedExceptionChanged = new Guid("11b15363-9216-449f-aa38-7e9e4860d447");
    internal static readonly Guid InteractiveUserAgentChanged = new Guid("11be8a83-0614-4e8c-864e-257f9573a87f");
    internal static readonly Guid NameResolutionEntriesChanged = new Guid("6400AA43-65BA-4E69-8FED-9BB2C1A599F0");
    internal static readonly Guid ExtensionEntitlementChanged = new Guid("3B4740D0-C49C-422B-8119-9E8382E5BEC5");
    internal static readonly Guid ExtensionEntitlementChangedForPlatformCache = new Guid("DA1CA8E3-8951-45C5-A3B1-C46591146632");
    internal static readonly Guid UserEntitlementChanged = new Guid("3D364A7D-6FE5-45B0-94DF-79A4535F3E83");
    public static readonly Guid OfferSubscriptionChanged = new Guid("52BE55F6-CCFF-44DA-B6A4-976C0DE375E0");
    public static readonly Guid OfferMeterPriceChanged = new Guid("E97F7114-06C8-489D-B9EE-4D9B15CB3725");
    public static readonly Guid OfferSubscriptionTrialStatusChanged = new Guid("725D1C70-66F0-476E-999D-255DF1C5A8C4");
    public static readonly Guid OfferMeterChanged = new Guid("EA1EFB4C-EA97-4644-8BB8-D49D591C7C2D");
    public static readonly Guid IdentityRefreshTokenChanged = new Guid("7BBBC589-696E-45AB-AE6F-7ED514D7EC65");
    internal static readonly Guid TokenRevocationRuleTableChanged = new Guid("2FB48554-99D9-4E58-8048-5A24383D3C62");
    internal static readonly Guid IdentityFederatedProviderDataChanged = new Guid("E88C71C7-7BE7-436D-8523-BDAADC303E5B");
    public static readonly Guid L2SqlNotificationEvent = new Guid("4350cbc8-146c-41f3-bf4e-076c4b55222a");
    public static readonly Guid OAuthWhitelistChanged = new Guid("A14796A2-9464-42D7-BA58-97CBBCDD8FB1");
    internal static readonly Guid ScopeTemplateEntriesChanged = new Guid("D502AF76-3196-40DE-9E58-88A2235010E0");
    internal static readonly Guid UserSettingsChanged = new Guid("5F1F8F75-5EE5-4701-9421-F4D9B6CD2A62");
    internal static readonly Guid UserSettingsCacheUpdated = new Guid("B79FA76B-F5D2-426B-BE14-C53A05CC76DE");
  }
}
