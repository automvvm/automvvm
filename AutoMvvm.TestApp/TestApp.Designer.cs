namespace AutoMvvm.TestApp
{
    partial class TestApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TestComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TestComboBox
            // 
            this.TestComboBox.FormattingEnabled = true;
            this.TestComboBox.Items.AddRange(new object[] {
            "Test1",
            "Test2",
            "Test3"});
            this.TestComboBox.Location = new System.Drawing.Point(21, 23);
            this.TestComboBox.Name = "TestComboBox";
            this.TestComboBox.Size = new System.Drawing.Size(121, 21);
            this.TestComboBox.TabIndex = 0;
            // 
            // TestApp
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TestComboBox);
            this.Name = "TestApp";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox TestComboBox;
    }
}