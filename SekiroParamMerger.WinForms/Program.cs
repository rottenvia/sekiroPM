using SekiroParamMerger.Core;

namespace SekiroParamMerger.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            // ── Validate oo2core DLL before showing any UI ────────────────────
            string toolDir = AppContext.BaseDirectory;
            OodleValidationResult oodleResult = OodleValidator.Validate(toolDir);

            if (!oodleResult.IsValid)
            {
                MessageBox.Show(
                    oodleResult.Message,
                    "Missing Required DLL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm());
        }
    }
}
