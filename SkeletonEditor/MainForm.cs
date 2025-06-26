// === MainForm.cs ===
using System;
using System.Windows.Forms;
using SkeletonEditor.UI;

namespace SkeletonEditor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private readonly MainFormUI ui;

        public MainForm()
        {
            this.Text = "Skeleton Editor";
            this.Size = new System.Drawing.Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            ui = new MainFormUI();
            ui.Dock = DockStyle.Fill;
            this.Controls.Add(ui);

            var menu = new MenuStrip();
            var aboutItem = new ToolStripMenuItem("关于");
            aboutItem.Click += (s, e) => ShowAbout();
            menu.Items.Add(aboutItem);
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }

        private void ShowAbout()
        {
            string message = "SkeletonEditor by 2666fff\nSpine 3.4.02 support only\nhttps://github.com/2666fff/SkeletonEditor";
            MessageBox.Show(message, "关于 SkeletonEditor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
