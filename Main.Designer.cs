
namespace Polybridge_2_Mod_Loader
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InstallEverythingButton = new System.Windows.Forms.Button();
            this.GameDirectoryLabel = new System.Windows.Forms.Label();
            this.OpenDirectory = new System.Windows.Forms.Button();
            this.DownloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.installBepInEx = new System.ComponentModel.BackgroundWorker();
            this.ModsListPanel = new System.Windows.Forms.Panel();
            this.Titles = new System.Windows.Forms.TableLayoutPanel();
            this.ModsSynopsis = new System.Windows.Forms.Label();
            this.ModsAuthor = new System.Windows.Forms.Label();
            this.ModsNames = new System.Windows.Forms.Label();
            this.ModsTags = new System.Windows.Forms.Label();
            this.ModsReadMe = new System.Windows.Forms.Label();
            this.ModsCheat = new System.Windows.Forms.Label();
            this.ModsUpdated = new System.Windows.Forms.Label();
            this.GetModsList = new System.ComponentModel.BackgroundWorker();
            this.InstallMod = new System.ComponentModel.BackgroundWorker();
            this.SearchBar = new System.Windows.Forms.TextBox();
            this.Search = new System.Windows.Forms.Button();
            this.ModsListPanel.SuspendLayout();
            this.Titles.SuspendLayout();
            this.SuspendLayout();
            // 
            // InstallEverythingButton
            // 
            this.InstallEverythingButton.Location = new System.Drawing.Point(12, 41);
            this.InstallEverythingButton.Name = "InstallEverythingButton";
            this.InstallEverythingButton.Size = new System.Drawing.Size(136, 48);
            this.InstallEverythingButton.TabIndex = 0;
            this.InstallEverythingButton.Text = "Install/Update\r\nBepInEx and PTF";
            this.InstallEverythingButton.UseVisualStyleBackColor = true;
            this.InstallEverythingButton.Click += new System.EventHandler(this.InstallBepInEx_Click);
            // 
            // GameDirectoryLabel
            // 
            this.GameDirectoryLabel.AutoSize = true;
            this.GameDirectoryLabel.Location = new System.Drawing.Point(154, 16);
            this.GameDirectoryLabel.Name = "GameDirectoryLabel";
            this.GameDirectoryLabel.Size = new System.Drawing.Size(103, 15);
            this.GameDirectoryLabel.TabIndex = 2;
            this.GameDirectoryLabel.Text = "Selected Folder: \"\"";
            this.GameDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OpenDirectory
            // 
            this.OpenDirectory.Location = new System.Drawing.Point(12, 12);
            this.OpenDirectory.Name = "OpenDirectory";
            this.OpenDirectory.Size = new System.Drawing.Size(136, 23);
            this.OpenDirectory.TabIndex = 4;
            this.OpenDirectory.Text = "Open Game Folder";
            this.OpenDirectory.UseVisualStyleBackColor = true;
            this.OpenDirectory.Click += new System.EventHandler(this.OpenDirectory_Click);
            // 
            // DownloadProgressBar
            // 
            this.DownloadProgressBar.Location = new System.Drawing.Point(154, 66);
            this.DownloadProgressBar.Name = "DownloadProgressBar";
            this.DownloadProgressBar.Size = new System.Drawing.Size(518, 23);
            this.DownloadProgressBar.Step = 100;
            this.DownloadProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DownloadProgressBar.TabIndex = 5;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(155, 48);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(0, 15);
            this.ProgressLabel.TabIndex = 6;
            // 
            // installBepInEx
            // 
            this.installBepInEx.DoWork += new System.ComponentModel.DoWorkEventHandler(this.installBepInEx_DoWork);
            // 
            // ModsListPanel
            // 
            this.ModsListPanel.AutoScroll = true;
            this.ModsListPanel.BackColor = System.Drawing.Color.Gainsboro;
            this.ModsListPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModsListPanel.Controls.Add(this.Titles);
            this.ModsListPanel.Location = new System.Drawing.Point(12, 124);
            this.ModsListPanel.Name = "ModsListPanel";
            this.ModsListPanel.Size = new System.Drawing.Size(660, 325);
            this.ModsListPanel.TabIndex = 7;
            // 
            // Titles
            // 
            this.Titles.ColumnCount = 7;
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.Titles.Controls.Add(this.ModsSynopsis, 0, 0);
            this.Titles.Controls.Add(this.ModsAuthor, 0, 0);
            this.Titles.Controls.Add(this.ModsNames, 0, 0);
            this.Titles.Controls.Add(this.ModsTags, 4, 0);
            this.Titles.Controls.Add(this.ModsReadMe, 3, 0);
            this.Titles.Controls.Add(this.ModsCheat, 5, 0);
            this.Titles.Controls.Add(this.ModsUpdated, 6, 0);
            this.Titles.Location = new System.Drawing.Point(149, 6);
            this.Titles.Name = "Titles";
            this.Titles.RowCount = 1;
            this.Titles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Titles.Size = new System.Drawing.Size(1050, 18);
            this.Titles.TabIndex = 0;
            // 
            // ModsSynopsis
            // 
            this.ModsSynopsis.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsSynopsis.AutoSize = true;
            this.ModsSynopsis.Location = new System.Drawing.Point(303, 1);
            this.ModsSynopsis.Name = "ModsSynopsis";
            this.ModsSynopsis.Size = new System.Drawing.Size(53, 15);
            this.ModsSynopsis.TabIndex = 5;
            this.ModsSynopsis.Text = "Synopsis";
            // 
            // ModsAuthor
            // 
            this.ModsAuthor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsAuthor.AutoSize = true;
            this.ModsAuthor.Location = new System.Drawing.Point(153, 1);
            this.ModsAuthor.Name = "ModsAuthor";
            this.ModsAuthor.Size = new System.Drawing.Size(44, 15);
            this.ModsAuthor.TabIndex = 3;
            this.ModsAuthor.Text = "Author";
            // 
            // ModsNames
            // 
            this.ModsNames.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsNames.AutoSize = true;
            this.ModsNames.Location = new System.Drawing.Point(3, 1);
            this.ModsNames.Name = "ModsNames";
            this.ModsNames.Size = new System.Drawing.Size(67, 15);
            this.ModsNames.TabIndex = 2;
            this.ModsNames.Text = "Mod Name";
            // 
            // ModsTags
            // 
            this.ModsTags.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsTags.AutoSize = true;
            this.ModsTags.Location = new System.Drawing.Point(603, 1);
            this.ModsTags.Name = "ModsTags";
            this.ModsTags.Size = new System.Drawing.Size(30, 15);
            this.ModsTags.TabIndex = 6;
            this.ModsTags.Text = "Tags";
            // 
            // ModsReadMe
            // 
            this.ModsReadMe.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsReadMe.AutoSize = true;
            this.ModsReadMe.Location = new System.Drawing.Point(453, 1);
            this.ModsReadMe.Name = "ModsReadMe";
            this.ModsReadMe.Size = new System.Drawing.Size(64, 15);
            this.ModsReadMe.TabIndex = 4;
            this.ModsReadMe.Text = "Full Details";
            // 
            // ModsCheat
            // 
            this.ModsCheat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsCheat.AutoSize = true;
            this.ModsCheat.Location = new System.Drawing.Point(753, 1);
            this.ModsCheat.Name = "ModsCheat";
            this.ModsCheat.Size = new System.Drawing.Size(77, 15);
            this.ModsCheat.TabIndex = 7;
            this.ModsCheat.Text = "Is Cheat Mod";
            // 
            // ModsUpdated
            // 
            this.ModsUpdated.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ModsUpdated.AutoSize = true;
            this.ModsUpdated.Location = new System.Drawing.Point(903, 1);
            this.ModsUpdated.Name = "ModsUpdated";
            this.ModsUpdated.Size = new System.Drawing.Size(76, 15);
            this.ModsUpdated.TabIndex = 8;
            this.ModsUpdated.Text = "Last Updated";
            // 
            // GetModsList
            // 
            this.GetModsList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.GetModsList_DoWork);
            // 
            // InstallMod
            // 
            this.InstallMod.DoWork += new System.ComponentModel.DoWorkEventHandler(this.InstallMod_DoWork);
            // 
            // SearchBar
            // 
            this.SearchBar.Location = new System.Drawing.Point(154, 95);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.PlaceholderText = "Search the list of mods...";
            this.SearchBar.Size = new System.Drawing.Size(518, 23);
            this.SearchBar.TabIndex = 8;
            // 
            // Search
            // 
            this.Search.Location = new System.Drawing.Point(12, 95);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(136, 23);
            this.Search.TabIndex = 9;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.ModsListPanel);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.DownloadProgressBar);
            this.Controls.Add(this.OpenDirectory);
            this.Controls.Add(this.GameDirectoryLabel);
            this.Controls.Add(this.InstallEverythingButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1259, 999999);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "Main";
            this.Text = "Polybridge 2 Mod Loader";
            this.ModsListPanel.ResumeLayout(false);
            this.Titles.ResumeLayout(false);
            this.Titles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button InstallEverythingButton;
        private System.Windows.Forms.Label GameDirectoryLabel;
        private System.Windows.Forms.Button OpenDirectory;
        private System.Windows.Forms.ProgressBar DownloadProgressBar;
        private System.Windows.Forms.Label ProgressLabel;
        private System.ComponentModel.BackgroundWorker installBepInEx;
        private System.Windows.Forms.Panel ModsListPanel;
        private System.Windows.Forms.TableLayoutPanel Titles;
        private System.Windows.Forms.Label ModsNames;
        private System.Windows.Forms.Label ModsSynopsis;
        private System.Windows.Forms.Label ModsAuthor;
        private System.Windows.Forms.Label ModsTags;
        private System.Windows.Forms.Label ModsReadMe;
        private System.Windows.Forms.Label ModsCheat;
        private System.Windows.Forms.Label ModsUpdated;
        private System.ComponentModel.BackgroundWorker GetModsList;
        private System.ComponentModel.BackgroundWorker InstallMod;
        private System.Windows.Forms.TextBox SearchBar;
        private System.Windows.Forms.Button Search;
    }
}

