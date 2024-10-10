using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.Json;
using System.Drawing;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Management;
using System.Diagnostics;


namespace demo_1
{
    public partial class Form1 : Form
    {
        private const string JsonFilePath = "savedapplication.json"; // Use a single JSON file name

        private bool shouldLaunchOnStartup = false; // Track if the app should launch other apps
        private ImageList imageListApp; // Define an ImageList

        private readonly string[] excludedKeywords = new string[] {
        "Runtime", "Targeting Pack", "Library", "Provider", "Host", "Setup", "Bootstrapper", "Appx Package",
        "Visual C++", "Update", "Redistributable", "WMI", "Framework"
    };

        public Form1()
        {
            InitializeComponent();
            EnsureJsonFileExists(); // Create JSON file if it doesn't exist
            imageListApp = new ImageList { ImageSize = new Size(32, 32) }; // Initialize ImageList once here
            InitializeListView();
            shouldLaunchOnStartup = Environment.GetCommandLineArgs().Contains("--launchapps");
        }
        private void InitializeListView()
        {
            // For displaying all applications
            listViewAllApps.Columns.Add("Application Name", 250);
            listViewAllApps.LargeImageList = imageListApp; // Set the ImageList here
            listViewAllApps.View = View.Details;
            listViewAllApps.CheckBoxes = true; // Allow selection

            // For displaying selected apps
            listViewSelectedApps.Columns.Add("Application Name", 250);
            listViewSelectedApps.LargeImageList = imageListApp; // Set the ImageList here
            listViewSelectedApps.View = View.Details;
            listViewSelectedApps.CheckBoxes = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataFromJson(); // Load saved applications when form loads
            LoadAllApplications(); // Load all applications
            AddApplicationsToListView();
            // If the application is started with the launch argument, launch apps and exit
            if (shouldLaunchOnStartup)
            {
                var apps = GetSavedApplications();
                if (apps != null) LaunchApplications(apps);
                Application.Exit();
            }
        }





        //--------Load Data from Json File----------//
        private void LoadDataFromJson()
        {
            try
            {
                var savedApplications = GetSavedApplications();

                if (savedApplications == null || savedApplications.Count == 0) return;

                imageListApp.Images.Clear();
                listViewSelectedApps.Items.Clear();

                foreach (var exePath in savedApplications)
                {
                    var item = new ListViewItem(Path.GetFileName(exePath))
                    {
                        Tag = exePath
                    };

                    var appIcon = GetIconFromExecutable(exePath);
                    if (appIcon != null)
                    {
                        imageListApp.Images.Add(exePath, appIcon);
                        item.ImageIndex = imageListApp.Images.Count - 1;
                    }
                    else
                    {
                        imageListApp.Images.Add(SystemIcons.Application); // Add the icon to the ImageList
                        item.ImageIndex = imageListApp.Images.Count - 1; // Set the ImageIndex to the last added icon
                    }

                    listViewSelectedApps.Items.Add(item);
                }
                listViewSelectedApps.LargeImageList = imageListApp;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading saved applications: {ex.Message}");
            }
        }
        private List<string> GetSavedApplications()
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(File.ReadAllText(JsonFilePath));
            }
            catch (Exception)
            {
                return null; // Return null if deserialization fails
            }
        }
        private Icon GetIconFromExecutable(string executablePath)
        {
            try
            {
                return Icon.ExtractAssociatedIcon(executablePath);
            }
            catch
            {
                return null;
            }
        }
        //=================//===============//






        //------------Load All Applications-----------//
        private void LoadAllApplications()
        {
            var applications = GetFilteredApplications();
            foreach (var app in applications)
            {
                // Filter out excluded applications
                if (!IsExcluded(app.Name))
                {
                    // Add the application to your ListView
                    ListViewItem item = new ListViewItem(app.Name)
                    {
                        Tag = app // Store the app object in the Tag property
                    };
                    // Add an icon if needed
                    // item.ImageIndex = GetImageIndexForApp(app); // Implement GetImageIndexForApp to set icons
                    listViewAllApps.Items.Add(item);
                }
            }
        }


        public List<ApplicationInfo> GetFilteredApplications()
        {
            var installedApplications = GetInstalledApplications();

            // Filter out system and irrelevant paths
            var filteredApps = installedApplications
                .Where(app => IsUserRelevantApplication(app.InstallLocation))
                .ToList();

            var recentApps = GetRecentlyUsedApplications();
            var jumpListApps = GetApplicationsFromJumpLists();

            // Combine all application names into a single HashSet for fast lookup
            var combinedAppNames = new HashSet<string>(filteredApps.Select(app => app.Name), StringComparer.OrdinalIgnoreCase);
            combinedAppNames.UnionWith(recentApps);
            combinedAppNames.UnionWith(jumpListApps);

            // Create a dictionary to keep track of unique applications by name
            var uniqueApps = new Dictionary<string, ApplicationInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var app in installedApplications)
            {
                if (combinedAppNames.Contains(app.Name))
                {
                    // Prioritize marking if found in recent or Jump List apps
                    if (recentApps.Any(ra => ra.Equals(app.Name, StringComparison.OrdinalIgnoreCase)) ||
                        jumpListApps.Any(ja => ja.Equals(app.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        app.IsPriority = true;  // Use a flag instead of modifying the name
                    }

                    // Add to dictionary if not already present
                    if (!uniqueApps.ContainsKey(app.Name))
                    {
                        uniqueApps.Add(app.Name, app);
                    }
                }
            }

            // Return only distinct applications
            return uniqueApps.Values.ToList();
        }

        // Method to get installed applications from the registry
        private List<ApplicationInfo> GetInstalledApplications()
        {
            var applicationList = new List<ApplicationInfo>();

            // Paths for installed applications (User and Local Machine)
            string[] registryKeys = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall"
        };

            // Search both HKEY_LOCAL_MACHINE and HKEY_CURRENT_USER for installed applications
            foreach (string key in registryKeys)
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key) ??
                                                 Registry.CurrentUser.OpenSubKey(key))
                {
                    if (registryKey == null)
                        continue;

                    foreach (string subkeyName in registryKey.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = registryKey.OpenSubKey(subkeyName))
                        {
                            try
                            {
                                var appName = subkey?.GetValue("DisplayName")?.ToString();
                                var installLocation = subkey?.GetValue("InstallLocation")?.ToString() ?? "";

                                if (!string.IsNullOrWhiteSpace(appName))
                                {
                                    applicationList.Add(new ApplicationInfo
                                    {
                                        Name = appName,
                                        InstallLocation = installLocation
                                    });
                                }
                            }
                            catch
                            {
                                continue;  // Handle cases where keys may be inaccessible
                            }
                        }
                    }
                }
            }

            return applicationList;
        }

        // Method to filter applications based on file paths or exclusion criteria
        private bool IsUserRelevantApplication(string installPath)
        {
            // Exclude paths that are system-related or contain certain keywords
            return !(installPath.StartsWith(@"C:\Windows") ||
                     installPath.StartsWith(@"C:\Program Files\Common Files") ||
                     installPath.Contains("Redistributable") ||
                     installPath.Contains("Framework") ||
                     string.IsNullOrWhiteSpace(installPath));
        }

        // Method to get recently used applications from Event Logs or Registry
        private List<string> GetRecentlyUsedApplications(int days = 100)
        {
            List<string> recentlyUsedApps = new List<string>();
            try
            {
                var eventLog = new EventLog("Application");

                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    if (entry.TimeWritten > DateTime.Now.AddDays(-days) && entry.EntryType == EventLogEntryType.Information)
                    {
                        string message = entry.Message;
                        if (message.Contains(".exe"))
                        {
                            string appName = message.Split(' ').FirstOrDefault(m => m.EndsWith(".exe"));
                            if (!string.IsNullOrEmpty(appName) && !recentlyUsedApps.Contains(appName))
                                recentlyUsedApps.Add(appName);
                        }
                    }
                }
            }
            catch
            {
                // In case accessing event logs fails, return an empty list
            }

            return recentlyUsedApps;
        }

        // Method to parse Jump List files and get applications from it
        private List<string> GetApplicationsFromJumpLists()
        {
            List<string> jumpListApps = new List<string>();
            string jumpListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                               @"Microsoft\Windows\Recent\AutomaticDestinations");

            if (Directory.Exists(jumpListPath))
            {
                var files = Directory.GetFiles(jumpListPath, "*.automaticDestinations-ms");

                foreach (string file in files)
                {
                    // Jump Lists are binary files, need a library like CS-ShellLink to parse
                    try
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            string content = sr.ReadToEnd();
                            var appName = Path.GetFileNameWithoutExtension(file);

                            if (!jumpListApps.Contains(appName))
                            {
                                jumpListApps.Add(appName);
                            }
                        }
                    }
                    catch
                    {
                        continue;  // Handle read exceptions gracefully
                    }
                }
            }

            return jumpListApps;
        }
        //====================//================//










        //--------------Save The Application-----------//
        private void SaveSelectedApplications()
        {
            try
            {
                // Retrieve previously saved applications
                var existingApps = GetSavedApplications() ?? new List<string>();

                // Get the newly selected applications from listViewAllApps
                var selectedApps = listViewAllApps.CheckedItems.Cast<ListViewItem>()
                    .Select(item => item.Tag.ToString())
                    .ToList();

                // Combine existing applications with newly selected ones
                var combinedApps = existingApps.Union(selectedApps).ToList(); // Use Union to avoid duplicates

                // Save the combined list to the JSON file
                File.WriteAllText(JsonFilePath, JsonConvert.SerializeObject(combinedApps, Formatting.Indented));
                MessageBox.Show("Configuration saved successfully.");

                // Uncheck all items in listViewAllApps after saving
                foreach (ListViewItem item in listViewAllApps.Items)
                {
                    item.Checked = false; // Uncheck the item
                }

                // Call SetStartup if needed
                SetStartup();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}");
            }
        }

        //==============//=========================//


   
        private bool IsExcluded(string displayName)
        {
            foreach (var keyword in excludedKeywords)
            {
                if (displayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        
        public class ApplicationInfo
        {
            public string Name { get; set; }
            public string InstallLocation { get; set; }
            public DateTime? LastUsed { get; set; }
            public bool IsPriority { get; set; } 


        }








        // Method to extract the base name from the display name
        private string GetBaseName(string displayName)
        {
            if (displayName.Contains("Microsoft Edge"))
                return "Microsoft Edge";

            return displayName;
        }

        // Method to get the primary executable file in the install location
        private string GetPrimaryExecutable(string installLocation, string baseName)
        {
            if (string.IsNullOrEmpty(installLocation) || !Directory.Exists(installLocation))
                return null;

            string[] exeFiles = Directory.GetFiles(installLocation, "*.exe", SearchOption.AllDirectories);

            // Additional fix for non-standard paths and executables
            if (exeFiles.Length == 0)
            {
                // If nothing is found, search in known custom paths like AppData\Local\Programs
                if (baseName.IndexOf("Code", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    exeFiles = Directory.GetFiles(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Microsoft VS Code"),
                        "*.exe",
                        SearchOption.AllDirectories
                    );
                }

                if (baseName.IndexOf("MySQL", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Corrected path for MySQL Workbench
                    string mysqlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "MySQL");
                    if (Directory.Exists(mysqlPath))
                    {
                        exeFiles = Directory.GetFiles(mysqlPath, "*.exe", SearchOption.AllDirectories);
                    }
                }
            }

            // Clean up the base name for matching (remove special characters and whitespace)
            string cleanedBaseName = Regex.Replace(baseName, @"[^a-zA-Z0-9]", string.Empty).ToLower();

            // Heuristic 1: Find executables with similar names to base name (e.g., 'msedge.exe' for 'Microsoft Edge')
            string primaryFile = exeFiles.FirstOrDefault(file =>
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file).ToLower();
                return fileNameWithoutExt.Contains(cleanedBaseName);
            });

            // If no good match is found, try additional matching patterns (fallbacks)
            if (primaryFile == null)
            {
                primaryFile = exeFiles.FirstOrDefault(file =>
                {
                    string fileName = Path.GetFileName(file).ToLower();

                    // Skip known irrelevant executables
                    if (fileName.Contains("uninstall") || fileName.Contains("setup") || fileName.Contains("update"))
                        return false;

                    // Use directory name as a hint
                    string directoryName = Path.GetFileName(Path.GetDirectoryName(file)).ToLower();
                    return directoryName.Contains(cleanedBaseName);
                });
            }

            // If no specific match found, fallback to selecting a reasonable first executable
            if (primaryFile == null)
            {
                primaryFile = exeFiles.FirstOrDefault(file =>
                {
                    string fileName = Path.GetFileName(file).ToLower();
                    return !fileName.Contains("uninstall") && !fileName.Contains("setup") && !fileName.Contains("update");
                });
            }

            return primaryFile;
        }








        // Method to check if an application should be excluded based on its display name
        private bool IsExcluded(string displayName, string[] excludedKeywords)
        {
            foreach (var keyword in excludedKeywords)
            {
                if (displayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }




   

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            // Remove selected applications from the selected applications ListView
            foreach (ListViewItem item in listViewSelectedApps.SelectedItems)
            {
                listViewSelectedApps.Items.Remove(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddApplicationsToListView(); // Populate ListView with installed applications
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSelectedApplications();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("The system will now restart. Do you want to continue?", "Restart Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start("shutdown", "/r /t 0");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to restart the system: {ex.Message}");
                }
            }
        }

        private void AddApplicationsToListView()
        {
            // Load the applications from JSON file
            var savedApplications = LoadApplicationsFromJson();

            listViewSelectedApps.Items.Clear();
            // Ensure the ImageList is set
            listViewSelectedApps.LargeImageList = imageListApp;

            // If saved applications are found, populate the ListView with them
            if (savedApplications != null && savedApplications.Count > 0)
            {
                foreach (var exeFile in savedApplications)
                {
                    Debug.WriteLine($"Processing file: {exeFile}"); // Log the file being processed

                    // Check if the file exists before trying to extract the icon
                    if (File.Exists(exeFile))
                    {
                        try
                        {
                            Icon appIcon = Icon.ExtractAssociatedIcon(exeFile);
                            if (appIcon != null)
                            {
                                imageListApp.Images.Add(appIcon);
                            }
                            else
                            {
                                // Use a default icon if extraction fails
                                imageListApp.Images.Add(SystemIcons.Application);
                            }

                            ListViewItem item = new ListViewItem(Path.GetFileName(exeFile))
                            {
                                Tag = exeFile,
                                ImageIndex = imageListApp.Images.Count - 1 // Assign the index of the added icon
                            };

                            listViewSelectedApps.Items.Add(item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error extracting icon for {exeFile}: {ex.Message}");
                            // Use a default icon if extraction fails
                            imageListApp.Images.Add(SystemIcons.Application);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {exeFile}");
                        // Use a default icon if the file is not found
                        imageListApp.Images.Add(SystemIcons.Application);
                    }
                }
            }
            else // If there are no saved applications, allow user to select from all applications
            {
                // Populate the ListView with selected applications from listViewAllApps
                foreach (ListViewItem item in listViewAllApps.CheckedItems)
                {
                    // Create a new ListViewItem for selected applications
                    ListViewItem selectedItem = new ListViewItem(item.Text)
                    {
                        Tag = item.Tag // Retain the executable path
                    };

                    // Ensure the item is not already in the selected apps ListView
                    if (listViewSelectedApps.Items.Cast<ListViewItem>().All(i => i.Tag.ToString() != selectedItem.Tag.ToString()))
                    {
                        // Load the icon from the executable
                        string exePath = selectedItem.Tag.ToString();
                        Debug.WriteLine($"Processing selected file: {exePath}"); // Log the file being processed

                        // Check if the file exists before trying to extract the icon
                        if (File.Exists(exePath))
                        {
                            try
                            {
                                Icon appIcon = Icon.ExtractAssociatedIcon(exePath);
                                if (appIcon != null)
                                {
                                    imageListApp.Images.Add(appIcon);
                                    selectedItem.ImageIndex = imageListApp.Images.Count - 1; // Assign the index of the added icon
                                }
                                else
                                {
                                    selectedItem.ImageIndex = imageListApp.Images.Count; // Use the default icon index
                                    imageListApp.Images.Add(SystemIcons.Application); // Default icon if extraction fails
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error extracting icon for {exePath}: {ex.Message}");
                                selectedItem.ImageIndex = imageListApp.Images.Count; // Use the default icon index
                                imageListApp.Images.Add(SystemIcons.Application); // Default icon if extraction fails
                            }
                        }
                        else
                        {
                            MessageBox.Show($"File not found: {exePath}");
                            selectedItem.ImageIndex = imageListApp.Images.Count; // Use the default icon index
                            imageListApp.Images.Add(SystemIcons.Application); // Default icon if the file is not found
                        }

                        // Add to the selected apps ListView
                        listViewSelectedApps.Items.Add(selectedItem);
                    }
                }
            }

            listViewSelectedApps.View = View.LargeIcon; // Set the view style
            listViewSelectedApps.Refresh(); // Refresh to display updates
        }


        // Method to load applications from the saved JSON file
        private List<string> LoadApplicationsFromJson()
        {
            try
            {
                string fullpath = Path.Combine(Application.StartupPath, "savedapplication.json");

                // Read the JSON data from the file
                string jsonData = File.ReadAllText(fullpath);
                return JsonConvert.DeserializeObject<List<string>>(jsonData) ?? new List<string>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading saved applications: {ex.Message}");
                return new List<string>(); // Return an empty list on error
            }
        }



        // Create the JSON file if it doesn't exist
        private void EnsureJsonFileExists()
        {
            string fullJsonFilePath = Path.Combine(Application.StartupPath, "savedapplication.json");

            if (!File.Exists(fullJsonFilePath))
            {
                File.WriteAllText(fullJsonFilePath, "[]"); // Create empty JSON array
            }
        }
        public class ApplicationData
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string IconKey { get; set; } // To link with imageListApps icons
        }
        // Load data from JSON into ListView
      

        // Launch applications from the saved list
        private void LaunchApplications(List<string> apps)
        {
            foreach (var app in apps)
            {
                try
                {
                    System.Diagnostics.Process.Start(app);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Failed to start application: {app}");
                }
            }
        }

        // Retrieve the saved applications list from JSON
      
     

        private void SetStartup()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key.GetValue("AppLauncher") == null)
                {
                    key.SetValue("AppLauncher", "\"" + Application.ExecutablePath + "\" --launchapps");
                    MessageBox.Show("Application set to start automatically on login.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting startup: {ex.Message}");
            }
        }
    }
}
