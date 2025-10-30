using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ei8.Avatar.Installer.Application;
using ei8.Avatar.Installer.Application.Avatar;
using ei8.Avatar.Installer.Common;
using ei8.Avatar.Installer.Domain.Model.Configuration;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.Views;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.Validation;
using ei8.Avatar.Installer.Port.Adapter.UI.Maui.Validation.Rules;
using MetroLog.Maui;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ei8.Avatar.Installer.Port.Adapter.UI.Maui.ViewModels;

public partial class CreateAvatarViewModel : BaseViewModel
{
    #region Constants
    public static class CreateAvatarConstants
    {
        public const string ReadyToCreateAvatar = "Ready to create avatar...";
        public const string StartingAvatarCreation = "Starting avatar creation...";
        public const string AvatarCreationCompleted = "Avatar creation completed successfully!";
        public const string AvatarCreationFailed = "Avatar creation failed!";
        public const string SelectPrivateKeyFile = "Select Private Key File";
        public const string FixValidationErrors = "Please fix validation errors before creating avatar";
        public const string AvatarServerConfigurationMustContainAvatar = "AvatarServerConfiguration must contain at least one avatar";
        public const string PrivateKeyPathTemplate = "/C/keys/{0}";
        public const string JsonFileExtension = "json";
        
        // Validation Messages
        public const string AvatarIpRequired = "Avatar IP address is required";
        public const string InvalidIpFormat = "Invalid IP address format (e.g., 172.20.10.4)";
        public const string Un8yIpRequired = "Un8y IP address is required";
        public const string AvatarPortRequired = "Avatar port is required";
        public const string PortRangeInvalid = "Port must be between 1 and 65535";
        public const string Un8yBlazorPortRequired = "Un8y Blazor port is required";
        public const string KeysPathRequiredWhenEncryptionEnabled = "Keys path is required when encryption is enabled";
        public const string InProcessPrivateKeyPathRequired = "In Process Private Key Path is required when encryption is enabled";
        public const string PrivateKeyFileExtensionInvalid = "Private key file must have .key, .pem, .p12, or .pfx extension";
        public static readonly string[] PrivateKeyFileExtensions = { ".key", ".pem", ".p12", ".pfx" };
        public const string EncryptedEventsKeyRequired = "Encrypted Events Key is required when encryption is enabled";
    }
    #endregion

    #region Fields
    private readonly IAvatarApplicationService avatarApplicationService;
    private readonly IProgressService progressService;
    private AvatarServerConfiguration avatarServerConfiguration = new(string.Empty);
    #endregion

    #region Constructors
    public CreateAvatarViewModel(IAvatarApplicationService avatarApplicationService, IProgressService progressService) 
    {
        AssertionConcern.AssertArgumentNotNull(avatarApplicationService, nameof(avatarApplicationService));
        AssertionConcern.AssertArgumentNotNull(progressService, nameof(progressService));

        this.avatarApplicationService = avatarApplicationService;
        this.progressService = progressService;

        this.progressService.ProgressChanged += this.ProgressService_ProgressChanged;
        this.progressService.DescriptionChanged += this.ProgressService_DescriptionChanged;

        // Initialize ValidatableObject properties
        this.InitializeValidation();
    }
    #endregion

    #region Properties
    // Server Properties
    [ObservableProperty]
    private string serverName = string.Empty;
    [ObservableProperty]
    private string destination = string.Empty;

    // Avatar Properties
    [ObservableProperty]
    private string name = string.Empty;
    [ObservableProperty]
    private string ownerName = string.Empty;
    [ObservableProperty]
    private string ownerUserId = string.Empty;

    // Orchestration Properties (with validation)
    public ValidatableObject<string> AvatarIp { get; private set; }
    public ValidatableObject<string> Un8yIp { get; private set; }
    public ValidatableObject<string> AvatarInPort { get; private set; }
    public ValidatableObject<string> Un8yBlazorPort { get; private set; }
    public ValidatableObject<string> GraphPersistencePort { get; private set; }
    public ValidatableObject<string> KeysPath { get; private set; }

    // Un8y Properties
    [ObservableProperty]
    private string certificatePassword = string.Empty;
    [ObservableProperty]
    private string basePath = string.Empty;

    // Event Sourcing Properties
    [ObservableProperty]
    private bool encryptionEnabled = false;
    public ValidatableObject<string> InProcessPrivateKeyPath { get; private set; }
    [ObservableProperty]
    private string privateKeyPath = string.Empty;
    public ValidatableObject<string> EncryptedEventsKey { get; private set; }

    // Configuration File
    [ObservableProperty]
    private string configPath = string.Empty;

    // Tab Navigation Properties
    [ObservableProperty]
    private bool isEditTabSelected = true;
    [ObservableProperty]
    private bool isLogTabSelected = false;

    // Log Properties (for the log tab)
    [ObservableProperty]
    private string editorLogs = string.Empty;
    [ObservableProperty]
    private double creationProgress = 0.0;
    [ObservableProperty]
    private string loadingText = CreateAvatarConstants.ReadyToCreateAvatar;

    // Note: Validation errors are now handled by ValidatableObject<T>.Errors property
    #endregion

    #region Methods
    private void ProgressService_DescriptionChanged(object sender, EventArgs e)
    {
        this.LoadingText = this.progressService.Description;
    }

    private void ProgressService_ProgressChanged(object sender, EventArgs e)
    {
        this.CreationProgress = this.progressService.Progress;
    }

    // Initialize validation for ValidatableObject properties
    private void InitializeValidation()
    {
        // Initialize ValidatableObject properties
        this.AvatarIp = new ValidatableObject<string>();
        this.Un8yIp = new ValidatableObject<string>();
        this.AvatarInPort = new ValidatableObject<string>();
        this.Un8yBlazorPort = new ValidatableObject<string>();
        this.GraphPersistencePort = new ValidatableObject<string>();
        this.KeysPath = new ValidatableObject<string>();
        this.InProcessPrivateKeyPath = new ValidatableObject<string>();
        this.EncryptedEventsKey = new ValidatableObject<string>();

        // Add validation rules
        this.AddValidationRules();

        // Subscribe to property changes for configuration updates
        this.SubscribeToValidationPropertyChanges();
    }

    private void AddValidationRules()
    {
        // IP Address validation rules
        this.AvatarIp.Validations.Add(new IsNotNullOrEmptyRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.AvatarIpRequired 
        });
        this.AvatarIp.Validations.Add(new IpAddressRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.InvalidIpFormat 
        });

        this.Un8yIp.Validations.Add(new IsNotNullOrEmptyRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.Un8yIpRequired 
        });
        this.Un8yIp.Validations.Add(new IpAddressRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.InvalidIpFormat 
        });

        // Port validation rules
        this.AvatarInPort.Validations.Add(new IsNotNullOrEmptyRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.AvatarPortRequired 
        });
        this.AvatarInPort.Validations.Add(new PortNumberRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.PortRangeInvalid 
        });

        this.Un8yBlazorPort.Validations.Add(new IsNotNullOrEmptyRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.Un8yBlazorPortRequired 
        });
        this.Un8yBlazorPort.Validations.Add(new PortNumberRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.PortRangeInvalid 
        });

        // Graph persistence port validation
        this.GraphPersistencePort.Validations.Add(new IsNotNullOrEmptyRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.Un8yBlazorPortRequired 
        });
        this.GraphPersistencePort.Validations.Add(new PortNumberRule<string> 
        { 
            ValidationMessage = CreateAvatarConstants.PortRangeInvalid 
        });

        // Conditional validation for encryption-related fields
        this.KeysPath.Validations.Add(new ConditionalRule<string>(
            () => this.EncryptionEnabled,
            new IsNotNullOrEmptyRule<string> 
            { 
                ValidationMessage = CreateAvatarConstants.KeysPathRequiredWhenEncryptionEnabled 
            }
        ));

        this.InProcessPrivateKeyPath.Validations.Add(new ConditionalRule<string>(
            () => this.EncryptionEnabled,
            new IsNotNullOrEmptyRule<string> 
            { 
                ValidationMessage = CreateAvatarConstants.InProcessPrivateKeyPathRequired 
            }
        ));
        this.InProcessPrivateKeyPath.Validations.Add(new ConditionalRule<string>(
            () => this.EncryptionEnabled,
            new FileExtensionRule<string>(CreateAvatarConstants.PrivateKeyFileExtensions) 
            { 
                ValidationMessage = CreateAvatarConstants.PrivateKeyFileExtensionInvalid 
            }
        ));

        this.EncryptedEventsKey.Validations.Add(new ConditionalRule<string>(
            () => this.EncryptionEnabled,
            new IsNotNullOrEmptyRule<string> 
            { 
                ValidationMessage = CreateAvatarConstants.EncryptedEventsKeyRequired 
            }
        ));
    }

    private void SubscribeToValidationPropertyChanges()
    {
        this.AvatarIp.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.AvatarIp.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, o => o.AvatarIp = this.AvatarIp.Value);
            }
        };

        this.Un8yIp.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.Un8yIp.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, o => o.Un8yIp = this.Un8yIp.Value);
            }
        };

        this.AvatarInPort.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.AvatarInPort.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, (o, port) => o.AvatarInPort = port, this.AvatarInPort.Value, v => int.TryParse(v, out int port) ? port : 0);
            }
        };

        this.Un8yBlazorPort.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.Un8yBlazorPort.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, (o, port) => o.Un8yBlazorPort = port, this.Un8yBlazorPort.Value, v => int.TryParse(v, out int port) ? port : 0);
            }
        };

        this.GraphPersistencePort.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.GraphPersistencePort.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, (o, port) => o.GraphPersistencePort = port, this.GraphPersistencePort.Value, v => int.TryParse(v, out int port) ? port : 0);
            }
        };

        this.KeysPath.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.KeysPath.Validate();
                this.UpdateAvatarIfExists(a => a.Orchestration, o => o.KeysPath = this.KeysPath.Value);
            }
        };

        this.InProcessPrivateKeyPath.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.InProcessPrivateKeyPath.Validate();
                if (!string.IsNullOrEmpty(this.InProcessPrivateKeyPath.Value))
                {
                    var fileName = Path.GetFileName(this.InProcessPrivateKeyPath.Value);
                    this.PrivateKeyPath = string.Format(CreateAvatarConstants.PrivateKeyPathTemplate, fileName);
                }
                else
                {
                    this.PrivateKeyPath = string.Empty;
                }
                this.UpdateAvatarIfExists(a => a.EventSourcing, e => e.InProcessPrivateKeyPath = this.InProcessPrivateKeyPath.Value);
            }
        };

        this.EncryptedEventsKey.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ValidatableObject<string>.Value))
            {
                this.EncryptedEventsKey.Validate();
                this.UpdateAvatarIfExists(a => a.EventSourcing, e => e.EncryptedEventsKey = this.EncryptedEventsKey.Value);
            }
        };
    }

    // Helper methods to eliminate code redundancy
    private void UpdateAvatarIfExists<TSection>(Func<AvatarConfigurationItem, TSection> sectionSelector, Action<TSection> updateAction)
        where TSection : class
    {
        if (this.avatarServerConfiguration?.Avatars?.Count() > 0)
        {
            var avatar = this.avatarServerConfiguration.Avatars[0];
            var section = sectionSelector(avatar);
            if (section != null)
            {
                updateAction(section);
            }
        }
    }

    private void UpdateAvatarIfExists<TSection, TValue>(Func<AvatarConfigurationItem, TSection> sectionSelector, Action<TSection, TValue> updateAction, string value, Func<string, TValue> valueConverter)
        where TSection : class
    {
        if (this.avatarServerConfiguration?.Avatars?.Count() > 0)
        {
            var avatar = this.avatarServerConfiguration.Avatars[0];
            var section = sectionSelector(avatar);
            if (section != null)
            {
                var convertedValue = valueConverter(value);
                updateAction(section, convertedValue);
            }
        }
    }

    private void UpdateAvatarIfExists(Action<AvatarConfigurationItem> updateAction)
    {
        if (this.avatarServerConfiguration?.Avatars?.Count() > 0)
        {
            updateAction(this.avatarServerConfiguration.Avatars[0]);
        }
    }

    // Property change handlers for automatic configuration updates
    partial void OnNameChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.Name = value);
    }

    partial void OnOwnerNameChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.OwnerName = value);
    }

    partial void OnOwnerUserIdChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.OwnerUserId = value);
    }

    partial void OnServerNameChanged(string value)
    {
        this.avatarServerConfiguration.ServerName = value;
    }

    partial void OnDestinationChanged(string value)
    {
        this.avatarServerConfiguration.Destination = value;
    }

    partial void OnCertificatePasswordChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.Un8y, u => u.CertificatePassword = value);
    }

    partial void OnBasePathChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.Un8y, u => u.BasePath = value);
    }

    partial void OnEncryptionEnabledChanged(bool value)
    {
        this.UpdateAvatarIfExists(a => a.EventSourcing, e => e.EncryptionEnabled = value);
        
        // Clear encryption fields when encryption is disabled
        if (!value)
        {
            this.KeysPath.Value = string.Empty;
            this.InProcessPrivateKeyPath.Value = string.Empty;
            this.PrivateKeyPath = string.Empty;
            this.EncryptedEventsKey.Value = string.Empty;
        }

        // Re-validate encryption fields since the condition has changed
        this.KeysPath.Validate();
        this.InProcessPrivateKeyPath.Validate();
        this.EncryptedEventsKey.Validate();
    }

    partial void OnPrivateKeyPathChanged(string value)
    {
        this.UpdateAvatarIfExists(a => a.EventSourcing, e => e.PrivateKeyPath = value);
    }


    private bool ValidateAllFields()
    {
        // Validate all ValidatableObject properties
        bool isAvatarIpValid = this.AvatarIp.Validate();
        bool isUn8yIpValid = this.Un8yIp.Validate();
        bool isAvatarInPortValid = this.AvatarInPort.Validate();
        bool isUn8yBlazorPortValid = this.Un8yBlazorPort.Validate();
        bool isKeysPathValid = this.KeysPath.Validate();
        bool isGraphPersistencePortValid = this.GraphPersistencePort.Validate();
        bool isInProcessPrivateKeyPathValid = this.InProcessPrivateKeyPath.Validate();
        bool isEncryptedEventsKeyValid = this.EncryptedEventsKey.Validate();

        return isAvatarIpValid && isUn8yIpValid && isAvatarInPortValid && isUn8yBlazorPortValid &&
               isGraphPersistencePortValid && isKeysPathValid && isInProcessPrivateKeyPathValid && isEncryptedEventsKeyValid;
    }


    [RelayCommand]
    private async Task ChooseConfigurationAsync()
    {
        if (!this.IsBusy)
        {
            this.IsBusy = true;

            try
            {
                var configFile = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = ei8.Avatar.Installer.Common.Constants.Messages.ChooseConfig
                });

                if (configFile is not null)
                {
                    if (configFile.FileName.EndsWith(CreateAvatarConstants.JsonFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        this.ConfigPath = configFile.FullPath;
                        this.avatarServerConfiguration = await this.avatarApplicationService.ReadAvatarConfiguration(this.ConfigPath);
                        
                        AssertionConcern.AssertArgumentNotNull(this.avatarServerConfiguration.Avatars, nameof(this.avatarServerConfiguration.Avatars));
                        AssertionConcern.AssertArgumentTrue(this.avatarServerConfiguration.Avatars.Count() > 0, CreateAvatarConstants.AvatarServerConfigurationMustContainAvatar);
                        
                        // Populate all fields from configuration
                        this.ServerName = this.avatarServerConfiguration.ServerName;
                        this.Destination = this.avatarServerConfiguration.Destination;
                        
                        var avatar = this.avatarServerConfiguration.Avatars[0];
                        this.Name = avatar.Name;
                        this.OwnerName = avatar.OwnerName;
                        this.OwnerUserId = avatar.OwnerUserId;
                        
                        if (avatar.Orchestration != null)
                        {
                            this.AvatarIp.Value = avatar.Orchestration.AvatarIp;
                            this.Un8yIp.Value = avatar.Orchestration.Un8yIp;
                            this.AvatarInPort.Value = avatar.Orchestration.AvatarInPort.ToString();
                            this.Un8yBlazorPort.Value = avatar.Orchestration.Un8yBlazorPort.ToString();
                            this.GraphPersistencePort.Value = avatar.Orchestration.GraphPersistencePort.ToString();
                            this.KeysPath.Value = avatar.Orchestration.KeysPath;
                        }
                        
                        if (avatar.Un8y != null)
                        {
                            this.CertificatePassword = avatar.Un8y.CertificatePassword;
                            this.BasePath = avatar.Un8y.BasePath;
                        }
                        
                        if (avatar.EventSourcing != null)
                        {
                            this.EncryptionEnabled = avatar.EventSourcing.EncryptionEnabled;
                            this.InProcessPrivateKeyPath.Value = avatar.EventSourcing.InProcessPrivateKeyPath;
                            this.PrivateKeyPath = avatar.EventSourcing.PrivateKeyPath;
                            this.EncryptedEventsKey.Value = avatar.EventSourcing.EncryptedEventsKey;
                        }
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.DisplayAlert(Constants.Statuses.Invalid,
                            Constants.Messages.InvalidConfig, Constants.Prompts.Ok);
                    }
                }
                else
                {
                    await Shell.Current.CurrentPage.DisplayAlert(Constants.Statuses.Cancelled,
                        Constants.Messages.ChooseConfig, Constants.Prompts.Ok);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert(Constants.Statuses.Error, ex.ToString(), Constants.Prompts.Ok);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }

    // Helper method to eliminate folder picking code duplication
    private async Task<string> PickFolderAsync()
    {
        string result = string.Empty;
        
        try
        {
            var folder = await FolderPicker.PickAsync(default);
            result = folder.Folder?.Path ?? string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert(Constants.Statuses.Error, ex.ToString(), Constants.Prompts.Ok);
        }
        
        return result;
    }

    [RelayCommand]
    private async Task ChooseDestinationAsync()
    {
        var folderPath = await this.PickFolderAsync();
        
        if (!string.IsNullOrEmpty(folderPath))
        {
            this.Destination = folderPath;
        }
    }

    [RelayCommand]
    private async Task ChooseKeysPathAsync()
    {
        var folderPath = await this.PickFolderAsync();
        
        if (!string.IsNullOrEmpty(folderPath))
        {
            this.KeysPath.Value = folderPath;
        }
    }

    [RelayCommand]
    private async Task ChooseInProcessPrivateKeyPathAsync()
    {
        try
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = CreateAvatarConstants.SelectPrivateKeyFile
            });
            
            if (file is not null)
            {
                this.InProcessPrivateKeyPath.Value = file.FullPath;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert(Constants.Statuses.Error, ex.ToString(), Constants.Prompts.Ok);
        }
    }

    [RelayCommand]
    private void SelectEditTab()
    {
        this.IsEditTabSelected = true;
        this.IsLogTabSelected = false;
    }

    [RelayCommand]
    private void SelectLogTab()
    {
        this.IsEditTabSelected = false;
        this.IsLogTabSelected = true;
    }


    [RelayCommand]
    private async Task CreateAvatarAsync()
    {
        if (!this.IsBusy)
        {
            this.IsBusy = true;

            try
            {
                if (!string.IsNullOrEmpty(this.ConfigPath))
                {
                    // Validate all fields before creating avatar
                    if (this.ValidateAllFields())
                    {
                        // Switch to log tab to show progress
                        this.SelectLogTab();
                        
                        // Start avatar creation directly
                        await this.StartAvatarCreationAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert(Constants.Statuses.Invalid, CreateAvatarConstants.FixValidationErrors, Constants.Prompts.Ok);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert(Constants.Statuses.Invalid, Constants.Messages.ChooseConfig, Constants.Prompts.Ok);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert(Constants.Statuses.Error, ex.ToString(), Constants.Prompts.Ok);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }

    private async Task StartAvatarCreationAsync()
    {
        try
        {
            this.EditorLogs = string.Empty;
            this.LoadingText = CreateAvatarConstants.StartingAvatarCreation;

            await this.avatarApplicationService.CreateAvatarAsync(this.avatarServerConfiguration);
            
            this.LoadingText = CreateAvatarConstants.AvatarCreationCompleted;
            await Shell.Current.DisplayAlert(Constants.Statuses.Success, Constants.Messages.AvatarInstalled, Constants.Prompts.Ok);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.LoadingText = CreateAvatarConstants.AvatarCreationFailed;
            await Shell.Current.DisplayAlert(Constants.Statuses.Error, ex.ToString(), Constants.Prompts.Ok);
        }
        finally
        {
            // Update logs
            var logController = new LogController();
            var logList = await logController.GetLogList();
            logList!.Reverse();
            this.EditorLogs = string.Join(Environment.NewLine, logList);
        }
    }
    #endregion
}
