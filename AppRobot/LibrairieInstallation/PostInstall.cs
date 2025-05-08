using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace LibrairieInstallation
{
    [RunInstaller(true)]
    public class PostInstall : Installer
    {
        /// <summary>
        /// This method is called after the installation of the component.
        /// </summary>
        /// <param name="savedState"></param>
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            string targetDir = Context.Parameters["targetdir"];
            string exePath = Path.Combine(targetDir, "InitialisationBD.exe");

            if (File.Exists(exePath))
                Process.Start(exePath);
        }
    }
}