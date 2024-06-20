using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireGuardManager
{
    public partial class MainForm : Form
    {
        private string directoryLicenseFireGuardPath;
        private string fireGuardLicenseActivatorPath;
        private string fireGuardExePath;
        private string installServicePath;
        private readonly string serviceName = "FireGuardServer";
        private string networkSettingsPath;
        private string serverNetworkSettingsPath;
        private int logCounter = 1;
        private Timer serviceStatusTimer;

        public MainForm()
        {
            InitializeComponent();
            SetDefaultPaths();
            InitializeServiceStatusTimer();
        }

        private void SetDefaultPaths()
        {
            directoryLicenseFireGuardPath = @"C:\ProgramData\MST\FireGuard 4 Ultimate\Licenses";
            txtLicensePath.Text = directoryLicenseFireGuardPath;
            txtLicensePath.ReadOnly = true; // Запретить редактирование
            txtProgramFolderPath.Text = @"C:\Program Files (x86)\MST\FireGuard 4 Ultimate";
            txtProgramFolderPath.ReadOnly = true; // Запретить редактирование

            UpdatePathsBasedOnProgramFolder();
        }

        private void UpdatePathsBasedOnProgramFolder()
        {
            fireGuardLicenseActivatorPath = Path.Combine(txtProgramFolderPath.Text, "LicenseActivator.exe");
            fireGuardExePath = Path.Combine(txtProgramFolderPath.Text, "FireGuard4.exe");
            installServicePath = Path.Combine(txtProgramFolderPath.Text, "FireGuardServer", "install.bat");

            // Получить название редакции из последней части пути программы
            string programFolderPath = txtProgramFolderPath.Text;
            string editionName = programFolderPath.Substring(programFolderPath.LastIndexOf("FireGuard 4 ") + "FireGuard 4 ".Length);

            networkSettingsPath = $@"C:\ProgramData\MST\FireGuard 4 {editionName}\network_settings.ini";
            serverNetworkSettingsPath = $@"C:\ProgramData\MST\FireGuard 4 {editionName}\server_network_settings.ini";

            UpdateServiceControlButtons();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (!IsRunningAsAdmin())
                {
                    MessageBox.Show("Программа не запущена с правами администратора. Пожалуйста, перезапустите программу с правами администратора.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                btnClearLog.Enabled = lstLog.Items.Count > 0;
                UpdateServiceControlButtons();
                serviceStatusTimer.Start();
            }, "проверке прав администратора");
        }

        private void InitializeServiceStatusTimer()
        {
            serviceStatusTimer = new Timer
            {
                Interval = 5000 // Проверка каждые 5 секунд
            };
            serviceStatusTimer.Tick += (s, e) => UpdateServiceControlButtons();
        }

        private void BtnSetLicensePath_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = directoryLicenseFireGuardPath;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    directoryLicenseFireGuardPath = folderBrowserDialog.SelectedPath;
                    txtLicensePath.Text = directoryLicenseFireGuardPath;
                    AddLog($"Путь к лицензии изменен на: {directoryLicenseFireGuardPath}");
                    UpdateServiceControlButtons();
                }
            }
        }

        private void BtnSetProgramFolderPath_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    txtProgramFolderPath.Text = folderBrowserDialog.SelectedPath;
                    UpdatePathsBasedOnProgramFolder();
                }
            }
        }

        private async void BtnDeleteLicense_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                if (Directory.Exists(directoryLicenseFireGuardPath))
                {
                    string licenseDatPath = Path.Combine(directoryLicenseFireGuardPath, "license.dat");

                    if (!File.Exists(licenseDatPath))
                    {
                        MessageBox.Show("Файл license.dat не найден в папке Licenses.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (chkCopyBeforeDelete.Checked)
                    {
                        using (var folderBrowserDialog = new FolderBrowserDialog())
                        {
                            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                            {
                                string destinationPath = folderBrowserDialog.SelectedPath;
                                await Task.Run(() => File.Copy(licenseDatPath, Path.Combine(destinationPath, "license.dat"), true));
                                AddLog($"Скопирован файл: {licenseDatPath} в {destinationPath}");
                            }
                            else
                            {
                                return;
                            }
                        }
                    }

                    await Task.Run(() => MoveToRecycleBin(licenseDatPath));
                    AddLog($"Файл перемещен в корзину: {licenseDatPath}");
                }
                else
                {
                    MessageBox.Show("Директория не существует: " + directoryLicenseFireGuardPath);
                    AddLog("Директория не существует: " + directoryLicenseFireGuardPath);
                }
            }, "удалении лицензий");
        }

        private async Task CopyDirectoryAsync(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            var fileTasks = Directory.GetFiles(sourceDir).Select(async file =>
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                await Task.Run(() => File.Copy(file, destFile, true));
            });

            var dirTasks = Directory.GetDirectories(sourceDir).Select(async dir =>
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                await CopyDirectoryAsync(dir, destSubDir);
            });

            await Task.WhenAll(fileTasks.Concat(dirTasks));
        }

        private void BtnCheckServiceStatus_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    AddLog($"Статус службы {serviceName}: {service.Status}");
                }
                catch (InvalidOperationException)
                {
                    AddLog($"Служба {serviceName} не найдена.");
                }
            }, "проверке статуса службы");
        }

        private void BtnRunLicenseActivator_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (IsProcessRunning("LicenseActivator"))
                {
                    MessageBox.Show("Программа LicenseActivator уже запущена.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddLog("Попытка запустить LicenseActivator, но программа уже запущена.");
                }
                else if (File.Exists(fireGuardLicenseActivatorPath))
                {
                    var result = MessageBox.Show("Запустить LicenseActivator.exe для FireGuard 4 Ultimate?", "Запуск LicenseActivator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fireGuardLicenseActivatorPath,
                            WorkingDirectory = Path.GetDirectoryName(fireGuardLicenseActivatorPath)
                        });
                        AddLog("Запуск LicenseActivator.exe");
                    }
                    else
                    {
                        AddLog("Запуск LicenseActivator.exe отменен пользователем.");
                    }
                }
                else
                {
                    MessageBox.Show("Программа LicenseActivator.exe не найдена: " + fireGuardLicenseActivatorPath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Программа LicenseActivator.exe не найдена: " + fireGuardLicenseActivatorPath);
                }
            }, "запуске LicenseActivator.exe");
        }

        private async void BtnDeleteService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    ServiceControllerStatus status = service.Status;

                    var result = MessageBox.Show("Хотите удалить службу FireGuardServer?", "Удаление службы", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (status != ServiceControllerStatus.Stopped)
                        {
                            service.Stop();
                            await service.WaitForStatusAsync(ServiceControllerStatus.Stopped);
                            AddLog("Служба остановлена.");
                        }

                        var process = new Process();
                        process.StartInfo.FileName = "sc.exe";
                        process.StartInfo.Arguments = $"delete {serviceName}";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.Start();
                        await WaitForExitAsync(process);
                        AddLog("Служба удалена.");
                    }
                    else
                    {
                        AddLog("Удаление службы отменено пользователем.");
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show($"Служба {serviceName} не найдена.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddLog($"Служба {serviceName} не найдена.");
                }
            }, "удалении службы");
        }

        private void BtnRunFireGuard_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (IsProcessRunning("FireGuard4"))
                {
                    MessageBox.Show("Программа FireGuard 4 Ultimate уже запущена.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddLog("Попытка запустить FireGuard 4 Ultimate, но программа уже запущена.");
                }
                else if (File.Exists(fireGuardExePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fireGuardExePath,
                        WorkingDirectory = Path.GetDirectoryName(fireGuardExePath)
                    });
                    AddLog("Запуск FireGuard 4 Ultimate");
                }
                else
                {
                    MessageBox.Show("Программа FireGuard4.exe не найдена: " + fireGuardExePath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Программа FireGuard4.exe не найдена: " + fireGuardExePath);
                }
            }, "запуске FireGuard 4 Ultimate");
        }

        private async void BtnInstallService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                if (IsServiceInstalled(serviceName))
                {
                    MessageBox.Show("Служба FireGuardServer уже установлена.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddLog("Попытка установить службу FireGuardServer, но она уже установлена.");
                }
                else if (File.Exists(installServicePath))
                {
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = installServicePath,
                        WorkingDirectory = Path.GetDirectoryName(installServicePath)
                    });
                    AddLog("Запуск install.bat для установки службы");

                    if (await WaitForServiceToBeInstalledAsync(serviceName))
                    {
                        ServiceController service = new ServiceController(serviceName);
                        AddLog($"Статус службы после установки: {service.Status}");
                    }
                    else
                    {
                        AddLog("Ошибка при установке службы: служба не установлена в течение ожидаемого времени.");
                    }
                }
                else
                {
                    MessageBox.Show("Файл install.bat не найден: " + installServicePath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Файл install.bat не найден: " + installServicePath);
                }
            }, "установке службы");
        }

        private async Task<bool> WaitForServiceToBeInstalledAsync(string serviceName, int timeoutSeconds = 60)
        {
            int elapsedSeconds = 0;
            while (elapsedSeconds < timeoutSeconds)
            {
                if (IsServiceInstalled(serviceName))
                {
                    return true;
                }
                await Task.Delay(2000);
                elapsedSeconds += 2;
            }
            return false;
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            Control activeControl = ActiveControl;
            lstLog.Items.Clear();
            logCounter = 1;
            btnClearLog.Enabled = false;
            AddLog("Лог очищен.");
            activeControl?.Focus();
        }

        private async void BtnOpenNetworkSettings_Click(object sender, EventArgs e)
        {
            await OpenSettingsFileAsync(networkSettingsPath, "network_settings.ini");
        }

        private async void BtnOpenServerNetworkSettings_Click(object sender, EventArgs e)
        {
            await OpenSettingsFileAsync(serverNetworkSettingsPath, "server_network_settings.ini");
        }

        private void BtnDeleteNetworkSettings_Click(object sender, EventArgs e)
        {
            DeleteNetworkSettingsFiles();
        }

        private async Task OpenSettingsFileAsync(string filePath, string fileName)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                Process process = Process.GetProcessesByName("notepad")
                    .FirstOrDefault(p => p.MainWindowTitle.Contains(fileName));

                if (process == null || process.HasExited)
                {
                    if (File.Exists(filePath))
                    {
                        process = Process.Start("notepad.exe", filePath);
                        AddLog($"Открыт файл {fileName}");
                        await WaitForExitAsync(process);
                    }
                    else
                    {
                        MessageBox.Show($"Файл {fileName} не найден: {filePath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        AddLog($"Файл {fileName} не найден: {filePath}");
                    }
                }
                else
                {
                    MessageBox.Show($"Файл {fileName} уже открыт.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddLog($"Попытка открыть {fileName}, но файл уже открыт.");
                }
            }, $"открытии {fileName}");
        }

        private void DeleteNetworkSettingsFiles()
        {
            ExecuteWithErrorHandling(() =>
            {
                bool fileExists = false;

                if (File.Exists(networkSettingsPath))
                {
                    File.Delete(networkSettingsPath);
                    AddLog($"Файл удален: {networkSettingsPath}");
                    fileExists = true;
                }
                else
                {
                    AddLog($"Файл не найден: {networkSettingsPath}");
                }

                if (File.Exists(serverNetworkSettingsPath))
                {
                    File.Delete(serverNetworkSettingsPath);
                    AddLog($"Файл удален: {serverNetworkSettingsPath}");
                    fileExists = true;
                }
                else
                {
                    AddLog($"Файл не найден: {serverNetworkSettingsPath}");
                }

                if (!fileExists)
                {
                    MessageBox.Show("Файлы network_settings.ini и server_network_settings.ini не найдены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Файлы настроек сети удалены.", "Удаление файлов", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }, "удалении файлов настроек сети");
        }

        private bool IsServiceInstalled(string serviceName)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                ServiceControllerStatus status = sc.Status;
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }

        private bool IsRunningAsAdmin()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void AddLog(string message)
        {
            string logEntry = $"{logCounter}: {DateTime.Now:dd-MM-yyyy HH:mm} - {message}";
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    lstLog.Items.Add(logEntry);
                    lstLog.TopIndex = lstLog.Items.Count - 1;
                    logCounter++;
                    btnClearLog.Enabled = true;
                }));
            }
            else
            {
                lstLog.Items.Add(logEntry);
                lstLog.TopIndex = lstLog.Items.Count - 1;
                logCounter++;
                btnClearLog.Enabled = true;
            }
        }

        private void HandleError(Exception ex, string action)
        {
            string errorMessage = $"Ошибка при {action}: {ex.Message}";
            AddLog(errorMessage);
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnClearLog.Enabled = lstLog.Items.Count > 0;
        }

        private async Task WaitForExitAsync(Process process)
        {
            var tcs = new TaskCompletionSource<object>();

            process.Exited += (sender, args) => tcs.TrySetResult(null);
            process.EnableRaisingEvents = true;

            if (process.HasExited)
                return;

            await tcs.Task;
        }

        private void BtnOpenLicenseFolder_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (Directory.Exists(directoryLicenseFireGuardPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = directoryLicenseFireGuardPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                    AddLog($"Открыта папка лицензий: {directoryLicenseFireGuardPath}");
                }
                else
                {
                    MessageBox.Show("Папка с лицензией не существует: " + directoryLicenseFireGuardPath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Папка с лицензией не существует: " + directoryLicenseFireGuardPath);
                }
            }, "открытии папки с лицензиями");
        }

        private async void BtnRestartService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                ServiceController service = new ServiceController(serviceName);
                if (IsServiceInstalled(serviceName))
                {
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        await service.WaitForStatusAsync(ServiceControllerStatus.Stopped);
                        AddLog("Служба остановлена.");
                    }

                    service.Start();
                    await service.WaitForStatusAsync(ServiceControllerStatus.Running);
                    AddLog("Служба перезапущена.");
                }
                else
                {
                    MessageBox.Show("Служба не установлена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Служба не установлена.");
                }
            }, "перезапуске службы");
        }

        private async void BtnStopService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                if (IsServiceInstalled(serviceName))
                {
                    ServiceController service = new ServiceController(serviceName);
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        await service.WaitForStatusAsync(ServiceControllerStatus.Stopped);
                        AddLog("Служба остановлена.");
                    }
                    else
                    {
                        AddLog("Служба уже остановлена.");
                    }
                }
                else
                {
                    MessageBox.Show("Служба не установлена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog("Служба не установлена.");
                }
            }, "остановке службы");
        }

        private void ExecuteWithErrorHandling(Action action, string actionDescription)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                HandleError(ex, actionDescription);
            }
            UpdateServiceControlButtons();
        }

        private async Task ExecuteWithErrorHandlingAsync(Func<Task> action, string actionDescription)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                HandleError(ex, actionDescription);
            }
            UpdateServiceControlButtons();
        }

        private void UpdateServiceControlButtons()
        {
            btnDeleteNetworkSettings.Enabled = File.Exists(networkSettingsPath) || File.Exists(serverNetworkSettingsPath);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        private const int FO_DELETE = 3;
        private const int FOF_ALLOWUNDO = 0x40;
        private const int FOF_NOCONFIRMATION = 0x10;

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        private static void MoveToRecycleBin(string path)
        {
            var fs = new SHFILEOPSTRUCT
            {
                wFunc = FO_DELETE,
                pFrom = path + '\0' + '\0',
                fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION
            };

            SHFileOperation(ref fs);
        }
    }

    public static class ServiceControllerExtensions
    {
        public static async Task WaitForStatusAsync(this ServiceController serviceController, ServiceControllerStatus desiredStatus, int timeoutMilliseconds = 60000)
        {
            int elapsedMilliseconds = 0;
            while (serviceController.Status != desiredStatus && elapsedMilliseconds < timeoutMilliseconds)
            {
                await Task.Delay(200);
                serviceController.Refresh();
                elapsedMilliseconds += 200;
            }
        }
    }
}
