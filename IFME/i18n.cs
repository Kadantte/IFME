﻿using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace IFME
{
    internal class i18n
    {
        // List installed languages
        public static Dictionary<string, string> Installed { get; set; } = new();

        public static void LoadLangFiles()
        {
            var langFiles = Directory.GetFiles("i18n", "*.json");

            foreach (var file in langFiles)
            {
                var lang = Path.GetFileNameWithoutExtension(file);

                try
                {
                    var culture = new CultureInfo(lang);
                    var displayName = culture.DisplayName;

                    Installed.Add(lang, displayName);

                    IFME.frmSplashScreen.PrintLogAppend($"i18n {displayName} by {GetLangAuthor(lang)[0]}");
                }
                catch (CultureNotFoundException)
                {
                    Installed.Add(lang, $"Unknown Language ({lang})");
                }
            }

            var langSetting = IFME.Properties.Settings.Default.UILanguage;
            var langDisplay = Thread.CurrentThread.CurrentUICulture.Name;
            var langSettingExists = Installed.TryGetValue(langSetting, out _);
            var langDisplayExists = Installed.TryGetValue(langDisplay, out _);

            if (langSettingExists)
            {
                IFME.Properties.Settings.Default.UILanguage = langSetting;
            }
            else if (langDisplayExists)
            {
                IFME.Properties.Settings.Default.UILanguage = langDisplay;
            }
            else
            {
                IFME.Properties.Settings.Default.UILanguage = "en-US";
            }

            IFME.Properties.Settings.Default.Save();
        }

        public static string[] GetLangAuthor(string currentLang = "eng")
        {
            var langFile = AppPath.Combine("i18n", $"{Path.GetFileNameWithoutExtension(currentLang)}.json");

            if (!File.Exists(langFile))
                return new string[] { "// Language File is Not Found", "// Error 19", "// Please check Json file exist at i18n folder" };

            var json = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile));

            if (json?.Forms == null)
                return new string[] { "// Json object is broken", "// Error 20", "// Please check that Json file formatting is valid" };

            return new string[] { json.AuthorName, json.AuthorProfile, json.AuthorEmail };
        }

        public static void Apply(Control parent, string formName, string currentLang = "eng")
        {
            var langDefault = AppPath.Combine("i18n", "en-US.json");
            var langFile = AppPath.Combine("i18n", $"{currentLang}.json");

            // When the default language file is not found, use the WinForm place holder as language
            if (!File.Exists(langDefault))
                return;

            // When the choosen language file is not found, use the default (en-US) language
            if (!File.Exists(langFile))
                langFile = langDefault;

            var json = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile));

            if (json?.Forms == null || !json.Forms.TryGetValue(formName, out var formStrings))
                return;

            // Store the current UI object in the memory for hidden controls
            i18nUI.Obj = json;

            var font = DefaultFont(json);

            foreach (Control ctrl in GetAllControls(parent))
            {
                if (IsLocalisable(ctrl))
                {
                    if (ctrl is ListView)
                    {
                        foreach (ColumnHeader header in ((ListView)ctrl).Columns)
                        {
                            if (formStrings.TryGetValue($"{ctrl.Name}{header.Index}", out string text))
                            {
                                header.Text = text;
                                ctrl.Font = font;
                            }
                        }
                    }
                    else
                    {
                        if (formStrings.TryGetValue(ctrl.Name, out string text))
                        {
                            ctrl.Text = text;
                            ctrl.Font = font;
                        }
                    }
                }
            }

            foreach (var cms in GetAllContextMenus((Form)parent))
            {
                foreach (ToolStripItem item in cms.Items)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        if (formStrings.TryGetValue(menuItem.Name, out string text))
                            menuItem.Text = text;
                    }
                }
            }
        }

        public static void Save(Control parent, string formName, string currentLang = "eng")
        {
            var langFile = AppPath.Combine("i18n", $"{currentLang}.json");

            i18nObj data;
            try
            {
                data = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile)) ?? new i18nObj();
            }
            catch
            {
                data = new i18nObj();
            }

            var formSorted = new SortedDictionary<string, string>();
            foreach (Control ctrl in GetAllControls(parent))
            {
                if (IsLocalisable(ctrl))
                {
                    if (ctrl is ListView)
                    {
                        foreach (ColumnHeader header in ((ListView)ctrl).Columns)
                        {
                            formSorted.Add($"{ctrl.Name}{header.Index}", header.Text);
                        }
                    }
                    else
                    {
                        formSorted.Add(ctrl.Name, ctrl.Text);
                    }
                }
            }

            foreach (var cms in GetAllContextMenus((Form)parent))
            {
                foreach (ToolStripItem item in cms.Items)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        formSorted.Add(menuItem.Name, menuItem.Text);
                    }
                }
            }

            try
            {
                data.Forms.Add(formName, formSorted);
            }
            catch
            {
                data.Forms[formName] = formSorted;
            }

            File.WriteAllText(langFile, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                foreach (Control child in GetAllControls(ctrl))
                    yield return child;

                yield return ctrl;
            }
        }

        private static IEnumerable<ContextMenuStrip> GetAllContextMenus(Form form)
        {
            var contextMenus = new List<ContextMenuStrip>();

            var componentsField = form.GetType().GetField("components",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (componentsField?.GetValue(form) is IContainer container)
            {
                foreach (var component in container.Components)
                {
                    if (component is ContextMenuStrip cms)
                        contextMenus.Add(cms);
                }
            }

            return contextMenus;
        }

        private static bool IsLocalisable(Control ctrl)
        {
            return ctrl is Label ||
                   ctrl is Button ||
                   ctrl is TabPage ||
                   ctrl is CheckBox ||
                   ctrl is RadioButton ||
                   ctrl is ListView ||
                   ctrl is GroupBox;
        }

        private static Font DefaultFont(i18nObj i18n)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return i18n.FontUIWindows;
            }
            else
            {
                return i18n.FontUILinux;
            }
        }
    }
}