﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AddressCoding.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class GeneralSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static GeneralSettings defaultInstance = ((GeneralSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new GeneralSettings())));
        
        public static GeneralSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Theme {
            get {
                return ((string)(this["Theme"]));
            }
            set {
                this["Theme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanSaveDataAsShot {
            get {
                return ((bool)(this["CanSaveDataAsShot"]));
            }
            set {
                this["CanSaveDataAsShot"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanSaveDataAsFull {
            get {
                return ((bool)(this["CanSaveDataAsFull"]));
            }
            set {
                this["CanSaveDataAsFull"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanOpenFolderAfter {
            get {
                return ((bool)(this["CanOpenFolderAfter"]));
            }
            set {
                this["CanOpenFolderAfter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(";")]
        public string SeparatorChar {
            get {
                return ((string)(this["SeparatorChar"]));
            }
            set {
                this["SeparatorChar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanUseParsinglevelRus {
            get {
                return ((bool)(this["CanUseParsinglevelRus"]));
            }
            set {
                this["CanUseParsinglevelRus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanSaveFileWhithSelectedField {
            get {
                return ((bool)(this["CanSaveFileWhithSelectedField"]));
            }
            set {
                this["CanSaveFileWhithSelectedField"] = value;
            }
        }
    }
}
