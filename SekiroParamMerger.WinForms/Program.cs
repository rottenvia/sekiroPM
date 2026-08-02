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
            // ── Global exception handling: never let a startup error die silently ──
            // WinForms exceptions on the UI thread terminate the process with no
            // dialog by default. Catching them here surfaces the real problem so the
            // user can see it instead of a "silent crash".
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
                // Generated from the project settings (net9.0-windows + UseWindowsForms).
                // The <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
                // property already makes this set high-DPI mode, so we don't call
                // Application.SetHighDpiMode separately.
                ApplicationConfiguration.Initialize();
            }
            catch (Exception ex)
            {
                ShowFatal(ex);
                return;
            }

            // ── Validate oo2core DLL ─────────────────────────────────────────
            // IMPORTANT: even if the DLL is missing we still open the main window
            // so the app is never "silently dead". MainForm reads OodleReady /
            // OodleMessage and shows a clear warning + disables merging until the
            // user drops oo2core_6_win64.dll next to the exe (or installs Sekiro).
            string toolDir = AppContext.BaseDirectory;
            try
            {
                OodleValidationResult oodleResult = OodleValidator.Validate(toolDir);
                OodleReady = oodleResult.IsValid;
                OodleMessage = oodleResult.Message;

                if (!oodleResult.IsValid)
                {
                    // Tell the user what to do, then still show the window.
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
                ShowFatal(ex);
            }

            // ── Write a small startup log for diagnosis ──────────────────────
            try
            {
                string logPath = Path.Combine(toolDir, "startup.log");
                File.WriteAllText(logPath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] oo2core ready: {OodleReady}\n" +
                    $"{OodleMessage}\n");
            }
            catch { /* logging is best-effort */ }

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ShowFatal(ex);
            }
        }

        private static void ShowFatal(Exception ex)
        {
            MessageBox.Show(
                "An unexpected error occurred:\n\n" + ex,
                "Sekiro Param Merger — Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
