// === MainForm.cs ===

using System;
using System.Diagnostics;
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
            // 创建自定义关于表单
            using (var aboutForm = new Form())
            {
                aboutForm.Text = "关于 SkeletonEditor";
                aboutForm.Size = new Size(600, 240);
                aboutForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                aboutForm.StartPosition = FormStartPosition.CenterParent;
                aboutForm.MinimizeBox = false;
                aboutForm.MaximizeBox = false;

                // 创建标签控件
                var label = new Label
                {
                    Text = "SkeletonEditor by 2666fff\nSpine 3.x-4.x support",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                // 创建超链接标签
                var linkLabel = new LinkLabel
                {
                    Text = "https://github.com/2666fff/SkeletonEditor",
                    Location = new Point(20, 80),
                    AutoSize = true,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                    LinkColor = Color.Blue,
                    ActiveLinkColor = Color.Red
                };

                // 添加点击事件
                linkLabel.LinkClicked += (sender, e) =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://github.com/2666fff/SkeletonEditor",
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开链接: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // 创建确定按钮
                var okButton = new Button
                {
                    Text = "确定",
                    DialogResult = DialogResult.OK,
                    Size = new Size(75, 30),
                    Location = new Point(450, 120)
                };

                // 添加控件到表单
                aboutForm.Controls.Add(label);
                aboutForm.Controls.Add(linkLabel);
                aboutForm.Controls.Add(okButton);

                // 设置接受按钮
                aboutForm.AcceptButton = okButton;

                // 显示对话框
                aboutForm.ShowDialog(this);
            }
        }
    }
}