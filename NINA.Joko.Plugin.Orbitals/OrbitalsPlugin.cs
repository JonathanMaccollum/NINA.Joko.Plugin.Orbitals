﻿#region "copyright"

/*
    Copyright © 2021 - 2021 George Hilios <ghilios+NINA@googlemail.com>

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using NINA.Joko.Plugin.Orbitals.Properties;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile.Interfaces;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.IO;
using NINA.Joko.Plugin.Orbitals.Calculations;
using System.Threading;
using System.Globalization;
using NINA.Core.Utility;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;
using System;
using System.Reflection;

namespace NINA.Joko.Plugin.Orbitals {

    [Export(typeof(IPluginManifest))]
    public class OrbitalsPlugin : PluginBase {
        public static readonly string OrbitalElementsDirectory = Path.Combine(CoreUtil.APPLICATIONTEMPPATH, "OrbitalElements");

        [ImportingConstructor]
        public OrbitalsPlugin(IProfileService profileService) {
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                Settings.Default.Save();
            }

            if (!Directory.Exists(OrbitalElementsDirectory)) {
                Directory.CreateDirectory(OrbitalElementsDirectory);
            }

            if (OrbitalsOptions == null) {
                OrbitalsOptions = new OrbitalsOptions(profileService);
            }

            if (OrbitalElementsAccessor == null) {
                OrbitalElementsAccessor = new OrbitalElementsAccessor();
            }

            if (JPLAccessor == null) {
                JPLAccessor = new JPLAccessor();
            }

            if (SystemCultureInfo == null) {
                var userDefaultCultureProp = typeof(CultureInfo).GetProperty("UserDefaultUICulture", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic);
                var defaultCulture = (CultureInfo)userDefaultCultureProp?.GetValue(null);
                SystemCultureInfo = defaultCulture ?? CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;
            }

            ResetOptionDefaultsCommand = new RelayCommand(OrbitalsOptions.ResetDefaults);
        }

        public static OrbitalsOptions OrbitalsOptions { get; private set; }

        public static OrbitalElementsAccessor OrbitalElementsAccessor { get; private set; }

        public static JPLAccessor JPLAccessor { get; private set; }

        public static CultureInfo SystemCultureInfo { get; private set; }

        public ICommand ResetOptionDefaultsCommand { get; private set; }
    }
}