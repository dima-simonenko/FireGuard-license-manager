namespace FireGuardManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnDeleteLicense = new System.Windows.Forms.Button();
            this.btnCheckServiceStatus = new System.Windows.Forms.Button();
            this.btnRunLicenseActivator = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.btnDeleteService = new System.Windows.Forms.Button();
            this.btnRunFireGuard = new System.Windows.Forms.Button();
            this.btnInstallService = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.chkCopyBeforeDelete = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStopService = new System.Windows.Forms.Button();
            this.btnRestartService = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnOpenLicenseFolder = new System.Windows.Forms.Button();
            this.btnOpenNetworkSettings = new System.Windows.Forms.Button();
            this.btnOpenServerNetworkSettings = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDeleteNetworkSettings = new System.Windows.Forms.Button();
            this.btnSetLicensePath = new System.Windows.Forms.Button();
            this.txtLicensePath = new System.Windows.Forms.TextBox();
            this.btnSetProgramFolderPath = new System.Windows.Forms.Button();
            this.txtProgramFolderPath = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDeleteLicense
            // 
            this.btnDeleteLicense.Location = new System.Drawing.Point(6, 48);
            this.btnDeleteLicense.Name = "btnDeleteLicense";
            this.btnDeleteLicense.Size = new System.Drawing.Size(218, 28);
            this.btnDeleteLicense.TabIndex = 0;
            this.btnDeleteLicense.Text = "Удалить license.dat";
            this.btnDeleteLicense.UseVisualStyleBackColor = true;
            this.btnDeleteLicense.Click += new System.EventHandler(this.BtnDeleteLicense_Click);
            // 
            // btnCheckServiceStatus
            // 
            this.btnCheckServiceStatus.Location = new System.Drawing.Point(6, 19);
            this.btnCheckServiceStatus.Name = "btnCheckServiceStatus";
            this.btnCheckServiceStatus.Size = new System.Drawing.Size(218, 28);
            this.btnCheckServiceStatus.TabIndex = 1;
            this.btnCheckServiceStatus.Text = "Статус";
            this.btnCheckServiceStatus.UseVisualStyleBackColor = true;
            this.btnCheckServiceStatus.Click += new System.EventHandler(this.BtnCheckServiceStatus_Click);
            // 
            // btnRunLicenseActivator
            // 
            this.btnRunLicenseActivator.Location = new System.Drawing.Point(6, 19);
            this.btnRunLicenseActivator.Name = "btnRunLicenseActivator";
            this.btnRunLicenseActivator.Size = new System.Drawing.Size(218, 28);
            this.btnRunLicenseActivator.TabIndex = 2;
            this.btnRunLicenseActivator.Text = "LicenseActivator";
            this.btnRunLicenseActivator.UseVisualStyleBackColor = true;
            this.btnRunLicenseActivator.Click += new System.EventHandler(this.BtnRunLicenseActivator_Click);
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.Location = new System.Drawing.Point(248, 12);
            this.lstLog.Name = "lstLog";
            this.lstLog.ScrollAlwaysVisible = true;
            this.lstLog.Size = new System.Drawing.Size(451, 537);
            this.lstLog.TabIndex = 3;
            this.lstLog.SelectedIndexChanged += new System.EventHandler(this.LstLog_SelectedIndexChanged);
            // 
            // btnDeleteService
            // 
            this.btnDeleteService.Location = new System.Drawing.Point(6, 53);
            this.btnDeleteService.Name = "btnDeleteService";
            this.btnDeleteService.Size = new System.Drawing.Size(218, 28);
            this.btnDeleteService.TabIndex = 4;
            this.btnDeleteService.Text = "Удалить";
            this.btnDeleteService.UseVisualStyleBackColor = true;
            this.btnDeleteService.Click += new System.EventHandler(this.BtnDeleteService_Click);
            // 
            // btnRunFireGuard
            // 
            this.btnRunFireGuard.Location = new System.Drawing.Point(6, 53);
            this.btnRunFireGuard.Name = "btnRunFireGuard";
            this.btnRunFireGuard.Size = new System.Drawing.Size(218, 28);
            this.btnRunFireGuard.TabIndex = 5;
            this.btnRunFireGuard.Text = "FireGuard 4";
            this.btnRunFireGuard.UseVisualStyleBackColor = true;
            this.btnRunFireGuard.Click += new System.EventHandler(this.BtnRunFireGuard_Click);
            // 
            // btnInstallService
            // 
            this.btnInstallService.Location = new System.Drawing.Point(6, 87);
            this.btnInstallService.Name = "btnInstallService";
            this.btnInstallService.Size = new System.Drawing.Size(218, 28);
            this.btnInstallService.TabIndex = 6;
            this.btnInstallService.Text = "Установить";
            this.btnInstallService.UseVisualStyleBackColor = true;
            this.btnInstallService.Click += new System.EventHandler(this.BtnInstallService_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.BackColor = System.Drawing.Color.Blue;
            this.btnClearLog.Location = new System.Drawing.Point(12, 521);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(230, 28);
            this.btnClearLog.TabIndex = 7;
            this.btnClearLog.Text = "Очистить лог";
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);
            // 
            // chkCopyBeforeDelete
            // 
            this.chkCopyBeforeDelete.Location = new System.Drawing.Point(7, 82);
            this.chkCopyBeforeDelete.Name = "chkCopyBeforeDelete";
            this.chkCopyBeforeDelete.Size = new System.Drawing.Size(218, 17);
            this.chkCopyBeforeDelete.TabIndex = 8;
            this.chkCopyBeforeDelete.Text = "Копировать при удалении";
            this.chkCopyBeforeDelete.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRunLicenseActivator);
            this.groupBox1.Controls.Add(this.btnRunFireGuard);
            this.groupBox1.Location = new System.Drawing.Point(12, 419);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 94);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Запуск программ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnStopService);
            this.groupBox2.Controls.Add(this.btnRestartService);
            this.groupBox2.Controls.Add(this.btnCheckServiceStatus);
            this.groupBox2.Controls.Add(this.btnDeleteService);
            this.groupBox2.Controls.Add(this.btnInstallService);
            this.groupBox2.Location = new System.Drawing.Point(12, 225);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 188);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Управление службой FireGuardServer";
            // 
            // btnStopService
            // 
            this.btnStopService.Location = new System.Drawing.Point(8, 151);
            this.btnStopService.Name = "btnStopService";
            this.btnStopService.Size = new System.Drawing.Size(216, 23);
            this.btnStopService.TabIndex = 8;
            this.btnStopService.Text = "Остановить";
            this.btnStopService.UseVisualStyleBackColor = true;
            this.btnStopService.Click += new System.EventHandler(this.BtnStopService_Click);
            // 
            // btnRestartService
            // 
            this.btnRestartService.Location = new System.Drawing.Point(7, 122);
            this.btnRestartService.Name = "btnRestartService";
            this.btnRestartService.Size = new System.Drawing.Size(217, 23);
            this.btnRestartService.TabIndex = 7;
            this.btnRestartService.Text = "Перезапустить";
            this.btnRestartService.UseVisualStyleBackColor = true;
            this.btnRestartService.Click += new System.EventHandler(this.BtnRestartService_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnOpenLicenseFolder);
            this.groupBox3.Controls.Add(this.chkCopyBeforeDelete);
            this.groupBox3.Controls.Add(this.btnDeleteLicense);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 108);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Управление лицензией";
            // 
            // btnOpenLicenseFolder
            // 
            this.btnOpenLicenseFolder.Location = new System.Drawing.Point(7, 19);
            this.btnOpenLicenseFolder.Name = "btnOpenLicenseFolder";
            this.btnOpenLicenseFolder.Size = new System.Drawing.Size(217, 23);
            this.btnOpenLicenseFolder.TabIndex = 9;
            this.btnOpenLicenseFolder.Text = "Открыть папку License";
            this.btnOpenLicenseFolder.UseVisualStyleBackColor = true;
            this.btnOpenLicenseFolder.Click += new System.EventHandler(this.BtnOpenLicenseFolder_Click);
            // 
            // btnOpenNetworkSettings
            // 
            this.btnOpenNetworkSettings.Location = new System.Drawing.Point(7, 19);
            this.btnOpenNetworkSettings.Name = "btnOpenNetworkSettings";
            this.btnOpenNetworkSettings.Size = new System.Drawing.Size(151, 28);
            this.btnOpenNetworkSettings.TabIndex = 1;
            this.btnOpenNetworkSettings.Text = "network_settings.ini";
            this.btnOpenNetworkSettings.UseVisualStyleBackColor = true;
            this.btnOpenNetworkSettings.Click += new System.EventHandler(this.BtnOpenNetworkSettings_Click);
            // 
            // btnOpenServerNetworkSettings
            // 
            this.btnOpenServerNetworkSettings.Location = new System.Drawing.Point(7, 53);
            this.btnOpenServerNetworkSettings.Name = "btnOpenServerNetworkSettings";
            this.btnOpenServerNetworkSettings.Size = new System.Drawing.Size(151, 28);
            this.btnOpenServerNetworkSettings.TabIndex = 2;
            this.btnOpenServerNetworkSettings.Text = "server_network_settings.ini";
            this.btnOpenServerNetworkSettings.UseVisualStyleBackColor = true;
            this.btnOpenServerNetworkSettings.Click += new System.EventHandler(this.BtnOpenServerNetworkSettings_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnDeleteNetworkSettings);
            this.groupBox4.Controls.Add(this.btnOpenServerNetworkSettings);
            this.groupBox4.Controls.Add(this.btnOpenNetworkSettings);
            this.groupBox4.Location = new System.Drawing.Point(12, 126);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 93);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Настройки сетевой лицензии";
            // 
            // btnDeleteNetworkSettings
            // 
            this.btnDeleteNetworkSettings.Location = new System.Drawing.Point(164, 20);
            this.btnDeleteNetworkSettings.Name = "btnDeleteNetworkSettings";
            this.btnDeleteNetworkSettings.Size = new System.Drawing.Size(60, 61);
            this.btnDeleteNetworkSettings.TabIndex = 3;
            this.btnDeleteNetworkSettings.Text = "Удалить";
            this.btnDeleteNetworkSettings.UseVisualStyleBackColor = true;
            this.btnDeleteNetworkSettings.Click += new System.EventHandler(this.BtnDeleteNetworkSettings_Click);
            // 
            // btnSetLicensePath
            // 
            this.btnSetLicensePath.Location = new System.Drawing.Point(12, 564);
            this.btnSetLicensePath.Name = "btnSetLicensePath";
            this.btnSetLicensePath.Size = new System.Drawing.Size(230, 28);
            this.btnSetLicensePath.TabIndex = 12;
            this.btnSetLicensePath.Text = "Расположение лицензии";
            this.btnSetLicensePath.UseVisualStyleBackColor = true;
            this.btnSetLicensePath.Click += new System.EventHandler(this.BtnSetLicensePath_Click);
            // 
            // txtLicensePath
            // 
            this.txtLicensePath.Location = new System.Drawing.Point(248, 569);
            this.txtLicensePath.Name = "txtLicensePath";
            this.txtLicensePath.Size = new System.Drawing.Size(451, 20);
            this.txtLicensePath.TabIndex = 13;
            this.txtLicensePath.Text = "C:\\ProgramData\\MST\\FireGuard 4 Ultimate\\Licenses";
            // 
            // btnSetProgramFolderPath
            // 
            this.btnSetProgramFolderPath.Location = new System.Drawing.Point(12, 598);
            this.btnSetProgramFolderPath.Name = "btnSetProgramFolderPath";
            this.btnSetProgramFolderPath.Size = new System.Drawing.Size(230, 28);
            this.btnSetProgramFolderPath.TabIndex = 14;
            this.btnSetProgramFolderPath.Text = "Расположение FireGuard 4";
            this.btnSetProgramFolderPath.UseVisualStyleBackColor = true;
            this.btnSetProgramFolderPath.Click += new System.EventHandler(this.BtnSetProgramFolderPath_Click);
            // 
            // txtProgramFolderPath
            // 
            this.txtProgramFolderPath.Location = new System.Drawing.Point(248, 603);
            this.txtProgramFolderPath.Name = "txtProgramFolderPath";
            this.txtProgramFolderPath.Size = new System.Drawing.Size(451, 20);
            this.txtProgramFolderPath.TabIndex = 15;
            this.txtProgramFolderPath.Text = "C:\\Program Files (x86)\\MST\\FireGuard 4 Ultimate";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 670);
            this.Controls.Add(this.txtProgramFolderPath);
            this.Controls.Add(this.btnSetProgramFolderPath);
            this.Controls.Add(this.txtLicensePath);
            this.Controls.Add(this.btnSetLicensePath);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.lstLog);
            this.Name = "MainForm";
            this.Text = "FireGuard license manager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteLicense;
        private System.Windows.Forms.Button btnCheckServiceStatus;
        private System.Windows.Forms.Button btnRunLicenseActivator;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnDeleteService;
        private System.Windows.Forms.Button btnRunFireGuard;
        private System.Windows.Forms.Button btnInstallService;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkCopyBeforeDelete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOpenServerNetworkSettings;
        private System.Windows.Forms.Button btnOpenNetworkSettings;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSetLicensePath;
        private System.Windows.Forms.TextBox txtLicensePath;
        private System.Windows.Forms.Button btnSetProgramFolderPath;
        private System.Windows.Forms.TextBox txtProgramFolderPath;
        private System.Windows.Forms.Button btnDeleteNetworkSettings;
        private System.Windows.Forms.Button btnOpenLicenseFolder;
        private System.Windows.Forms.Button btnRestartService;
        private System.Windows.Forms.Button btnStopService;
    }
}
