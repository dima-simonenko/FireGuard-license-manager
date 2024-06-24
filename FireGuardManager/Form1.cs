using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireGuardManager
{
    public partial class MainForm : Form
    {
        private string directoryLicenseFireGuardPath;
        private string fireGuardLicenseActivatorPath;
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
            txtLicensePath.ReadOnly = true;
            txtProgramFolderPath.Text = @"C:\Program Files (x86)\MST\FireGuard 4 Ultimate";
            txtProgramFolderPath.ReadOnly = true;

            UpdatePathsBasedOnProgramFolder();
        }

        private void UpdatePathsBasedOnProgramFolder()
        {
            fireGuardLicenseActivatorPath = Path.Combine(txtProgramFolderPath.Text, "LicenseActivator.exe");
            installServicePath = Path.Combine(txtProgramFolderPath.Text, "FireGuardServer", "install.bat");

            string editionName = GetEditionName(txtProgramFolderPath.Text);
            networkSettingsPath = $@"C:\ProgramData\MST\FireGuard 4 {editionName}\network_settings.ini";
            serverNetworkSettingsPath = $@"C:\ProgramData\MST\FireGuard 4 {editionName}\server_network_settings.ini";

            UpdateServiceControlButtons();
        }

        private string GetEditionName(string programFolderPath)
        {
            return programFolderPath.Substring(programFolderPath.LastIndexOf("FireGuard 4 ") + "FireGuard 4 ".Length);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (!IsRunningAsAdmin())
                {
                    ShowErrorAndExit("Программа не запущена с правами администратора. Пожалуйста, перезапустите программу с правами администратора.");
                }
                btnClearLog.Enabled = lstLog.Items.Count > 0;
                UpdateServiceControlButtons();
                serviceStatusTimer.Start();
            }, "проверке прав администратора");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AddLog("Программа закрыта.");
        }

        private void ShowErrorAndExit(string message)
        {
            ShowError(message);
            Application.Exit();
        }

        private void InitializeServiceStatusTimer()
        {
            serviceStatusTimer = new Timer { Interval = 5000 };
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
                    AddLog($"Путь к лицензии обновлен: {directoryLicenseFireGuardPath}");

                    if (!directoryLicenseFireGuardPath.EndsWith("Licenses"))
                    {
                        AddLog("Предупреждение: по указанному пути отсутствует папка Licenses.");
                    }
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
                    AddLog($"Путь к FireGuard 4 обновлен: {txtProgramFolderPath.Text}");
                }
            }
        }

        private async void BtnDeleteLicense_Click(object sender, EventArgs e)
        {
            if (!directoryLicenseFireGuardPath.EndsWith("Licenses") || !Directory.Exists(directoryLicenseFireGuardPath))
            {
                ShowError($"По указанному пути папка Licenses не существует: {directoryLicenseFireGuardPath}");
                AddLog($"Ошибка: по указанному пути {directoryLicenseFireGuardPath} папка Licenses не существует.");
                return;
            }

            string licenseDatPath = Path.Combine(directoryLicenseFireGuardPath, "license.dat");

            if (!File.Exists(licenseDatPath))
            {
                ShowError($"По указанному пути файл license.dat не существует: {directoryLicenseFireGuardPath}");
                AddLog($"Ошибка: по указанному пути {directoryLicenseFireGuardPath} файл license.dat не существует.");
                return;
            }

            var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить файл license.dat?", "Подтвердите удаление", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                await ExecuteWithErrorHandlingAsync(async () =>
                {
                    if (chkCopyBeforeDelete.Checked)
                    {
                        using (var folderBrowserDialog = new FolderBrowserDialog())
                        {
                            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                            {
                                string destinationPath = Path.Combine(folderBrowserDialog.SelectedPath, "license.dat");
                                if (directoryLicenseFireGuardPath == Path.GetDirectoryName(destinationPath))
                                {
                                    AddLog("Ошибка: Невозможно копировать файл в ту же папку. Удаление license.dat не выполнено.");
                                    return;
                                }
                                await Task.Run(() => File.Copy(licenseDatPath, destinationPath, true));
                                AddLog($"Скопирован файл: {licenseDatPath} в {destinationPath}");
                            }
                        }
                    }

                    MoveToRecycleBin(licenseDatPath);
                    AddLog($"Файл перемещен в корзину: {licenseDatPath}");
                }, "удалении лицензий");
            }
        }

        private async void BtnCheckServiceStatus_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                await AddServiceStatusLogAsync(serviceName);
            }, "проверке статуса службы");
        }

        private async Task AddServiceStatusLogAsync(string serviceName)
        {
            await Task.Run(() =>
            {
                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    AddLog($"Статус службы {serviceName}: {service.Status}");
                }
                catch (InvalidOperationException)
                {
                    AddLog($"Служба {serviceName} не найдена.");
                }
            });
        }

        private void BtnRunLicenseActivator_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                if (IsProcessRunning("LicenseActivator"))
                {
                    ShowMessage("Программа LicenseActivator уже запущена.");
                    AddLog("LicenseActivator уже запущен.");
                }
                else if (File.Exists(fireGuardLicenseActivatorPath))
                {
                    if (ConfirmAction("Запустить LicenseActivator.exe для FireGuard 4 Ultimate?"))
                    {
                        StartProcess(fireGuardLicenseActivatorPath, string.Empty);
                        AddLog("LicenseActivator успешно запущен.");
                    }
                    else
                    {
                        AddLog("Запуск LicenseActivator отменен пользователем.");
                    }
                }
                else
                {
                    ShowError($"LicenseActivator.exe не найден: {fireGuardLicenseActivatorPath}");
                    AddLog("Ошибка: LicenseActivator не найден.");
                }
            }, "запуске LicenseActivator.exe");
        }

        private async void BtnDeleteService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    if (ConfirmAction("Хотите удалить службу FireGuardServer?"))
                    {
                        if (service.Status != ServiceControllerStatus.Stopped)
                        {
                            service.Stop();
                            await service.WaitForStatusAsync(ServiceControllerStatus.Stopped);
                            AddLog("Служба остановлена.");
                        }

                        var process = StartProcess("sc.exe", $"delete {serviceName}");
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
                    ShowMessage($"Служба {serviceName} не найдена.");
                }
            }, "удалении службы");
        }

        private void BtnRunFireGuard_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                string fireGuardExePath = Path.Combine(txtProgramFolderPath.Text, "FireGuard4.exe");

                if (IsProcessRunning("FireGuard4"))
                {
                    ShowMessage("Программа FireGuard 4 уже запущена.");
                    AddLog("FireGuard 4 уже запущена.");
                }
                else if (File.Exists(fireGuardExePath))
                {
                    if (File.Exists(Path.Combine(directoryLicenseFireGuardPath, "license.dat")))
                    {
                        StartProcess(fireGuardExePath, string.Empty);
                        AddLog("FireGuard 4 успешно запущена.");
                    }
                    else
                    {
                        AddLog("Файл license.dat не найден, запуск LicenseActivator.");
                        BtnRunLicenseActivator_Click(sender, e);
                    }
                }
                else
                {
                    ShowError($"FireGuard4.exe не найден: {fireGuardExePath}");
                    AddLog("Ошибка: FireGuard 4 не найден.");
                }
            }, "запуске FireGuard 4");
        }

        private async void BtnInstallService_Click(object sender, EventArgs e)
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                if (IsServiceInstalled(serviceName))
                {
                    ShowMessage("Служба FireGuardServer уже установлена.");
                    AddLog("Попытка установить службу FireGuardServer, но она уже установлена.");
                }
                else if (File.Exists(installServicePath))
                {
                    var process = StartProcess(installServicePath, "install.bat для установки службы");

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
                    ShowError($"Файл install.bat не найден: {installServicePath}");
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
            lstLog.Items.Clear();
            logCounter = 1;
            btnClearLog.Enabled = false;
            AddLog("Лог очищен.");
        }

        private async void BtnOpenNetworkSettings_Click(object sender, EventArgs e)
        {
            await OpenSettingsFileAsync(networkSettingsPath, "network_settings.ini");
        }

        private async void BtnOpenServerNetworkSettings_Click(object sender, EventArgs e)
        {
            await OpenSettingsFileAsync(serverNetworkSettingsPath, "server_network_settings.ini");
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
                        ShowError($"Файл {fileName} не найден: {filePath}");
                    }
                }
                else
                {
                    ShowMessage($"Файл {fileName} уже открыт.");
                }
            }, $"открытии {fileName}");
        }

        private void BtnDeleteNetworkSettings_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
                DeleteFileIfExists(networkSettingsPath);
                DeleteFileIfExists(serverNetworkSettingsPath);

                if (!File.Exists(networkSettingsPath) && !File.Exists(serverNetworkSettingsPath))
                {
                    ShowMessage("Файлы network_settings.ini и server_network_settings.ini не найдены.");
                }
                else
                {
                    ShowMessage("Файлы настроек сети удалены.");
                }
            }, "удалении файлов настроек сети");
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                AddLog($"Файл удален: {filePath}");
            }
            else
            {
                AddLog($"Файл не найден: {filePath}");
            }
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
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    AddLogEntry(message);
                }));
            }
            else
            {
                AddLogEntry(message);
            }
        }

        private void AddLogEntry(string message)
        {
            string logEntry = $"{logCounter}: {DateTime.Now:dd-MM-yyyy HH:mm} - {message}";
            lstLog.Items.Add(logEntry);
            lstLog.TopIndex = lstLog.Items.Count - 1;
            logCounter++;
            btnClearLog.Enabled = true;
        }

        private void HandleError(Exception ex, string action)
        {
            string errorMessage = $"Ошибка при {action}: {ex.Message}";
            AddLog(errorMessage);
            ShowError(errorMessage);
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
                if (!directoryLicenseFireGuardPath.EndsWith("Licenses") || !Directory.Exists(directoryLicenseFireGuardPath))
                {
                    ShowError($"По указанному пути папка Licenses не существует.");
                    AddLog($"Ошибка: По указанному пути {directoryLicenseFireGuardPath} папка Licenses не существует.");
                    return;
                }

                if (IsFolderOpen(directoryLicenseFireGuardPath))
                {
                    ShowMessage("Папка Licenses уже открыта.");
                }
                else
                {
                    StartProcess(directoryLicenseFireGuardPath, $"Открыта папка {directoryLicenseFireGuardPath}");
                }
            }, "открытии папки с лицензиями");
        }

        private bool IsFolderOpen(string folderPath)
        {
            IntPtr shellWindow = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;
                StringBuilder sb = new StringBuilder(length);
                GetWindowText(hWnd, sb, length + 1);
                string windowText = sb.ToString();
                if (windowText.Contains(folderPath))
                {
                    shellWindow = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return shellWindow != IntPtr.Zero;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private Process StartProcess(string fileName, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            process.Start();
            return process;
        }

        private bool ConfirmAction(string message)
        {
            var result = MessageBox.Show(message, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void BtnRestartService_Click(object sender, EventArgs e)
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

                    service.Start();
                    await service.WaitForStatusAsync(ServiceControllerStatus.Running);
                    AddLog("Служба перезапущена.");
                }
                else
                {
                    ShowError("Служба не установлена.");
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
                    ShowError("Служба не установлена.");
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
