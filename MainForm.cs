using sshWT.Animation;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace sshWT
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        readonly KeyboardHook hook;
        readonly string defaultEnvironmentCommand;
        private readonly bool floatOnOpenTerminal;
        private int focusCount = 0;
        const string configFile = "app-config.json";

        public MainForm()
        {
            hook = new KeyboardHook();
            InitializeComponent();

            connectionsLbl.Text = "Connections [\U0001F50D]";
            var deltaY = placeHolder.Location.Y - connectionsLbl.Location.Y;
            container.Location = new Point(0, placeHolder.Location.Y);
            container.Size = new Size(this.Width - 5, (ClientRectangle.Height - container.Location.Y) / deltaY * deltaY);

            LoadAll();
            bool focusOnFloat;
            try
            {
                var text = File.ReadAllBytes(configFile);
                var json = System.Text.Json.JsonDocument.Parse(text);
                var command = json.RootElement.GetProperty("defaultEnvironmentCommand").GetString();
                if (string.IsNullOrWhiteSpace(command))
                {
                    throw new Exception();
                }
                defaultEnvironmentCommand = command + ' ';
                floatOnOpenTerminal = json.RootElement.GetProperty("floatOnOpenTerminal").GetBoolean();
                focusOnFloat = json.RootElement.GetProperty("focusOnFloat").GetBoolean();
            }
            catch (Exception)
            {
                defaultEnvironmentCommand = GetDefaultCommand();
                floatOnOpenTerminal = true;
                focusOnFloat = true;
                var data = "{\"defaultEnvironmentCommand\": \"__wsl__\", \"floatOnOpenTerminal\": \"__float__\", \"focusOnFloat\": \"__focus__\"}"
                    .Replace("__wsl__", defaultEnvironmentCommand!.TrimEnd())
                    .Replace("\"__float__\"", floatOnOpenTerminal.ToString().ToLower())
                    .Replace("\"__focus__\"", focusOnFloat.ToString().ToLower());
                File.WriteAllBytes(configFile, System.Text.Encoding.UTF8.GetBytes(data));
            }
            if (Screen.AllScreens.Length > 1 && floatOnOpenTerminal)
            {
                if (focusOnFloat) focusCount = 3;
                CheckWindowsTerminalForegroundAndActivateThisWindow();
            }
            else
            {
                RegisterKey();
            }

        }

        private static string GetDefaultCommand()
        {
            var proc = Process.Start(new ProcessStartInfo()
            {
                StandardOutputEncoding = System.Text.Encoding.ASCII,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                FileName = "wsl",
                Arguments = "-l --all"
            });
            if (proc == null)
            {
                throw new Exception("No wsl found");
            }
            var output = proc.StandardOutput
                .ReadToEnd()
                .Replace("\0", "")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var line in output)
            {
                if (line.Contains("(Default)"))
                {
                    return "wsl -d " + line.Replace("(Default)", "").Trim() + ' ';
                }
            }
            return "wsl ";
        }

        private void RegisterKey()
        {
            var error = hook.RegisterHotKey(sshWT.ModifierKeys.Control | sshWT.ModifierKeys.Win | sshWT.ModifierKeys.Alt, Keys.B);
            if (error is not null)
            {
                MessageBox.Show(error, "Register key");
            }
            hook.KeyPressed += Hook_KeyPressed;
        }

        bool running = false;
        private void CheckWindowsTerminalForegroundAndActivateThisWindow()
        {
            running = true;
            Task.Run(async () =>
            {
                bool isContinuous = false;
                while (running)
                {
                    await Task.Delay(200);
                    var foregroundWND = GetForegroundWindow();
                    if (foregroundWND != IntPtr.Zero)
                    {
                        var retCode = GetWindowThreadProcessId(foregroundWND, out var pid);
                        if (pid != 0)
                        {
                            if (pid == Environment.ProcessId)
                            {
                                isContinuous = true;
                                continue;
                            }
                            if (IsWindowsTerminalProc((int)pid, out var process))
                            {
                                if (!isContinuous)
                                {
                                    Invoke(() =>
                                    {
                                        TopMost = true;
                                        if (focusCount-- > 0) Activate();
                                        Task.Run(async () =>
                                        {
                                            await Task.Delay(15);
                                            Invoke(() =>
                                            {
                                                SetForegroundWindow(process.MainWindowHandle);
                                                TopMost = false;
                                            });
                                        });
                                    });
                                }
                                isContinuous = true;
                            }
                            else
                            {
                                isContinuous = false;
                            }
                        }
                    }
                }
            });
        }

        private void Hook_KeyPressed(object? sender, KeyPressedEventArgs e)
        {
            Activate();
        }

        readonly Dictionary<int, Process> procs = new();

        private bool IsWindowsTerminalProc(int pid,[NotNullWhen(true)] out Process? proc)
        {
            if (procs.TryGetValue(pid, out proc))
            {
                return proc.ProcessName == "WindowsTerminal";
            }
            procs.Add(pid, Process.GetProcessById(pid));
            return false;
        }

        private void LoadAll()
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Empty);
            }
            var allConnections = File.ReadAllLines(fileName).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            if (allConnections.Length == 0 && entries.Count == 0)
            {
                return;
            }

            if (allConnections.Length == entries.Count)
            {
                var allCurrentConnections = entries.Select(entry => entry.GetCommand()).ToArray();
                var allEqual = true;
                for (int i = 0; i < allConnections.Length; i++)
                {
                    if (allConnections[i] != allCurrentConnections[i])
                    {
                        allEqual = false;
                        break;
                    }
                }
                if (allEqual) return;
            }
            RemoveAll();

            foreach (var item in allConnections)
            {
                AddNewEntry(item.Trim());
            }
        }

        private void RemoveAll()
        {
            foreach (var entry in entries)
            {
                entry.Remove();
            }
            entries.Clear();
        }

        private readonly string fileName = "commands.conf";

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var connectionStr = connection.Text.Trim();
            if (string.IsNullOrEmpty(connectionStr))
            {
                return;
            }

            AddNewEntry(connectionStr);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            var connectionStr = connection.Text.Trim();
            if (string.IsNullOrEmpty(connectionStr))
            {
                return;
            }

            RemoveEntry(connectionStr);
        }

        private void ConnectionsLbl_Click(object sender, EventArgs e)
        {
            using Process fileopener = new();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + fileName + "\"";
            fileopener.Start();
        }

        private void ReloadBtn_Click(object sender, EventArgs e)
        {
            LoadAll();
        }

        private void connection_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddBtn_Click(sender, e);
            }
        }

        private readonly List<Entry> entries = new();

        private void AddNewEntry(string command)
        {
            var allConnections = entries.Select(entry => entry.GetCommand()).ToList();
            if (allConnections.Contains(command))
            {
                return;
            }

            allConnections.Add(command);
            File.WriteAllLines(fileName, allConnections);

            entries.Add(new Entry(this, command));
            entries.Sort((a, b) => a.Index - b.Index);
        }

        private void RemoveEntry(string command)
        {
            var allConnections = entries.Select(entry => entry.GetCommand()).ToList();
            if (!allConnections.Contains(command))
            {
                return;
            }
            allConnections.Remove(command);
            File.WriteAllLines(fileName, allConnections);

            var entry = entries.FindIndex(e => e.Match(command));
            var removeingLine = entries[entry].IsLineSeparator;
            if (entry >= 0)
            {
                entries[entry].Remove(true);
            }
            var scrolled = container.VerticalScroll.Value != 0;
            container.VerticalScroll.Value = 0;
            _ = moveUp();
            async Task moveUp()
            {
                if (scrolled) await Task.Delay(100);
                List<Control> controls = new();
                for (int i = entry; i < entries.Count; i++)
                {
                    //entries[i].MoveUp();
                    controls.AddRange(entries[i].Controls);
                }
                var deltaY = placeHolder.Location.Y - connectionsLbl.Location.Y;
                Animator.Animate(controls, Animatee.LocationY, EasingFunctions.OutSine, removeingLine ? -Entry.SEP_H : -deltaY, 200, 0);
            }
        }

        class Entry
        {
            static int sid;
            readonly int id;
            readonly Label label;
            readonly Label line;
            readonly Button startBtn;
            readonly Button startSplitPaneHorizontalBtn;
            readonly MainForm mainForm;

            public const int SEP_H = 10; // Separator height

            public bool IsLineSeparator { get; }

            public int Index => id;

            public Entry(MainForm mainForm, string data)
            {
                id = Interlocked.Increment(ref sid);
                IsLineSeparator = data.All(c => c == '-');
                var deltaY = mainForm.placeHolder.Location.Y - mainForm.connectionsLbl.Location.Y;
                var lastEntry = mainForm.entries.LastOrDefault();
                var y = lastEntry != null 
                    ? lastEntry.label.Location.Y + (lastEntry.IsLineSeparator ? SEP_H : deltaY)
                    : 5;
                label = new Label
                {
                    Text = data,
                    Location = new Point(mainForm.connectionsLbl.Location.X, y),
                    Size = new Size(IsLineSeparator ? mainForm.startSplitPaneHorizontal.Right - mainForm.connectionsLbl.Left : mainForm.placeHolder.Size.Width, IsLineSeparator ? 3 : mainForm.connectionsLbl.Size.Height),
                    BackColor = Color.Transparent,
                };
                label.Click += Label_Click;
                y = lastEntry != null
                    ? lastEntry.startBtn.Location.Y + (lastEntry.IsLineSeparator ? SEP_H : deltaY)
                    : 5 - mainForm.connectionsLbl.Location.Y + mainForm.start.Location.Y;
                startBtn = new Button()
                {
                    Text = mainForm.start.Text,
                    Location = new Point(mainForm.start.Location.X, y),
                    Size = mainForm.start.Size,
                };
                startBtn.Click += (sender, @event) =>
                {
                    StartCommand(WindowType.NewTab);
                };
                startSplitPaneHorizontalBtn = new Button()
                {
                    Text = mainForm.startSplitPaneHorizontal.Text,
                    Location = new Point(mainForm.startSplitPaneHorizontal.Location.X, y),
                    Size = mainForm.startSplitPaneHorizontal.Size,
                };
                startSplitPaneHorizontalBtn.Click += (sender, @event) =>
                {
                    StartCommand(WindowType.SplitHorizontallyCurrentTab);
                };
                line = new Label
                {
                    Text = string.Empty,
                    Location = new Point(label.Location.X, (IsLineSeparator ? label.Height : startBtn.Height) + y),
                    AutoSize = false,
                    Size = new Size(startSplitPaneHorizontalBtn.Right - label.Left, 1),
                    BackColor = IsLineSeparator ? Color.Gray : Color.Gainsboro,
                };
                label.Parent = line;

                new ToolTip().SetToolTip(startBtn, "Start in New Tab");
                new ToolTip().SetToolTip(startSplitPaneHorizontalBtn, "Start in Split Pane of current Tab");

                mainForm.container.Controls.Add(label);
                if (!IsLineSeparator)
                {
                    mainForm.container.Controls.Add(startBtn);
                    mainForm.container.Controls.Add(startSplitPaneHorizontalBtn);
                }
                mainForm.container.Controls.Add(line);
                this.mainForm = mainForm;
            }

            private void Label_Click(object? sender, EventArgs e)
            {
                if (sender is Label l)
                {
                    mainForm.connection.Text = l.Text;
                }
            }

            public Control[] Controls => IsLineSeparator? new Control[] { label, line } : new Control[] { label, startBtn, startSplitPaneHorizontalBtn, line };

            public void Remove(bool clean = false)
            {
                mainForm.container.Controls.Remove(label);
                if (!IsLineSeparator)
                {
                    mainForm.container.Controls.Remove(startBtn);
                    mainForm.container.Controls.Remove(startSplitPaneHorizontalBtn);
                }
                mainForm.container.Controls.Remove(line);
                if (clean)
                {
                    mainForm.entries.Remove(this);
                }
            }

            public bool Match(string data)
            {
                return label.Text == data;
            }

            private void StartCommand(WindowType windowType)
            {
                string window = windowType switch
                {
                    WindowType.NewTab => "nt ",
                    WindowType.SplitVerticallyCurrentTab => "sp ",
                    WindowType.SplitHorizontallyCurrentTab => "sp ",
                    _ => "",
                };

                var command = label.Text.Split(new char[] { '`', '~' }, 2).Last();
                if (command.StartsWith('-'))
                {
                    command = mainForm.defaultEnvironmentCommand + command[1..];
                }
                else if (command.StartsWith('='))
                {
                    command = command[1..];
                }
                else
                {
                    command = mainForm.defaultEnvironmentCommand + "ssh " + command;
                }
                ProcessStartInfo processStart = new()
                {
                    FileName = "wt", // Windows Terminal
                    Arguments = window + command,
                    CreateNoWindow = true,
                };
                Process.Start(processStart);
            }

            internal string GetCommand()
            {
                return label.Text;
            }

            enum WindowType
            {
                NewTab,
                SplitHorizontallyCurrentTab,
                SplitVerticallyCurrentTab,
            }
        }
    }
}