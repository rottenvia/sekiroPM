using SekiroParamMerger.Core;

namespace SekiroParamMerger.WinForms
{
    internal static class Program
    {
        /// <summary>True when the oo2core DLL was found / auto-copied. Read by MainForm to decide if merging is allowed.</summary>
        internal static bool OodleReady;

        /// <summary>Human-readable message about the DLL status (shown in the status bar when not ready).</summary>
        internal static string OodleMessage = string.Empty;

        [STAThread]
        static void Main()
        {
            Log("=== Application starting ===");

            // ── Global exception handling: never let a startup error die silently ──
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => ShowFatal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex) ShowFatal(ex);
            };
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                ShowFatal(e.Exception);
                e.SetObserved();
            };

            try
            {
                ApplicationConfiguration.Initialize();
            }
            catch (Exception ex)
            {
                ShowFatal(ex);
                return;
            }

            // ── Validate oo2core DLL ─────────────────────────────────────────
            string toolDir = AppContext.BaseDirectory;
            try
            {
                OodleValidationResult oodleResult = OodleValidator.Validate(toolDir);
                OodleReady = oodleResult.IsValid;
                OodleMessage = oodleResult.Message;
                Log($"Oodle: ready={OodleReady}  {OodleMessage}");

                if (!oodleResult.IsValid)
                {
                    MessageBox.Show(
                        oodleResult.Message,
                        "Missing Required DLL",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                OodleReady = false;
                OodleMessage = "Could not validate oo2core_6_win64.dll: " + ex.Message;
                LogError(ex);
            }

            try
            {
                Log("Constructing MainForm...");
                var form = new MainForm();
                Log($"MainForm constructed. Visible={form.Visible} WindowState={form.WindowState} IsHandleCreated={form.IsHandleCreated}");
                Log("Calling Application.Run(form)...");
                Application.Run(form);
                Log($"Application.Run returned. Visible={form.Visible} IsDisposed={form.IsDisposed}");
                Log("Application exited normally (window was closed).");
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>Shows the error to the user AND writes it to error.log.</summary>
        private static void ShowFatal(Exception ex)
        {
            LogError(ex);
            try
            {
                MessageBox.Show(
                    "An unexpected error occurred:\n\n" + ex,
                    "Sekiro Param Merger — Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch { /* message box itself failed — log is the fallback */ }
        }

        private static void Log(string message)
        {
            try
            {
                string logPath = Path.Combine(AppContext.BaseDirectory, "startup.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
            }
            catch { /* best-effort */ }
        }

        private static void LogError(Exception ex)
        {
            string full = ex.ToString();
            Log("EXCEPTION: " + full);
            try
            {
                string errorPath = Path.Combine(AppContext.BaseDirectory, "error.log");
                File.AppendAllText(
                    errorPath,
                    $"===== {DateTime.Now:yyyy-MM-dd HH:mm:ss} =====\n{full}\n\n");
            }
            catch { /* best-effort */ }
        }
    }
}
