using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Polybridge_2_Mod_Loader
{
    public partial class Main : Form
    {
        public string GameDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Poly Bridge 2";
        public string CurrentDownloadName = "";
        public string PTFName = "";

        public const string GoogleSheetID = "<Google Sheet Id Here>";
        public const string GoogleAPIToken = "<Google API Token Here>";

        public const string ModLoaderLink = "https://api.github.com/repos/Bram2323/PB-Mod-Loader/releases/latest";
        public const string BepInExLink = "https://api.github.com/repos/BepInEx/BepInEx/releases/latest";
        public const string PTFLink = "https://api.github.com/repos/PolyTech-Modding/PolyTechFramework/releases/latest";

        private List<ModData> AllMods = new List<ModData>();

        private ModData InstalledMod = null;

        private Dictionary<ModData, ModLabels> AllLabels = new Dictionary<ModData, ModLabels>();
        private List<Button> AllButtons = new List<Button>();
        public int[] TableStartPos = new int[] { 0, 30 };
        public int[] TableSize = new int[] { 1200, 100 };

        public List<string> InstalledMods = new List<string>();

        private bool ButtonsActive = true;

        public const string Version = "1.0.0";


        public Main()
        {
            InitializeComponent();
            Text = Text + " - v" + Version;

            
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "Nothing");

                string address = ModLoaderLink;
                string s;

                s = webClient.DownloadString(address);

                GithubRoot response = JsonSerializer.Deserialize<GithubRoot>(s);

                if (response.GetVersion().CompareTo(new System.Version(Version)) > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("A new version of the mod loader is available!\nDownload new version?", "New version available!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        OpenUrl(response.assets_url);
                    }
                }
            }
            catch { }

            GameDirectoryLabel.Text = "Selected Folder: \"" + GameDirectory + "\"";

            InstalledMods = GetInstalledMods();

            GetModsList.RunWorkerCompleted += GetModsList_Done;
            GetModsList.RunWorkerAsync();

            SizeChanged += delegate { Window_SizeChanged(); };
            Search.Click += delegate { SearchForMods(); };
            SearchBar.KeyDown += new KeyEventHandler(SearchBar_KeyDown);
        }



        private void GetModsList_DoWork(object sender, DoWorkEventArgs e)
        {
            ButtonsSetActive(false);


            GoogleSheetInfoRoot sheetInfo = GetSheetInfo(GoogleSheetID);

            if (sheetInfo == null || sheetInfo.sheets.Count < 1)
            {
                MessageBox.Show("Something went wrong while trying to get the list of mods!\nFailed to get the spreadsheet data");
                ButtonsSetActive(true);
                return;
            }

            int rowCount = sheetInfo.sheets[0].properties.gridProperties.rowCount;
            int columnCount = sheetInfo.sheets[0].properties.gridProperties.columnCount;


            GoogleValuesRoot AllCells = GetCellsFromSpreadsheet(GoogleSheetID, "A1", ToBase26(columnCount) + rowCount);

            if (AllCells == null || AllCells.values == null || AllCells.values.Count < 1 || AllCells.values[0] == null || AllCells.values[0].Count < 1)
            {
                MessageBox.Show("Something went wrong while trying to get the list of mods!\nFailed to get cells data");
                ButtonsSetActive(true);
                return;
            }

            string[] settingStrings = AllCells.values[0][0].Split(',');
            int[] settings = new int[settingStrings.Length];

            if (settingStrings.Length < 9)
            {
                MessageBox.Show("Something went wrong while trying to get the list of mods!\nSheet settings are not right!");
                ButtonsSetActive(true);
                return;
            }
            for (int i = 0; i < settings.Length; i++)
            {
                string settingStr = settingStrings[i];
                if (!int.TryParse(settingStr, out int setting))
                {
                    MessageBox.Show("Something went wrong while trying to get the list of mods!\nCould not parse data");
                    ButtonsSetActive(true);
                    return;
                }
                settings[i] = setting;
            }

            List<List<string>> Cells = AllCells.values;

            for (int i = settings[0] - 1; i < rowCount; i++)
            {
                if (Cells[i].Count >= 9)
                {
                    ModData data = new ModData();
                    List<string> stringData = Cells[i];

                    if (AnalyseDLLName(stringData[settings[8]], out bool NotSuported, out bool MoreFiles, out bool Absolete, out string DllName))
                    {
                        data.dllName = DllName;
                        data.NotSuported = NotSuported;
                        data.MoreFiles = MoreFiles;
                        data.Absolete = Absolete;

                        data.Name = stringData[settings[1]];
                        data.Author = stringData[settings[2]];
                        data.Synopsis = stringData[settings[3]];
                        data.ReadMe = stringData[settings[4]].Replace("/releases", "").Replace(" ", "").Replace("\n", "");
                        data.ReadMeLink = stringData[settings[4]].Replace("/releases", "").Replace(" ", "").Replace("\n", "");
                        data.Tags = stringData[settings[5]];
                        data.Cheat = stringData[settings[6]];
                        data.Updated = stringData[settings[7]];

                        data.NotSuported = data.NotSuported || !stringData[settings[4]].Contains("https://github.com/");
                        data.GitLink = (stringData[settings[4]].Replace("github.com", "api.github.com/repos") + "/latest").Replace(" ", "").Replace("\n", "");

                        AllMods.Add(data);
                    }
                    else
                    {
                        break;
                    }
                }
                else break;
            }

            ButtonsSetActive(true);
        }

        public bool AnalyseDLLName(string name, out bool NotSuported, out bool MoreFiles, out bool Absolete, out string DllName)
        {
            name = name.ToLower();

            NotSuported = false;
            MoreFiles = false;
            Absolete = false;
            DllName = "";

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            else if (name.EndsWith(" -m"))
            {
                if (AnalyseDLLName(name.Replace(" -m", ""), out bool notSup, out bool moreFile, out bool abso, out string dll))
                {
                    NotSuported = notSup;
                    MoreFiles = true;
                    Absolete = abso;
                    DllName = dll;
                    return true;
                }
                else return false;
            }
            else if (name.EndsWith(" -a"))
            {
                if (AnalyseDLLName(name.Replace(" -a", ""), out bool notSup, out bool moreFile, out bool abso, out string dll))
                {
                    NotSuported = notSup;
                    MoreFiles = moreFile;
                    Absolete = true;
                    DllName = dll;
                    return true;
                }
                else return false;
            }
            else
            {
                name = ConvertToAllowedDLLName(name);
                if (name.Contains("private") || name.Contains("n/a"))
                {
                    return false;
                }
                NotSuported = !name.EndsWith(".dll");
                DllName = name;
                return true;
            }
        }

        public string ConvertToAllowedDLLName(string old)
        {
            old = old.Replace(" ", "").Replace("\n", "");
            for (int i = 0; i < 10; i++) old = old.Replace(i.ToString(), "x");
            old = old.ToLower();
            return old;
        }

        public GoogleValuesRoot GetCellsFromSpreadsheet(string sheetID, string begin, string end)
        {
            try
            {
                WebClient webClient = new WebClient();

                string address = "https://sheets.googleapis.com/v4/spreadsheets/" + sheetID + "/values/" + begin + ":" + end + "?key=" + GoogleAPIToken;
                string s;

                s = webClient.DownloadString(address);

                GoogleValuesRoot response = JsonSerializer.Deserialize<GoogleValuesRoot>(s);

                return response;
            }
            catch
            {
                return null;
            }
        }

        public GoogleSheetInfoRoot GetSheetInfo(string sheetID)
        {
            try
            {
                WebClient webClient = new WebClient();

                string address = "https://sheets.googleapis.com/v4/spreadsheets/" + sheetID + "?key=" + GoogleAPIToken;
                string s;

                s = webClient.DownloadString(address);

                GoogleSheetInfoRoot response = JsonSerializer.Deserialize<GoogleSheetInfoRoot>(s);

                return response;
            }
            catch
            {
                return null;
            }
        }

        public void GetModsList_Done(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (ModData mod in AllMods)
            {
                AddModToList(mod);
            }
        }



        public void AddModToList(ModData data)
        {
            if (AllLabels.ContainsKey(data)) return;

            ModLabels labels = new ModLabels();

            bool installed = IsModInstalled(data);

            int panelSize = TableSize[0] / 8;

            Panel[] panels = new Panel[8];
            for (int i = 0; i < 8; i++)
            {
                Panel panel = new Panel();
                panel.Location = new Point(TableStartPos[0] + (panelSize * i), TableStartPos[1] + TableSize[1] * AllLabels.Count);
                panel.Size = new Size(panelSize, TableSize[1]);
                panel.BorderStyle = BorderStyle.FixedSingle;
                if (AllLabels.Count % 2 == 0) panel.BackColor = Color.FromArgb(230, 230, 230);
                if (data.Absolete) panel.BackColor = Color.LightYellow;
                else if (data.NotSuported) panel.BackColor = Color.DarkGray;
                ModsListPanel.Controls.Add(panel);
                panels[i] = panel;
            }

            Point defLoc = new Point(3, 3);
            Size defSize = new Size(panelSize - 6, TableSize[1] - 6);

            Size buttonSize = new Size(panelSize - 8, TableSize[1] / 2 - 5);
            string text = (installed ? "Update/Reinstall Mod" : "Install Mod") + (data.MoreFiles ? "\n(needs additional files)" : "");
            if (data.NotSuported) text = "Not Suported by mod loader!";
            Button install = CreateButton(defLoc, buttonSize, text);
            install.BackColor = Color.FromArgb(230, 230, 230);
            install.Enabled = ButtonsActive;
            install.Click += delegate { InstallModFromData(data); };
            panels[0].Controls.Add(install);
            AllButtons.Add(install);

            Point deleteLoc = new Point(3, TableSize[1] / 2);
            Button delete = CreateButton(deleteLoc, buttonSize, "Delete Mod");
            delete.BackColor = Color.FromArgb(230, 230, 230);
            delete.Visible = installed;
            delete.Enabled = ButtonsActive;
            delete.Click += delegate { DeleteModFromData(data); };
            panels[0].Controls.Add(delete);
            AllButtons.Add(delete);

            Label name = CreateLabel(defLoc, defSize, data.Name);
            panels[1].Controls.Add(name);

            Label author = CreateLabel(defLoc, defSize, data.Author);
            panels[2].Controls.Add(author);

            Label synopsis = CreateLabel(defLoc, defSize, data.Synopsis);
            panels[3].Controls.Add(synopsis);

            LinkLabel readMe = CreateLinkLabel(defLoc, defSize, data.ReadMe, data.ReadMeLink);
            panels[4].Controls.Add(readMe);

            Label tags = CreateLabel(defLoc, defSize, data.Tags);
            panels[5].Controls.Add(tags);

            Label cheat = CreateLabel(defLoc, defSize, data.Cheat);
            panels[6].Controls.Add(cheat);

            Label updated = CreateLabel(defLoc, defSize, data.Updated);
            panels[7].Controls.Add(updated);

            labels.Panels = panels;
            labels.InstallButton = install;
            labels.DeleteButton = delete;
            labels.Name = name;
            labels.Author = author;
            labels.Synopsis = synopsis;
            labels.ReadMe = readMe;
            labels.Tags = tags;
            labels.Cheat = cheat;
            labels.Updated = updated;

            AllLabels.Add(data, labels);
        }

        public void RemoveModFromList(ModData data)
        {
            if (AllLabels.ContainsKey(data))
            {
                ModLabels labels = AllLabels[data];

                AllButtons.Remove(labels.InstallButton);
                AllButtons.Remove(labels.DeleteButton);

                for (int i = 0; i < labels.Panels.Length; i++)
                {
                    Panel panel = labels.Panels[i];
                    panel.Parent.Controls.Remove(panel);
                }

                AllLabels.Remove(data);
            }
        }

        public void UpdateModInList(ModData data)
        {
            if (AllLabels.ContainsKey(data))
            {
                ModLabels labels = AllLabels[data];
                bool installed = IsModInstalled(data);

                string text = (installed ? "Update/Reinstall Mod" : "Install Mod") + (data.MoreFiles ? "\n(needs additional files)" : "");
                if (data.NotSuported) text = "Not Suported by mod loader!";
                labels.InstallButton.Invoke(new Action(() => labels.InstallButton.Text = text));
                labels.DeleteButton.Invoke(new Action(() => labels.DeleteButton.Visible = installed));
            }
        }

        public void RemoveEverythingFromList()
        {
            List<ModData> Keys = AllLabels.Keys.ToList();

            for (int i = AllLabels.Keys.Count - 1; i >= 0; i--)
            {
                RemoveModFromList(Keys[i]);
            }
        }



        private void SearchForMods()
        {
            if (!ButtonsActive) return;

            string search = SearchBar.Text.ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                RemoveEverythingFromList();
                foreach (ModData mod in AllMods)
                {
                    AddModToList(mod);
                }
            }
            else
            {
                RemoveEverythingFromList();
                foreach (ModData mod in AllMods)
                {
                    if (mod.Name.ToLower().Contains(search)) AddModToList(mod);
                    else if (mod.Author.ToLower().Contains(search)) AddModToList(mod);
                    else if (mod.Synopsis.ToLower().Contains(search)) AddModToList(mod);
                    else if (mod.Tags.ToLower().Contains(search)) AddModToList(mod);
                    else if (mod.Updated.ToLower().Contains(search)) AddModToList(mod);
                }
            }
        }

        private void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchForMods();
            }
        }




        public bool IsModInstalled(ModData data)
        {
            foreach (string name in InstalledMods)
            {
                if (name == data.dllName) return true;
            }

            return false;
        }

        public List<string> GetInstalledMods()
        {
            List<string> mods = new List<string>();

            string pluginsPath = Path.Combine(GameDirectory, @"BepInEx\plugins");
            if (Directory.Exists(pluginsPath))
            {
                foreach (string path in Directory.GetFiles(pluginsPath))
                {
                    if (Path.GetExtension(path) == ".dll") mods.Add(ConvertToAllowedDLLName(Path.GetFileName(path)));
                }
            }

            return mods;
        }


        public void ButtonsSetActive(bool active)
        {
            ButtonsActive = active;

            OpenDirectory.Invoke(new Action(() => OpenDirectory.Enabled = active));
            InstallEverythingButton.Invoke(new Action(() => InstallEverythingButton.Enabled = active));
            Search.Invoke(new Action(() => Search.Enabled = active));

            foreach (Button button in AllButtons)
            {
                button.Invoke(new Action(() => button.Enabled = active));
            }
        }



        public void InstallModFromData(ModData data)
        {
            if (data.NotSuported)
            {
                DialogResult dialogResult = MessageBox.Show("File type of this mod is not supported by the mod loader!\nOpen GitHub page to download this mod manualy?", "Not supported!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    OpenUrl(data.ReadMeLink += "/releases/latest");
                }
                return;
            }
            else if (data.Absolete)
            {
                DialogResult dialogResult = MessageBox.Show("This mod is absolete and may not work!\nDownload anyway?", "Absolete!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No) return;
            }

            if (InstalledMod != null) return;
            ButtonsSetActive(false);
            InstalledMod = data;
            InstallMod.RunWorkerAsync();
        }

        private void InstallMod_DoWork(object sender, DoWorkEventArgs e)
        {
            ModData mod = InstalledMod;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Nothing");
            string address = mod.GitLink;
            string s;
            try
            {
                SetTextProgressLabel("Getting Latest Version Of " + mod.Name);
                s = webClient.DownloadString(address);

                GithubRoot response = JsonSerializer.Deserialize<GithubRoot>(s);

                string assetURL = "";
                string name = "";
                foreach (Asset asset in response.assets)
                {
                    if (ConvertToAllowedDLLName(asset.name) == mod.dllName)
                    {
                        assetURL = asset.browser_download_url;
                        name = asset.name;
                    }
                }

                if (string.IsNullOrWhiteSpace(assetURL))
                {
                    MessageBox.Show("Could not find version of " + mod.Name);
                    InstalledMod = null;
                    ButtonsSetActive(true);
                    return;
                }

                CurrentDownloadName = name;
                webClient.DownloadProgressChanged += DownloadProgressChanged;
                webClient.DownloadDataCompleted += DownloadModCompleted;
                webClient.DownloadDataAsync(new Uri(assetURL));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetTextProgressLabel("Download Failed");
                InstalledMod = null;
                ButtonsSetActive(true);
            }
        }

        private void DownloadModCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            ModData mod = InstalledMod;

            SetTextProgressLabel("Installing ");
            byte[] modBytes = e.Result;

            try
            {
                string path = GameDirectory + "/BepInEx/plugins/" + CurrentDownloadName;
                File.WriteAllBytes(path, modBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download mod!\n" + ex.Message);
                SetTextProgressLabel("Download Failed");
                SetValueProgressBar(0);
                InstalledMod = null;
                ButtonsSetActive(true);
                return;
            }

            if (mod.MoreFiles)
            {
                DialogResult dialogResult = MessageBox.Show("You may need more files for this mod to function!\nOnly downloaded: '" + CurrentDownloadName + "'\nOpen GitHub page to download them manualy?", "More files needed!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    OpenUrl(mod.ReadMeLink += "/releases/latest");
                }
                else
                {
                    MessageBox.Show("The mod may break the game without the extra files!");
                }
            }

            InstalledMods = GetInstalledMods();
            UpdateModInList(mod);
            SetTextProgressLabel("Download Completed");
            SetValueProgressBar(0);
            InstalledMod = null;
            ButtonsSetActive(true);
        }

        public void DeleteModFromData(ModData data)
        {
            try
            {
                string pluginsPath = Path.Combine(GameDirectory, @"BepInEx\plugins");
                if (Directory.Exists(pluginsPath))
                {
                    foreach (string path in Directory.GetFiles(pluginsPath))
                    {
                        if (ConvertToAllowedDLLName(Path.GetFileName(path)) == data.dllName)
                        {
                            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete " + data.Name + "?", "Delete " + data.Name + "?", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }

                InstalledMods = GetInstalledMods();
                UpdateModInList(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete mod!\n" + ex.Message);
            }
        }



        private void OpenDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = @"C:\Program Files (x86)\Steam\steamapps\common\Poly Bridge 2";
                fbd.Description = "Select the game folder";
                fbd.ShowNewFolderButton = false;
                fbd.UseDescriptionForTitle = true;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    GameDirectory = fbd.SelectedPath;
                }
            }

            InstalledMods = GetInstalledMods();
            RemoveEverythingFromList();
            foreach (ModData mod in AllMods) AddModToList(mod);
            GameDirectoryLabel.Text = "Selected Folder: \"" + GameDirectory + "\"";
        }


        private void InstallBepInEx_Click(object sender, EventArgs e)
        {
            ButtonsSetActive(false);
            SetValueProgressBar(0);

            if (string.IsNullOrWhiteSpace(GameDirectory))
            {
                MessageBox.Show("You have to open the game folder to install BepInEx and the Poly Tech Framework!");
                ButtonsSetActive(true);
            }
            else if (!Directory.Exists(GameDirectory))
            {
                MessageBox.Show(string.Format("Could not find folder \"{0}\"!\nMake sure you have selected the right folder!", GameDirectory));
                ButtonsSetActive(true);
            }
            else
            {
                installBepInEx.RunWorkerAsync();
            }
        }

        public void SetTextProgressLabel(string text)
        {
            ProgressLabel.Invoke(new Action(() => ProgressLabel.Text = text));
        }

        public void SetValueProgressBar(int value)
        {
            DownloadProgressBar.Invoke(new Action(() => DownloadProgressBar.Value = value));
        }

        private void installBepInEx_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Nothing");
            string address = BepInExLink;
            string s;
            try
            {
                SetTextProgressLabel("Getting Latest Version Of BepInEx");
                s = webClient.DownloadString(address);

                GithubRoot response = JsonSerializer.Deserialize<GithubRoot>(s);

                string assetURL = "";
                string name = "";
                foreach (Asset asset in response.assets)
                {
                    if (asset.name.Contains("x64"))
                    {
                        assetURL = asset.browser_download_url;
                        name = asset.name;
                    }
                }

                if (string.IsNullOrWhiteSpace(assetURL))
                {
                    MessageBox.Show("Could not find version of BepInEx");
                    ButtonsSetActive(true);
                    return;
                }

                CurrentDownloadName = name;
                webClient.DownloadProgressChanged += DownloadProgressChanged;
                webClient.DownloadDataCompleted += DownloadBepInExCompleted;
                webClient.DownloadDataAsync(new Uri(assetURL));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetTextProgressLabel("Download Failed");
                ButtonsSetActive(true);
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            SetTextProgressLabel(string.Format("Downloading {0} - {1}%", CurrentDownloadName, e.ProgressPercentage));
            SetValueProgressBar(e.ProgressPercentage);
        }

        private void DownloadBepInExCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            SetTextProgressLabel("Extracting BepInEx");
            byte[] bepInExZip = e.Result;

            try
            {
                string path = GameDirectory + "/BepInEx-Temp.zip";
                File.WriteAllBytes(path, bepInExZip);
                ZipFile.ExtractToDirectory(path, GameDirectory, true);
                File.Delete(path);

                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "Nothing");

                string address = PTFLink;
                string s;

                SetTextProgressLabel("Getting Latest Version Of Poly Tech Framework");
                s = webClient.DownloadString(address);

                GithubRoot response = JsonSerializer.Deserialize<GithubRoot>(s);

                string assetURL = "";
                string name = "";
                foreach (Asset asset in response.assets)
                {
                    if (asset.name.Contains(".zip"))
                    {
                        assetURL = asset.browser_download_url;
                        name = asset.name;
                        PTFName = asset.name.Replace(".zip", "");
                    }
                }

                if (string.IsNullOrWhiteSpace(assetURL))
                {
                    MessageBox.Show("Could not find version of BepInEx");
                    ButtonsSetActive(true);
                    return;
                }

                CurrentDownloadName = name;
                webClient.DownloadProgressChanged += DownloadProgressChanged;
                webClient.DownloadDataCompleted += DownloadPTFCompleted;
                webClient.DownloadDataAsync(new Uri(assetURL));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetTextProgressLabel("Download Failed");
                SetValueProgressBar(0);
                ButtonsSetActive(true);
            }
        }

        private void DownloadPTFCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            SetTextProgressLabel("Extracting Poly Tech Framework");
            byte[] PTFZip = e.Result;

            try
            {
                string path = GameDirectory + "/PTF-Temp.zip";
                File.WriteAllBytes(path, PTFZip);
                ZipFile.ExtractToDirectory(path, GameDirectory, true);
                File.Delete(path);
                string ptfPath = Path.Join(GameDirectory, PTFName, "BepInEx");
                MergeDirectories(ptfPath, GameDirectory + "/BepInEx");
                Directory.Delete(Path.Join(GameDirectory, PTFName), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetTextProgressLabel("Download Failed");
                SetValueProgressBar(0);
                ButtonsSetActive(true);
                return;
            }

            MessageBox.Show("BepInEx and PTF succesfully installed!\nPress F1 ingame for mod settings!");
            SetTextProgressLabel("Download Completed");
            SetValueProgressBar(0);
            ButtonsSetActive(true);
        }

        public static void MergeDirectories(string source, string target)
        {

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            foreach (string path in Directory.GetFiles(source))
            {
                FileInfo file = new FileInfo(path);
                file.CopyTo(Path.Combine(target.ToString(), file.Name), true);
            }

            foreach (string path in Directory.GetDirectories(source))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                MergeDirectories(path, Directory.CreateDirectory(Path.Join(target, dir.Name)).FullName);
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }



        public void Window_SizeChanged()
        {
            ModsListPanel.Size = new Size(Size.Width - 40, Size.Height - 175);
            SearchBar.Size = new Size(Size.Width - 182, SearchBar.Height);
        }



        private Button CreateButton(Point loc, Size size, string text)
        {
            Button button = new Button();
            button.Location = loc;
            button.Size = size;
            button.Text = text;
            return button;
        }

        private Label CreateLabel(Point loc, Size size, string text)
        {
            Label label = new Label();
            label.Location = loc;
            label.Size = size;
            label.Text = text;
            return label;
        }

        private LinkLabel CreateLinkLabel(Point loc, Size size, string text, string url)
        {
            LinkLabel label = new LinkLabel();
            label.Location = loc;
            label.Size = size;
            label.Text = text;
            label.LinkClicked += delegate { OpenUrl(url); };
            return label;
        }

        private static string ToBase26(int number)
        {
            var list = new LinkedList<int>();
            list.AddFirst((number - 1) % 26);
            while ((number = --number / 26 - 1) > 0) list.AddFirst(number % 26);
            return new string(list.Select(s => (char)(s + 65)).ToArray());
        }
    }


    public class ModData
    {
        public string Name;
        public string Author;
        public string Synopsis;
        public string ReadMe;
        public string ReadMeLink;
        public string Tags;
        public string Cheat;
        public string Updated;
        public string GitLink;

        public bool Absolete = false;
        public bool MoreFiles = false;
        public bool NotSuported = false;
        public string dllName;
    }

    public class ModLabels
    {
        public Panel[] Panels;
        public Button InstallButton;
        public Button DeleteButton;
        public Label Name;
        public Label Author;
        public Label Synopsis;
        public LinkLabel ReadMe;
        public Label Tags;
        public Label Cheat;
        public Label Updated;
    }


    public class GoogleValuesRoot
    {
        public string range { get; set; }
        public string majorDimension { get; set; }
        public List<List<string>> values { get; set; }
    }

    public class BackgroundColor
    {
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class Padding
    {
        public int top { get; set; }
        public int right { get; set; }
        public int bottom { get; set; }
        public int left { get; set; }
    }

    public class ForegroundColor
    {
    }

    public class RgbColor
    {
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class ForegroundColorStyle
    {
        public RgbColor rgbColor { get; set; }
    }

    public class TextFormat
    {
        public ForegroundColor foregroundColor { get; set; }
        public string fontFamily { get; set; }
        public int fontSize { get; set; }
        public bool bold { get; set; }
        public bool italic { get; set; }
        public bool strikethrough { get; set; }
        public bool underline { get; set; }
        public ForegroundColorStyle foregroundColorStyle { get; set; }
    }

    public class BackgroundColorStyle
    {
        public RgbColor rgbColor { get; set; }
    }

    public class DefaultFormat
    {
        public BackgroundColor backgroundColor { get; set; }
        public Padding padding { get; set; }
        public string verticalAlignment { get; set; }
        public string wrapStrategy { get; set; }
        public TextFormat textFormat { get; set; }
        public BackgroundColorStyle backgroundColorStyle { get; set; }
    }

    public class ThemeColor
    {
        public string colorType { get; set; }
    }

    public class SpreadsheetTheme
    {
        public string primaryFontFamily { get; set; }
        public List<ThemeColor> themeColors { get; set; }
    }

    public class Properties
    {
        public string title { get; set; }
        public string locale { get; set; }
        public string autoRecalc { get; set; }
        public string timeZone { get; set; }
        public DefaultFormat defaultFormat { get; set; }
        public SpreadsheetTheme spreadsheetTheme { get; set; }
        public int sheetId { get; set; }
        public int index { get; set; }
        public string sheetType { get; set; }
        public GridProperties gridProperties { get; set; }
    }

    public class GridProperties
    {
        public int rowCount { get; set; }
        public int columnCount { get; set; }
    }

    public class Merge
    {
        public int startRowIndex { get; set; }
        public int endRowIndex { get; set; }
        public int startColumnIndex { get; set; }
        public int endColumnIndex { get; set; }
    }

    public class Sheet
    {
        public Properties properties { get; set; }
        public List<Merge> merges { get; set; }
    }

    public class GoogleSheetInfoRoot
    {
        public string spreadsheetId { get; set; }
        public Properties properties { get; set; }
        public List<Sheet> sheets { get; set; }
        public string spreadsheetUrl { get; set; }
    }


    public class Author
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    public class Uploader
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    public class Asset
    {
        public string url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public object label { get; set; }
        public Uploader uploader { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string browser_download_url { get; set; }
    }

    public class GithubRoot
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public Author author { get; set; }
        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public List<Asset> assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
        public string body { get; set; }

        public Version GetVersion()
        {
            string text = this.tag_name.ToLower().Replace("v", "");
            int num = text.IndexOf("-");
            bool flag = num != -1;
            if (flag)
            {
                text = text.Substring(0, num);
            }
            num = text.IndexOf("+");
            bool flag2 = num != -1;
            if (flag2)
            {
                text = text.Substring(0, num);
            }
            Version result;
            try
            {
                result = new Version(text);
            }
            catch
            {
                result = new Version("0.0.0");
            }
            return result;
        }
    }
}
