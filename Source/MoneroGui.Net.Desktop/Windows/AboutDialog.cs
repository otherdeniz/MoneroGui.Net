﻿using Eto.Drawing;
using Eto.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class AboutDialog : Dialog
    {
        private readonly TextArea _textAreaLicense = new TextArea { ReadOnly = true };
        private readonly Button _buttonShowThirdPartyLicenses = new Button { Text = Desktop.Properties.Resources.AboutWindowThirdPartyLicenses };

        private TextArea TextAreaLicense {
            get { return _textAreaLicense; }
        }

        private Button ButtonShowThirdPartyLicenses {
            get { return _buttonShowThirdPartyLicenses; }
        }

        private static string LicenseText { get; set; }

        public AboutDialog()
        {
            this.SetWindowProperties(
                Desktop.Properties.Resources.AboutWindowTitle,
                new Size(600, 0)
            );

            RenderContent();

            if (LicenseText == null) {
                Task.Factory.StartNew(LoadLicenseText);
            } else {
                TextAreaLicense.Text = LicenseText;
            }

            CheckThirdPartyLicensesAvailability();
        }

        void RenderContent()
        {
            var linkButtonCreditIcons = new LinkButton { Text = "VisualPharm" };
            linkButtonCreditIcons.Click += delegate { Process.Start("http://www.visualpharm.com"); };

            ButtonShowThirdPartyLicenses.Click += delegate {
                if (CheckThirdPartyLicensesAvailability()) {
                    Process.Start(Utilities.PathDirectoryThirdPartyLicenses);
                }
            };

            var dynamicLayoutMain = new DynamicLayout { Padding = new Padding(Utilities.Padding6), Spacing = Utilities.Spacing6 };

            dynamicLayoutMain.BeginHorizontal();
            dynamicLayoutMain.BeginVertical();
            dynamicLayoutMain.AddCentered(new ImageView { Image = Utilities.LoadImage("Icon-192x192", true) });
            dynamicLayoutMain.EndVertical();

            dynamicLayoutMain.BeginVertical();
            dynamicLayoutMain.Add(new TableLayout(
                new TableRow(
                    new Label {
                        Text = Desktop.Properties.Resources.TextClientName,
                        TextAlignment = TextAlignment.Center,
                        Font = new Font(SystemFont.Default, Utilities.FontSize3)
                    }
                ),

                new TableRow(
                    new Label {
                        Text = "v" + Utilities.ApplicationVersionString,
                        TextAlignment = TextAlignment.Center
                    }
                ),

                new Panel { Height = Utilities.Padding3 },

                new TableRow(
                    TextAreaLicense
                ) { ScaleHeight = true },

                new Panel { Height = Utilities.Padding3 },

                new TableRow(
                    new TableRow(
                        new TableCell(
                            new TableLayout(
                                new TableRow(
                                    new TableCell(new Label { Text = Desktop.Properties.Resources.AboutWindowCreditIcons + " " }, true)
                                ),
                                new TableRow(
                                    new TableCell(linkButtonCreditIcons, true)
                                )
                            ),
                            true
                        ),

                        new TableCell(ButtonShowThirdPartyLicenses)
                    )
                )
            ));
            dynamicLayoutMain.EndVertical();
            dynamicLayoutMain.EndHorizontal();

            Content = dynamicLayoutMain;
        }

        void LoadLicenseText()
        {
            if (File.Exists(Utilities.PathFileLicense)) {
                using (var stream = new StreamReader(Utilities.PathFileLicense)) {
                    LicenseText = stream.ReadToEnd();
                    LicenseText = LicenseText.ReWrap();
                }
                Application.Instance.AsyncInvoke(() => TextAreaLicense.Text = LicenseText);

            } else {
                Application.Instance.AsyncInvoke(() => TextAreaLicense.Text = Desktop.Properties.Resources.AboutWindowLicenseFileNotFound);
            }
        }

        bool CheckThirdPartyLicensesAvailability()
        {
            var output = Directory.Exists(Utilities.PathDirectoryThirdPartyLicenses);
            ButtonShowThirdPartyLicenses.Enabled = output;
            return output;
        }
    }
}
