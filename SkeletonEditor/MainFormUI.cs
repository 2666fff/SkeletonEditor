using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using SkeletonEditor.Core;

namespace SkeletonEditor.UI
{
    public class MainFormUI : UserControl
    {
        private TextBox txtFilePath;
        private TextBox txtXOffset;
        private TextBox txtYOffset;
        private TextBox txtScale;
        private TextBox txtBoneName; // 新增：骨骼名称输入框
        private Label lblStatus;
        private Label lblBoneInfo; // 用于显示骨骼信息
        private AppSettings settings;

        private readonly string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SkeletonEditorSettings.json");

        public MainFormUI()
        {
            LoadSettings();
            SetupUI();

            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    settings = new AppSettings();
                }
            }
            catch
            {
                settings = new AppSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                settings.FilePath = txtFilePath.Text;
                settings.XOffset = txtXOffset.Text;
                settings.YOffset = txtYOffset.Text;
                settings.Scale = txtScale.Text;
                settings.BoneName = txtBoneName.Text; // 保存骨骼名称

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "保存设置失败: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

       private void SetupUI()
{
    this.BackColor = Color.White;
    int yPos = 20; // 垂直位置跟踪器
    int labelWidth = 120; // 标签统一宽度
    
    // 标题
    Label lblTitle = new Label
    {
        Text = "Skeleton Editor (Spine 3.4.02)",
        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
        Location = new Point(20, yPos),
        AutoSize = true
    };
    yPos += 60;
    
    // 文件选择区域
    Label lblFile = new Label 
    { 
        Text = "文件/文件夹路径:", 
        Location = new Point(20, yPos), 
        Size = new Size(labelWidth, 20),
    };
    
    txtFilePath = new TextBox 
    { 
        Location = new Point(20 + labelWidth, yPos), 
        Size = new Size(500, 30),
        Text = settings.FilePath,
        Anchor = AnchorStyles.Left | AnchorStyles.Top 
    };
    txtFilePath.AllowDrop = true;
    txtFilePath.DragEnter += MainForm_DragEnter;
    txtFilePath.DragDrop += MainForm_DragDrop;
    txtFilePath.TextChanged += FilePath_TextChanged;
    
    Button btnBrowse = new Button 
    { 
        Text = "浏览", 
        Location = new Point(530 + labelWidth, yPos), 
        Size = new Size(80, 30) 
    };
    btnBrowse.Click += BrowseButton_Click;
    yPos += 40;
    
    // 骨骼信息显示区域（竖向排列）
    lblBoneInfo = new Label 
    { 
        Location = new Point(20, yPos), 
        Size = new Size(700, 100),
        ForeColor = Color.DarkBlue,
        Font = new Font("Segoe UI", 9F, FontStyle.Regular),
        BorderStyle = BorderStyle.FixedSingle,
        Padding = new Padding(5)
    };
    yPos += 110;
    
    // 偏移调整区域（X和Y在同一行）
    Label lblOffset = new Label 
    { 
        Text = "偏移调整:", 
        Location = new Point(20, yPos), 
        Size = new Size(labelWidth, 20),
    };
    
    Label lblX = new Label 
    { 
        Text = "X:", 
        Location = new Point(20 + labelWidth, yPos), 
        Size = new Size(30, 20),
    };
    
    txtXOffset = new TextBox 
    { 
        Location = new Point(50 + labelWidth, yPos), 
        Size = new Size(80, 30), 
        Text = settings.XOffset 
    };
    
    Label lblY = new Label 
    { 
        Text = "Y:", 
        Location = new Point(140 + labelWidth, yPos), 
        Size = new Size(30, 20),
    };
    
    txtYOffset = new TextBox 
    { 
        Location = new Point(170 + labelWidth, yPos), 
        Size = new Size(80, 30), 
        Text = settings.YOffset 
    };
    yPos += 40;
    
    // 缩放比例区域（单独一行）
    Label lblScale = new Label 
    { 
        Text = "缩放比例:", 
        Location = new Point(20, yPos), 
        Size = new Size(labelWidth, 20),
    };
    
    txtScale = new TextBox 
    { 
        Location = new Point(20 + labelWidth, yPos), 
        Size = new Size(80, 30), 
        Text = settings.Scale 
    };
    yPos += 40;
    
    // 骨骼名称输入框（单独一行）
    Label lblBoneName = new Label 
    { 
        Text = "骨骼名称:", 
        Location = new Point(20, yPos), 
        Size = new Size(labelWidth, 20),
    };
    
    txtBoneName = new TextBox 
    { 
        Location = new Point(20 + labelWidth, yPos), 
        Size = new Size(300, 30), 
        Text = settings.BoneName,
        PlaceholderText = "输入骨骼名称（如：hero.skel），留空则处理所有"
    };
    yPos += 40;
    
    // 处理按钮
    Button btnProcess = new Button 
    { 
        Text = "开始处理", 
        Location = new Point(20, yPos), 
        Size = new Size(120, 40),
        BackColor = Color.LightBlue
    };
    btnProcess.Click += ProcessButton_Click;
    yPos += 50;
    
    // 状态信息
    lblStatus = new Label 
    { 
        Location = new Point(20, yPos), 
        Size = new Size(700, 40),
        ForeColor = Color.DarkRed
    };
    
    // 添加所有控件
    Controls.AddRange(new Control[] {
        lblTitle, 
        lblFile, txtFilePath, btnBrowse,
        lblBoneInfo,
        lblOffset, lblX, txtXOffset, lblY, txtYOffset,
        lblScale, txtScale,
        lblBoneName, txtBoneName,
        btnProcess, 
        lblStatus
    });
    
    // 初始化时显示信息
    if (!string.IsNullOrWhiteSpace(settings.FilePath))
    {
        DisplayBoneInfo(settings.FilePath);
    }
    
    // 确保窗口足够大以显示所有内容
    this.MinimumSize = new Size(750, yPos + 100);
    this.Size = new Size(750, yPos + 100);
}
        // 文件路径变化时显示信息
        private void FilePath_TextChanged(object sender, EventArgs e)
        {
            DisplayBoneInfo(txtFilePath.Text);
        }

        // 显示骨骼信息（竖向排列）
// 显示骨骼信息（竖向排列）
        // 显示骨骼信息（竖向排列）
        private void DisplayBoneInfo(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) 
            {
                lblBoneInfo.Text = "未选择文件或文件夹";
                lblBoneInfo.ForeColor = Color.DarkBlue;
                return;
            }
    
            try
            {
                if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".skel", StringComparison.OrdinalIgnoreCase))
                {
                    var boneInfo = SkeletonProcessor.GetBoneInfo(filePath);
                    lblBoneInfo.Text = $"骨骼信息:\n" +
                                       $"X偏移: {boneInfo.XOffset}   " +
                                       $"Y偏移: {boneInfo.YOffset}\n" +
                                       $"X缩放: {boneInfo.ScaleX}   " +
                                       $"Y缩放: {boneInfo.ScaleY}";
                    lblBoneInfo.ForeColor = Color.DarkBlue;
                }
                else if (Directory.Exists(filePath))
                {
                    lblBoneInfo.Text = $"已选择文件夹:\n{filePath}";
                    lblBoneInfo.ForeColor = Color.DarkBlue;
                }
                else
                {
                    lblBoneInfo.Text = $"无效路径:\n{filePath}";
                    lblBoneInfo.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblBoneInfo.Text = $"读取骨骼信息失败:\n{ex.Message}";
                lblBoneInfo.ForeColor = Color.Red;
            }
        }
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                txtFilePath.Text = files[0];
                lblStatus.Text = "已选择: " + Path.GetFileName(files[0]);
                DisplayBoneInfo(files[0]);
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Skel 文件|*.skel|所有文件|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = dialog.FileName;
                lblStatus.Text = "已选择文件: " + Path.GetFileName(dialog.FileName);
                DisplayBoneInfo(dialog.FileName);
            }
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtXOffset.Text, out int xOffsetAdd) ||
                !int.TryParse(txtYOffset.Text, out int yOffsetAdd) ||
                !float.TryParse(txtScale.Text, out float scaleAdd))
            {
                lblStatus.Text = "输入格式错误";
                return;
            }

            string path = txtFilePath.Text;
            if (string.IsNullOrWhiteSpace(path))
            {
                lblStatus.Text = "请选择路径";
                return;
            }

            try
            {
                if (File.Exists(path))
                {
                    // 单个文件处理
                    string backup = Path.Combine(Path.GetDirectoryName(path), "backup");
                    SkeletonProcessor.ProcessFile(path, xOffsetAdd, yOffsetAdd, scaleAdd, backup);
                    lblStatus.Text = "单文件处理完成";
                    
                    // 处理完成后刷新显示
                    DisplayBoneInfo(path);
                }
                else if (Directory.Exists(path))
                {
                    // 文件夹处理（新增骨骼名称过滤）
                    string backupRoot = Path.Combine(Directory.GetParent(path).FullName, "backup");
                    string boneName = txtBoneName.Text.Trim();
                    
                    int count = SkeletonProcessor.ProcessDirectory(
                        path, 
                        xOffsetAdd, 
                        yOffsetAdd, 
                        scaleAdd, 
                        backupRoot,
                        boneName
                    );
                    
                    lblStatus.Text = $"处理完成，共处理 {count} 个文件";
                }
                else
                {
                    lblStatus.Text = "路径无效";
                }
                SaveSettings();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "处理失败: " + ex.Message;
            }
        }
    }

    public class AppSettings
    {
        public string FilePath { get; set; } = "";
        public string XOffset { get; set; } = "0";
        public string YOffset { get; set; } = "-260";
        public string Scale { get; set; } = "1.0";
        public string BoneName { get; set; } = ""; // 新增：骨骼名称
    }
}